using SampleApplicationISEE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace ISEE
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
        protected void Application_AcquireRequestState(object sender, EventArgs e)
        {

            var languageSession = "en";
            if (Request.UserLanguages != null)
            {
                languageSession = Request.UserLanguages[0];
                HttpContext context = HttpContext.Current;
                if (context != null && context.Session != null)
                    languageSession = ISEE.Common.SessionManegment.SessionManagement.Language;

                Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(languageSession);
                Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(languageSession);

            }



        }
    }
}
