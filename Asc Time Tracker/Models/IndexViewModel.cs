using ChartJSCore.Helpers;
using ChartJSCore.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Asc_Time_Tracker.Models
{
    public class IndexViewModel
    {
        public TimeLog TimeLog { get; set; }

        public IQueryable<TimeLog> TimeLogs { get; set; }

        public IndexViewModel(IQueryable<TimeLog> timeLogs)
        {
            TimeLogs = timeLogs;
        }

        /// <summary>
        /// Take an IQueryable of TimeLogs and return the ones with the given
        /// employee id (usually their email).
        /// </summary>
        public void FilterTimeLogsByEmpId(string empId)
        {
            TimeLogs = TimeLogs.Where(t => t.EmpId.Equals(empId));
        }

        /// <summary>
        /// Take an IQueryable of TimeLogs and return the ones within the given time frame.
        /// </summary>
        public void FilterTimeLogsByDate(DateTime? startDate, DateTime? endDate)
        {
            if (startDate == null || endDate == null)
            {
                return;
            }
            // Set hours back to 0 (since Javascript sends them through with timezone adjustment).
            DateTime convStart = (DateTime)startDate;
            DateTime convEnd = (DateTime)endDate;
            startDate = new DateTime(convStart.Year, convStart.Month, convStart.Day,
                0, 0, 0);
            endDate = new DateTime(convEnd.Year, convEnd.Month, convEnd.Day,
                0, 0, 0);

            // Filter by date.
            TimeLogs = TimeLogs.Where(t =>
                t.Date >= startDate &&
                t.Date < endDate);
        }

        /// <summary>
        /// Take an IQueryable of TimeLogs and return them in sorted order of
        /// time spent, descending, up to the given limit x.
        /// </summary>
        public IQueryable<TimeLog> TakeTopXTimeLogs(int x)
        {
            return TimeLogs.GroupBy(t => t.JobNum)
                .Select(tg => new TimeLog
                {
                    JobNum = tg.Key,
                    Time = tg.Sum(t => t.Time)
                })
                .Where(t => t.JobNum != null)  // Skip null job num logs
                .OrderByDescending(t => t.Time)
                .Take(x);
        }

        /// <summary>
        /// Return a pie chart of the top job numbers worked on for this time frame.
        /// </summary>
        public Chart GenerateTopXPieChart(int x)
        {
            // Sort time logs by top (given number) by the most time spent on them.
            IQueryable<TimeLog> timeLogs = TakeTopXTimeLogs(x);

            Chart chart = new()
            {
                Type = Enums.ChartType.Pie
            };

            List<string> labels = new();
            List<double?> time = new();
            List<ChartColor> colors = new();

            foreach (TimeLog timeLog in timeLogs)
            {
                labels.Add(timeLog.JobNum);

                double hours = Math.Round(timeLog.Time / 3600, 2);
                time.Add(hours);

                string hex = TimeLog.JobNumToRgb(timeLog.JobNum);
                colors.Add(ChartColor.FromHexString(hex));
            }

            ChartJSCore.Models.Data data = new()
            {
                Labels = labels
            };

            PieDataset dataset = new()
            {
                BackgroundColor = colors,
                HoverBackgroundColor = colors,
                Data = time
            };

            data.Datasets = new List<Dataset> { dataset };

            chart.Data = data;

            return chart;
        }

        public Chart GenerateWeekBarChart()
        {
            // Filter time logs to this week.
            IQueryable<TimeLog> timeLogs = TakeTopXTimeLogs(5);

            Chart chart = new()
            {
                Type = Enums.ChartType.Bar
            };

            ChartJSCore.Models.Data data = new()
            {
                Labels = new List<string>() { "Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat" }
            };

            /*
            BarDataset dataset = new()
            {
                Label = "# of Hours",
                Data = new List<double?>() { 12, 19, 3, 5, 2, 3 },
                BackgroundColor = new List<ChartColor>
                {
                    ChartColor.FromRgba(255, 99, 132, 0.2),
                    ChartColor.FromRgba(54, 162, 235, 0.2),
                    ChartColor.FromRgba(255, 206, 86, 0.2),
                    ChartColor.FromRgba(75, 192, 192, 0.2),
                    ChartColor.FromRgba(153, 102, 255, 0.2),
                    ChartColor.FromRgba(255, 159, 64, 0.2)
                },
                BorderColor = new List<ChartColor>
                {
                    ChartColor.FromRgb(255, 99, 132),
                    ChartColor.FromRgb(54, 162, 235),
                    ChartColor.FromRgb(255, 206, 86),
                    ChartColor.FromRgb(75, 192, 192),
                    ChartColor.FromRgb(153, 102, 255),
                    ChartColor.FromRgb(255, 159, 64)
                },
                BorderWidth = new List<int>() { 1 }
            }; */

            data.Datasets = new List<Dataset>();

            foreach (TimeLog timeLog in timeLogs)
            {
                Dataset temp = new()
                {
                    Data = new List<double?>() { 12, 19, 3, 5, 2, 3, timeLog.Time / 3600 },
                    Label = timeLog.JobNum
                };

                data.Datasets.Add(temp);
            }

            chart.Data = data;

            var options = new Options
            {
                Scales = new Scales()
            };

            var scales = new Scales
            {
                YAxes = new List<Scale>
                {
                    new CartesianScale
                    {
                        Ticks = new CartesianLinearTick
                        {
                            BeginAtZero = true
                        },
                        Stacked = true
                    }
                },
                XAxes = new List<Scale>
                {
                    new BarScale
                    {
                        BarPercentage = 0.5,
                        BarThickness = 6,
                        MaxBarThickness = 8,
                        MinBarLength = 2,
                        GridLines = new GridLine()
                        {
                            OffsetGridLines = true
                        },
                        Stacked = true
                    }
                }
            };

            options.Scales = scales;

            chart.Options = options;

            chart.Options.Layout = new Layout
            {
                Padding = new Padding
                {
                    PaddingObject = new PaddingObject
                    {
                        Left = 10,
                        Right = 12
                    }
                }
            };

            return chart;
        }
    }
}