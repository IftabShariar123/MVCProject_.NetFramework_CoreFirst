using System.Web;
using System.Web.Mvc;

namespace Project_MVC_CF_Special
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
