using System;
using System.Collections.Generic;
using System.Linq;
using ChartJSCore.Helpers;
using ChartJSCore.Models;

namespace Asc_Time_Tracker.Models.TimeLog
{
    public class TimeLogStats
    {
        public IQueryable<TimeLog> TimeLogs { get; set; }

        /// <summary>
        /// The total amount of time spent for this time frame.
        /// </summary>
        public string TotalTimeSpent { get; set; }

        /// <summary>
        /// The job number with the most hours spent for this time frame.
        /// </summary>
        public string TopJobNum { get; set; }

        /// <summary>
        /// The top jop number, represented as a hex string.
        /// </summary>
        public string TopJobNumColor { get; set; }

        /// <summary>
        /// The hours spent on the top job number.
        /// </summary>
        public string TopTime { get; set; }

        /// <summary>
        /// Pie chart for hours spent on top x job numbers.
        /// </summary>
        public Chart TimeSpentPieChart { get; set; }

        /// <summary>
        /// Bar chart for hours spent on different jobs through the week.
        /// </summary>
        // public Chart WeekBarChart { get; set; }

        public TimeLogStats(IQueryable<TimeLog> timeLogs, int pieCount)
        {
            TimeLogs = timeLogs;
            InitializeStats(pieCount);
        }

        /// <summary>
        /// Return a pie chart of the top job numbers worked on for this time frame.
        /// </summary>
        public static Chart GenerateTopXPieChart(IQueryable<TimeLog> timeLogs, int limit)
        {
            // Sort time logs by top (given number) by the most time spent on them.
            timeLogs = IndexViewModel.TakeTopXTimeLogs(timeLogs, limit);

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

        /* Add when chartjscore gets updated
        public static Chart GenerateWeekBarChart(IQueryable<TimeLog> timeLogs)
        {
            // Filter time logs to this week.
            timeLogs = IndexViewModel.TakeTopXTimeLogs(timeLogs, 1000);

            Chart chart = new()
            {
                Type = Enums.ChartType.Bar
            };

            ChartJSCore.Models.Data data = new()
            {
                Datasets = new List<Dataset>(),
                Labels = new List<string>() { "Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat" }
            };

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
            };

            foreach (TimeLog timeLog in timeLogs)
            {
                string hex = TimeLog.JobNumToRgb(timeLog.JobNum);
                //colors.Add(ChartColor.FromHexString(hex));

                BarDataset temp = new()
                {
                    Label = timeLog.JobNum,
                    Data = new List<double?>() { 12, 19, 3, 5, 2, 3, timeLog.Time / 3600 },
                    BackgroundColor = new List<ChartColor>
                    {
                        ChartColor.FromRgba(255, 99, 132, 0.2),
                        ChartColor.FromRgba(54, 162, 235, 0.2),
                        ChartColor.FromRgba(255, 206, 86, 0.2),
                        ChartColor.FromRgba(75, 192, 192, 0.2),
                        ChartColor.FromRgba(153, 102, 255, 0.2),
                        ChartColor.FromRgba(255, 159, 64, 0.2),
                        ChartColor.FromRgba(153, 102, 255, 0.2)
                    },
                    BorderColor = new List<ChartColor>
                    {
                        ChartColor.FromRgb(255, 99, 132),
                        ChartColor.FromRgb(54, 162, 235),
                        ChartColor.FromRgb(255, 206, 86),
                        ChartColor.FromRgb(75, 192, 192),
                        ChartColor.FromRgb(153, 102, 255),
                        ChartColor.FromRgb(255, 159, 64),
                        ChartColor.FromRgb(153, 102, 255)
                    },
                    BorderWidth = new List<int>() { 1 }
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
        } */

        /// <summary>
        /// Populate the stats for this model from the timelogs.
        /// </summary>
        private void InitializeStats(int pieCount)
        {
            if (TimeLogs.Any())
            {
                // Grab statistics.
                TotalTimeSpent = TimeLog.SecondsToHoursAndMinutesString(
                    TimeLogs.Sum(t => t.Time));

                IQueryable<TimeLog> topTimeLogs = IndexViewModel.TakeTopXTimeLogs(TimeLogs, 1);
                if (topTimeLogs.Any())
                {
                    TimeLog topTimeLog = topTimeLogs.First();
                    TopJobNum = topTimeLog.JobNum;
                    TopJobNumColor = TimeLog.JobNumToRgb(topTimeLog.JobNum);
                    TopTime = TimeLog.SecondsToHoursAndMinutesString(topTimeLog.Time);
                }

                // Draw charts.
                TimeSpentPieChart = GenerateTopXPieChart(TimeLogs, pieCount);
                //WeekBarChart = GenerateWeekBarChart(TimeLogs);
            }
        }
    }
}