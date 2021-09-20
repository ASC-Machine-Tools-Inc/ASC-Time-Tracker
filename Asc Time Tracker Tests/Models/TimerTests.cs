using System;
using Asc_Time_Tracker.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Asc_Time_Tracker_Tests.Models
{
    [TestClass]
    public class TimerTests
    {
        [TestMethod]
        public void TimerTest()
        {
            // Arrange
            int timerId = 3;

            // Act
            Timer timer = new(timerId);

            // Assert
            Assert.AreEqual(timer.Id, timerId);
        }
    }
}