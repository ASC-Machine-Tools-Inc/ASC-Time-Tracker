using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Asc_Time_Tracker.Data;
using Asc_Time_Tracker.Models;

namespace Asc_Time_Tracker.Controllers
{
    public class TimeLogController : Controller
    {
        private readonly ApplicationDbContext _context;

        public IndexViewModel IndexViewModel { get; set; }

        public DateTime? CurrentStartDate { get; set; }

        public DateTime? CurrentEndDate { get; set; }

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
            if (startDate == null || endDate == null)
            {
                startDate = CurrentStartDate;
                endDate = CurrentEndDate;
            }
            else
            {
                // Set hours back to 0 (since Javascript sends them through with timezone adjustment).
                DateTime convStart = (DateTime)startDate;
                DateTime convEnd = (DateTime)endDate;
                startDate = new DateTime(convStart.Year, convStart.Month, convStart.Day, 0, 0, 0);
                endDate = new DateTime(convEnd.Year, convEnd.Month, convEnd.Day, 0, 0, 0);
            }
            CurrentStartDate = startDate;
            CurrentEndDate = endDate;

            IQueryable<TimeLog> timeLogs = from log in _context.TimeLog select log;

            // Filter by date.
            timeLogs = timeLogs.Where(logs =>
                logs.Date >= startDate &&
                logs.Date < endDate);

            return PartialView(await timeLogs.ToListAsync());
        }

        // GET: TimeLog/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var timeLog = await _context.TimeLog
                .FirstOrDefaultAsync(m => m.Id == id);
            if (timeLog == null)
            {
                return NotFound();
            }

            return View(timeLog);
        }

        // GET: TimeLog/Create
        public IActionResult Create()
        {
            return View();
        }

        // Handles modal submission.
        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            // Convert input to seconds.
            int hours = (string.IsNullOrEmpty(Request.Form["timeHours"])) ? 0 : Convert.ToInt32(Request.Form["timeHours"]);
            int minutes = (string.IsNullOrEmpty(Request.Form["timeMinutes"])) ? 0 : Convert.ToInt32(Request.Form["timeMinutes"]);

            // Convert input to seconds.
            TimeLog.Time = hours * 3600 + minutes * 60;

            _context.TimeLog.Add(TimeLog);
            await _context.SaveChangesAsync();

            return RedirectToPage("TimeLogs/Index");
        }

        // POST: TimeLog/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,EmpId,JobNum,Date,Time,Notes,Rd")] TimeLog timeLog)
        {
            if (ModelState.IsValid)
            {
                _context.Add(timeLog);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(timeLog);
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
                return RedirectToAction(nameof(Index));
            }
            return View(timeLog);
        }

        // GET: TimeLog/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var timeLog = await _context.TimeLog
                .FirstOrDefaultAsync(m => m.Id == id);
            if (timeLog == null)
            {
                return NotFound();
            }

            return View(timeLog);
        }

        // POST: TimeLog/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var timeLog = await _context.TimeLog.FindAsync(id);
            _context.TimeLog.Remove(timeLog);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TimeLogExists(int id)
        {
            return _context.TimeLog.Any(e => e.Id == id);
        }
    }
}