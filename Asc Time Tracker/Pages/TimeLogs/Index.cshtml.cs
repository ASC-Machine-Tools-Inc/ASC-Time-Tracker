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

        public IList<TimeLog> TimeLog { get; set; }

        public async Task OnGetAsync()
        {
            IQueryable<TimeLog> timeLog = from log in _context.TimeLog select log;

            timeLog = timeLog.OrderByDescending(log => log.DATE);

            TimeLog = await timeLog.ToListAsync();
        }
    }
}