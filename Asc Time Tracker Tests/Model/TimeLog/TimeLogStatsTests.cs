using System.Linq;
using Asc_Time_Tracker.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Asc_Time_Tracker.Models.TimeLog
{
    [TestClass]
    public class TimeLogStatsTests
    {
        [TestMethod]
        public void TimeLogStatsTest()
        {
            // Arrange
            int timeLogsCount = 250;
            int pieCount = 10;
            IQueryable<TimeLog> timeLogs = ApplicationDbContext.SeedTimeLogs(timeLogsCount).AsQueryable();

            // Act
            TimeLogStats stats = new(timeLogs, pieCount);

            // Assert
            Assert.AreEqual(stats.TimeSpentPieChart.Data.Datasets[0].Data.Count, pieCount);
        }

        [TestMethod]
        public void TimeLogStatsFewLogsTest()
        {
            // Arrange
            int timeLogsCount = 5;
            int pieCount = 10;
            IQueryable<TimeLog> timeLogs = ApplicationDbContext.SeedTimeLogs(timeLogsCount).AsQueryable();

            // Act
            TimeLogStats stats = new(timeLogs, pieCount);

            // Assert
            Assert.AreEqual(stats.TimeSpentPieChart.Data.Datasets[0].Data.Count, timeLogsCount);
        }

        [TestMethod]
        public void TimeLogStatsSingleLogTest()
        {
            // Arrange
            int timeLogsCount = 1;
            int pieCount = 10;
            IQueryable<TimeLog> timeLogs = ApplicationDbContext.SeedTimeLogs(timeLogsCount).AsQueryable();

            // Act
            TimeLogStats stats = new(timeLogs, pieCount);

            // Assert
            Assert.AreEqual(stats.TotalTimeSpent, TimeLog.SecondsToHoursAndMinutesString(timeLogs.First().Time));
            Assert.AreEqual(stats.TopJobNum, timeLogs.First().JobNum);
            Assert.AreEqual(stats.TopJobNumColor, TimeLog.JobNumToRgb(timeLogs.First().JobNum));
            Assert.AreEqual(stats.TopTime, TimeLog.SecondsToHoursAndMinutesString(timeLogs.First().Time));
            Assert.AreEqual(stats.TimeSpentPieChart.Data.Datasets[0].Data.Count, timeLogsCount);
        }

        [TestMethod]
        public void TimeLogStatsNoLogsTest()
        {
            // Arrange
            int timeLogsCount = 0;
            int pieCount = 10;
            IQueryable<TimeLog> timeLogs = ApplicationDbContext.SeedTimeLogs(timeLogsCount).AsQueryable();

            // Act
            TimeLogStats stats = new(timeLogs, pieCount);

            // Assert
            Assert.AreEqual(stats.TotalTimeSpent, null);
            Assert.AreEqual(stats.TopJobNum, null);
            Assert.AreEqual(stats.TopJobNumColor, null);
            Assert.AreEqual(stats.TopTime, null);
            Assert.AreEqual(stats.TimeSpentPieChart, null);
        }
    }
}