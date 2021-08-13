using System;
using System.Collections.Generic;
using System.Linq;

namespace Asc_Time_Tracker.Models
{
    public class IndexViewModel
    {
        public TimeLog TimeLog;
        public IEnumerable<TimeLog> TimeLogs { get; set; }

        public IQueryable<TimeLog> FilterTimeLogsByDate(IQueryable<TimeLog> timeLogs,
            DateTime? startDate, DateTime? endDate)
        {
            if (startDate == null || endDate == null)
            {
                return null;
            }
            // Set hours back to 0 (since Javascript sends them through with timezone adjustment).
            DateTime convStart = (DateTime)startDate;
            DateTime convEnd = (DateTime)endDate;
            startDate = new DateTime(convStart.Year, convStart.Month, convStart.Day,
                0, 0, 0);
            endDate = new DateTime(convEnd.Year, convEnd.Month, convEnd.Day,
                0, 0, 0);

            // Filter by date.
            timeLogs = timeLogs.Where(logs =>
                logs.Date >= startDate &&
                logs.Date < endDate);

            return timeLogs;
        }
    }
}