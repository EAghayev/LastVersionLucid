using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LucidHR.Models
{
    public class AddedEmp
    {
        public string fullName { get; set; }
        public Employee employee { get; set; }
        public string joinDate { get; set; }
        public bool gender { get; set; }

    }
}