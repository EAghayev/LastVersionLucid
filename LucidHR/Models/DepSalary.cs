using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LucidHR.Models
{
    public class monsal
    {
        public string month { get; set; }
        public int salary { get; set; }
    }
    public class DepSalary
    {
        public string name { get; set; }
        public List<monsal> mon { get; set; }
    }
}