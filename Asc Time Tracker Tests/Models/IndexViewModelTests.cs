using Asc_Time_Tracker.Data;
using Asc_Time_Tracker.Models;
using Asc_Time_Tracker.Models.TimeLog;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Asc_Time_Tracker_Tests.Models
{
    [TestClass]
    public class IndexViewModelTests
    {
        // Basic model construction and assignment testing for coverage.
        [TestMethod]
        public void IndexViewModelTest()
        {
            // Arrange
            IQueryable<TimeLog> timeLogs = ApplicationDbContext.SeedTimeLogs(250).AsQueryable();
            IndexViewModel model = new(timeLogs);

            // Act
            TimeLog timeLog = model.TimeLogs.First();
            model.TimeLog = timeLog;

            // Assert
            Assert.AreEqual(model.TimeLogs.Count(), 250);
            Assert.AreEqual(model.TimeLog.Id, 1);
            Assert.IsTrue(model.Categories.Any());
        }

        [TestMethod]
        public void FilterTimeLogsTest()
        {
            // Arrange
            List<TimeLog> timeLogsList = ApplicationDbContext.SeedTimeLogs(250);

            for (int i = 0; i < 10; i++)
            {
                timeLogsList.Add(new TimeLog
                {
                    Id = timeLogsList.Count + 1,
                    EmpId = "TEST USER ID",
                    Category = "TEST CATEGORY",
                    Date = DateTime.Today,
                    JobNum = "TEST JOB NUM",
                    Notes = "TEST NOTES",
                    Rd = true,
                    Time = 3600
                });
            }

            IQueryable<TimeLog> timeLogs = timeLogsList.AsQueryable();

            // Act
            IQueryable<TimeLog> filteredTimeLogs1 =
                IndexViewModel.FilterTimeLogs(
                    timeLogs,
                    new List<string> { "TEST USER ID" },
                    DateTime.Today,
                    DateTime.Today.AddDays(1),
                    "All",
                    "TEST JOB NUM",
                    "Test",
                    true
                );
            IQueryable<TimeLog> filteredTimeLogs2 =
                IndexViewModel.FilterTimeLogs(
                    timeLogs,
                    new List<string> { "all" },
                    DateTime.Today,
                    DateTime.Today.AddDays(1),
                    "TEST CATEGORY",
                    null,
                    null,
                    true
                );

            // Assert
            Assert.AreEqual(filteredTimeLogs1.Count(), 10);
            Assert.AreEqual(filteredTimeLogs2.Count(), 10);
        }

        [TestMethod]
        public void FilterTimeLogsByEmpIdTest()
        {
            // Arrange
            List<TimeLog> timeLogsList = ApplicationDbContext.SeedTimeLogs(250);

            for (int i = 0; i < 10; i++)
            {
                timeLogsList.Add(new TimeLog
                {
                    Id = timeLogsList.Count + 1,
                    EmpId = "TEST USER ID",
                    Category = "TEST CATEGORY",
                    Date = DateTime.Today,
                    JobNum = "TEST JOB NUM",
                    Notes = "TEST NOTES",
                    Rd = false,
                    Time = 3600
                });
            }
            IQueryable<TimeLog> timeLogs = timeLogsList.AsQueryable();

            // Act
            IQueryable<TimeLog> filteredTimeLogs1 = IndexViewModel.FilterTimeLogsByEmpIds(timeLogs, new List<string> { "timeuser@ascmt.com" });
            IQueryable<TimeLog> filteredTimeLogs2 = IndexViewModel.FilterTimeLogsByEmpIds(timeLogs, new List<string> { "TEST USER ID" });
            IQueryable<TimeLog> emptyTimeLogs = IndexViewModel.FilterTimeLogsByEmpIds(timeLogs, new List<string> { "NONEXISTENT EMP ID" });

            // Assert
            Assert.AreEqual(filteredTimeLogs1.Count(), 250);
            Assert.AreEqual(filteredTimeLogs2.Count(), 10);
            Assert.AreEqual(emptyTimeLogs.Count(), 0);
        }

        [TestMethod]
        public void FilterTimeLogsByDateTest()
        {
            // Arrange
            IQueryable<TimeLog> timeLogs = ApplicationDbContext.SeedTimeLogs(250).AsQueryable();

            // Act
            IQueryable<TimeLog> filteredTimeLogsDay =
                IndexViewModel.FilterTimeLogsByDate(timeLogs, DateTime.Today.AddDays(-1), DateTime.Today);
            IQueryable<TimeLog> filteredTimeLogsWeek =
                IndexViewModel.FilterTimeLogsByDate(timeLogs, DateTime.Today.AddDays(-7), DateTime.Today);
            IQueryable<TimeLog> filteredTimeLogsMonth =
                IndexViewModel.FilterTimeLogsByDate(timeLogs, DateTime.Today.AddMonths(-1), DateTime.Today);
            IQueryable<TimeLog> filteredTimeLogsAll =
                IndexViewModel.FilterTimeLogsByDate(timeLogs, DateTime.UnixEpoch, DateTime.Today.AddDays(1));
            IQueryable<TimeLog> filteredTimeLogsNone =
                IndexViewModel.FilterTimeLogsByDate(timeLogs, DateTime.Today, DateTime.Today.AddDays(-1));
            IQueryable<TimeLog> filteredTimeLogsNull =
                IndexViewModel.FilterTimeLogsByDate(timeLogs, DateTime.Today, null);

            // Assert
            Assert.AreEqual(filteredTimeLogsDay.Count(), 5);
            Assert.AreEqual(filteredTimeLogsWeek.Count(), 37);
            Assert.AreEqual(filteredTimeLogsMonth.Count(), 149);
            Assert.AreEqual(filteredTimeLogsAll.Count(), 250);
            Assert.AreEqual(filteredTimeLogsNone.Count(), 0);
            Assert.AreEqual(filteredTimeLogsNull.Count(), 0);
        }

        // TODO: additional tests for other methods? FilterTimeLogsTest might be enough

        [TestMethod]
        public void TakeTopXTimeLogsTest()
        {
            // Arrange
            IQueryable<TimeLog> timeLogs = ApplicationDbContext.SeedTimeLogs(250).AsQueryable();

            // Act
            IQueryable<TimeLog> filteredTimeLogsTopThree = IndexViewModel.TakeTopXTimeLogs(timeLogs, 3);
            IQueryable<TimeLog> filteredTimeLogsGreaterThanCount = IndexViewModel.TakeTopXTimeLogs(timeLogs, 300);

            // Assert
            Assert.AreEqual(filteredTimeLogsTopThree.Count(), 3);
            // Count can actually be less than 250 due to grouping of job numbers.
            Assert.AreEqual(filteredTimeLogsGreaterThanCount.Count(), 243);
        }
    }
}