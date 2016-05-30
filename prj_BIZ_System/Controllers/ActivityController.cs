using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using prj_BIZ_System.App_Start;
using prj_BIZ_System.Models;
using prj_BIZ_System.Services;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace prj_BIZ_System.Controllers
{
    public class ActivityController : Controller
    {
        public ActivityService activityService;
        public UserService userService;
        public Activity_ViewModel activityModel;

        public ActivityController()
        {
            activityService = new ActivityService();
            userService = new UserService();
            activityModel = new Activity_ViewModel();
        }

        ////Activityinfo
        #region 活動列表
        public ActionResult ActivityList()
        {
            activityModel.activityinfoList = activityService.GetActivityInfoList();
            return View(activityModel);
        }
        #endregion

        #region 新增修改刪除活動
        [HttpGet]
        public ActionResult EditActivity()
        {

            ViewBag.Action = "ActivityInsertUpdate";
            if (Request["Id"] == null)
            {
                activityModel.activityinfo = new ActivityInfoModel();
                ViewBag.PageType = "Create";
                ViewBag.SubmitName = "新增";
            }
            else {
                activityModel.activityinfo = activityService.GetActivityInfoOne(int.Parse(Request["Id"]));
                ViewBag.PageType = "Edit";
                ViewBag.SubmitName = "修改";
            }

            return View(activityModel.activityinfo);
        }

        public ActionResult DeleteActivity()
        {
            activityService.ActivityInfoDelectOne(int.Parse(Request["Id"]));
            return Redirect("ActivityList");
        }

        [HttpPost]
        public ActionResult ActivityInsertUpdate(ActivityInfoModel model)
        {
            model.manager_id = Request.Cookies["UserInfo"]["user_id"];

            if (model.activity_id == 0)
                activityService.ActivityInfoInsertOne(model);
            else
            {
                model.update_time = DateTime.Now;
                activityService.ActivityInfoUpdateOne(model);

            }
            return Redirect("ActivityList");
        }
        #endregion

        ////News
        #region 新聞列表*/
        [HttpGet]
        public ActionResult B_NewsList()
        {
            activityModel.newsList = activityService.GetNewsAll();
            foreach (NewsModel newsModel in activityModel.newsList)
            {
                if (newsModel.news_type == "1")
                {
                    newsModel.content = HttpUtility.HtmlDecode(newsModel.content);
                }
            }
            return View(activityModel);
        }
        #endregion

        #region 新增修改刪除活動訊息
        [HttpGet]
        public ActionResult EditNewsActivity()
        {
            activityModel.activityinfoList = activityService.GetActivityInfoList();
            ViewBag.Action = "EditNewsActivityInsertUpdate";
            if (Request["Id"] == null)
            {
                activityModel.news = new NewsModel();
                ViewBag.PageType = "Create";
                ViewBag.SubmitName = "新增";
            }
            else {

                activityModel.news = activityService.GetNewsOne(int.Parse(Request["Id"]));
                ViewBag.PageType = "Edit";
                ViewBag.SubmitName = "修改";
            }
            return View(activityModel);
        }

        [HttpPost]
        public ActionResult EditNewsActivityInsertUpdate(NewsModel model)
        {
            model.manager_id = Request.Cookies["UserInfo"]["user_id"];

            if (model.news_no == 0)
                activityService.NewsInsertOne(model);
            else
                activityService.NewsUpdateOne(model);

            return Redirect("B_NewsList");
        }

        [HttpGet]
        public ActionResult EditNewsActivityDelete()
        {
            activityService.NewsDeleteOne(int.Parse(Request["Id"]));
            return Redirect("B_NewsList");
        }
        #endregion

        #region 新增修改刪除新聞訊息
        [HttpGet]
        public ActionResult EditNewsInfo()
        {
            ViewBag.Action = "EditNewsInfoInsertUpdate";
            if (Request["Id"] == null)
            {
                activityModel.news = new NewsModel();
                ViewBag.PageType = "Create";
                ViewBag.SubmitName = "新增";
            }
            else {
                //                activityModel.news.news_no = int.Parse(Request["Id"]);
                activityModel.news = activityService.GetNewsOne(int.Parse(Request["Id"]));
                activityModel.news.content = HttpUtility.HtmlDecode(activityModel.news.content);
                ViewBag.PageType = "Edit";
                ViewBag.SubmitName = "修改";
            }
            return View(activityModel.news);
        }

        [HttpPost]
        public ActionResult EditNewsInfoInsertUpdate(NewsModel news)
        {
            //            activityModel.news.news_no = int.Parse(Request["news_no"]);
            /*
                        news.news_title = Request["news_title"];
                        news.news_date = DateTime.Parse(Request["news_date"]);
                        news.website = Request["website"];
                        news.news_type = Request["news_type"];
                        news.content = Request["content"];
            */
            news.manager_id = Request.Cookies["UserInfo"]["user_id"];

            if (news.news_no == 0)
                activityService.NewsInsertOne(news);
            else
                activityService.NewsUpdateOne(news);

            return Redirect("B_NewsList");
        }

        [HttpGet]
        public ActionResult EditNewsInfoDelete()
        {
            activityService.NewsDeleteOne(int.Parse(Request["Id"]));
            return Redirect("B_NewsList");
        }
        #endregion

        #region 新聞文字編輯器圖片上傳
        public ActionResult NewsInfoUpload(HttpPostedFileBase upload, string CKEditorFuncNum)
        {
            string result = "";
            string user_id = Request.Cookies["UserInfo"]["user_id"];
            UploadHelper.doUploadFile(upload, UploadConfig.subDirForNews, user_id);

            var imageUrl = Url.Content(UploadConfig.CatalogRootPath + user_id + "/" + UploadConfig.subDirForNews + upload.FileName);

            var vMessage = string.Empty;

            result = @"<html><body><script>window.parent.CKEDITOR.tools.callFunction(" + CKEditorFuncNum + ", \"" + imageUrl + "\", \"" + vMessage + "\");</script></body></html>";
            return Content(result);
        }
        #endregion

        ////BuyerInfo
        #region 買主資訊列表
        [HttpGet]
        public ActionResult BuyerInfoList()
        {
            activityModel.buyerinfoList = activityService.GetBuyerInfoAll();
            return View(activityModel);

        }
        #endregion

        #region 新增修改刪除買主資訊*/
        [HttpGet]
        public ActionResult EditBuyerInfo()
        {
            activityModel.userinfotoidandcpList = activityService.GetUserInfoToIdandCp();
            activityModel.activityinfoList = activityService.GetActivityInfoList();
            ViewBag.Action = "EditBuyerInfoInsertUpdate";
            if (Request["Id"] == null)
            {
                activityModel.buyerinfo = new BuyerInfoModel();
                ViewBag.PageType = "Create";
                ViewBag.SubmitName = "新增";
            }
            else {
                activityModel.buyerinfo = activityService.GetBuyerInfoOne(int.Parse(Request["Id"]));
                ViewBag.PageType = "Edit";
                ViewBag.SubmitName = "修改";
            }
            return View(activityModel);
        }

        [HttpPost]
        public ActionResult EditBuyerInfoInsertUpdate(BuyerInfoModel model)
        {
            if (model.serial_no == 0)
            {
                activityService.BuyerInfoInsertOne(model);
            }
            else {
                activityService.BuyerInfoUpdateOne(model);
            }
            return Redirect("BuyerInfoList");
        }

        public ActionResult GetUserInfoToIdCp(string term)
        {
            activityModel.userinfotoidandcpList = activityService.GetUserInfoToIdandCp();
            ArrayList arrayList = new ArrayList();

            foreach (UserInfoToIdAndCpModel model in activityModel.userinfotoidandcpList)
            {
                arrayList.Add(model.user_id + "," + model.company);
            }

            string[] items = (string[])arrayList.ToArray(typeof(string));

            var filteredItems = items.Where(
                item => item.IndexOf(term,
                StringComparison.InvariantCultureIgnoreCase) >= 0
            );
            return Json(filteredItems, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult EditBuyerInfoDelete()
        {
            activityService.BuyerInfoDeleteOne(int.Parse(Request["Id"]));
            return Redirect("BuyerInfoList");
        }
        #endregion

        ////ActivityRegister
        #region 活動報名
        [HttpGet]
        public ActionResult EditActivityRegister()
        {
            activityModel.activityinfoList = activityService.GetActivityInfoList();
            ViewBag.Action = "EditActivityRegisterInsert";

            activityModel.activityregister = new ActivityRegisterModel();
            activityModel.activityregister.user_id = Request.Cookies["UserInfo"]["user_id"];

            activityModel.userinfo = new UserInfoModel();
            activityModel.userinfo.company = Request.Cookies["UserInfo"]["company"];
            activityModel.userinfo.website = Request.Cookies["UserInfo"]["website"];
            activityModel.userinfo.addr = Request.Cookies["UserInfo"]["addr"];
            activityModel.userinfo.info = Request.Cookies["UserInfo"]["info"];

            activityModel.enterprisesortandlistList = activityService.GetEnterpriseSortAndListOne(Request.Cookies["UserInfo"]["user_id"]);
            activityModel.productsortList = userService.getAllProduct(Request.Cookies["UserInfo"]["user_id"]);
            activityModel.cataloglistList = userService.getAllCatalog(Request.Cookies["UserInfo"]["user_id"]);

            ViewBag.coverDir = UploadConfig.CatalogRootPath + activityModel.activityregister.user_id + "/" +
                UploadConfig.subDirForCover;

            return View(activityModel);
        }
        [HttpPost]
        public ActionResult EditActivityRegisterInsert(ActivityRegisterModel activityRegisterModel, int[] product_id, int[] catalog_no)
        {
            activityRegisterModel.user_id = Request.Cookies["UserInfo"]["user_id"];
            activityRegisterModel.user_info = Request.Cookies["UserInfo"]["info"];
            activityRegisterModel.manager_check = "0";
            activityRegisterModel.create_time = DateTime.Now;
            activityService.ActivityRegisterInserOne(activityRegisterModel);

            ActivityProductSelectModel activityProductSelectModel = new ActivityProductSelectModel();
            activityProductSelectModel.user_id = activityRegisterModel.user_id;
            activityProductSelectModel.activity_id = activityRegisterModel.activity_id;
            
            foreach (int id in product_id)
            {
                activityProductSelectModel.product_id = id;
                activityService.ActivityProductInsertOne(activityProductSelectModel);
            }

            ActivityCatalogSelectModel activityCatalogSelectModel = new ActivityCatalogSelectModel();
            activityCatalogSelectModel.user_id = activityRegisterModel.user_id;
            activityCatalogSelectModel.activity_id = activityRegisterModel.activity_id;

            foreach (int no in catalog_no)
            {
                activityCatalogSelectModel.catalog_no = no;
                activityService.ActivityCatalogInsertOne(activityCatalogSelectModel);
            }

            return Content("新增成功");
        }

        public ActionResult selectedActivityId(int activity_id)
        {
            activityModel.activityinfo = activityService.GetActivityInfoOne(activity_id);
            return Json(activityModel.activityinfo, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region 活動報名審核
        public ActionResult ActivityRegisterCheck()
        {
            return View();
        }
        #endregion
    }
}