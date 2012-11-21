using System.Web;
using System.Web.Mvc;

namespace Microsoft.Live.Web.Samples.AuthWithMSAccount
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}