using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LucidHR.Models;
namespace LucidHR.Controllers
{
    public class EventsController : Controller
    {
        // GET: Events
        LucidEntities db = new LucidEntities();
        public ActionResult Index(int page=1)
        {
            int count = (page - 1) * 4;
            ViewHome data = new ViewHome
            {
                Events = db.Events.OrderBy(e=>e.StartDate).Skip(count).Take(4).ToList(),
                EventTypes = db.EventTypes.ToList()
            };
            ViewBag.EventCount = db.Events.Count();
            return View(data);
        }
        public JsonResult Calendar()
        {
            List<MyEvent> events = new List<MyEvent>();

            foreach (var item in db.Events)
            {
                MyEvent evnt = new MyEvent
                {
                    Title = item.Title,
                    StartDate = item.StartDate.Value.ToString("yyyy-MM-dd"),
                    StartTime = item.StartTime.Value.ToString("HH:mm:ss"),
                    EndDate = item.EndDate.Value.ToString("yyyy-MM-dd"),
                    EndTime = item.EndTime.ToString("HH:mm:ss"),
                    Desc = item.Desc,
                    Lacation =item.Lacation,
                    Type=item.EventType.Name
                    
                };
                events.Add(evnt);
            }
            return Json(new
            {
                status = 200,
                dateNow = DateTime.Now.ToString("yyyy-MM-dd"),
                data = new
                {
                   evnt=events
                }
            }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        //public JsonResult Create(Event evnt)
        //{
        //    if (evnt.StartDate == null || evnt.EndDate == null || evnt.EndTime == null || evnt.StartTime == null || String.IsNullOrWhiteSpace(evnt.Title)||evnt.TypeId==null)
        //    {
        //        return Json(new
        //        {
        //            status = 404,
        //            message = "Please fill inputs correctly"
        //        }, JsonRequestBehavior.AllowGet);
        //    }
        //    db.Events.Add(evnt);
        //    db.SaveChanges();
        //    List<MyEvent> events = new List<MyEvent>();
        //    foreach (var item in db.Events)
        //    {
        //        MyEvent event1 = new MyEvent
        //        {
        //            Title = item.Title,
        //            StartDate = item.StartDate.Value.ToString("yyyy-MM-dd"),
        //            StartTime = item.StartTime.Value.ToString("HH:mm:ss"),
        //            EndDate = item.EndDate.Value.ToString("yyyy-MM-dd"),
        //            EndTime = item.EndTime.ToString("HH:mm:ss"),
        //            Desc = item.Desc,
        //            Lacation = item.Lacation,
        //            Type = item.EventType.Name

        //        };
        //        events.Add(event1);
        //    }
          
        //    string evntType = db.EventTypes.Find(evnt.TypeId).Name;
        //    return Json(new
        //    {
        //        status = 200,
        //        data=events,
        //        message = "You added a new event",
        //        type= evntType,
        //        dateNow = DateTime.Now.ToString("yyyy-MM-dd")
        //    }, JsonRequestBehavior.AllowGet); 
        //}
        public ActionResult Create(Event evnt)
        {
            if (evnt.StartDate == null || evnt.EndDate == null || evnt.EndTime == null || evnt.StartTime == null || String.IsNullOrWhiteSpace(evnt.Title) || evnt.TypeId == null)
            {
                Session["eventError"] = "Please fill inputs correctly";
                return RedirectToAction("index");
            }
            evnt.EndDate = evnt.EndDate.Value.AddDays(-1);

            if (evnt.StartDate > evnt.EndDate || evnt.StartTime >= evnt.EndTime||evnt.StartTime<DateTime.Now)
            {
                Session["eventError"] = "Please fill inputs correctly";
                return RedirectToAction("index");
            }
            db.Events.Add(evnt);
            db.SaveChanges();

            Session["eventAdd"] = "You added a new event";
            return RedirectToAction("index");
        }

        public JsonResult Dropevent(DateTime start,DateTime end)
        {


            return Json(new
            {
                data=start
            },JsonRequestBehavior.AllowGet);
        }
        public ActionResult delete(int id)
        {
            if (db.Events.Find(id) != null)
            {
                db.Events.Remove(db.Events.Find(id));
                db.SaveChanges();
                Session["deleteEvent"] = "You removed an event";
                return RedirectToAction("index");
            }
            Session["errorEventDelete"] = "This event is not exist";
            return RedirectToAction("index");
        }
      
    }
}