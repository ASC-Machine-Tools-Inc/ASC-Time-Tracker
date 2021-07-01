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

            // Convert hours to seconds before submitting
            TimeLog.TIME *= 3600;

            _context.TimeLog.Add(TimeLog);
            await _context.SaveChangesAsync();

            return RedirectToPage("TimeLogs/Index");
        }
    }
}