using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LucidHR.Filter;

namespace LucidHR.Controllers
{
    [Auth]
    public class ActivitiesController : Controller
    {
        // GET: Activities
        public ActionResult Index()
        {
            return View();
        }
    }
}