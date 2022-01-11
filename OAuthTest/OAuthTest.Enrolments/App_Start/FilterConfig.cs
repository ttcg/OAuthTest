using System.Web;
using System.Web.Mvc;

namespace OAuthTest.Enrolments
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
