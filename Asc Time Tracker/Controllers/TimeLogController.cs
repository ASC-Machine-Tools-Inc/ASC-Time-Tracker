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

        // Used for creating time logs.
        [BindProperty]
        public TimeLog TimeLog { get; set; }

        public TimeLogController(ApplicationDbContext context)
        {
            _context = context;
            IndexViewModel = new IndexViewModel(_context.TimeLog);
        }

        // GET: TimeLog
        public IActionResult Index()
        {
            // Send along the time logs for today by default.
            IndexViewModel.TimeLogs = IndexViewModel
                .FilterTimeLogsByDate(DateTime.Today, DateTime.Today.AddDays(1));

            return View(IndexViewModel);
        }

        // GET: IndexLogs partial view
        public async Task<IActionResult> IndexLogs(DateTime? startDate, DateTime? endDate)
        {
            return PartialView(await IndexViewModel
                .FilterTimeLogsByDate(startDate, endDate).ToListAsync());
        }

        // GET: IndexStats partial view
        public async Task<IActionResult> IndexStats(DateTime? startDate, DateTime? endDate, int pieCount)
        {
            IQueryable<TimeLog> timeLogs = IndexViewModel.FilterTimeLogsByDate(startDate, endDate);

            // Draw charts.
            if (timeLogs.Any())
            {
                ViewData["TimeSpentChart"] = IndexViewModel.GenerateTopXPieChart(pieCount);
                ViewData["WeekBarChart"] = IndexViewModel.GenerateWeekBarChart();
            }

            return PartialView(await timeLogs.ToListAsync());
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
                int hours = (string.IsNullOrEmpty(Request.Form["timeHours"])) ? 0 :
                    Convert.ToInt32(Request.Form["timeHours"]);
                int minutes = (string.IsNullOrEmpty(Request.Form["timeMinutes"])) ? 0 :
                    Convert.ToInt32(Request.Form["timeMinutes"]);

                // Convert input to seconds.
                TimeLog.Time = TimeLog.HoursAndMinutesToSeconds(hours, minutes);

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