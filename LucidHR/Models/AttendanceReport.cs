using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LucidHR.Models
{
   
    public class AttendanceReport
    {
        public ShortEmp employee { get; set; }
        public int leaveDays { get; set; }
        public int holiDays { get; set; }
        public int withoutLeaveDays { get; set; }
        public string leaveDayString { get; set; }
        public string holiDayString { get; set; }
        public string withoutLeaveDayString { get; set; }
        public int totalDays { get; set; }

    }
}