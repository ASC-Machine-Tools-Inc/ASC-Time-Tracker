using Asc_Time_Tracker.Models.TimeLog;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Asc_Time_Tracker_Tests.Models
{
    [TestClass]
    public class TimeLogTests
    {
        [TestMethod]
        [DataRow("TEST JOB NUM", "TEST JOB NUM")]
        [DataRow("Super duper long string! I7I`&N/{Dl^Ao}2x9)$*h", "Super duper long string! I7I`&N/{Dl^Ao}2x9)$*h")]
        [DataRow("29685.000", "29685.000")]  // Cover hash adjusting if negative
        [DataRow("s", "s")]
        public void JobNumToRgbTest(string jobNum1, string jobNum2)
        {
            // Testing for consistent results - should return same color for same input.
            Assert.AreEqual(
                TimeLog.JobNumToRgb(jobNum1),
                TimeLog.JobNumToRgb(jobNum2));
        }

        [TestMethod]
        [DataRow(null, "#FFFFFF")]
        [DataRow("", "#FFFFFF")]
        [DataRow(null, "#CCC", "#CCC")]
        [DataRow("", "#000", "#000")]
        public void JobNumToRgbNullOrEmptyTest(string jobNum, string color, string fallback = "#FFFFFF")
        {
            Assert.AreEqual(TimeLog.JobNumToRgb(jobNum, fallback), color);
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