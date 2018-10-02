using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LucidHR.Models;
using LucidHR.Filter;

namespace LucidHR.Controllers
{
    [Auth]
    public class ReportsController : Controller
    {
        LucidEntities db = new LucidEntities();
        // GET: Reports
        public ActionResult Index()
        {
            ViewHome data = new ViewHome
            {
                Employees = db.Employees.ToList(),
                Attendances = db.Attendances.ToList(),
                Holidays = db.Holidays.ToList(),
                LeaveRequests = db.LeaveRequests.Where(l=>l.IsAccepted==true).ToList(),
                LeaveTypes = db.LeaveTypes.ToList()
            };
            return View(data);
        }

        public JsonResult Attendance(int year, int quoter, int month)
        {


            List<AttendanceReport> atnList = new List<AttendanceReport>();
            //===============================================================================================
            //All attendance for all years
            //================================================================================================
            if (year == 0)
            {
                var list = db.Employees.ToList();
                foreach (var item in list)
                {
                    DateTime day = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
                    int dayCount = 0;
                    string dayCountToolTip = "";

                    int leaveDays = 0;
                    string leaveDaysToolTip = "";

                    int holidayCount = 0;
                    string holidayCountToolTip = "";
                    //leavedays
                    if (db.LeaveRequests.Where(l => l.EmployeeId == item.Id && l.IsAccepted==true).ToList() != null)
                    {
                        foreach (var leave in db.LeaveRequests.Where(l => l.EmployeeId == item.Id && l.IsAccepted==true).ToList())
                        {
                            for (int i = 0; i < (leave.FinishDate - leave.StratDate).Value.TotalDays; i++)
                            {
                                leaveDaysToolTip += " " + leave.StratDate.Value.AddDays(i).ToShortDateString();
                                leaveDays++;
                            }
                        }
                    }
                    //withoutLeaveDays
                    foreach (var atn in db.Attendances.Where(a => a.EmployeeId == item.Id).ToList())
                    {


                        if ((int)atn.Date.Value.DayOfWeek != 6 && (int)atn.Date.Value.DayOfWeek != 0 && atn.Atd == false)
                        {
                            dayCount++;
                            dayCountToolTip += " " + atn.Date.Value.ToShortDateString();
                        }
                    }

                    //holidayCount
                    foreach (var hol in db.Holidays.ToList())
                    {
                        if (hol.StartDate >= item.JoinDate)
                        {
                            for (int i = 0; i < (hol.EndDate - hol.StartDate).Value.TotalDays; i++)
                            {
                                holidayCountToolTip += " " + hol.StartDate.Value.AddDays(i).ToShortDateString();
                                holidayCount++;

                            }
                        }
                    }
                    ShortEmp emp = new ShortEmp
                    {
                        Name = item.Name,
                        Surname = item.Surname,
                        JoinDate = item.JoinDate.Value.ToShortDateString(),
                        Id = item.Id
                    };
                    AttendanceReport atnRep = new AttendanceReport
                    {
                        employee = emp,
                        leaveDays = leaveDays,
                        leaveDayString = leaveDaysToolTip,
                        withoutLeaveDays = dayCount,
                        withoutLeaveDayString = dayCountToolTip,
                        holiDays = holidayCount,
                        holiDayString = holidayCountToolTip,
                        totalDays = leaveDays + holidayCount + dayCount
                    };
                    atnList.Add(atnRep);


                }

                return Json(new
                {
                    status = 200,
                    data = atnList
                }, JsonRequestBehavior.AllowGet);
            }

            //===============================================================================================
            // Attendances for a year
            //================================================================================================
            if (year != 0 && quoter == 0)
            {

                DateTime Feb = new DateTime(year, 2, 1);
                int daysInYear = DateTime.DaysInMonth(Feb.Year, Feb.Month) == 28 ? 365 : 366;

                var list = db.Employees.ToList();
                foreach (var item in list)
                {
                    DateTime day = new DateTime(year, 1, 1);
                    int dayCount = 0;
                    string dayCountToolTip = "";

                    int leaveDays = 0;
                    string leaveDaysToolTip = "";

                    int holidayCount = 0;
                    string holidayCountToolTip = "";
                    for (int j = 0; j < daysInYear; j++)
                    {
                        DateTime date = day.AddDays(j);
                        if (item.JoinDate.Value.Year<=year)
                    {
                            if (db.LeaveRequests.FirstOrDefault(l => l.EmployeeId == item.Id && (l.StratDate <= date && l.FinishDate > date && l.IsAccepted==true)) != null)
                            {
                                leaveDays++;
                                leaveDaysToolTip += " " + date.ToShortDateString();
                            }
                            if (db.Holidays.FirstOrDefault(h => (h.StartDate <= date && h.EndDate > date)) != null)
                            {
                                holidayCount++;
                                holidayCountToolTip += " " + date.ToShortDateString();
                            }
                            if (db.Attendances.FirstOrDefault(a => a.EmployeeId == item.Id && a.Date == date && a.Atd==false) != null)
                            {
                                dayCount++;
                                dayCountToolTip += " " + date.ToShortDateString();
                            }
                        }
                    }
                    ShortEmp emp = new ShortEmp
                    {
                        Name = item.Name,
                        Surname = item.Surname,
                        JoinDate = item.JoinDate.Value.ToShortDateString(),
                        Id = item.Id
                    };
                    AttendanceReport atnRep = new AttendanceReport
                    {
                        employee = emp,
                        leaveDays = leaveDays,
                        leaveDayString = leaveDaysToolTip,
                        withoutLeaveDays = dayCount,
                        withoutLeaveDayString = dayCountToolTip,
                        holiDays = holidayCount,
                        holiDayString = holidayCountToolTip,
                        totalDays = leaveDays + holidayCount + dayCount
                    };
                    atnList.Add(atnRep);
                }
                return Json(new
                {
                    status = 200,
                    data = atnList
                }, JsonRequestBehavior.AllowGet);
            }

            ////===============================================================================================
            //// Attendances for a quoter
            ////===============================================================================================
            if (year != 0 && quoter != 0 && month == 0)
            {
                int[] quoterMonths = new int[3];

                switch (quoter)
                {
                    case 1:
                        quoterMonths[0] = 1;
                        quoterMonths[1] = 2;
                        quoterMonths[2] = 3;
                        break;
                    case 2:
                        quoterMonths[0] = 4;
                        quoterMonths[1] = 5;
                        quoterMonths[2] = 6;
                        break;
                    case 3:
                        quoterMonths[0] = 7;
                        quoterMonths[1] = 8;
                        quoterMonths[2] = 9;
                        break;
                    case 4:
                        quoterMonths[0] = 10;
                        quoterMonths[1] = 11;
                        quoterMonths[2] = 12;
                        break;
                    default:
                        return Json(new
                        {
                            status = 200
                        }, JsonRequestBehavior.AllowGet);
                }

               
                var list = db.Employees.ToList();
                List<AttendanceReport> attnList = new List<AttendanceReport>();
                foreach (var item in list)
                {
                    DateTime day = new DateTime(year, quoterMonths[0], 1);
                    int daysOfEndMonth = DateTime.DaysInMonth(year, quoterMonths[2]);
                    DateTime endDay = new DateTime(year, quoterMonths[2], daysOfEndMonth);
                    int dayCount = 0;
                    string dayCountToolTip = "";

                    int leaveDays = 0;
                    string leaveDaysToolTip = "";

                    int holidayCount = 0;
                    string holidayCountToolTip = "";
                  
                        for (int i = 0; i < (endDay - day).TotalDays; i++)
                        {
                            DateTime date = day.AddDays(i);
                        if (item.JoinDate <= date)
                        {
                            if (db.LeaveRequests.FirstOrDefault(l => l.EmployeeId == item.Id && (l.StratDate <= date && l.FinishDate > date && l.IsAccepted==true)) != null)
                            {
                                leaveDays++;
                                leaveDaysToolTip +=" " + date.ToShortDateString();
                            }
                            if(db.Holidays.FirstOrDefault(h=>h.StartDate<= date && h.EndDate > date) != null)
                            {
                                holidayCount++;
                                holidayCountToolTip += " " + date.ToShortDateString();
                            }
                            if ((int)date.DayOfWeek != 0 && (int)date.DayOfWeek != 6)
                            {
                                if (db.Attendances.FirstOrDefault(a => a.EmployeeId == item.Id && a.Date == date && a.Atd == false) != null)
                                {
                                    dayCount++;
                                    dayCountToolTip += " " + date.ToShortDateString();
                                }
                            }
                        }
                       
                    }
                    ShortEmp emp = new ShortEmp
                    {
                        Id = item.Id,
                        Name = item.Name,
                        Surname = item.Surname,
                        JoinDate = item.JoinDate.Value.ToShortDateString()
                    };
                    AttendanceReport attnRep = new AttendanceReport
                    {
                        employee = emp,
                        withoutLeaveDays = dayCount,
                        withoutLeaveDayString = dayCountToolTip,
                        leaveDays = leaveDays,
                        leaveDayString = leaveDaysToolTip,
                        holiDays = holidayCount,
                        holiDayString = holidayCountToolTip,
                        totalDays = dayCount + leaveDays + holidayCount
                    };
                    attnList.Add(attnRep);
                }
                return Json(new
                {
                    status = 200,
                    data = attnList
                }, JsonRequestBehavior.AllowGet);
            }

            ////===============================================================================================
            //// Attendances for a month
            ////===============================================================================================
            if (year != 0 && quoter != 0 && month != 0)
            {
                var list = db.Employees.ToList();
                List<AttendanceReport> attnList = new List<AttendanceReport>();
                foreach (var item in list)
                {
                    DateTime day = new DateTime(year, month, 1);
                    int daysOfMonth = DateTime.DaysInMonth(year, month);
                    int dayCount = 0;
                    string dayCountToolTip = "";

                    int leaveDays = 0;
                    string leaveDaysToolTip = "";

                    int holidayCount = 0;
                    string holidayCountToolTip = "";
                    
                        for (int i = 0; i < daysOfMonth; i++)
                        {
                            DateTime date = day.AddDays(i);
                        if (item.JoinDate <= date)
                        {
                            if (db.LeaveRequests.FirstOrDefault(l => l.EmployeeId == item.Id && (l.StratDate <= date && l.FinishDate > date && l.IsAccepted==true)) != null)
                            {
                                leaveDays++;
                                leaveDaysToolTip += " " + date.ToShortDateString();
                            }
                            if (db.Holidays.FirstOrDefault(h => h.StartDate <= date && h.EndDate > date) != null)
                            {
                                holidayCount++;
                                holidayCountToolTip += " " + date.ToShortDateString();
                            }
                            if ((int)date.DayOfWeek != 0 && (int)date.DayOfWeek != 6)
                            {
                                if (db.Attendances.FirstOrDefault(a => a.EmployeeId == item.Id && a.Date == date && a.Atd == false) != null)
                                {
                                    dayCount++;
                                    dayCountToolTip += " " + date.ToShortDateString();
                                }
                            }
                        }
                    }
                    ShortEmp emp = new ShortEmp
                    {
                        Id = item.Id,
                        Name = item.Name,
                        Surname = item.Surname,
                        JoinDate = item.JoinDate.Value.ToShortDateString()
                    };
                    AttendanceReport attnRep = new AttendanceReport
                    {
                        employee = emp,
                        withoutLeaveDays = dayCount,
                        withoutLeaveDayString = dayCountToolTip,
                        leaveDays = leaveDays,
                        leaveDayString = leaveDaysToolTip,
                        holiDays = holidayCount,
                        holiDayString = holidayCountToolTip,
                        totalDays = dayCount + leaveDays + holidayCount
                    };
                    attnList.Add(attnRep);
                }
                return Json(new
                {
                    status = 200,
                    data = attnList
                }, JsonRequestBehavior.AllowGet);
            }


            return Json(new
            {
                status = 404
            }, JsonRequestBehavior.AllowGet);

        }

        public ActionResult Department()
        {
            ViewHome data = new ViewHome
            {
                Employees = db.Employees.ToList(),
                Departments = db.Departments.ToList(),
                Roles = db.Roles.ToList()
            };
            return View(data);
        }
        public ActionResult Employee()
        {
            return View();
        }
    }


}