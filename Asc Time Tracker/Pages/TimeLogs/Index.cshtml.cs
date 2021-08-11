using Asc_Time_Tracker.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Asc_Time_Tracker.Pages.TimeLogs
{
    public class IndexModel : PageModel
    {
        private readonly Data.Asc_Time_TrackerContext _context;

        public IndexModel(Data.Asc_Time_TrackerContext context)
        {
            _context = context;
        }

        public DateTime? CurrentStartDate { get; set; }

        public DateTime? CurrentEndDate { get; set; }

        public IList<TimeLog> TimeLogs { get; set; }

        [BindProperty]
        public TimeLog TimeLog { get; set; }

        public async Task<ActionResult> Index()
        {
            await OnGetAsync();
            return ActionResult(TimeLogs);
        }

        public async Task OnGetAsync(DateTime? startDate, DateTime? endDate)
        {
            if (startDate == null || endDate == null)
            {
                startDate = CurrentStartDate;
                endDate = CurrentEndDate;
            }
            CurrentStartDate = startDate;
            CurrentEndDate = endDate;

            IQueryable<TimeLog> timeLogs = from log in _context.TimeLog select log;

            // Filter by date.
            timeLogs = timeLogs.Where(logs =>
                logs.DATE >= startDate &&
                logs.DATE <= endDate);

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
                await OnGetAsync(CurrentStartDate, CurrentEndDate);
            }
        }
    }
}