using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Asc_Time_Tracker.Data;
using Asc_Time_Tracker.Models;

namespace Asc_Time_Tracker.Pages.TimeLogs
{
    public class IndexModel : PageModel
    {
        private readonly Asc_Time_Tracker.Data.Asc_Time_TrackerContext _context;

        public IndexModel(Asc_Time_Tracker.Data.Asc_Time_TrackerContext context)
        {
            _context = context;
        }

        public string DateSort { get; set; }
        public string JobSort { get; set; }
        public string RDSort { get; set; }
        public string CurrentFilter { get; set; }
        public string CurrentSort { get; set; }

        public IList<TimeLog> TimeLogs { get; set; }

        [BindProperty]
        public TimeLog TimeLog { get; set; }

        public async Task OnGetAsync(string sortOrder)
        {
            CurrentSort = sortOrder;
            DateSort = String.IsNullOrEmpty(sortOrder) ? "date_asc" : "";
            JobSort = sortOrder == "job_desc" ? "job_asc" : "job_desc";
            RDSort = sortOrder == "rd_desc" ? "" : "rd_desc";

            IQueryable<TimeLog> timeLogs = from log in _context.TimeLog select log;

            switch (sortOrder)
            {
                case "job_asc":
                    timeLogs = timeLogs.OrderBy(log => log.JOBNUM);
                    break;

                case "job_desc":
                    timeLogs = timeLogs.OrderByDescending(log => log.JOBNUM);
                    break;

                case "rd_desc":
                    timeLogs = timeLogs.OrderByDescending(log => log.RD);
                    break;

                case "date_asc":
                    timeLogs = timeLogs.OrderBy(log => log.DATE);
                    break;

                default:
                    timeLogs = timeLogs.OrderByDescending(log => log.DATE);
                    break;
            }

            TimeLogs = await timeLogs.AsNoTracking().ToListAsync();
        }

        public async Task OnPostDeleteAsync()
        {
            // Grab the ID to delete from our hidden input field in the modal.
            TimeLog = await _context.TimeLog.FindAsync(Int32.Parse(Request.Form["TimeLog.ID"]));

            if (TimeLog != null)
            {
                // Important: need to get model again after removing and saving changes
                // to display correctly.
                _context.TimeLog.Remove(TimeLog);
                await _context.SaveChangesAsync();
                await OnGetAsync(CurrentSort);
            }
        }
    }
}