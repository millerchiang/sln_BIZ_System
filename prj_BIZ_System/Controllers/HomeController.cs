using IBatisNet.DataMapper;
using prj_BIZ_System.App_Start;
using prj_BIZ_System.Models;
using prj_BIZ_System.Services;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;
using prj_BIZ_System.Extensions;
using System.Web.Script.Serialization;

namespace prj_BIZ_System.Controllers
{

    public class HomeController : _BaseController
    {

        public UserService userService;
        public SalesService salesService;
        public Index_ViewModel indexModel;
        public User_ViewModel userModel;
        public ActivityService activityService;
        public PasswordService passwordService;
        public Password_ViewModel passwordViewModel;

        public HomeController()
        {
            userService = new UserService();
            salesService = new SalesService();
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

            indexModel.videolistList = userService.getVideoListActive();// getAllVideoTop(1);

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
            indexModel.activityphotoList = activityService.getActivityViewPhoto();
            indexModel.bannerphotoList = activityService.getBannerViewPhoto();
            indexModel.cataloglistList = userService.getAllCatalogTop(4);

            if (Request.Cookies["_culture"] != null && Request.Cookies["_culture"].Value != "zh-TW")
            {
                indexModel.newsList = activityService.GetNewsLimit_e(6);
            }
            else
            {
                indexModel.newsList = activityService.GetNewsLimit(6);
            }


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
            string news_style = "1";
            if (Request.Cookies["_culture"] != null && Request.Cookies["_culture"].Value != "zh-TW")
            {
                news_style = "2";
            }

            if (Request["Type"] == null)
            {
                ViewBag.tname = LanguageResource.User.lb_latest_activitynews;
                indexModel.newsList = activityService.GetNewsAll(null, news_style).Pages(Request, this, 10);
            }
            else
            {
                if (Request["Type"] == "0")
                    ViewBag.tname = LanguageResource.User.lb_latest_activity;
                else
                    ViewBag.tname = LanguageResource.User.lb_latest_news;

                indexModel.newsList = activityService.GetNewsTypeView(Request["Type"], null,news_style).Pages(Request, this, 10);
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
                ViewBag.keyword = LanguageResource.Localization.getPropValue(Request.Cookies["_culture"], scope, "enterprise_sort_name");
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

        public ActionResult ActivityPhotoView()
        {
            if (Request["Id"] != null)
            {
                indexModel.activityphoto = activityService.getPhotoOne(int.Parse(Request["Id"]));
                ViewBag.photoDir = UploadHelper.getPictureDirPath(indexModel.activityphoto.manager_id, "activity");
                docookie("_mainmenu", "ActivityPhotoView");
                return View(indexModel);
            }
            else
                return Redirect("Index");
        }

        public ActionResult NewsView()
        {
            if (Request["Id"] !=null)
            {
                doNewsView();

                indexModel.buyerinfoList = activityService.GetBuyerInfoActivity(indexModel.news.activity_id);
                if (indexModel.buyerinfoList.Count==0)
                {
                    ViewBag.buyerinfo = "none";
                } else
                {
                    ViewBag.buyerinfo = "ok";
                }
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
            //replaceImgSrcParamToUrlContent();
        }

        private void replaceImgSrcParamToUrlContent()
        {
            var replacePattern = "src=[\"'](.+?)[\"'].*?";
            string matchString = Regex.Match(indexModel.news.content, replacePattern, RegexOptions.IgnoreCase).Groups[1].Value;
            if (!matchString.IsNullOrEmpty() && matchString[0]=='/')
            {
                indexModel.news.content = Regex.Replace(indexModel.news.content, replacePattern, "src=\"" + Url.Content("~/" + matchString) + "\"");
            }
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

        public ActionResult NewsViewForAppByLocale(string nvkey, string locale)
        {
            string code = "BizNewsContent" + DateTime.Now.ToString("yyyyMMdd");
            if (!SecurityHelper.Encrypt256(code).Equals(nvkey, StringComparison.CurrentCultureIgnoreCase))
            {
                return Content("很抱歉!您沒有觀看這則新聞的權限");
            }
            else
            {
                ViewBag.locale = locale;
                doNewsView();
                return View(indexModel);
            }

        }

        public ActionResult Logout()
        {
            clearcookie("UserInfo");
            return Redirect("Index");
        }

        public ActionResult LogoutForSales()
        {

            clearcookie("SalesInfo");
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

                model.last_login_time = DateTime.Now;
                userService.UserInfoUpdateOne(model);

                clearcookie("SalesInfo"); 
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

        public ActionResult IdentifyUserForSales()
        {
            //var securityPassword = SecurityHelper.Encrypt256(Request["sales_pw"]);
            var securityPassword = Request["sales_pw"];
            string sales_id = Request["sales_id"];
            SalesInfoModel model = salesService.ChkSalesInfoOne(Request["sales_id"], securityPassword);

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
                    TempData["userpw_errMsg"] = "此帳號停用中" ;
                    return Redirect("Index");
                }


                Dictionary<string, string> limitsDict = new Dictionary<string, string>();
                limitsDict = new JavaScriptSerializer().Deserialize<Dictionary<string, string>>(model.limit);

                UserInfoModel userinfo = userService.GeUserInfoOneBySales(sales_id);
                clearcookie("UserInfo");
                cookie = new HttpCookie("SalesInfo");
                cookie.Values.Add("id_enable", model.id_enable);
                cookie.Values.Add("sales_id", model.sales_id);
                cookie.Values.Add("sales_name", HttpUtility.UrlEncode(model.sales_name));
                cookie.Values.Add("limit_of_company", limitsDict["company"]);
                cookie.Values.Add("limit_of_video", limitsDict["video"]);
                cookie.Values.Add("limit_of_sales", limitsDict["sales"]);
                cookie.Values.Add("limit_of_message", limitsDict["message"]);
                cookie.Values.Add("phone", model.phone);
                cookie.Values.Add("email", model.email);
                cookie.Values.Add("user_id", model.user_id);
                cookie.Values.Add("company", HttpUtility.UrlEncode(userinfo.company));
                cookie.Values.Add("company_en", HttpUtility.UrlEncode(userinfo.company_en));
                //                cookie.Values.Add("website", model.website);
                //                cookie.Values.Add("info", HttpUtility.UrlEncode(model.info));
                //                cookie.Values.Add("info_en", model.info_en);
                //cookie.Values.Add("logo_img", HttpUtility.UrlEncode(model.logo_img));
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

        public ActionResult LatestVideoList()
        {

            IList<VideoListModel> videoLists = userService.getVideoListAll().Pages(Request, this, 10);
            docookie("_mainmenu", "LatestVideoList");
            return View(videoLists);
        }
        public ActionResult LatestCatalogList()
        {

            IList<CatalogListModel> catalogLists = userService.getAllCatalog(null).Pages(Request, this, 12);
            ViewBag.coverDir = UploadConfig.UploadRootPath;
            docookie("_mainmenu", "LatestCatalogList");
            return View(catalogLists);
        }


    }
}