using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading;
using System.Web.Mvc;
using System.Globalization;

namespace LucidHR.Attributes
{
    public class LocalizationAttribute:ActionFilterAttribute
    {
        private readonly string _defaultLanguage;
        public LocalizationAttribute(string defaultLanguage)
        {
            _defaultLanguage = defaultLanguage;
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            string lang = (string)filterContext.RouteData.Values["lang"] ?? _defaultLanguage;
            if (lang != null)
            {
                try
                {
                    Thread.CurrentThread.CurrentCulture = Thread.CurrentThread.CurrentUICulture = new CultureInfo(lang);
                }
                catch(Exception)
                {
                    throw new NotSupportedException($"Invalid language code '{lang}'.");
                }
            }
        }
    }
}