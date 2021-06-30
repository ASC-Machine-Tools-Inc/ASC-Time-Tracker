using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Asc_Time_Tracker.Models
{
    public class TimeLog
    {
        // TODO: Add more fields based on ENGTIME.
        public int ID { get; set; }

        [Display(Name = "Employee ID")]
        [Required]
        public int EMPID { get; set; }

        [Display(Name = "Job #")]
        public string JOBNUM { get; set; }

        // Date the timelog was started on.
        [DataType(DataType.Date)]
        [Display(Name = "Start Date")]
        [Required]
        public DateTime DATE { get; set; }

        // Time stored in seconds.
        [Display(Name = "Time Spent", Prompt = "In hours (decimals accepted)")]
        [Required]
        public double TIME { get; set; }

        [Display(Name = "Notes")]
        public string NOTES { get; set; }

        // Flag for research & development, default false.
        [Display(Name = "R & D")]
        [Required]
        public bool RD { get; set; } = false;
    }
}