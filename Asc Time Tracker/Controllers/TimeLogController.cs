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

        // GET: IndexLogs partial view
        public async Task<IActionResult> IndexLogs(DateTime? startDate, DateTime? endDate)
        {
            IQueryable<TimeLog> timeLogs = from log in _context.TimeLog select log;

            return PartialView(await IndexViewModel.FilterTimeLogsByDate(timeLogs, startDate, endDate).ToListAsync());
        }

        // GET: IndexStats partial view
        public async Task<IActionResult> IndexStats(DateTime? startDate, DateTime? endDate)
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

            Chart chart = new()
            {
                Type = Enums.ChartType.Pie
            };

            List<string> labels = new();
            List<double?> time = new();
            List<ChartColor> colors = new();

            foreach (TimeLog timeLog in timeLogs)
            {
                labels.Add(timeLog.JobNum);

                double hours = Math.Round(timeLog.Time / 3600, 2);
                time.Add(hours);

                string hex = TimeLog.JobNumToRgb(timeLog.JobNum);
                colors.Add(ChartColor.FromHexString(hex));
            }

            ChartJSCore.Models.Data data = new()
            {
                Labels = labels
            };

            PieDataset dataset = new()
            {
                BackgroundColor = colors,
                HoverBackgroundColor = colors,
                Data = time
            };

            data.Datasets = new List<Dataset> { dataset };

            chart.Data = data;

            return chart;
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