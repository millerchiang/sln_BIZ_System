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
using System.Xml.Linq;

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
//            IList<EnterpriseSortListModel> result;
            indexModel.enterprisesortList = userService.GetSortList();
            indexModel.userinfoList = userService.GetUserInfoList();
            indexModel.activityinfoList = activityService.GetActivityInfoListLimit(6);
            indexModel.videolistList = userService.getAllVideoTop(1);

            var isCacheON = CacheConfig._NavSearchPartial_load_cache_isOn;
            if (isCacheON)
            {
                if (CacheDataStore.EnterpriseSortListModelCache == null)
                {
                    CacheDataStore.EnterpriseSortListModelCache = userService.GetSortList();
                }
                indexModel.enterprisesortList = CacheDataStore.EnterpriseSortListModelCache;
            }
            else
            {
                indexModel.enterprisesortList = userService.GetSortList();
            }

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

            docookie("_version", "3.0");

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
            docookie("_mainmenu", "News");
            return View(indexModel);
        }

        public ActionResult Activity()
        {
            indexModel.activityinfoList = activityService.GetActivityInfoListNotStart(null).Pages(Request, this, 10);
            docookie("_mainmenu", "Activity");
            return View(indexModel);
        }


        public ActionResult Company()
        {
            userModel.cataloglistList = userService.getAllCatalogTop(4);
            userModel.enterprisesortList = userService.GetSortList();
            ViewBag.coverDir = UploadConfig.UploadRootPath;

            docookie("_mainmenu", "Company");
            return View(userModel);
        }

        public ActionResult CompanyList()
        {
//            userModel.cataloglistList = userService.getAllCatalogTop(4);
            string sort_id = "";
            string kw = "";
            string productname = "";
            string catalogname = "";
            if (Request["companyName"]!= null)
                kw = Request["companyName"];
            if (Request["sort_id"] != null)
                sort_id = Request["sort_id"];
            if (Request["productname"] != null)
                productname = Request["productname"];
            if (Request["catalogname"] != null)
                catalogname = Request["catalogname"];

            if (sort_id != "")
            {
                EnterpriseSortListModel scope = userService.GetSortById(int.Parse(sort_id));
                userModel.companysortList = userService.SelectUserSortBySortId(int.Parse(sort_id), kw);
                ViewBag.model = "companysortList";
                if (Request.Cookies["_culture"] != null && Request.Cookies["_culture"].Value != "zh-TW")
                {
                    ViewBag.keyword = scope.enterprise_sort_name_en;
                }
                else
                {
                    ViewBag.keyword = scope.enterprise_sort_name;
                }
            }
            else if (kw!="")
            {
                userModel.userinfoList = userService.SelectUserKw(kw);
                ViewBag.model = "userinfoList";
                ViewBag.keyword = kw;
            }
            else if (productname != "")
            {
//                userModel.userinfoList = userService.SelectUserByProductName(productname);
                userModel.productsortList = userService.getProductListByKw(productname).Pages<ProductListModel>(Request, this, 10);
                ViewBag.model = "productList";
                ViewBag.keyword = productname;
            }
            else if (catalogname != "")
            {
//                userModel.userinfoList = userService.SelectUserByCatalogName(catalogname);
                userModel.cataloglistList = userService.getCatalogListByKw(catalogname);
                ViewBag.model = "catalogList";
                ViewBag.keyword = catalogname;
            }

            ViewBag.UploadRootPath = UploadConfig.UploadRootPath;

            docookie("_mainmenu", "CompanyList");
            return View(userModel);
        }

        
        public ActionResult NewsView()
        {
            if (Request["Id"] !=null)
            {
                doNewsView();
                
                docookie("_mainmenu", "NewsView");
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

            string remail_Msg = LanguageResource.User.lb_verifacationresent;
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
                TempData["userpw_errMsg"] = LanguageResource.User.lb_accountpw_wrong;
                return Redirect("Index");
            }
            else
            {
                if (model.id_enable == "0")
                {
                    return Redirect("Verification?user_id=" + model.user_id + "&name=" + model.company + "&email=" + model.email);
                }

                cookie = new HttpCookie("UserInfo");
                cookie.Values.Add("id_enable", model.id_enable);
                cookie.Values.Add("user_id", model.user_id);
                cookie.Values.Add("company", HttpUtility.UrlEncode(model.company));
                cookie.Values.Add("company_en", HttpUtility.UrlEncode(model.company_en));
//                cookie.Values.Add("website", model.website);
//                cookie.Values.Add("info", HttpUtility.UrlEncode(model.info));
//                cookie.Values.Add("info_en", model.info_en);
                cookie.Values.Add("logo_img", HttpUtility.UrlEncode(model.logo_img));
                Response.AppendCookie(cookie);

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


        //public ActionResult MailValidateResult()
        //{
        //    var result = TempData["MailValidateResult"];
        //    if(result == null)
        //    {
        //        result = "";
        //    }
        //    return Json(result, JsonRequestBehavior.AllowGet);
        //}

        public ActionResult ForgetPassword()
        {

            return View();
        }

        //忘記密碼 只有前端有
        public ActionResult ReSetPassword(string user_id, string email)
        {

            string errMsg = LanguageResource.User.lb_newpwemail;
            UserInfoModel md = passwordService.SelectOneByIdEmail(user_id, email);
            if (md != null)
            {
                string new_pw = MailHelper.sendForgetPassword(md.email);
                var securityPassword = SecurityHelper.Encrypt256(new_pw);
                bool isUpdateSuccess = passwordService.UpdateUserPassword(md.user_id, securityPassword);
                if (!isUpdateSuccess)
                {
                    errMsg = LanguageResource.User.lb_pwmailfail;
                    TempData["fp_errMsg"] = errMsg;
                    return Redirect("ForgetPassword");
                }
            }
            else
            {
                errMsg = LanguageResource.User.lb_data_wrong;
                TempData["fp_errMsg"] = errMsg;
                return Redirect("ForgetPassword");
            }

            TempData["fp_errMsg"] = errMsg;

            return Redirect("Index");
        }

        public ActionResult AboutUs()
        {
            docookie("mainclass", "main2");
            return View();
        }

        public ActionResult AppDownload()
        {
            docookie("mainclass", "main2 appbox");
            return View();
        }

        public ActionResult Errfile()
        {
            return View();
        }

        public ActionResult Privacy()
        {
            return View();
        }

        public ActionResult Servicepolicy()
        {
            return View();
        }

        public ActionResult Support()
        {
            return View();
        }

        public ActionResult Faq()
        {
            return View();
        }

    }
}