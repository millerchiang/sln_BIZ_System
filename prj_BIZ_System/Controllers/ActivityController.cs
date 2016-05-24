using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using prj_BIZ_System.Models;
using prj_BIZ_System.Services;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

        public ActionResult ActivityList()
        {
            activityModel.activityinfoList = activityService.GetActivityInfoList();
            return View(activityModel);
        }

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

            //            model.activity_id = int.Parse(Request["activity_id"]);
            /*
                model.activity_type = Request["activity_type"];
                model.activity_name = Request["activity_name"];
                model.starttime = DateTime.Parse(Request["starttime"]);
                model.endtime = DateTime.Parse(Request["endtime"]);
                model.addr = Request["addr"];
                model.organizer = Request["organizer"];
                model.name = Request["name"];
                model.phone = Request["phone"];
                model.email = Request["email"];
                model.seller_select = Request["seller_select"];
                model.matchmaking_select = Request["matchmaking_select"];
                model.activity_name_en = Request["activity_name_en"];
                model.addr_en = Request["addr_en"];
                model.organizer_en = Request["organizer_en"];
            */
            model.manager_id = Request.Cookies["UserInfo"]["user_id"];

            if (model.activity_id == 0)
                activityService.ActivityInfoInsertOne(model);
            else
            {
                model.update_time = DateTime.Now;
                activityService.ActivityInfoUpdateOne(model);

            }
            return Redirect("ActivityList");

            //            return Content("修改失敗");
        }

        //////News
        /* 新聞列表*/
        [HttpGet]
        public ActionResult B_NewsList()
        {
            activityModel.newsList = activityService.GetNewsAll();
            return View(activityModel);
        }

        /* 新增活動訊息*/
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
            //            NewsModel news = model.news;
            //            news.news_no = int.Parse(Request["news_no"]);
            /*
                        model.news_title = Request["news_title"];
                        model.news_date = DateTime.Parse(Request["news_date"]);
                        model.activity_id = int.Parse(Request["activity_id"]);
                        model.news_type = Request["news_type"];
                        model.content = Request["content"];
            */
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

        /*新增新聞訊息*/
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

        ////BuyerInfo
        /*買主資訊列表*/
        [HttpGet]
        public ActionResult BuyerInfoList()
        {
            activityModel.buyerinfoList = activityService.GetBuyerInfoAll();
            return View(activityModel);
               
        }

        /*新增買主資訊*/
        [HttpGet]
        public ActionResult EditBuyerInfo()
        {
            activityModel.userinfotoidandcpList = activityService.GetUserInfoToIdandCp();
            activityModel.activityinfoList = activityService.GetActivityInfoList();
            ViewBag.Action = "EditBuyerInfoInsertUpdate";
            if(Request["Id"] == null)
            {
                activityModel.buyerinfo = new BuyerInfoModel();
                ViewBag.PageType = "Create";
                ViewBag.SubmitName = "新增";
            }
            else{
                activityModel.buyerinfo = activityService.GetBuyerInfoOne(int.Parse(Request["Id"]));
                ViewBag.PageType = "Edit";
                ViewBag.SubmitName = "修改";
            }
            return View(activityModel);
        }

        [HttpPost]
        public ActionResult EditBuyerInfoInsertUpdate(BuyerInfoModel model)
        {
            if(model.serial_no == 0)
            {
                activityService.BuyerInfoInsertOne(model);
            }
            else{
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

        //////ActivityRegister
        [HttpGet]
        public ActionResult EditRegister(ActivityRegisterModel registerModel)
        {
            activityModel.activityinfoList = activityService.GetActivityInfoList();
            registerModel.user_id = Request.Cookies["UserInfo"]["user_id"];
            activityModel.activityregister = new ActivityRegisterModel();
            activityModel.activityregister.user_id = registerModel.user_id;
            return View(activityModel);
        }


    }
}