using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Asc_Time_Tracker.Models;

namespace Asc_Time_Tracker.Pages
{
    public class DashboardModel : PageModel
    {
        private readonly Data.Asc_Time_TrackerContext _context;
        private readonly ILogger<DashboardModel> _logger;

        public DashboardModel(Data.Asc_Time_TrackerContext context, ILogger<DashboardModel> logger)
        {
            _context = context;
            _logger = logger;
        }

        public void OnGet()
        {
        }

        [BindProperty]
        public TimeLog TimeLog { get; set; }

        // Handles modal submission.
        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Convert input to seconds.
            int hours = (string.IsNullOrEmpty(Request.Form["timeHours"])) ? 0 : Convert.ToInt32(Request.Form["timeHours"]);
            int minutes = (string.IsNullOrEmpty(Request.Form["timeMinutes"])) ? 0 : Convert.ToInt32(Request.Form["timeMinutes"]);

            // Convert input to seconds.
            TimeLog.TIME = hours * 3600 + minutes * 60;

            _context.TimeLog.Add(TimeLog);
            await _context.SaveChangesAsync();

            return RedirectToPage("TimeLogs/Index");
        }
    }
}