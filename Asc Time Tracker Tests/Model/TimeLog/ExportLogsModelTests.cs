using System;
using System.Collections.Generic;
using System.Linq;
using Asc_Time_Tracker.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Asc_Time_Tracker.Models.TimeLog
{
    [TestClass]
    public class ExportLogsModelTests
    {
        [TestMethod]
        public void ExportTest()
        {
            // Arrange
            IQueryable<TimeLog> timeLogs = ApplicationDbContext.SeedTimeLogs(250).AsQueryable();
            // Test adding a log with empty fields to the table.
            timeLogs.First().Time = 0;
            timeLogs.First().JobNum = null;
            timeLogs.First().Notes = null;

            // Act
            byte[] results = new ExportLogsModel(
                timeLogs,
                new List<string> { "all" },
                DateTime.Today.AddDays(-10),
                DateTime.Today.AddDays(1),
                "All",
                "",
                "",
                false).Export();
            byte[] resultsToday = new ExportLogsModel(
                timeLogs,
                new List<string> { "all" },
                DateTime.Today,
                DateTime.Today.AddDays(1),
                "Software Development",
                "12345",
                "",
                false).Export();
            byte[] resultsAll = new ExportLogsModel(
                timeLogs,
                new List<string> { "testuser@ascmt.com" },
                DateTime.UnixEpoch,
                DateTime.Today.AddDays(1),
                "All",
                "TEST JOB NUMBER",
                "TEST NOTES",
                true).Export();

            // Assert
            Assert.IsTrue(results.Length > 0);
        }
    }
}