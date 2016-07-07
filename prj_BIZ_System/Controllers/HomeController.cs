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


        public ActionResult Index()
        {

            if (Request.Cookies["UserInfo"] != null)
            {
                indexModel.enterprisesortList = userService.GetSortList();
                indexModel.userinfoList = userService.GetUserInfoList();
                indexModel.activityinfoList = activityService.GetActivityInfoListLimit(6); 
                indexModel.newsList = activityService.GetNewsLimit(6);
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

                if (Request["Type"] == null)
                {
                    ViewBag.tname = "最新消息";
                    indexModel.newsList = activityService.GetNewsAll(null).Pages(Request, this, 10);
                }
                else
                {
                    if (Request["Type"]=="0")
                        ViewBag.tname = "活動消息";
                    else
                        ViewBag.tname = "最新新聞";

                    indexModel.newsList = activityService.GetNewsType(Request["Type"],null).Pages(Request, this, 10);
                }


                return View(indexModel);
            }
            else
                return Redirect("Login");
        }

        public ActionResult Activity()
        {
            if (Request.Cookies["UserInfo"] != null)
            {
                indexModel.activityinfoList = activityService.GetActivityInfoList(null).Pages(Request, this, 10);
                return View(indexModel);
            }
            else
                return Redirect("Login");
        }


        public ActionResult Company()
        {
            if (Request.Cookies["UserInfo"] != null)
            {
                userModel.cataloglistList = userService.getAllCatalogTop(4);
                userModel.enterprisesortList = userService.GetSortList();
                ViewBag.coverDir = UploadConfig.CatalogRootPath;

                return View(userModel);
            }
            else
                return Redirect("Login");
        }

        public ActionResult CompanyList()
        {
            if (Request.Cookies["UserInfo"] != null)
            {
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


                ViewBag.coverDir = UploadConfig.CatalogRootPath;

                return View(userModel);
            }
            else
                return Redirect("Login");
        }

        
        public ActionResult NewsView()
        {
            if (Request.Cookies["UserInfo"] != null && Request["Id"] !=null)
            {
                doNewsView();

                return View(indexModel);
            }
            else
                return Redirect("Login");
        }

        //與App共用內容
        private void doNewsView()
        {
            indexModel.news = activityService.GetNewsOne(int.Parse(Request["Id"]));
            indexModel.news.content = HttpUtility.HtmlDecode(indexModel.news.content);
        }

        public ActionResult NewsViewForApp()
        {
            doNewsView();
            return View(indexModel);
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
            return Redirect("Login");
        }



        public ActionResult Login()
        {
            return View();
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
            UserInfoModel model = userService.ChkUserInfoOne(Request["user_id"], Request["user_pw"]);

            HttpCookie cookie = null;

            if (model == null)
            {
                TempData["pw_errMsg"] = "帳號或密碼錯誤!!";
                return Redirect("Login");
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
                bool isUpdateSuccess = passwordService.UpdateUserPassword(md.user_id, new_pw);
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

            return Redirect("Login");
        }
    }
}