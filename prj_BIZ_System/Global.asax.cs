﻿using prj_BIZ_System.App_Start;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace prj_BIZ_System
{
    public class MvcApplication : System.Web.HttpApplication
    {
        private string uploadFileBaseDir = "Content";
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            CustomConfig.RegisterCustomSetting(uploadFileBaseDir,Server.MapPath("~/"+ uploadFileBaseDir));
        }
    }
}
