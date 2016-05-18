using prj_BIZ_System.Models;
using prj_BIZ_System.Services;
using prj_BIZ_System.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace prj_BIZ_System.Controllers
{
    public class NewsController : Controller
    {
        private ActivityService activityService;
        private NewsService newsService;

        public NewsController()
        {
            activityService = new ActivityService();
            newsService = new NewsService();
        }

        /* 新聞列表*/
        [HttpGet]
        public ActionResult B_NewsList()
        {
            News_BNList_ViewModel viewModels = new News_BNList_ViewModel();
            viewModels.NewsList = newsService.GetNewsAll();
            return View(viewModels);
        }

        /* 新增活動訊息*/
        [HttpGet]
        public ActionResult EditActivityInfo()
        {
            NewsModel newsModel;
            News_EAInfo_ViewModel viewModels = new News_EAInfo_ViewModel();
            viewModels.ActivityInfoList = activityService.GetActivityInfoList();
            if (Request["Id"] == null)
            {
                newsModel = new NewsModel();
                viewModels.NewsModel = newsModel;
                ViewBag.PageType = "Create";
                ViewBag.NextAction = "EditActivityInfoInsert";
                ViewBag.NextName = "新增";       
            }
            else {
                newsModel = new NewsModel();
                newsModel.news_no = int.Parse(Request["Id"]);
                newsModel = newsService.GetNewsOne(newsModel.news_no);
                viewModels.NewsModel = newsModel;
                ViewBag.PageType = "Edit";
                ViewBag.NextAction = "EditActivityInfoUpdate";
                ViewBag.NextName = "修改";
            }
            return View(viewModels);
        }

        [HttpPost]
        public ActionResult EditActivityInfoInsert()
        {
            if (Request["manager_id"] != null)
            {
                NewsModel newsModel = new NewsModel();
                newsModel.news_title = Request["news_title"];
                newsModel.news_date = DateTime.Parse(Request["news_date"]);
                newsModel.news_type = Request["news_type"];
                newsModel.activity_id = int.Parse(Request["activity_id"]);
                newsModel.manager_id = Request.Cookies["UserInfo"]["user_id"];
                newsModel.content = Request["content"];
                newsService.InsertOne(newsModel);
            }
            return Redirect("B_NewsList");
        }

        [HttpPost]
        public ActionResult EditActivityInfoUpdate()
        {
            NewsModel newsModel = new NewsModel();
            newsModel.news_no = int.Parse(Request["news_no"]);
            newsModel.news_title = Request["news_title"];
            newsModel.news_date = DateTime.Parse(Request["news_date"]);
            newsModel.activity_id = int.Parse(Request["activity_id"]);
            newsModel.news_type = Request["news_type"];
            newsModel.manager_id = Request.Cookies["UserInfo"]["user_id"];
            newsModel.content = Request["content"];
            newsService.UpdateOne(newsModel);
            return Redirect("B_NewsList");
        }

        [HttpGet]
        public ActionResult EditActivityInfoDelete()
        {
            NewsModel newsModel = new NewsModel();
            newsModel.news_no = int.Parse(Request["Id"]);
            newsService.DeleteOne(newsModel);
            return Redirect("B_NewsList");
        }

        /*新增新聞訊息*/
        [HttpGet]
        public ActionResult EditNewsInfo()
        {
            NewsModel newsModel;
            if (Request["Id"] == null)
            {
                newsModel = new NewsModel();
                ViewBag.PageType = "Create";
                ViewBag.NextAction = "EditNewsInfoInsert";
                ViewBag.NextName = "新增";
            }
            else {
                newsModel = new NewsModel();
                newsModel.news_no = int.Parse(Request["Id"]);
                newsModel = newsService.GetNewsOne(newsModel.news_no);
                ViewBag.PageType = "Edit";
                ViewBag.NextAction = "EditNewsInfoUpdate";
                ViewBag.NextName = "修改";
            }
            return View(newsModel);
        }

        [HttpPost]
        public ActionResult EditNewsInfoInsert()
        {
            if (Request["manager_id"] != null)
            {
                NewsModel newsModel = new NewsModel();
                newsModel.news_title = Request["news_title"];
                newsModel.news_date = DateTime.Parse(Request["news_date"]);
                newsModel.website = Request["website"];
                newsModel.news_type = Request["news_type"];
                newsModel.manager_id = Request.Cookies["UserInfo"]["user_id"];
                newsModel.content = Request["content"];
                newsService.InsertOne(newsModel);
            }
            return Redirect("B_NewsList");
        }

        [HttpPost]
        public ActionResult EditNewsInfoUpdate()
        {
            NewsModel newsModel = new NewsModel();
            newsModel.news_no = int.Parse(Request["news_no"]);
            newsModel.news_title = Request["news_title"];
            newsModel.news_date = DateTime.Parse(Request["news_date"]);
            newsModel.website = Request["website"];
            newsModel.news_type = Request["news_type"];
            newsModel.manager_id = Request.Cookies["UserInfo"]["user_id"];
            newsModel.content = Request["content"];
            newsService.UpdateOne(newsModel);
            return Redirect("B_NewsList");
        }

        [HttpGet]
        public ActionResult EditNewsInfoDelete()
        {
            NewsModel newsModel = new NewsModel();
            newsModel.news_no = int.Parse(Request["Id"]);
            newsService.DeleteOne(newsModel);
            return Redirect("B_NewsList");
        }
    }
}