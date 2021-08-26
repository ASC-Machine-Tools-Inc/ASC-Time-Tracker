using System;
using System.Collections.Generic;
using System.Linq;
using Asc_Time_Tracker.Controllers;
using Asc_Time_Tracker.Data;
using Asc_Time_Tracker.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Asc_Time_Tracker_Tests.Models
{
    [TestClass]
    public class IndexViewModelTests
    {
        [TestMethod]
        public void FilterTimeLogsByEmpIdTest()
        {
            // Arrange
            IndexViewModel model = new IndexViewModel(ApplicationDbContext.SeedTimeLogs(250).AsQueryable());

            // Act
            IQueryable<TimeLog> filteredTimeLogs = model.FilterTimeLogsByEmpId(model.TimeLogs, "TEST EMP ID");

            // Assert
            Assert.AreEqual(filteredTimeLogs.Count(), 10);
        }

        [TestMethod]
        public void FilterTimeLogsByDateTest()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void TakeTopXTimeLogsTest()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void GenerateTopXPieChartTest()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void GenerateWeekBarChartTest()
        {
            throw new NotImplementedException();
        }
    }
}