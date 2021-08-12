using System;
using System.Collections.Generic;

namespace Asc_Time_Tracker.Models
{
    public class IndexViewModel
    {
        public TimeLog TimeLog;
        public IEnumerable<TimeLog> TimeLogs { get; set; }
    }
}