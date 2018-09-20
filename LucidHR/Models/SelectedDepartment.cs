using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LucidHR.Models
{
    public class SelectedDepartment
    {
        public List<Role> DepRoles{ get; set; }
        public Department Dep { get; set; }
        public Employee Emp { get; set; }
    }
}