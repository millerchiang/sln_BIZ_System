﻿using IBatisNet.DataMapper;
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

    public class HomeController : _BaseController
    {

        public UserService userService;
        public Index_ViewModel indexModel;
        public User_ViewModel userModel;
        public ActivityService activityService;
        public PasswordService passwordService;
        public Password_ViewModel passwordViewModel;

        public HomeController()
        {
            userService = new UserService();
            activityService = new ActivityService();
            indexModel = new Index_ViewModel();
            userModel = new User_ViewModel();

            passwordService = new PasswordService();
            passwordViewModel = new Password_ViewModel();

        }

        public ActionResult SetCulture(string culture, string returnUrl)
        {
            // Validate input 
            culture = CultureHelper.GetImplementedCulture(culture);

            // Save culture in a cookie 
            HttpCookie cookie = Request.Cookies["_culture"];

            if (cookie != null)
            {
                // update cookie value 
                cookie.Value = culture;
            }
            else
            {
                // create cookie value 
                cookie = new HttpCookie("_culture");
                cookie.Value = culture;
                cookie.Expires = DateTime.Now.AddYears(1);
            }

            Response.Cookies.Add(cookie);
            return Redirect(returnUrl);
        }

        public ActionResult _HomeLeftPartial()
        {
            indexModel.userinfoList = userService.GetUserInfoList();
            indexModel.enterprisesortList = userService.GetSortList();
            indexModel.activityinfoList = activityService.GetActivityInfoListLimit(6);
            if (Request.Cookies["UserInfo"] != null)
            {
                ViewBag.logoDir = UploadHelper.getPictureDirPath(Request.Cookies["UserInfo"]["user_id"], "logo");
            }
            return PartialView(indexModel);
        }


        public ActionResult Index()
        {
            indexModel.newsList = activityService.GetNewsLimit(6);
            indexModel.cataloglistList = userService.getAllCatalogTop(4);
            ViewBag.coverDir = UploadConfig.UploadRootPath;

            foreach (NewsModel newsModel in indexModel.newsList)
            {
                    newsModel.content = HttpUtility.HtmlDecode(newsModel.content);
            }

            docookie("_mainmenu", "Index");
            return View(indexModel);
        }

        public ActionResult News()
        {

            if (Request["Type"] == null)
            {
                ViewBag.tname = LanguageResource.User.lb_latest_activitynews;
                indexModel.newsList = activityService.GetNewsAll(null).Pages(Request, this, 10);
            }
            else
            {
                if (Request["Type"] == "0")
                    ViewBag.tname = LanguageResource.User.lb_latest_activity;
                else
                    ViewBag.tname = LanguageResource.User.lb_latest_news;

                indexModel.newsList = activityService.GetNewsType(Request["Type"], null).Pages(Request, this, 10);
            }
            return View(indexModel);
        }

        public ActionResult Activity()
        {
            indexModel.activityinfoList = activityService.GetActivityInfoListNotStart(null).Pages(Request, this, 10);
            return View(indexModel);
        }


        public ActionResult Company()
        {
//            if (Request.Cookies["UserInfo"] != null)
//            {
                userModel.cataloglistList = userService.getAllCatalogTop(4);
                userModel.enterprisesortList = userService.GetSortList();
                ViewBag.coverDir = UploadConfig.UploadRootPath;

                return View(userModel);
 //           }
 //           else
 //               return Redirect("Login");
        }

        public ActionResult CompanyList()
        {
//            if (Request.Cookies["UserInfo"] != null)
//            {
                userModel.cataloglistList = userService.getAllCatalogTop(4);
                string sort_id = "";
                string kw = "";
                if (Request["companyName"]!= null)
                    kw = Request["companyName"];
                if (Request["sort_id"] != null)
                    sort_id = Request["sort_id"];

                if (sort_id != "")
                {
                    userModel.companysortList = userService.SelectUserSortBySortId(int.Parse(sort_id), kw);
                    ViewBag.model = "companysortList";
                }
                else
                {
                    userModel.userinfoList = userService.SelectUserKw(kw);
                    ViewBag.model = "userinfoList";
                }


                ViewBag.coverDir = UploadConfig.UploadRootPath;

                return View(userModel);
 //           }
 //           else
 //               return Redirect("Login");
        }

        
        public ActionResult NewsView()
        {
            if (Request["Id"] !=null)
            {
                doNewsView();

                return View(indexModel);
            }
            else
                return Redirect("Index");
        }

        //與App共用內容
        private void doNewsView()
        {
            indexModel.news = activityService.GetNewsOne(int.Parse(Request["Id"]));
            indexModel.news.content = HttpUtility.HtmlDecode(indexModel.news.content);
        }

        public ActionResult NewsViewForApp(string nvkey)
        {
            string code = "BizNewsContent"+ DateTime.Now.ToString("yyyyMMdd");
            if (!SecurityHelper.Encrypt256(code).Equals(nvkey,StringComparison.CurrentCultureIgnoreCase))
            {
                return Content("很抱歉!您沒有觀看這則新聞的權限");
            }
            else
            {
                doNewsView();
                return View(indexModel);
            }

        }

        public ActionResult Logout()
        {

            Session.Clear();
            HttpCookie aCookie;
            string cookieName;
//            int limit = Request.Cookies.Count;
//            for (int i = 0; i < limit; i++)
            {
                cookieName = "UserInfo";// Request.Cookies[i].Name;
                aCookie = new HttpCookie(cookieName);
                aCookie.Expires = DateTime.Now.AddDays(-1);
                Response.Cookies.Add(aCookie);
            }
            return Redirect("Index");
        }


        public ActionResult ReAccountMailValidate()
        {
            string user_id = Request["user_id"];
            string name = Request["name"];
            string email = Request["email"];
            var id = userService.GeUserInfoOne(user_id).id;
            MailHelper.sendAccountMailValidate(id, user_id, email);

            string remail_Msg = "重發驗證信完成!!";
            TempData["remail_Msg"] = remail_Msg;

            return Redirect("Verification?user_id=" + user_id + "&name=" + name + "&email=" + email);

        }


        public ActionResult IdentifyUser()
        {
            var securityPassword = SecurityHelper.Encrypt256(Request["user_pw"]);
            UserInfoModel model = userService.ChkUserInfoOne(Request["user_id"], securityPassword);

            HttpCookie cookie = null;

            if (model == null)
            {
                TempData["pw_errMsg"] = LanguageResource.User.lb_accountpw_wrong;
                return Redirect("Index");
            }
            else
            {
//                cookie = new HttpCookie("UserInfo");
//                cookie.Values.Add("id_enable", model.id_enable);

                //                Response.Cookies["UserInfo"]["id_enable"] = model.id_enable;
                //                Response.Cookies["UserInfo"]["user_id"] = model.user_id;
                if (model.id_enable == "0")
                {
                    //                    Response.AppendCookie(cookie);
                    //                    return Redirect("../User/register?user_id=" + model.user_id);
                    return Redirect("Verification?user_id=" + model.user_id + "&name=" + model.company + "&email=" + model.email);

                }

                cookie = new HttpCookie("UserInfo");
                cookie.Values.Add("id_enable", model.id_enable);
                cookie.Values.Add("user_id", model.user_id);
                cookie.Values.Add("company", HttpUtility.UrlEncode(model.company));
                cookie.Values.Add("website", model.website);
                cookie.Values.Add("info", HttpUtility.UrlEncode(model.info));
                cookie.Values.Add("info_en", model.info_en);
                cookie.Values.Add("logo_img", HttpUtility.UrlEncode(model.logo_img));
                Response.AppendCookie(cookie);

                //                Response.Cookies["UserInfo"]["company"] = model.company;
                //                Response.Cookies["UserInfo"]["website"] = model.website;
                //                Response.Cookies["UserInfo"]["info"] = model.info;
            }
            return Redirect("Index");
        }


        public ActionResult Verification()
        {
            string user_id = Request["user_id"];
            string name = Request["name"];
            string email = Request["email"];

            if (user_id==null || email==null)
                return Redirect("Index");
            else
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

        public ActionResult ForgetPassword()
        {

            return View();
        }

        //忘記密碼 只有前端有
        public ActionResult ReSetPassword(string user_id, string email)
        {

            string errMsg = "新的註冊密碼通知信已寄出，請至你註冊填寫的信箱收取!!";
            UserInfoModel md = passwordService.SelectOneByIdEmail(user_id, email);
            if (md != null)
            {
                string new_pw = MailHelper.sendForgetPassword(md.email);
                var securityPassword = SecurityHelper.Encrypt256(new_pw);
                bool isUpdateSuccess = passwordService.UpdateUserPassword(md.user_id, securityPassword);
                if (!isUpdateSuccess)
                {
                    errMsg = "新的註冊密碼通知信更新失敗，請重新操作!!";
                    TempData["fp_errMsg"] = errMsg;
                    return Redirect("ForgetPassword");
                }
            }
            else
            {
                errMsg = "輸入的資料不正確，請重新操作!!";
                TempData["fp_errMsg"] = errMsg;
                return Redirect("ForgetPassword");
            }

            TempData["fp_errMsg"] = errMsg;

            return Redirect("Index");
        }
    }
}