using prj_BIZ_System.App_Start;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using prj_BIZ_System.Controllers;

namespace prj_BIZ_System
{
    public class MvcApplication : System.Web.HttpApplication
    {
        private string mailBaseDir = "App_Start/";
        private string keyBaseDir  = "App_Start/";
        private string cachePropertyBaseDir = "App_Start/";
        public static string uploadFileBaseDir = "Content/UploadRootDir/";
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);

            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            MailConfig.RegisterCustomSetting(Server.MapPath("~/"+ mailBaseDir));
            SecurityHelper.RegisterCustomSetting(Server.MapPath("~/"+ keyBaseDir));

            UploadConfig.RegisterCustomSetting(uploadFileBaseDir,Server.MapPath("~/"+ uploadFileBaseDir));
            CacheConfig.RegisterCustomSetting(Server.MapPath("~/" + cachePropertyBaseDir));
        }

        protected void Application_BeginRequest(Object sender, EventArgs e)
        {
            HttpCookie cultureCookie = Request.Cookies["_culture"];
            if (cultureCookie != null)
            {
                // get culture name
                var cultureInfoName = CultureHelper.GetImplementedCulture(cultureCookie.Value);

                // set culture
                System.Threading.Thread.CurrentThread.CurrentCulture =
                new System.Globalization.CultureInfo(cultureInfoName);
                System.Threading.Thread.CurrentThread.CurrentUICulture =
                new System.Globalization.CultureInfo(cultureInfoName);

            }
        }
    }
}
