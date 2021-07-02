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

        public IList<TimeLog> TimeLogs { get; set; }

        [BindProperty]
        public TimeLog TimeLog { get; set; }

        public async Task OnGetAsync()
        {
            IQueryable<TimeLog> timeLogs = from log in _context.TimeLog select log;

            timeLogs = timeLogs.OrderByDescending(log => log.DATE);

            TimeLogs = await timeLogs.ToListAsync();
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
                await OnGetAsync();
            }
        }
    }
}