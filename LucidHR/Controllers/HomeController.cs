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
    public class HomeController : Controller
    {
        LucidEntities db = new LucidEntities();
        //private IEnumerable<object> list;
        DateTime remove = DateTime.Now.AddHours(-4);
        public ActionResult Index()
        {
 
            Employee emp = (Employee)Session["user"];
            int userId = db.Users.FirstOrDefault(u => u.EmployeeId == emp.Id).Id;
            var todoList = db.Todolists.Where(l => l.UserId == userId);

            ViewHome data = new ViewHome()
            {
                Todolists = todoList.Where(t => t.Date >= remove).ToList(),
                Employees = db.Employees.ToList()
            };
            return View(data);
        }

        [HttpPost]
        public JsonResult createlist(string toDoTitle, DateTime toDoDate)
        {
            Employee emp = (Employee)Session["user"];
            int userId = db.Users.FirstOrDefault(u => u.EmployeeId == emp.Id).Id;
            var todoList = db.Todolists.Where(l => l.UserId == userId);
            if (!String.IsNullOrWhiteSpace(toDoTitle) && toDoDate != null)
            {
                var myDate = toDoDate;
                //you con not add a list for later than 4 hours later now
                if (myDate < DateTime.Now.AddHours(-4))
                {
                    return Json(new
                    {
                        status = 404,
                        message = "You can not add a list for later than now"
                    }, JsonRequestBehavior.AllowGet);
                }
                Todolist myTodo = new Todolist()
                {
                    Title = toDoTitle,
                    Date = myDate
                };
                if (todoList.FirstOrDefault(t => t.Date == myTodo.Date && t.Title == myTodo.Title) == null)
                {
                    myTodo.UserId = userId;
                    db.Todolists.Add(myTodo);
                    db.SaveChanges();

                    double pages = Math.Ceiling(todoList.Where(t => t.Date >= remove).ToList().Count() / 4.00);
                    return Json(new
                    {
                        status = 200,
                        data = new
                        {
                            title = myTodo.Title,
                            date = myTodo.Date,
                            page = pages
                        },
                        message = "You added a new plan"
                    }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new
                    {
                        status = 402,
                        message = "This plan is already exist in your schedule"
                    }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(new
                {
                    status = 404
                }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult Salary(string name)
        {

            if (!String.IsNullOrWhiteSpace(name))
            {
                var moment = DateTime.Now;

                var devEmps = db.Employees.Where(e => e.Role.DepartmentId == db.Departments.FirstOrDefault(d => d.Name == "Web Development").Id || e.Role.DepartmentId == db.Departments.FirstOrDefault(d => d.Name == "App Development").Id).ToList();
                var marketingEmps = db.Employees.Where(e => e.Role.DepartmentId == db.Departments.FirstOrDefault(d => d.Name == "Marketing").Id).ToList();
                var hrEmps = db.Employees.Where(e => e.Role.DepartmentId == db.Departments.FirstOrDefault(d => d.Name == "Human Resource").Id).ToList();

                if (name == "year")
                {
                    //dev quoters
                    double firstDevEmpSalary = 0;
                    double secondDevEmpSalary = 0;
                    double thirdDevEmpSalary = 0;
                    double fourthDevEmpSalary = 0;

                    //marketing quoters
                    double firstMarketingEmpsSalary = 0;
                    double secondMarketingEmpsSalary = 0;
                    double thirdMarketingEmpsSalary = 0;
                    double fourthMarketingEmpsSalary = 0;

                    //hr quoters
                    double firstHrEmpsSalary = 0;
                    double secondHrEmpsSalary = 0;
                    double thirdHrEmpsSalary = 0;
                    double fourthHrEmpsSalary = 0;

                    foreach (var item in db.Employees.ToList())
                    {

                        //first quoter
                        if (Convert.ToDateTime(item.JoinDate) < moment.AddMonths(-9))
                        {

                            var diffDates = moment.AddMonths(-9) - Convert.ToDateTime(item.JoinDate);
                            int days = diffDates.Days >= 90 ? 90 : diffDates.Days;
                            int months = days / 30;

                            if (devEmps.FirstOrDefault(d => d.Id == item.Id) != null)
                            {
                                firstDevEmpSalary += months * Convert.ToInt32(item.Salary);
                                firstDevEmpSalary += Math.Round((days % 30) * (Convert.ToDouble(item.Salary) / 30));
                            }
                            if (marketingEmps.FirstOrDefault(m => m.Id == item.Id) != null)
                            {
                                firstMarketingEmpsSalary += months * Convert.ToInt32(item.Salary);
                                firstMarketingEmpsSalary += Math.Round((days % 30) * (Convert.ToDouble(item.Salary) / 30));
                            }
                            if (hrEmps.FirstOrDefault(h => h.Id == item.Id) != null)
                            {
                                firstHrEmpsSalary += months * Convert.ToInt32(item.Salary);
                                firstHrEmpsSalary += Math.Round((days % 30) * (Convert.ToDouble(item.Salary) / 30));
                            }
                        }

                        //second quoter
                        else if (item.JoinDate < moment.AddMonths(-6))
                        {
                            var diffDates = moment.AddMonths(-6) - Convert.ToDateTime(item.JoinDate);
                            int days = diffDates.Days >= 90 ? 90 : diffDates.Days;
                            int months = days / 30;

                            if (devEmps.FirstOrDefault(d => d.Id == item.Id) != null)
                            {
                                secondDevEmpSalary += months * Convert.ToInt32(item.Salary);
                                secondDevEmpSalary += Math.Round((days % 30) * (Convert.ToDouble(item.Salary) / 30));
                            }
                            if (marketingEmps.FirstOrDefault(m => m.Id == item.Id) != null)
                            {
                                secondMarketingEmpsSalary += months * Convert.ToInt32(item.Salary);
                                secondMarketingEmpsSalary += Math.Round((days % 30) * (Convert.ToDouble(item.Salary) / 30));
                            }
                            if (hrEmps.FirstOrDefault(h => h.Id == item.Id) != null)
                            {
                                secondHrEmpsSalary += months * Convert.ToInt32(item.Salary);
                                secondHrEmpsSalary += Math.Round((days % 30) * (Convert.ToDouble(item.Salary) / 30));
                            }
                        }

                        //third quoter
                        else if (item.JoinDate < moment.AddMonths(-3))
                        {
                            var diffDates = moment.AddMonths(-3) - Convert.ToDateTime(item.JoinDate);
                            int days = diffDates.Days >= 90 ? 90 : diffDates.Days;
                            int months = days / 30;


                            if (devEmps.FirstOrDefault(d => d.Id == item.Id) != null)
                            {
                                thirdDevEmpSalary += months * Convert.ToInt32(item.Salary);
                                thirdDevEmpSalary += Math.Round((days % 30) * (Convert.ToDouble(item.Salary) / 30));
                            }
                            if (marketingEmps.FirstOrDefault(m => m.Id == item.Id) != null)
                            {
                                thirdMarketingEmpsSalary += months * Convert.ToInt32(item.Salary);
                                thirdMarketingEmpsSalary += Math.Round((days % 30) * (Convert.ToDouble(item.Salary) / 30));
                            }
                            if (hrEmps.FirstOrDefault(h => h.Id == item.Id) != null)
                            {
                                thirdHrEmpsSalary += months * Convert.ToInt32(item.Salary);
                                thirdHrEmpsSalary += Math.Round((days % 30) * (Convert.ToDouble(item.Salary) / 30));
                            }
                        }

                        //fourth quoter
                        else
                        {
                            var diffDates = moment - Convert.ToDateTime(item.JoinDate);
                            //int days = diffDates.Days >= 90 ? 90 : diffDates.Days;
                            //int months = days / 30;
                            int months = 1;
                            int days = 0;

                            if (devEmps.FirstOrDefault(d => d.Id == item.Id) != null)
                            {
                                fourthDevEmpSalary += months * Convert.ToDouble(item.Salary);
                                fourthDevEmpSalary += Math.Round((days % 30) * (Convert.ToDouble(item.Salary) / 30));
                            }
                            if (marketingEmps.FirstOrDefault(m => m.Id == item.Id) != null)
                            {
                                fourthMarketingEmpsSalary += months * Convert.ToDouble(item.Salary);
                                fourthMarketingEmpsSalary += Math.Round((days % 30) * (Convert.ToDouble(item.Salary) / 30));
                            }
                            if (hrEmps.FirstOrDefault(h => h.Id == item.Id) != null)
                            {
                                fourthHrEmpsSalary += months * Convert.ToDouble(item.Salary);
                                fourthHrEmpsSalary += Math.Round((days % 30) * (Convert.ToDouble(item.Salary) / 30));
                            }
                        }
                    }

                    return Json(new
                    {
                        status = 200,
                        first = new
                        {
                            dev = firstDevEmpSalary,
                            marketing = firstMarketingEmpsSalary,
                            hr = firstHrEmpsSalary
                        },
                        second = new
                        {
                            dev = secondDevEmpSalary,
                            marketing = secondMarketingEmpsSalary,
                            hr = secondHrEmpsSalary
                        },
                        third = new
                        {
                            dev = thirdDevEmpSalary,
                            marketing = thirdMarketingEmpsSalary,
                            hr = thirdHrEmpsSalary
                        },
                        fourth = new
                        {
                            dev = fourthDevEmpSalary,
                            marketing = fourthMarketingEmpsSalary,
                            hr = fourthHrEmpsSalary
                        }
                    }, JsonRequestBehavior.AllowGet);
                }

                return Json(new
                {
                    status = 404
                }, JsonRequestBehavior.AllowGet);
            }
            return Json(new
            {
                status = 404
            }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult pagenation(int page = 1)
        {
            Employee emp = (Employee)Session["user"];
            int userId = db.Users.FirstOrDefault(u => u.EmployeeId == emp.Id).Id;
            var todoList = db.Todolists.Where(l => l.UserId ==userId);
            List<AddedList> addedList = new List<AddedList>();
            var list = todoList.Where(t => t.Date >= remove).OrderBy(l => l.Date).Skip((page - 1) * 4).Take(4);
            foreach (var item in list)
            {
                AddedList added = new AddedList
                {
                    todolist = item,
                    date = item.Date.Value.ToString("hh:mm tt dd MMMM yyyy")
                };
                addedList.Add(added);
            }
           
            
            return Json(new
            {
                
                list = addedList.Select(l => new {
                    date = l.date,
                    title = l.todolist.Title,
                    id = l.todolist.Id,
                    isCompleted = l.todolist.IsCompleted
                })
              

            }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Completed(bool isCompleted, int id)
        {
            if (db.Todolists.Find(id) != null)
            {
                db.Todolists.Find(id).IsCompleted = isCompleted;
                db.SaveChanges();
                return Json(new
                {
                    status = 200
                }, JsonRequestBehavior.AllowGet);
            }
            return Json(new
            {
                status = 404
            }, JsonRequestBehavior.AllowGet);

        }

        public JsonResult Deletelist(int[] id)
        {
            Employee emp = (Employee)Session["user"];
            int userId = db.Users.FirstOrDefault(u => u.EmployeeId == emp.Id).Id;
            var todoList = db.Todolists.Where(l => l.UserId == userId);
            if (id != null)
            {
                if (id.Length > 0)
                {

                    for (int i = 0; i < id.Length; i++)
                    {
                        if (db.Todolists.Find(id[i]) == null)
                        {
                            return Json(new
                            {
                                status = 404,
                                id=id[0],
                                message = "You don't select any task"
                            }, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            db.Todolists.Remove(db.Todolists.Find(id[i]));
                            db.SaveChanges();
                        }
                    }

                    double pages = Math.Ceiling(todoList.Where(t => t.Date >= remove).ToList().Count() / 4.00);
                    return Json(new
                    {
                        status = 200,
                        message = "You delete selected tasks",
                        data = new
                        {
                            page = pages
                        }
                    }, JsonRequestBehavior.AllowGet);
                }
            }


            return Json(new
            {
                status = 404,
                message = "You don't select any task"
            }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Fullsalary()
        {
            List<string> depNames = new List<string>();
            List<int> depSalaries = new List<int>();
            List<DepSalary> deps = new List<DepSalary>();
            int totalSalary = 0;



            foreach (var item in db.Departments.ToList())
            {
                DepSalary dep = new DepSalary();
                dep.name = item.Name;
                var firstMonth = DateTime.Now.AddMonths(-12);
                List<monsal> mons = new List<monsal>();
                totalSalary = 0;
                for (int i = 1; i <= 12; i++)
                {
                    totalSalary = 0;
                    DateTime date = DateTime.Now.AddMonths((-1 * (12 - i)));
                    DateTime newDate = new DateTime(date.Year, date.Month, 1, 00, 00, 00);
                    foreach (var emp in db.Employees.Where(e => e.Role.DepartmentId == item.Id && e.JoinDate <= newDate).ToList())
                    {
                        totalSalary += Convert.ToInt32(emp.Salary);
                    }
                    monsal monthSalary = new monsal
                    {
                        salary = totalSalary,
                        month = date.ToString("MMM")
                    };
                    mons.Add(monthSalary);
                }
                dep.mon = mons;
                deps.Add(dep);
            }


            string[] depName = depNames.ToArray();
            int[] depSalary = depSalaries.ToArray();
            return Json(new
            {
                data = new
                {
                    depart = deps
                }
            }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult Progressbar()
        {
            double allEmp = db.Employees.ToList().Count;
            double maleEmps = db.Employees.Where(e => e.Gender == false).ToList().Count;
            double femaleEmps = db.Employees.Where(s => s.Gender == true).ToList().Count;
            double maleEmpsPer = (maleEmps / allEmp) * 100;
            double femaleEmpsPer = (femaleEmps / allEmp) * 100;

            return Json(new
            {
                status = 200,
                data = new
                {
                    male = Convert.ToInt32(maleEmpsPer),
                    female =Convert.ToInt32(femaleEmpsPer)
                }
            }, JsonRequestBehavior.AllowGet);
        }
    }

}
