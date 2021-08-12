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
        /// The employee ID of the user creating this log.
        /// </summary>
        [Display(Name = "Employee ID")]
        [Required]
        public int EmpId { get; set; }

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
    }
}