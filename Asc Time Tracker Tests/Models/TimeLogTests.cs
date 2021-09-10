using Asc_Time_Tracker.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Asc_Time_Tracker_Tests.Models
{
    [TestClass]
    public class TimeLogTests
    {
        [TestMethod]
        public void JobNumToRgbTest()
        {
            // Arrange
            string jobNumA1 = "TEST JOB NUM";
            string jobNumA2 = "TEST JOB NUM";
            string jobNumB = "TEST JOB NUM 2";
            string jobNumEmpty = "";

            // Act
            string rgbA1 = TimeLog.JobNumToRgb(jobNumA1);
            string rgbA2 = TimeLog.JobNumToRgb(jobNumA2);
            string rgbB = TimeLog.JobNumToRgb(jobNumB);
            string rgbNull = TimeLog.JobNumToRgb(null);
            string rgbEmpty = TimeLog.JobNumToRgb(jobNumEmpty);

            // Assert
            Assert.AreEqual(rgbA1, rgbA2);
            Assert.AreNotEqual(rgbA1, rgbB);
            Assert.AreEqual(rgbNull, "#FFFFFF");
            Assert.AreEqual(rgbEmpty, "#FFFFFF");
        }

        [TestMethod]
        [DataRow(0, 0, 0)]
        [DataRow(0, 30, 1800)]
        [DataRow(1, 0, 3600)]
        [DataRow(2, 15, 8100)]
        [DataRow(-2, 30, -5400)]
        public void HoursAndMinutesToSecondsTest(int hours, int minutes, int seconds)
        {
            Assert.AreEqual(TimeLog.HoursAndMinutesToSeconds(hours, minutes), seconds);
        }

        [TestMethod]
        [DataRow(30, "")]
        [DataRow(60, "1 minute")]
        [DataRow(420, "7 minutes")]
        [DataRow(437, "7 minutes")]
        [DataRow(437.12345, "7 minutes")]
        [DataRow(3600, "1 hour")]
        [DataRow(3660, "1 hour, 1 minute")]
        [DataRow(6300, "1 hour, 45 minutes")]
        [DataRow(46800, "13 hours")]
        [DataRow(47535, "13 hours, 12 minutes")]
        [DataRow(0, "")]
        [DataRow(-123.456, "")]
        public void SecondsToHoursAndMinutesStringTest(double seconds, string result)
        {
            Assert.AreEqual(TimeLog.SecondsToHoursAndMinutesString(seconds), result);
        }
    }
}