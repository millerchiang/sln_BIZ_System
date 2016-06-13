using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace prj_BIZ_System.Controllers
{
    public class PasswordController : Controller
    {
        // GET: Password
        public ActionResult EditPasswd()
        {
            return View();
        }

        public ActionResult PasswordInsertUpdate()
        {
            return View();
        }

        public ActionResult ForgetPassword()
        {
            return View();
        }

        public ActionResult ReSendPassword()
        {
            return View();
        }
    }
}