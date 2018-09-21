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
    public class HolidaysController : Controller
    {

        LucidEntities db = new LucidEntities();
        // GET: Holiday
        public ActionResult Index()
        {

            ViewHome data = new ViewHome
            {
                Holidays = db.Holidays.ToList()
            };

            return View(data);
        }

        [HttpPost]
        public ActionResult Create(Holiday hol)
        {

            if (!String.IsNullOrWhiteSpace(hol.Name) && hol.StartDate!=null && hol.EndDate!=null)
            {

                if (hol.Name.Length > 150)
                {   
                    Session["inputError"] = "Holiday name must be less than 150 char";
                    return RedirectToAction("index");
                }
                if (hol.StartDate >= hol.EndDate)
                {
                    Session["inputError"] = "Start date can not be later EndDate";
                    return RedirectToAction("index");
                }
                if(hol.EndDate<=DateTime.Now)
                {
                    Session["inputError"] = "You can not add a holiday in past time";
                    return RedirectToAction("index");
                }
                var list = db.Holidays.ToList();
                if (list.FirstOrDefault(h=>(h.StartDate.Value.Date<=hol.StartDate.Value.Date&&h.EndDate.Value.Date>=hol.StartDate.Value.Date)
                    && (h.StartDate.Value.Date <= hol.EndDate.Value.Date && h.EndDate.Value.Date >= hol.EndDate.Value.Date)) != null)
                {
                    Session["inputError"] = "There is a holiday in this time";
                    return RedirectToAction("index");
                }
                if (list.FirstOrDefault(l => l.StartDate.Value.Date >= hol.StartDate.Value.Date && l.EndDate.Value.Date <= hol.EndDate.Value.Date) != null)
                {
                    db.Holidays.Remove(list.FirstOrDefault(l => l.StartDate.Value.Date >= hol.StartDate.Value.Date && l.EndDate.Value.Date <= hol.EndDate.Value.Date));
                    db.SaveChanges();
                }

               db.Holidays.Add(hol);
                db.SaveChanges();
                Session["holidayAdd"] = "You added new holiday";
                return RedirectToAction("index");
            }
            Session["inputError"] = "Please fill inputs correctly";
            return RedirectToAction("index");

        }

        //Holiday edit
        public JsonResult Edit(int id)
        {
            if (id != null)
            {
                if (db.Holidays.Find(id) != null)
                {
                    Holiday hol = db.Holidays.Find(id);
                    return Json(new
                    {
                        status=200,
                        data = new
                        {
                            name = hol.Name,
                            startDate = hol.StartDate.Value.ToShortDateString(),
                            endDate = hol.EndDate.Value.ToShortDateString()
                        }
                    },JsonRequestBehavior.AllowGet);
                }
            }
            return Json(new
            {
                status = 404
            }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult Edit(Holiday hol)
        {
            if (hol.Id==0 || String.IsNullOrEmpty(hol.Name) || hol.StartDate==null || hol.EndDate == null|| db.Holidays.Find(hol.Id) == null)
            {
                Session["inputError"] = "Please fill inputs correctly";
                return RedirectToAction("index");
            }
            
                Holiday holiday = db.Holidays.Find(hol.Id);
                if (hol.StartDate >= hol.EndDate)
                {
                    Session["inputError"] = "Start date can not be later EndDate";
                    return RedirectToAction("index");
                }
            if (holiday.Name != hol.Name || holiday.StartDate != hol.StartDate || holiday.EndDate != hol.EndDate)
                {
                //db.Entry(hol).State = System.Data.Entity.EntityState.Modified;
                    db.Holidays.Find(hol.Id).Name= hol.Name;
                    db.Holidays.Find(hol.Id).StartDate = hol.StartDate;
                    db.Holidays.Find(hol.Id).EndDate = hol.EndDate;
                    db.SaveChanges();
                    Session["holidayAdd"] = "You change a holiday successfully";
                    return RedirectToAction("index");
            }
            else
                {
                    Session["inputError"] = "You did not change anything";
                    return RedirectToAction("index");
                }
        }

        public ActionResult Delete(int id)
        {
            
            if (db.Holidays.Find(id) != null)
            {
                db.Holidays.Remove(db.Holidays.Find(id));
                db.SaveChanges();
                Session["holidayAdd"] = "You delete a holiday";
                return RedirectToAction("index");
            }
            Session["deleteError"] = "Please,select holiday correctly";
            return RedirectToAction("index");
        }

       
    }
}