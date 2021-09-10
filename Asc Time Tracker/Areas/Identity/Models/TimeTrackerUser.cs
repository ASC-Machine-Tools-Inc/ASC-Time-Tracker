using Microsoft.AspNetCore.Identity;
using System;

namespace Asc_Time_Tracker.Areas.Identity.Models
{
    public class TimeTrackerUser : IdentityUser
    {
        public DateTime? LastLoginDate { get; set; }
    }
}