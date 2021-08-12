using System;
using System.Threading.Tasks;
using Asc_Time_Tracker.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Asc_Time_Tracker.Views.Home
{
    public class DashboardModel : PageModel
    {
        private readonly Data.ApplicationDbContext _context;

        public DashboardModel(Data.ApplicationDbContext context)
        {
            _context = context;
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
            TimeLog.Time = hours * 3600 + minutes * 60;

            _context.TimeLog.Add(TimeLog);
            await _context.SaveChangesAsync();

            return RedirectToPage("TimeLogs/Index");
        }
    }
}