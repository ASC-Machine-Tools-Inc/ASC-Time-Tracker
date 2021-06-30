using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Asc_Time_Tracker.Pages.TimeLogs;

namespace Asc_Time_Tracker.Pages
{
    public class DashboardModel : PageModel
    {
        // Needed for the modals
        public _CreateModel _CreateModel { get; set; }

        private readonly ILogger<DashboardModel> _logger;

        public DashboardModel(ILogger<DashboardModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
        }
    }
}