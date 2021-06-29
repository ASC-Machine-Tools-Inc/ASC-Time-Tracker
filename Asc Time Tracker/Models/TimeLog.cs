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

        public int EMPID { get; set; }

        public string JOBNUM { get; set; }

        // Date the timelog was started on.
        [DataType(DataType.Date)]
        public DateTime DATE { get; set; }

        // Time stored in seconds.
        public int TIME { get; set; }

        public string NOTES { get; set; }

        // Flag for research & development.
        public int RD { get; set; }
    }
}