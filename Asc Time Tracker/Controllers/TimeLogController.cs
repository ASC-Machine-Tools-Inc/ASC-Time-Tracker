using Asc_Time_Tracker.Data;
using Asc_Time_Tracker.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
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
            IndexViewModel.TimeLogs = _context.TimeLog;
            return View(IndexViewModel);
        }

        // GET: IndexInfo
        public IActionResult IndexInfo()
        {
            return MainIndex();
        }

        // GET: IndexInfo partial view
        [ActionName("_IndexInfo")]
        public IActionResult IndexInfoPartial()
        {
            return PartialView(IndexViewModel);
        }

        // GET: IndexLogs partial view
        [ActionName("_IndexLogs")]
        public async Task<IActionResult> IndexLogs(
            List<string> empIds,
            DateTime? startDate,
            DateTime? endDate,
            string category,
            string jobNum,
            string notes,
            bool rd)
        {
            IQueryable<TimeLog> timeLogs = IndexViewModel.TimeLogs;
            timeLogs = IndexViewModel.FilterTimeLogs(
                timeLogs, empIds, startDate, endDate,
                category, jobNum, notes, rd);

            return PartialView(await timeLogs.ToListAsync());
        }

        // GET: IndexStats partial view
        [ActionName("_IndexStats")]
        public IActionResult IndexStats(
            DateTime? startDate,
            DateTime? endDate,
            List<string> empIds,
            int pieCount)
        {
            IQueryable<TimeLog> timeLogs = IndexViewModel.TimeLogs;
            timeLogs = IndexViewModel.FilterTimeLogsByEmpIds(timeLogs, empIds);
            timeLogs = IndexViewModel.FilterTimeLogsByDate(timeLogs, startDate, endDate);

            TimeLogStats stats = new(timeLogs, pieCount);

            return PartialView(stats);
        }

        // GET: Timer partial view
        [ActionName("_Timer")]
        public IActionResult Timer(int timerId)
        {
            Timer timer = new(timerId);
            return PartialView(timer);
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
        public async Task<IActionResult> Edit([Bind("Id,EmpId,Category,JobNum,Date,Time,Notes,Rd")] TimeLog timeLog)
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