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
    public class DepartmentsController : Controller
    {
        LucidEntities db = new LucidEntities();
        // GET: Departments
        public ActionResult Index()
        {

            ViewHome data = new ViewHome
            {
                Departments = db.Departments.ToList(),
                Employees = db.Employees.ToList()
            };

            return View(data);
        }

        //create department
        [HttpPost]
        public ActionResult Create(string Name, string[] Role)
        {

            if (!String.IsNullOrWhiteSpace(Name) && Role != null)
            {

                //name length control
                if (Name.Length > 100)
                {
                    Session["DepError"] = "Department name must be less than 100 char";
                    return RedirectToAction("index");
                }
                if (db.Departments.FirstOrDefault(d => d.Name == Name) != null)
                {
                    Session["DepError"] = "This department is already exist";
                    return RedirectToAction("index");
                }
                //roles length control
                for (int i = 0; i < Role.Length; i++)
                {
                    if (Role[i].Length > 100)
                    {
                        Session["DepError"] = "Role name must be less than 100 char";
                        return RedirectToAction("index");
                    }
                }

                //dep existing control
                if (db.Departments.FirstOrDefault(d => d.Name == Name) == null)
                {
                    Department dep = new Department
                    {
                        Name = Name
                    };
                    db.Departments.Add(dep);
                    db.SaveChanges();
                    Role depHead = new Role
                    {
                        Name = "Department Head",
                        IsHead = true,
                        DepartmentId = db.Departments.FirstOrDefault(d => d.Name == Name).Id
                    };
                    db.Roles.Add(depHead);
                    db.SaveChanges();


                    for (int i = 0; i < Role.Length; i++)
                    {
                        if (Role[i] == "")
                        {
                            continue;
                        }
                        Role rol = new Role
                        {
                            Name = Role[i],
                            DepartmentId = db.Departments.FirstOrDefault(d => d.Name == Name).Id
                        };
                        db.Roles.Add(rol);
                        db.SaveChanges();
                    }

                    Session["CreateSuccess"] = "You added new department";
                    return RedirectToAction("index");
                }
            }
            Session["DepError"] = "Please,fill inputs correctly";
            return RedirectToAction("index");
        }

        public ActionResult Edit(int? id)
        {
            if (id != null)
            {
                if (db.Departments.Find(id) != null)
                {
                    SelectedDepartment dep = new SelectedDepartment
                    {
                        Dep = db.Departments.Find(id),
                        DepRoles = db.Roles.Where(r => r.DepartmentId == id).ToList(),
                        Emp = db.Employees.Find(db.Departments.Find(id).Head)
                    };
                    return View(dep);
                }
            }
            Session["DepError"] = "This employee is not exist";
            return RedirectToAction("index");
        }

        //Removing role control
        public JsonResult Roleremove(int? id)
        {
            
            if (id != null)
            {
                if (db.Roles.Find(id) != null)
                {
                    if (db.Employees.FirstOrDefault(e => e.RoleId == id) == null) {
                       
                        return Json(new
                        {
                            status = 200,
                        }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new
                        {
                            status = 404,
                            message = "There is an emoloyee with this role"
                        }, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            return Json(new
            {
                status = 404,
                message = "This role is not exist"
            }, JsonRequestBehavior.AllowGet);
        }

        //
        [HttpPost]
        public ActionResult Edit(string Name, string[] Role, int?id,int[]RoleId)
        {
            if (!String.IsNullOrWhiteSpace(Name)  && id != null)
            {
                List<int> rolesId=new List<int>();
                IEnumerable<int> deletedRoles;

                //dep name control
                if (Name.Length > 100)
                {
                    Session["DepEditError"] = "Department name must be less than 100 char";
                    return RedirectToAction("edit");
                }
                //Role name control
                if (Role != null)
                {
                    for (int i = 0; i < Role.Length; i++)
                    {
                        if (Role[i].Length > 100)
                        {
                            Session["DepEditError"] = "Role name must be less than 100 char";
                            return RedirectToAction("edit");
                        }
                    }
                }

                //all roles id array
                foreach (var item in db.Roles.Where(r=>r.DepartmentId==id).ToList())
                {
                    rolesId.Add(item.Id);
                }


                    if (RoleId != null)
                    {
                    
                        deletedRoles = rolesId.Except(RoleId);
                    }
                    else
                    {
                        deletedRoles = rolesId;
                    }

                    int[] deletedRolesArray = deletedRoles.ToArray();
               
                    //deleted roles control
                    for (int i = 0; i < deletedRolesArray.Length; i++)
                    {
                        int delRoleId = deletedRolesArray[i]; 

                        if (db.Roles.Find(delRoleId) == null)
                        {
                            Session["DepEditError"] = "Your removing role is not exist";
                            return RedirectToAction("edit");
                        }

                        if (db.Roles.Find(delRoleId).IsHead == true)
                        {
                            continue;
                        }

                        if (db.Employees.FirstOrDefault(e => e.RoleId == delRoleId) != null)
                        {
                            Session["DepEditError"] = "Employee exist with" + db.Roles.Find(delRoleId).Name + " role";
                            return RedirectToAction("edit");
                        }
                    }

                    //remove all deleted roles
                    for (int i = 0; i < deletedRolesArray.Length; i++)
                    {
                        int delRoleId = deletedRolesArray[i];

                        if (db.Roles.Find(delRoleId).IsHead == true)
                        {
                            continue;
                        }
                        db.Roles.Remove(db.Roles.Find(delRoleId));
                        db.SaveChanges();
                    }


                //edit roles
                if (RoleId != null && Role!=null)
                {
                    for (int i = 0; i < RoleId.Length; i++)
                    {
                        db.Roles.Find(RoleId[i]).Name = Role[i];
                        db.SaveChanges();

                    }
                }
                int roleLength = Role != null ? Role.Length : 0;
                int roleIdLength = RoleId != null ? RoleId.Length : 0;


                if (roleLength > roleIdLength)
                {
                    for (int i = roleIdLength; i < roleLength; i++)
                    {
                        Role rol = new Role
                        {
                            Name = Role[i],
                            IsHead = false,
                            DepartmentId = id
                        };
                        db.Roles.Add(rol);
                        db.SaveChanges();

                    }
                }

                //edit department
                db.Departments.Find(id).Name = Name;
                db.SaveChanges();
                Session["DepEditSuccess"] = "You edited department";
                return RedirectToAction("edit");
            }
            Session["DepEditError"] = "Please fill inputs correctly";
            return RedirectToAction("edit");
        }

        public ActionResult Delete(int id)
        {
            if (db.Departments.Find(id) != null)
            {
                foreach (var item in db.Roles.Where(r=>r.DepartmentId==id&&r.IsHead!=true).ToList())
                {
                    if (db.Employees.FirstOrDefault(e => e.RoleId == item.Id) != null)
                    {
                        Session["DepError"] = "This department has employee";
                        return RedirectToAction("index");
                    }
                }
                db.Departments.Remove(db.Departments.Find(id));
                db.SaveChanges();
                Session["CreateSuccess"] = "You delete a department";
                return RedirectToAction("index");
            }
            Session["DepError"] = "This department not exist";
            return RedirectToAction("index");
        }
        
    }
    }