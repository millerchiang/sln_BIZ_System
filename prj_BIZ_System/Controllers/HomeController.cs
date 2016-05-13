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

    public class HomeController : Controller
    {

        public HomeController()
        {
//            homeService = new ActivityService();
        }

        public ActionResult Index()
        {
            if (Request.Cookies["user_id"] != null )
                return View();
            else
                return Redirect("Login");
        }

        public ActionResult Login()
        {
            return View();
        }


        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        
    }
}