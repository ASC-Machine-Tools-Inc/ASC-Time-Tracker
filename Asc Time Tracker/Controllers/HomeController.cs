using Asc_Time_Tracker.Models;
using Asc_Time_Tracker.Models.Shared;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Asc_Time_Tracker.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult About()
        {
            return View(new IndexViewModel(null));
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}