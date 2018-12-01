using System.Web;
using System.Web.Mvc;
using LucidHR.Attributes;

namespace LucidHR
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new LocalizationAttribute("en"), 0);
        }
    }
}
