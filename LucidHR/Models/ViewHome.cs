using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LucidHR.Models
{
    public class ViewHome
    {
        public List<Employee> Employees  { get; set; }
        public List<Client> Clients { get; set; }
        public List<Project> Projects { get; set; }
        public List<ClientsProject> ClientsProjects { get; set; }
        public List<Attendance> Attendances { get; set; }
        public List<Department> Departments { get; set; }
        public List<Event> Events { get; set; }
        public List<Expens> Expens { get; set; }
        public List<Holiday> Holidays { get; set; }
        public List<Invoice> Invoices { get; set; }
        public List<LeaveRequest> LeaveRequests { get; set; }
        public List<LeaveType> LeaveTypes { get; set; }
        public List<Payment> Payments { get; set; }
        public List<PaymentType> PaymentTypes { get; set; }
        public List<Role> Roles { get; set; }
        public List<User> Users { get; set; }
        public List<Todolist> Todolists { get; set; }
        public List<EventType> EventTypes { get; set; }

    }
}