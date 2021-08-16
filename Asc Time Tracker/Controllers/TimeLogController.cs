using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Asc_Time_Tracker.Data;
using Asc_Time_Tracker.Models;
using ChartJSCore.Helpers;
using ChartJSCore.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Asc_Time_Tracker.Controllers
{
    public class TimeLogController : Controller
    {
        private readonly ApplicationDbContext _context;

        public IndexViewModel IndexViewModel { get; set; }

        // Used for creating timelogs.
        [BindProperty]
        public TimeLog TimeLog { get; set; }

        public TimeLogController(ApplicationDbContext context)
        {
            _context = context;
            IndexViewModel = new IndexViewModel();
        }

        // GET: TimeLog
        public IActionResult Index()
        {
            // Send along an empty IQueryable for the first render so there's no flashing.
            IQueryable<TimeLog> timeLogs = _context.TimeLog.Take(0);
            IndexViewModel.TimeLogs = timeLogs;

            return View(IndexViewModel);
        }

        // GET: _IndexLogs partial view
        public async Task<IActionResult> _IndexLogs(DateTime? startDate, DateTime? endDate)
        {
            IQueryable<TimeLog> timeLogs = from log in _context.TimeLog select log;

            return PartialView(await IndexViewModel.FilterTimeLogsByDate(timeLogs, startDate, endDate).ToListAsync());
        }

        // GET: _IndexStats partial view
        public async Task<IActionResult> _IndexStats(DateTime? startDate, DateTime? endDate)
        {
            IQueryable<TimeLog> timeLogs = from log in _context.TimeLog select log;
            timeLogs = IndexViewModel.FilterTimeLogsByDate(timeLogs, startDate, endDate);

            // Draw charts.
            if (timeLogs.Any())
            {
                ViewData["TimeSpentChart"] = GeneratePieChart(timeLogs);
            }

            /*
            IEnumerable<TimeLog> timeLogsList = timeLogs.ToList();
            foreach (TimeLog timeLog in timeLogs)
            {
            }
            */

            return PartialView(await timeLogs.ToListAsync());
        }

        private static Chart GeneratePieChart(IQueryable<TimeLog> timeLogs)
        {
            // Sort time logs by top 5 with most time spent on them.
            timeLogs = timeLogs.GroupBy(t => t.JobNum)
                .Select(tg => new TimeLog
                {
                    JobNum = tg.Key,
                    Time = tg.Sum(t => t.Time)
                })
                .OrderByDescending(t => t.Time)
                .Take(5);

            Chart chart = new Chart
            {
                Type = Enums.ChartType.Pie
            };

            List<string> labels = new List<string>();
            List<double?> time = new List<double?>();

            foreach (TimeLog timeLog in timeLogs)
            {
                labels.Add(timeLog.JobNum);
                double adjTime = Math.Round(timeLog.Time / 3600, 2);
                time.Add(adjTime);
            }

            ChartJSCore.Models.Data data = new ChartJSCore.Models.Data
            {
                Labels = labels
            };

            PieDataset dataset = new PieDataset()
            {
                BackgroundColor = new List<ChartColor>() {
                    ChartColor.FromHexString("#FF6384"),
                    ChartColor.FromHexString("#36A2EB"),
                    ChartColor.FromHexString("#FFCE56"),
                    ChartColor.FromHexString()
                },
                HoverBackgroundColor = new List<ChartColor>() {
                    ChartColor.FromHexString("#FA3A3A"),
                    ChartColor.FromHexString("#36A2EB"),
                    ChartColor.FromHexString("#FFCE56")
                },
                Data = time
            };

            data.Datasets = new List<Dataset>();
            data.Datasets.Add(dataset);

            chart.Data = data;

            return chart;
        }

        public string jobNumToColor(string jobNum)
        {
            // Convert to hsl.
            var s = 0.75;
            var l = 0.5;

            // Get hashcode from job number.
            var hash = 0;
            foreach (char c in jobNum)
            {
                hash = c + ((hash << 5) - hash);
                hash = hash & hash; // Convert to 32bit integer.
            }
            var h = hash % 360;

            // Convert hsl to rgb.
            l /= 100;
            var a = s * Math.Min(l, 1 - l) / 100;
            Func<int, string> f = n =>
            {
                var k = (n + h / 30) % 12;
                var color = l - a * Math.Max(Math.Min(Math.Min(k - 3, 9 - k), 1), -1);

                // Convert to Hex and prefix "0" if needed.
                // TODO SEE: https://stackoverflow.com/questions/36721830/convert-hsl-to-rgb-and-hex
                return Math.Round(255 * color).ToString().PadLeft(2, '0');
            };
            return "${f(0)}${f(8)}${f(4)}";
        }

        // POST: TimeLog/Create
        // Handles modal submission.
        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAsync()
        {
            if (ModelState.IsValid)
            {
                // Convert input to seconds.
                int hours = (string.IsNullOrEmpty(Request.Form["timeHours"])) ? 0 : Convert.ToInt32(Request.Form["timeHours"]);
                int minutes = (string.IsNullOrEmpty(Request.Form["timeMinutes"])) ? 0 : Convert.ToInt32(Request.Form["timeMinutes"]);

                // Convert input to seconds.
                TimeLog.Time = hours * 3600 + minutes * 60;

                _context.TimeLog.Add(TimeLog);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }

        // POST: TimeLog/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,EmpId,JobNum,Date,Time,Notes,Rd")] TimeLog timeLog)
        {
            if (id != timeLog.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(timeLog);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TimeLogExists(timeLog.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Index");
            }
            return View(timeLog);
        }

        // POST: TimeLog/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            TimeLog = await _context.TimeLog.FindAsync(id);

            if (TimeLog != null)
            {
                // Important: need to get model again after removing and saving changes
                // to display correctly.
                _context.TimeLog.Remove(TimeLog);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }

        private bool TimeLogExists(int id)
        {
            return _context.TimeLog.Any(e => e.Id == id);
        }
    }
}