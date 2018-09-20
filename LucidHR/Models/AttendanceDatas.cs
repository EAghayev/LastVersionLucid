using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LucidHR.Models
{
    public class AttendanceDatas
    {
        public List<Employee> Employees { get; set; }
        public List<DateTime> Dates { get; set; }
        public List<Attendance> Attendances { get; set; }
        public List<LeaveRequest> LeaveRequests { get; set; }
        public List<Holiday> Holidays { get; set; }
    }
}