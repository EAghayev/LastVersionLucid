using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LucidHR.Models;
using System.IO;
using Simple.ImageResizer;
using LucidHR.Filter;

namespace LucidHR.Controllers
{
    [Auth]
    public class EmployeeController : Controller
    {
        LucidEntities db = new LucidEntities();
        // GET: Employee
        public ActionResult Index()
        {
            ViewHome data = new ViewHome
            {
                Employees = db.Employees.ToList(),
                Departments = db.Departments.ToList(),
                Roles = db.Roles.ToList(),
            };
            return View(data);
        }

        public JsonResult Roles(int id)
        {
            List<Role> roles = db.Roles.Where(r => r.DepartmentId == id).ToList();
            var response = roles.Select(r => new
            {
                r.Id,
                r.Name
            }).ToList();
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Requests()
        {
            //var shouldBeDeleted = db.LeaveRequests.Where(l =>(l.FinishDate) < DateTime.Now).ToList();
            //db.LeaveRequests.RemoveRange(shouldBeDeleted);
            //db.SaveChanges();
            ViewHome data = new ViewHome();
            data.LeaveRequests = db.LeaveRequests.ToList();
            List<LeaveType> list = new List<LeaveType>();

            foreach (var item in db.LeaveTypes)
            {
                if (db.LeaveRequests.FirstOrDefault(l => l.LeaveTypeId == item.Id) != null)
                {
                    list.Add(item);
                }
            }
            data.LeaveTypes = list.ToList();


            return View(data);
        }
        //accept request
        public ActionResult Accept(int? id)
        {
            if (id != null)
            {
                if (db.LeaveRequests.Find(id) != null)
                {
                    db.LeaveRequests.Find(id).IsAccepted = true;
                    db.SaveChanges();
                    return RedirectToAction("requests");
                }
            }
            return HttpNotFound();
        }

        //reject request
        public ActionResult Reject(int? id)
        {
            if (id != null)
            {
                if (db.LeaveRequests.Find(id) != null)
                {
                    db.LeaveRequests.Remove(db.LeaveRequests.Find(id));
                    db.SaveChanges();
                    Session["RequestAdded"] = "You delete a requst successfully";
                    return RedirectToAction("requests");
                }
            }
            return HttpNotFound();

        }

        //add request
        [HttpPost]
        public ActionResult Addleave(string EmployeeId, string LeaveTypeId, DateTime StartDate, DateTime FinishDate, string Reason)
        {

            if (!String.IsNullOrWhiteSpace(EmployeeId) && !String.IsNullOrWhiteSpace(LeaveTypeId) && StartDate != null && FinishDate != null && !String.IsNullOrWhiteSpace(Reason))
            {
                int empId = 0;
                int leaveId = 0;
                var holidayList = db.Holidays.ToList();
                DateTime toDate = DateTime.Now, fromDate = DateTime.Now;
                if (EmployeeId.Contains("-"))
                {
                    var empArray = EmployeeId.Split('-');
                    if (empArray.Length == 2)
                    {
                        //emp id control
                        if (int.TryParse(empArray[1], out int id))
                        {
                            empId = id;
                            if (db.Employees.Find(id) == null)
                            {
                                Session["RequestError"] = "This employee is not exist";
                                return RedirectToAction("requests");
                            }
                        }
                        else
                        {
                            Session["RequestError"] = "Employee id is not correct";
                            return RedirectToAction("requests");
                        }

                        if (holidayList.FirstOrDefault(h => (h.StartDate.Value.Date <= StartDate.Date && h.EndDate.Value.Date >= StartDate.Date) && (h.StartDate.Value.Date <= FinishDate.Date && h.EndDate.Value.Date >= FinishDate.Date)) != null)
                        {
                            Session["RequestError"] = "There is a holiday in this time";
                            return RedirectToAction("requests");
                        }
                        if (holidayList.FirstOrDefault(h => h.StartDate <= StartDate.Date && h.EndDate >= StartDate.Date) != null)
                        {
                            StartDate = holidayList.FirstOrDefault(h => h.StartDate <= StartDate.Date && h.EndDate >= StartDate.Date).EndDate ?? DateTime.Now;
                        }
                        if (holidayList.FirstOrDefault(h => h.StartDate.Value.Date <= FinishDate.Date && h.EndDate.Value.Date >= FinishDate.Date) != null)
                        {
                            FinishDate = holidayList.FirstOrDefault(h => h.StartDate.Value.Date <= FinishDate.Date && h.EndDate.Value.Date >= FinishDate.Date).StartDate ?? DateTime.Now;
                        }

                        if (db.LeaveRequests.FirstOrDefault(l => l.EmployeeId == id) != null)
                        {

                            foreach (var item in db.LeaveRequests.Where(l => l.EmployeeId == id).ToList())
                            {
                                if ((item.StratDate.Value.Date <= StartDate.Date && item.FinishDate.Value.Date >= StartDate.Date) ||
                               (item.StratDate.Value.Date <= FinishDate.Date && item.FinishDate.Value.Date >= FinishDate.Date) ||
                              (item.StratDate.Value.Date >= StartDate.Date && item.StratDate.Value.Date <= FinishDate.Date) ||
                              (StartDate.Date <= item.FinishDate.Value.Date && item.FinishDate.Value.Date <= FinishDate.Date))
                                {
                                    Session["RequestError"] = "This employee will be holiday in this time";
                                    return RedirectToAction("requests");
                                }
                            }
                        }




                        //dates control

                        if (FinishDate.Date < DateTime.Now.Date)
                        {
                            Session["RequestError"] = "Date can not be earlier than the present date";
                            return RedirectToAction("requests");
                        }
                        //leavetype and reson length control
                        if (LeaveTypeId.Length > 150 || Reason.Length > 500)
                        {
                            Session["RequestError"] = "Leave type can not be contains more than 150 char";
                            return RedirectToAction("requests");
                        }
                        if (db.LeaveTypes.FirstOrDefault(l => l.Name == LeaveTypeId) == null)
                        {
                            LeaveType type = new LeaveType();
                            type.Name = LeaveTypeId;
                            db.LeaveTypes.Add(type);
                            db.SaveChanges();
                        }
                        LeaveRequest data = new LeaveRequest
                        {
                            EmployeeId = empId,
                            LeaveTypeId = db.LeaveTypes.FirstOrDefault(l => l.Name == LeaveTypeId).Id,
                            StratDate = StartDate,
                            FinishDate = FinishDate,
                            Reason = Reason,
                            IsAccepted = true
                        };
                        db.LeaveRequests.Add(data);
                        db.SaveChanges();



                        Session["RequestAdded"] = "You added new request";
                        return RedirectToAction("requests");


                    }

                }
                Session["RequestError"] = "Employee id is not correct";
                return RedirectToAction("requests");
            }
            Session["RequestError"] = "Please fill all inputs correctly";
            return RedirectToAction("requests");
        }

        //Upload profile file
        [HttpPost]
        public JsonResult Upload()
        {

            string fileName = DateTime.Now.ToString("yyyyMMddHHmmssss") + Request.Files[0].FileName.Replace(" ", "");
            string path = Path.Combine(Server.MapPath("~/Public/Upload/Profiles"), fileName);
            Request.Files[0].SaveAs(path);

            ImageResizer resizer = new ImageResizer(path);
            resizer.Resize(40, 40, ImageEncoding.Jpg90);
            string thumPath = Path.Combine(Server.MapPath("~/Public/Upload/Profiles/ProfileThumbnails"), fileName);
            resizer.SaveToFile(thumPath);


            return Json(new
            {
                status = 200,
                data = new
                {
                    fileUrl = "/Upload/Profiles/" + fileName,
                    fileName
                }
            }, JsonRequestBehavior.AllowGet);
        }
        //Remove uploaded file
        public JsonResult RemoveFile(string fileName)
        {
            if (fileName != "female.png" && fileName != "male.png")
            {
                string path = Path.Combine(Server.MapPath("~/Public/Upload/Profiles"), fileName);
                string thumPath = Path.Combine(Server.MapPath("~/Public/Upload/Profiles/ProfileThumbnails"), fileName);
                System.IO.File.Delete(path);
                System.IO.File.Delete(thumPath);
            }
            return Json(new
            {
                status = 200,
                fileName
            }, JsonRequestBehavior.AllowGet);

        }


        //Create emp in add-emp form
        [HttpPost]
        public JsonResult Create(string FullName, DateTime JoinDate, int RoleId, int DepartmentId, string Email, string Phone, int Gender = 2, decimal Salary = 0, string Facebook = null, string Twitter = null, string Linkedin = null, string Instagram = null, string Profile = null)
        {
            //string[] fullName = emp.fullName.Split(' ');
            //emp.employee.Name = fullName[0];
            //emp.employee.JoinDate = DateTime.Parse(emp.joinDate);
            //emp.employee.Surname = fullName[1];
            //db.Employees.Add(emp.employee);
            //db.SaveChanges();

            //=======
            if (!String.IsNullOrWhiteSpace(FullName) && JoinDate!=null && Salary > 0
                && !String.IsNullOrWhiteSpace(RoleId.ToString()) && !String.IsNullOrWhiteSpace(Email) && !String.IsNullOrWhiteSpace(Phone)
                && Gender < 2 && Gender >= 0)

            {
                if (FullName.Contains(' '))
                {
                    //full name control
                    string[] fullName = FullName.Split(' ');
                    if (fullName.Length == 2)
                    {
                        if (fullName[0].Length > 50 || fullName[1].Length > 50)
                        {
                            return Json(new
                            {
                                status = 402,
                                message = "Name and Surname must be less than 50 charactest"
                            }, JsonRequestBehavior.AllowGet);
                        }

                        //email control
                        else if (Email.Length > 100)
                        {
                            return Json(new
                            {
                                status = 402,
                                message = "Email must be less than 100 characters"
                            }, JsonRequestBehavior.AllowGet);
                        }
                        else if (db.Employees.FirstOrDefault(e => e.Email == Email) != null)
                        {
                            return Json(new
                            {
                                status = 404,
                                message = "This email already exist"
                            }, JsonRequestBehavior.AllowGet);
                        }

                        //phone control
                        else if (Phone.Length > 25)
                        {
                            return Json(new
                            {
                                status = 402,
                                message = "Phone number must be less than 25 characters"
                            }, JsonRequestBehavior.AllowGet);
                        }
                        else if (db.Employees.FirstOrDefault(e => e.Phone == Phone) != null)
                        {
                            return Json(new
                            {
                                status = 404,
                                message = "This phone already exist"
                            }, JsonRequestBehavior.AllowGet);
                        }
                        //salary control
                        else if (Salary > 1000000000)
                        {
                            return Json(new
                            {
                                status = 406,
                                message = "Salary is too long"
                            }, JsonRequestBehavior.AllowGet);
                        }

                        //joinDate control
                        else if (JoinDate > DateTime.Now)
                        {
                            return Json(new
                            {
                                status = 407,
                                message = "Join date can not be feature date"
                            }, JsonRequestBehavior.AllowGet);
                        }

                        //selected role control
                        else if (db.Roles.Find(RoleId) == null)
                        {
                            return Json(new
                            {
                                status = 407,
                                message = "Pleae select employee role correctly"
                            }, JsonRequestBehavior.AllowGet);
                        }



                        //profile control

                        else
                        {
                            if (Profile == null)
                            {
                                if (Gender == 1)
                                {
                                    Profile = "female.png";
                                }
                                else
                                {
                                    Profile = "male.png";
                                }
                            }


                            Employee myEmp = new Employee
                            {
                                Name = fullName[0],
                                Surname = fullName[1],
                                Salary = Salary,
                                JoinDate = JoinDate,
                                RoleId = RoleId,
                                Email = Email,
                                Phone = Phone,
                                Profile = Profile,
                                Gender = Gender == 1 ? true : false,
                                Facebook = Facebook != "" ? Facebook : null,
                                Linkedin = Linkedin != "" ? Linkedin : null,
                                Instagram = Instagram != "" ? Instagram : null,
                                Twitter = Twitter != "" ? Twitter : null,
                            };

                            //existing employee control
                            if (db.Employees.FirstOrDefault(e => e.Email == myEmp.Email && e.Phone == myEmp.Phone) == null)
                            {

                                //department head control

                                if (db.Roles.Find(RoleId).IsHead == true)
                                {
                                    if (db.Departments.Find(DepartmentId).Head != null)
                                    {
                                        return Json(new
                                        {
                                            status = 404,
                                            message = "This department has a head.You can't designate anyone to this department like head"
                                        }, JsonRequestBehavior.AllowGet);
                                    }
                                    else
                                    {
                                        db.Employees.Add(myEmp);
                                        db.SaveChanges();
                                        db.Departments.Find(DepartmentId).Head = db.Employees.FirstOrDefault(e => e.Email == myEmp.Email && e.Phone == myEmp.Phone).Id;
                                        db.SaveChanges();

                                    }
                                }
                                else
                                {
                                    db.Employees.Add(myEmp);
                                    db.SaveChanges();
                                }
                                DateTime date = new DateTime(DateTime.Today.Year, DateTime.Today.AddMonths(1).Month, 1);
                                int c = 0;
                                //DateTime day = new DateTime(DateTime.Today.Year, DateTime.Today.Month,DateTime.Parse(myEmp.JoinDate.ToString()).Day, 06, 00, 00);

                                DateTime day = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);

                                for (int i = 0; i < DateTime.DaysInMonth(DateTime.Today.Year,DateTime.Today.Month); i++)
                                {
                                    bool isAttn;
                                    day=day.AddDays(i);
                                    if (JoinDate <= day)
                                     {
                                        if (day.DayOfWeek.ToString() == "Sunday" || day.DayOfWeek.ToString() == "Saturday")
                                        {
                                            isAttn = false;
                                        }
                                        else
                                        {
                                            isAttn = true;
                                        }
                                        Attendance atn = new Attendance
                                        {
                                            EmployeeId = db.Employees.FirstOrDefault(e => e.Email == myEmp.Email && e.Phone == myEmp.Phone).Id,
                                            Date = day,
                                            Atd = isAttn
                                        };
                                        db.Attendances.Add(atn);
                                        db.SaveChanges();
                                        day = day.AddDays(1);
                                    }
                                }


                                Employee emp = db.Employees.FirstOrDefault(e => e.Email == myEmp.Email);
                                string empRole = db.Roles.Find(emp.RoleId).Name;
                                return Json(new
                                {
                                    status = 200,
                                    data = new
                                    {
                                        id = db.Employees.FirstOrDefault(e => e.Phone == myEmp.Phone).Id,
                                        role = empRole,
                                        profile = db.Employees.FirstOrDefault(e => e.Phone == myEmp.Phone).Profile
                                        //role= emp.Role.Name
                                    }
                                }, JsonRequestBehavior.AllowGet);
                            }
                            else
                            {
                                return Json(new
                                {
                                    status = 403,
                                    message = "This employee already exsist"
                                }, JsonRequestBehavior.AllowGet);
                            }
                        }
                    }
                    else
                    {
                        return Json(new
                        {
                            status = 404,
                            message = "Employee fullname must be 2 words"
                        }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    return Json(new
                    {
                        status = 404,
                        message = "Employee fullname must be 2 words"
                    }, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(new
            {
                status = 404,
                message = "Please fill inputs correctly"
            }, JsonRequestBehavior.AllowGet);

        }

        //Update emp
        public JsonResult Edit(int? id)
        {
            if (db.Employees.Find(id) != null)
            {
                Employee emp = db.Employees.Find(id);
                return Json(new
                {
                    status = 200,
                    data = new
                    {
                        name = emp.Name,
                        surname = emp.Surname,
                        role = emp.Role.Id,
                        department = db.Departments.Find(emp.Role.DepartmentId).Id,
                        salary = emp.Salary,
                        profile = emp.Profile,
                        date = emp.JoinDate.Value.ToString("MM/dd/yyyy"),
                        email = emp.Email,
                        gender = emp.Gender,
                        phone = emp.Phone,
                        fb = emp.Facebook ?? "",
                        linkedin = emp.Linkedin ?? "",
                        twitter = emp.Twitter ?? "",
                        instagram = emp.Instagram ?? ""
                    }
                }, JsonRequestBehavior.AllowGet);
            }
            return Json(new
            {
                status = 404,
                message = "not found"
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult Edit(Employee emp, string FullName, string JoinDate, int Gender, int DepartmentId)
        {

            if (!String.IsNullOrWhiteSpace(FullName) && !String.IsNullOrWhiteSpace(JoinDate) && !String.IsNullOrWhiteSpace(emp.Salary.ToString())
           && !String.IsNullOrWhiteSpace(emp.RoleId.ToString()) && !String.IsNullOrWhiteSpace(emp.Email) && !String.IsNullOrWhiteSpace(emp.Phone)
           && Gender != null)
            {
                if (FullName.Contains(' '))
                {

                    //full name control
                    string[] fullName = FullName.Split(' ');
                    if (fullName.Length == 2)
                    {

                        if (fullName[0].Length > 50 || fullName[1].Length > 50)
                        {
                            return Json(new
                            {
                                status = 402,
                                message = "Name and Surname must be less than 50 charactest"
                            }, JsonRequestBehavior.AllowGet);
                        }

                        //email control
                        else if (emp.Email.Length > 100)
                        {
                            return Json(new
                            {
                                status = 402,
                                message = "Email must be less than 100 characters"
                            }, JsonRequestBehavior.AllowGet);
                        }
                        else if (db.Employees.FirstOrDefault(e => e.Email == emp.Email && e.Id != emp.Id) != null)
                        {
                            return Json(new
                            {
                                status = 402,
                                message = "This email is exist"
                            }, JsonRequestBehavior.AllowGet);
                        }
                        //phone control
                        else if (emp.Phone.Length > 25)
                        {
                            return Json(new
                            {
                                status = 402,
                                message = "Phone number must be less than 25 characters"
                            }, JsonRequestBehavior.AllowGet);
                        }
                        else if (db.Employees.FirstOrDefault(e => e.Phone == emp.Phone && e.Id != emp.Id) != null)
                        {
                            return Json(new
                            {
                                status = 402,
                                message = "This phone is exist"
                            }, JsonRequestBehavior.AllowGet);
                        }
                        //salary control
                        else if (emp.Salary > 1000000000)
                        {
                            return Json(new
                            {
                                status = 406,
                                message = "Salary is too long"
                            }, JsonRequestBehavior.AllowGet);
                        }

                        //joinDate control
                        else if (DateTime.Parse(JoinDate) > DateTime.Now)
                        {
                            return Json(new
                            {
                                status = 407,
                                message = "Join date can not be feature date"
                            }, JsonRequestBehavior.AllowGet);
                        }

                        //selected role control
                        else if (db.Roles.Find(emp.RoleId) == null)
                        {
                            return Json(new
                            {
                                status = 407,
                                message = "Pleae select employee role correctly"
                            }, JsonRequestBehavior.AllowGet);
                        }


                        //profile control
                        else
                        {


                            if (emp.Profile == null)
                            {
                                if (Gender == 0)
                                {
                                    emp.Profile = "male.png";
                                }
                                else if (Gender == 1)
                                {
                                    emp.Profile = "female.png";
                                }
                            }

                        }




                        emp.Name = fullName[0];
                        emp.Surname = fullName[1];
                        emp.JoinDate = DateTime.Parse(JoinDate);
                        emp.Gender = Gender == 1 ? true : false;
                        db.Entry(emp).State = System.Data.Entity.EntityState.Modified;

                        //employee attendance
                        List<DateTime> dates = new List<DateTime>();
                        for (var date = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1); date.Month == DateTime.Today.Month; date = date.AddDays(1))
                        {
                            dates.Add(date);
                        }
                        var attnList = db.Attendances.ToList();
                        List<Attendance> removedAttn = new List<Attendance>();

                        foreach (var item in dates)
                        {
                            if (DateTime.Parse(JoinDate) <= item.Date)
                            {
                                bool isAttn;
                                if (attnList.FirstOrDefault(atn => atn.EmployeeId == emp.Id && atn.Date.Value.ToShortDateString() == item.Date.ToShortDateString()) == null)
                                {
                                    if (item.DayOfWeek.ToString() == "Sunday" || item.DayOfWeek.ToString() == "Saturday")
                                    {
                                        isAttn = false;
                                    }
                                    else
                                    {
                                        isAttn = true;
                                    }

                                    Attendance atn = new Attendance
                                    {
                                        EmployeeId = emp.Id,
                                        Date = item.Date,
                                        Atd = isAttn
                                    };
                                    db.Attendances.Add(atn);
                                    db.SaveChanges();
                                }
                            }
                            else
                            {
                                if (attnList.FirstOrDefault(a => a.EmployeeId == emp.Id && a.Date.Value.ToShortDateString() == item.Date.ToShortDateString()) != null)
                                {
                                    Attendance attndance = attnList.FirstOrDefault(a => a.EmployeeId == emp.Id && a.Date.Value.ToShortDateString() == item.Date.ToShortDateString());
                                    db.Attendances.Remove(db.Attendances.Find(attndance.Id));
                                    db.SaveChanges();
                                }
                            }
                        }



                        //var fillDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month,1,06,00,00);
                        int daysOfMonth = DateTime.DaysInMonth(DateTime.Today.Year, DateTime.Today.Month);


                        ////department head update

                        if (db.Departments.FirstOrDefault(d => d.Head == emp.Id) != null)
                        {
                            db.Departments.FirstOrDefault(d => d.Head == emp.Id).Head = null;
                            db.SaveChanges();
                        }
                        if (emp.Role.IsHead == true)
                        {
                            if (db.Departments.Find(DepartmentId).Head != null && db.Departments.Find(DepartmentId).Head != emp.Id)
                            {
                                return Json(new
                                {
                                    status = 404,
                                    message = "This department has a head"
                                }, JsonRequestBehavior.AllowGet);
                            }
                            else
                            {
                                db.Departments.Find(DepartmentId).Head = emp.Id;
                            }
                        }


                        db.SaveChanges();
                        string role = db.Roles.Find(emp.RoleId).Name;
                        return Json(new
                        {
                            newrole = emp.RoleId,
                            status = 200,
                            message = "You update an employee",
                            data = new
                            {
                                id = emp.Id,
                                name = emp.Name,
                                surname = emp.Surname,
                                roleId = emp.Role.Id,
                                empRole = role,
                                department = db.Departments.Find(emp.Role.DepartmentId).Id,
                                salary = emp.Salary,
                                profile = emp.Profile,
                                date = emp.JoinDate.Value.ToString("MM/dd/yyyy"),
                                email = emp.Email,
                                gender = emp.Gender,
                                phone = emp.Phone,
                                fb = emp.Facebook ?? "",
                                linkedin = emp.Linkedin ?? "",
                                twitter = emp.Twitter ?? "",
                                instagram = emp.Instagram ?? ""
                            }

                        }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new
                        {
                            status = 404,
                            message = "Employee fullname must be 2 words"
                        }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    return Json(new
                    {
                        status = 404,
                        message = "Employee fullname must be 2 words"
                    }, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(new
            {
                status = 404,
                message = "Please fill inputs correctly"
            }, JsonRequestBehavior.AllowGet);

        }

        public JsonResult Delete(int[] id)
        {
            for (int i = 0; i < id.Length; i++)
            {
                int myId = id[i];
                if (db.Employees.Find(myId) == null)
                {

                    return Json(new
                    {
                        status = 404
                    }, JsonRequestBehavior.AllowGet);

                }
                if (db.Departments.FirstOrDefault(d => d.Head == myId) != null)
                {
                    db.Departments.FirstOrDefault(d => d.Head == myId).Head = null;
                    db.SaveChanges();
                }
                if (db.LeaveRequests.FirstOrDefault(l => l.EmployeeId == myId) != null)
                {
                    db.LeaveRequests.Remove(db.LeaveRequests.FirstOrDefault(l => l.EmployeeId == myId));
                    db.SaveChanges();
                }
                //string fullName = db.Employees.Find(id[i]).Name + " " + db.Employees.Find(id[i]).Surname;
                db.Employees.Remove(db.Employees.Find(myId));
                db.SaveChanges();
            }

            return Json(new
            {
                status = 200,
                message = "You deleted succesfully"
            }, JsonRequestBehavior.AllowGet);

        }

        //Attendance
        public ActionResult Attendance()
        {
            DateTime moment = DateTime.Now;
            int month = moment.Month;
            int year = moment.Year;
            var dates = new List<DateTime>();

            foreach (var item in db.Employees.ToList())
            {
                Attendance attn = new Attendance
                {
                    Date = DateTime.Now
                };
            }

            for (var date = new DateTime(year, month, 1); date.Month == month; date = date.AddDays(1))
            {
                dates.Add(date);
            }

            //var fillDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month,1,06,00,00);
            int daysOfMonth = DateTime.DaysInMonth(DateTime.Today.Year, DateTime.Today.Month);

            if (db.Attendances.FirstOrDefault(a => a.Date.Value.Month == month) == null)
            {
                foreach (var item in dates)
                {
                    foreach (var emp in db.Employees.ToList())
                    {
                        if (emp.JoinDate > item.Date)
                        {
                            continue;
                        }
                        var fillDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, item.Day, 06, 00, 00);
                        bool isAttn;
                        if (item.DayOfWeek.ToString() == "Sunday" || item.DayOfWeek.ToString() == "Saturday")
                        {
                            isAttn = false;
                        }
                        else
                        {
                            isAttn = true;
                        }

                        Attendance atn = new Attendance
                        {
                            EmployeeId = emp.Id,
                            Date = item.Date,
                            Atd = isAttn,

                        };
                        db.Attendances.Add(atn);
                        db.SaveChanges();
                    }
                }
            }



            AttendanceDatas data = new AttendanceDatas
            {
                Dates = dates,
                Employees = db.Employees.ToList(),
                Attendances = db.Attendances.ToList(),
                LeaveRequests = db.LeaveRequests.ToList(),
                Holidays = db.Holidays.ToList()
            };
            return View(data);
        }

        public JsonResult Empattendance(int? id, string day, bool isAttn)
        {
            DateTime myDate = DateTime.Now;
            if (id != null && day != null && isAttn != null)
            {
                if (db.Employees.Find(id) != null)
                {
                    myDate = DateTime.Parse(day);

                    List<int> listIds = new List<int>();

                    foreach (var item in db.Attendances.Where(a => a.EmployeeId == id))
                    {

                        listIds.Add(item.Id);

                    }
                    int[] arrIds = listIds.ToArray();

                    for (int i = 0; i < arrIds.Length; i++)
                    {
                        int atnId = arrIds[i];
                        if (db.Attendances.Find(atnId).Date.Value.ToString("dd.MM.yyyy") == myDate.ToString("dd.MM.yyyy"))
                        {
                            db.Attendances.Find(atnId).Atd = isAttn;
                            db.SaveChanges();
                            break;
                        }
                    }

                    return Json(new
                    {
                        status = 200,
                    }, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(new
            {
                status = 404,
            }, JsonRequestBehavior.AllowGet);

        }



    }

}