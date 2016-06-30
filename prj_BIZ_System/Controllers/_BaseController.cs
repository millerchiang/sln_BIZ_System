using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace prj_BIZ_System.Controllers
{
    public class _BaseController : Controller
    {
        public Logger logger = LogManager.GetCurrentClassLogger();
    }
}