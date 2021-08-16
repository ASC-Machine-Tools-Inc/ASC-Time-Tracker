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
                Chart barChart = GenerateLineChart();
                ViewData["BarChart"] = barChart;
            }

            /*
            IEnumerable<TimeLog> timeLogsList = timeLogs.ToList();
            foreach (TimeLog timeLog in timeLogs)
            {
            }
            */

            return PartialView(await timeLogs.ToListAsync());
        }

        private static Chart GenerateLineChart()
        {
            Chart chart = new Chart();
            chart.Type = Enums.ChartType.Line;

            ChartJSCore.Models.Data data = new ChartJSCore.Models.Data();
            data.Labels = new List<string>() { "January", "February", "March", "April", "May", "June", "July" };

            LineDataset dataset = new LineDataset()
            {
                Label = "My First dataset",
                Data = new List<double?>() { 65, 59, 80, 81, 56, 55, 40 },
                Fill = "false",
                LineTension = 0.1,
                BackgroundColor = ChartColor.FromRgba(75, 192, 192, 0.4),
                BorderColor = ChartColor.FromRgba(75, 192, 192, 1),
                BorderCapStyle = "butt",
                BorderDash = new List<int> { },
                BorderDashOffset = 0.0,
                BorderJoinStyle = "miter",
                PointBorderColor = new List<ChartColor>() { ChartColor.FromRgba(75, 192, 192, 1) },
                PointBackgroundColor = new List<ChartColor>() { ChartColor.FromHexString("#fff") },
                PointBorderWidth = new List<int> { 1 },
                PointHoverRadius = new List<int> { 5 },
                PointHoverBackgroundColor = new List<ChartColor>() { ChartColor.FromRgba(75, 192, 192, 1) },
                PointHoverBorderColor = new List<ChartColor>() { ChartColor.FromRgba(220, 220, 220, 1) },
                PointHoverBorderWidth = new List<int> { 2 },
                PointRadius = new List<int> { 1 },
                PointHitRadius = new List<int> { 10 },
                SpanGaps = false
            };

            data.Datasets = new List<Dataset>();
            data.Datasets.Add(dataset);

            Options options = new Options()
            {
                Scales = new Scales()
            };

            Scales scales = new Scales()
            {
                YAxes = new List<Scale>()
                {
                    new CartesianScale()
                }
            };

            CartesianScale yAxes = new CartesianScale()
            {
                Ticks = new Tick()
            };

            Tick tick = new Tick()
            {
                Callback = "function(value, index, values) {return '$' + value;}"
            };

            yAxes.Ticks = tick;
            scales.YAxes = new List<Scale>() { yAxes };
            options.Scales = scales;
            chart.Options = options;

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

        // GET: TimeLog/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var timeLog = await _context.TimeLog.FindAsync(id);
            if (timeLog == null)
            {
                return NotFound();
            }
            return View(timeLog);
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
            // Grab the ID to delete from our hidden input field in the modal.
            int logId;
            if (int.TryParse(Request.Form["TimeLog.Id"], out logId))
            {
                TimeLog = await _context.TimeLog.FindAsync(logId);

                if (TimeLog != null)
                {
                    // Important: need to get model again after removing and saving changes
                    // to display correctly.
                    _context.TimeLog.Remove(TimeLog);
                    await _context.SaveChangesAsync();
                }
            }

            return RedirectToAction("Index");
        }

        private bool TimeLogExists(int id)
        {
            return _context.TimeLog.Any(e => e.Id == id);
        }
    }
}