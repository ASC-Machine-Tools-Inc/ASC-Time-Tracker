using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using NUglify.Helpers;

namespace Asc_Time_Tracker.Models
{
    public class IndexViewModel
    {
        public TimeLog TimeLog { get; set; }

        public IQueryable<TimeLog> TimeLogs { get; set; }

        public SelectList Categories { get; set; }

        public IndexViewModel(IQueryable<TimeLog> timeLogs)
        {
            TimeLogs = timeLogs;
        }

        /// <summary>
        /// Take an IQueryable of TimeLogs and filter them based
        /// on the various given criteria.
        /// </summary>
        public static IQueryable<TimeLog> FilterTimeLogs(
            IQueryable<TimeLog> timeLogs,
            IEnumerable<string> empIds,
            DateTime? startDate,
            DateTime? endDate,
            string category,
            string jobNum,
            string notes,
            bool rd)
        {
            timeLogs = FilterTimeLogsByEmpIds(timeLogs, empIds);
            timeLogs = FilterTimeLogsByDate(timeLogs, startDate, endDate);
            if (category != "All")
            {
                timeLogs = FilterTimeLogsByCategory(timeLogs, category);
            }

            if (!jobNum.IsNullOrWhiteSpace())
            {
                timeLogs = FilterTimeLogsByJobNumber(timeLogs, jobNum);
            }

            if (!notes.IsNullOrWhiteSpace())
            {
                timeLogs = FilterTimeLogsByNotes(timeLogs, notes);
            }

            // Show all time logs if false, else show only those flagged.
            if (rd)
            {
                timeLogs = FilterTimeLogsRd(timeLogs);
            }

            return timeLogs;
        }

        /// <summary>
        /// Take an IQueryable of TimeLogs and return the ones
        /// with the given employee id (usually their email).
        /// </summary>
        public static IQueryable<TimeLog> FilterTimeLogsByEmpIds(
            IQueryable<TimeLog> timeLogs,
            IEnumerable<string> empIds)
        {
            if (empIds.Contains("all"))
            {
                return timeLogs;
            }

            return timeLogs.Where(t => empIds.Contains(t.EmpId));
        }

        /// <summary>
        /// Take an IQueryable of TimeLogs and return the ones
        /// within the given time frame.
        /// </summary>
        public static IQueryable<TimeLog> FilterTimeLogsByDate(
            IQueryable<TimeLog> timeLogs,
            DateTime? startDate,
            DateTime? endDate)
        {
            if (startDate == null || endDate == null)
            {
                return Enumerable.Empty<TimeLog>().AsQueryable();
            }
            // Set hours back to 0 (since Javascript sends them through with timezone adjustment).
            DateTime convStart = (DateTime)startDate;
            DateTime convEnd = (DateTime)endDate;
            startDate = new DateTime(convStart.Year, convStart.Month, convStart.Day,
                0, 0, 0);
            endDate = new DateTime(convEnd.Year, convEnd.Month, convEnd.Day,
                0, 0, 0);

            // Filter by date.
            return timeLogs
                .Where(t => t.Date >= startDate && t.Date < endDate)
                .OrderBy(t => t.Date);
        }

        public static IQueryable<TimeLog> FilterTimeLogsByCategory(
            IQueryable<TimeLog> timeLogs,
            string category)
        {
            return timeLogs.Where(t => t.Category.Equals(category));
        }

        /// <summary>
        /// Take an IQueryable of TimeLogs and return the ones
        /// matching the given job number, ignoring case.
        /// </summary>
        public static IQueryable<TimeLog> FilterTimeLogsByJobNumber(
            IQueryable<TimeLog> timeLogs,
            string jobNum)
        {
            return timeLogs.Where(t => t.JobNum.Equals(jobNum));
        }

        /// <summary>
        /// Take an IQueryable of TimeLogs and return the ones
        /// containing the given notes, ignoring case.
        /// </summary>
        public static IQueryable<TimeLog> FilterTimeLogsByNotes(
            IQueryable<TimeLog> timeLogs,
            string notes)
        {
            return timeLogs.Where(t => t.Notes.Contains(notes));
        }

        /// <summary>
        /// Take an IQueryable of TimeLogs and return the ones
        /// where the research and design flag is true.
        /// </summary>
        public static IQueryable<TimeLog> FilterTimeLogsRd(IQueryable<TimeLog> timeLogs)
        {
            return timeLogs.Where(t => t.Rd);
        }

        /// <summary>
        /// Take an IQueryable of TimeLogs and return them in sorted order of
        /// time spent, descending, up to the given limit.
        /// </summary>
        public static IQueryable<TimeLog> TakeTopXTimeLogs(IQueryable<TimeLog> timeLogs, int limit)
        {
            return timeLogs.GroupBy(t => t.JobNum)
                .Select(tg => new TimeLog
                {
                    JobNum = tg.Key,
                    Time = tg.Sum(t => t.Time)
                })
                .Where(t => t.JobNum != null)  // Skip null job num logs
                .OrderByDescending(t => t.Time)
                .Take(limit);
        }
    }
}