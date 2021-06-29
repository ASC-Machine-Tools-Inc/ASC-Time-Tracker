using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Asc_Time_Tracker.Models;

namespace Asc_Time_Tracker.Data
{
    public class Asc_Time_TrackerContext : DbContext
    {
        public Asc_Time_TrackerContext (DbContextOptions<Asc_Time_TrackerContext> options)
            : base(options)
        {
        }

        public DbSet<Asc_Time_Tracker.Models.TimeLog> TimeLog { get; set; }
    }
}
