using IBatisNet.DataMapper;
using prj_BIZ_System.Models;
using prj_BIZ_System.Services;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace prj_BIZ_System.Controllers
{

    public class UserController : Controller
    {
        public ActionResult Register()
        {
            return View();
        }
    }

}