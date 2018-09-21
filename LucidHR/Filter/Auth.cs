using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LucidHR.Filter
{
    public class Auth:ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (HttpContext.Current.Session["login"] == null)
            {
                filterContext.Result = new RedirectResult("~/login");
                return;
            }

            base.OnActionExecuting(filterContext);
        }
    }
}