using System;
using System.Collections.Generic;
using Asc_Time_Tracker.Models;
using Bogus;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Asc_Time_Tracker.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<TimeLog> TimeLog { get; set; }

        // Seed our database with some time logs for testing.
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            Random rnd = new();
            const int logsToAdd = 250;

            // Amount of hours to generate logs for up to daily.
            const int dailyHoursLimit = 8;

            // Number of days we subtract from the current day (working back as we add logs).
            int daysOffset = 0;

            // Count of hours in logs for each day.
            int dailyHours = 0;

            // Seed our data.
            var timeLogs = new List<TimeLog>();

            for (int i = 1; i <= logsToAdd; i++)
            {
                int rndTimeHours = rnd.Next(1, Math.Min(4, dailyHoursLimit - dailyHours + 1));
                int rndJobNum = rnd.Next(20000, 30000);
                var faker = new Faker();

                TimeLog timeLog = new()
                {
                    Id = i,
                    EmpId = "timeuser@ascmt.com",
                    Date = DateTime.Today.AddDays(daysOffset),
                    Time = rndTimeHours * 3600,  // Convert to seconds
                    JobNum = rndJobNum.ToString(),
                    Notes = faker.Hacker.Phrase()
                };

                timeLogs.Add(timeLog);

                // If we reach our daily limit, go back a day and add logs to that.
                dailyHours += rndTimeHours;
                if (dailyHours == dailyHoursLimit)
                {
                    dailyHours = 0;
                    daysOffset -= 1;
                }
            }

            builder.Entity<TimeLog>().HasData(timeLogs);
        }
    }
}