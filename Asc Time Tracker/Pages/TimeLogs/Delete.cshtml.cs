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
    public class DeleteModel : PageModel
    {
        private readonly Asc_Time_Tracker.Data.Asc_Time_TrackerContext _context;

        public DeleteModel(Asc_Time_Tracker.Data.Asc_Time_TrackerContext context)
        {
            _context = context;
        }

        [BindProperty]
        public TimeLog TimeLog { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            TimeLog = await _context.TimeLog.FirstOrDefaultAsync(m => m.ID == id);

            if (TimeLog == null)
            {
                return NotFound();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            TimeLog = await _context.TimeLog.FindAsync(id);

            if (TimeLog != null)
            {
                _context.TimeLog.Remove(TimeLog);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
