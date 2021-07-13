using Microsoft.EntityFrameworkCore;

namespace Asc_Time_Tracker.Data
{
    public class Asc_Time_TrackerContext : DbContext
    {
        public Asc_Time_TrackerContext(DbContextOptions<Asc_Time_TrackerContext> options)
            : base(options)
        {
        }

        public DbSet<Asc_Time_Tracker.Models.TimeLog> TimeLog { get; set; }
    }
}