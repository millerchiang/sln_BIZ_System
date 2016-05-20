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

        public UserService userService;

        public HomeController()
        {
            userService = new UserService();
        }

        public ActionResult Index()
        {
            if (Request.Cookies["UserInfo"] != null )
                return View();
            else
                return Redirect("Login");
        }

        public ActionResult Login()
        {
            return View();
        }

        public ActionResult IdentifyUser()
        {
            UserInfoModel model = userService.ChkUserInfoOne(Request["user_id"], Request["user_pw"]);

            if (model == null)
            {
                return Redirect("Login");
            }
            else
            {
                if (model.id_enable=="0")
                    return Redirect("../User/register?user_id="+ model.user_id);

                Response.Cookies["UserInfo"]["user_id"] = model.user_id;
                Response.Cookies["UserInfo"]["company"] = model.user_id;
                Response.Cookies["UserInfo"]["website"] = model.user_id;
                Response.Cookies["UserInfo"]["info"] = model.user_id;
            }
            return Redirect("Index");
        }


        public ActionResult Verification()
        {
            return View();
        }
        
    }
}