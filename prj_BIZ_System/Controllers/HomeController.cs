using IBatisNet.DataMapper;
using prj_BIZ_System.App_Start;
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
        public Index_ViewModel indexModel;
        public ActivityService activityService;

        public HomeController()
        {
            userService = new UserService();
            activityService = new ActivityService();
            indexModel = new Index_ViewModel();
        }


        public ActionResult Index()
        {

            if (Request.Cookies["UserInfo"] != null)
            {
                indexModel.enterprisesortList = userService.GetSortList();
                indexModel.userinfoList = userService.GetUserInfoList();
                indexModel.activityinfoList = activityService.GetActivityInfoList();
                indexModel.newsList = activityService.GetNewsAll();
                foreach (NewsModel newsModel in indexModel.newsList)
                {
                        newsModel.content = HttpUtility.HtmlDecode(newsModel.content);
                }

                return View(indexModel);
            }
            else
                return Redirect("Login");
        }

        public ActionResult News()
        {
            if (Request.Cookies["UserInfo"] != null)
            {
//                indexModel.enterprisesortList = userService.GetSortList();
 //               indexModel.userinfoList = userService.GetUserInfoList();
//                indexModel.activityinfoList = activityService.GetActivityInfoList();
                indexModel.newsList = activityService.GetNewsAll();


//                foreach (NewsModel newsModel in indexModel.newsList)
//                {
//                    newsModel.content = HttpUtility.HtmlDecode(newsModel.content);
//                }

                return View(indexModel);
            }
            else
                return Redirect("Login");
        }

        public ActionResult NewsView()
        {
            if (Request.Cookies["UserInfo"] != null)
            {
                //                indexModel.enterprisesortList = userService.GetSortList();
                //               indexModel.userinfoList = userService.GetUserInfoList();
//                indexModel.activityinfoList = activityService.GetActivityInfoList();
                indexModel.news = activityService.GetNewsOne(int.Parse(Request["Id"]));


                //                foreach (NewsModel newsModel in indexModel.newsList)
                //  
//                NewsModel newsModel =new NewsModel();
                indexModel.news.content = HttpUtility.HtmlDecode(indexModel.news.content);
                //                }

                return View(indexModel);
            }
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

            HttpCookie cookie = null;

            if (model == null)
            {
                return Redirect("Login");
            }
            else
            {
                cookie = new HttpCookie("UserInfo");
                cookie.Values.Add("id_enable", model.id_enable);
                cookie.Values.Add("user_id", model.user_id);

                //                Response.Cookies["UserInfo"]["id_enable"] = model.id_enable;
                //                Response.Cookies["UserInfo"]["user_id"] = model.user_id;
                if (model.id_enable == "0")
                {
                    Response.AppendCookie(cookie);
                    return Redirect("../User/register?user_id=" + model.user_id);
                }

                cookie.Values.Add("company", model.company);
                cookie.Values.Add("website", model.website);
                cookie.Values.Add("info", model.info);
                Response.AppendCookie(cookie);

                //                Response.Cookies["UserInfo"]["company"] = model.company;
                //                Response.Cookies["UserInfo"]["website"] = model.website;
                //                Response.Cookies["UserInfo"]["info"] = model.info;
            }
            return Redirect("Index");
        }


        public ActionResult Verification()
        {
            return View();
        }
        

        public ActionResult MailValidateResult()
        {
            var result = TempData["MailValidateResult"];
            if(result == null)
            {
                result = "";
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}