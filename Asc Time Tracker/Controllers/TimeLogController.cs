using Asc_Time_Tracker.Data;
using Asc_Time_Tracker.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using Newtonsoft.Json;

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
            DateTime startDate,
            DateTime endDate,
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
            List<string> empIds,
            DateTime startDate,
            DateTime endDate,
            string category,
            string jobNum,
            string notes,
            bool rd,
            int pieCount)
        {
            IQueryable<TimeLog> timeLogs = IndexViewModel.TimeLogs;
            timeLogs = IndexViewModel.FilterTimeLogs(
                timeLogs, empIds, startDate, endDate,
                category, jobNum, notes, rd);

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

        // POST: TimeLog/Export
        [HttpPost]
        public ActionResult Export(
            List<string> empIds,
            DateTime startDate,
            DateTime endDate,
            string category,
            string jobNum,
            string notes,
            bool rd)
        {
            using MemoryStream stream = new MemoryStream();
            PdfWriter writer = new(stream);
            PdfDocument pdf = new(writer);
            Document document = new(pdf);

            Paragraph header =
                new Paragraph()
                .SetFontSize(24)
                .SetTextAlignment(TextAlignment.CENTER);
            header.Add("Time Log Report\n");

            string startDateString = startDate.ToShortDateString();
            // Subtract one day since end date is always set to midnight of the next.
            string endDateString = endDate.AddDays(-1).ToShortDateString();

            header.Add("Timeframe: ");
            if (startDateString.Equals("1/1/1970"))
            {  // Epoch, filtering for all
                header.Add("All logs");
            }
            else if (startDateString.Equals(endDateString))  // Same date, filtering by day
            {
                header.Add(startDateString);
            }
            else
            {
                header.Add(startDateString + " - " + endDateString);
            }

            header.Add("\n");

            // TODO: dynamically adjust table side and which fields shown based on parameters
            // TODO: convert filters to list display
            header.Add("Filters:" + string.Join(", ", empIds) + ", " + category + ", " + jobNum + ", " + notes + ", " + rd);

            document.Add(header);

            // TODO: Filtering by multiple employee ids not working - look into this?
            IQueryable<TimeLog> timeLogs = IndexViewModel.TimeLogs;
            timeLogs = IndexViewModel.FilterTimeLogs(
                timeLogs, empIds, startDate, endDate,
                category, jobNum, notes, rd);

            // TODO: write current logs and move this all to the model
            int tableWidth = 5;
            Table table = new(UnitValue.CreatePercentArray(tableWidth));

            table.AddHeaderCell(new Paragraph("Date").SetBold());
            table.AddHeaderCell(new Paragraph("Category").SetBold());
            table.AddHeaderCell(new Paragraph("Job Number").SetBold());
            table.AddHeaderCell(new Paragraph("Time").SetBold());
            table.AddHeaderCell(new Paragraph("Employee").SetBold());

            foreach (TimeLog log in timeLogs)
            {
                table.AddCell(log.Date.ToShortDateString());
                table.AddCell(log.Category);
                table.AddCell(log.JobNum);

                // Make the time look nice, by converting from seconds into hours and minutes.
                // TODO: make this model method and replace the one in IndexLogs with it.
                string formattedTime = (Math.Floor((log.Time % 3600) / 60) + "").PadLeft(2, '0');
                formattedTime = Math.Floor(log.Time / 3600) + ":" + formattedTime;

                table.AddCell(formattedTime);

                //table.AddCell(log.Notes);
                table.AddCell(log.EmpId);
            }
            document.Add(table);
            document.Close();

            return File(stream.ToArray(), "application/pdf", "Timelogs.pdf");
        }

        private bool TimeLogExists(int id)
        {
            return _context.TimeLog.Any(e => e.Id == id);
        }
    }
}