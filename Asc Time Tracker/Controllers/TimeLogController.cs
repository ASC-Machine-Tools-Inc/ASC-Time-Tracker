using Asc_Time_Tracker.Data;
using Asc_Time_Tracker.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

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

        // GET: MainIndex
        public IActionResult MainIndex()
        {
            // Send along the time logs for today and the current user by default.
            IndexViewModel.FilterTimeLogsByEmpId(User.Identity.Name);
            IndexViewModel.FilterTimeLogsByDate(DateTime.Today, DateTime.Today.AddDays(1));

            return View(IndexViewModel);
        }

        // GET: IndexInfo
        public IActionResult IndexInfo()
        {
            return MainIndex();
        }

        // GET: IndexInfo partial view
        public IActionResult IndexInfoPartial()
        {
            return PartialView(IndexViewModel);
        }

        // GET: IndexLogs partial view
        public async Task<IActionResult> IndexLogs(
            DateTime? startDate,
            DateTime? endDate,
            string empId)
        {
            IndexViewModel.FilterTimeLogsByEmpId(empId);
            IndexViewModel.FilterTimeLogsByDate(startDate, endDate);
            return PartialView(await IndexViewModel.TimeLogs.ToListAsync());
        }

        // GET: IndexStats partial view
        public async Task<IActionResult> IndexStats(
            DateTime? startDate,
            DateTime? endDate,
            string empId,
            int pieCount)
        {
            IndexViewModel.FilterTimeLogsByEmpId(empId);
            IndexViewModel.FilterTimeLogsByDate(startDate, endDate);

            if (IndexViewModel.TimeLogs.Any())
            {
                // Grab statistics.
                // Move to model?
                ViewData["TotalTimeSpent"] = TimeLog.SecondsToHoursAndMinutesString(
                    IndexViewModel.TimeLogs.Sum(t => t.Time));

                TimeLog topTimeLog = IndexViewModel.TakeTopXTimeLogs(1).First();
                ViewData["TopJobNum"] = topTimeLog.JobNum;
                ViewData["TopJobNumColor"] = TimeLog.JobNumToRgb(topTimeLog.JobNum);
                ViewData["TopTime"] = TimeLog.SecondsToHoursAndMinutesString(topTimeLog.Time);

                // Draw charts.
                ViewData["TimeSpentChart"] = IndexViewModel.GenerateTopXPieChart(pieCount);
                ViewData["WeekBarChart"] = IndexViewModel.GenerateWeekBarChart();
            }

            return PartialView(await IndexViewModel.TimeLogs.ToListAsync());
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

            return RedirectToAction("MainIndex");
        }

        // POST: TimeLog/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([Bind("Id,EmpId,JobNum,Date,Time,Notes,Rd")] TimeLog timeLog)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Convert input to seconds.
                    int hours = (string.IsNullOrEmpty(Request.Form["timeHours"])) ? 0 :
                        Convert.ToInt32(Request.Form["timeHours"]);
                    int minutes = (string.IsNullOrEmpty(Request.Form["timeMinutes"])) ? 0 :
                        Convert.ToInt32(Request.Form["timeMinutes"]);

                    // Convert input to seconds.
                    timeLog.Time = TimeLog.HoursAndMinutesToSeconds(hours, minutes);

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
            }
            return RedirectToAction("MainIndex");
        }

        // POST: TimeLog/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            TimeLog = await _context.TimeLog.FindAsync(id);

            if (TimeLog != null)
            {
                _context.TimeLog.Remove(TimeLog);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("MainIndex");
        }

        private bool TimeLogExists(int id)
        {
            return _context.TimeLog.Any(e => e.Id == id);
        }
    }
}