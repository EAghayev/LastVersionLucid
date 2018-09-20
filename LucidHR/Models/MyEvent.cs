using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LucidHR.Models
{
    public class MyEvent
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Desc { get; set; }
        public string Lacation { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public string Type { get; set; }
    }
}