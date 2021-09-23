using System;
using System.ComponentModel.DataAnnotations;

namespace Asc_Time_Tracker.Models
{
    /// <summary>
    /// A model for keeping track of the time spent on jobs.
    /// </summary>
    public class TimeLog
    {
        public int Id { get; set; }

        /// <summary>
        /// The employee ID for user creating this log.
        /// </summary>
        [Display(Name = "Employee ID")]
        [Required]
        public string EmpId { get; set; }

        [Display(Name = "Category")]
        [Required]
        public string Category { get; set; }

        /// <summary>
        /// The job number that was worked on for this log.
        /// </summary>
        [Display(Name = "Job #")]
        public string JobNum { get; set; }

        /// <summary>
        /// Date the log was started on.
        /// </summary>
        [DataType(DataType.Date)]
        [Display(Name = "Start Date")]
        [Required]
        public DateTime Date { get; set; }

        /// <summary>
        /// Time spent, stored in seconds.
        /// </summary>
        [Display(Name = "Time Spent")]
        [Required]
        public double Time { get; set; }

        /// <summary>
        /// Additional description of how the time was spent.
        /// </summary>
        [Display(Name = "Notes")]
        public string Notes { get; set; }

        /// <summary>
        /// Flag for research & development, default set to false.
        /// </summary>
        [Display(Name = "R & D")]
        [Required]
        public bool Rd { get; set; } = false;

        /// <summary>
        /// Return the RGB hex string for identifying a specific job number.
        /// </summary>
        /// <param name="jobNum">
        /// The job number to convert.
        /// </param>
        /// <param name="fallback">
        /// Optional color to return if job number is null/empty. White by default.
        /// </param>
        /// <returns>An RGB hex string.</returns>
        public static string JobNumToRgb(string jobNum, string fallback = "#FFFFFF")
        {
            if (string.IsNullOrEmpty(jobNum))
            {
                return fallback;
            }

            // Convert to hsl to match javascript formula in jobColor.js.
            double s = 75;
            double l = 50;

            // Get hashcode from job number.
            var hash = 0;
            foreach (char c in jobNum)
            {
                hash = c + ((hash << 5) - hash);
                hash &= hash; // Convert to 32bit integer.
            }
            double h = hash % 360;

            /* Forgot why I added this. May re-add? Seems fine for now.
            // Update: I remembered why. 29680.000 broke it - fixed it with this.
            // Adjust color if negative. */
            if (h < 0)
            {
                h = 360 + h;
            }

            // Convert hsl to rgb.
            l /= 100;
            double a = s * Math.Min(l, 1 - l) / 100;

            // See https://en.wikipedia.org/wiki/HSL_and_HSV#HSL_to_RGB_alternative
            // for fancy math done.
            string Func(int n)
            {
                double k = (n + h / 30) % 12;
                double color = l - a * Math.Max(Math.Min(Math.Min(k - 3, 9 - k), 1), -1);

                // Convert to Hex and prefix "0" if needed.
                return Convert.ToByte(Math.Round(255 * color)).ToString("X2");
            }

            return "#" + Func(0) + Func(8) + Func(4);
        }

        public static int HoursAndMinutesToSeconds(int hours, int minutes)
        {
            return hours * 3600 + minutes * 60;
        }

        public static string SecondsToHoursAndMinutesString(double seconds)
        {
            if (seconds < 60)
            {
                return "";
            }

            string result = "";

            int hours = (int)(seconds / 3600);
            int minutes = (int)(seconds % 3600 / 60);

            if (hours >= 1)
            {
                result = hours + " hour";
                if (hours > 1) result += "s";
                if (minutes > 0) result += ", ";
            }

            if (minutes > 0)
            {
                result += minutes + " minute";
                if (minutes > 1) result += "s";
            }

            return result;
        }
    }
}