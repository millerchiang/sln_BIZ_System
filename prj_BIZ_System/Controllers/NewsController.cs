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
            if (Request["Id"] == null)
            {
                newsModel = new NewsModel();
                viewModels.ActivityInfoList =  activityService.GetActivityInfoList();
                viewModels.NewsModel = newsModel;
                ViewBag.NextAction = "EditActivityInfoInsert";
                ViewBag.NextName = "新增";       
            }
            else {
                newsModel = new NewsModel();
                newsModel.news_no = int.Parse(Request["Id"]);
                newsModel = newsService.GetNewsOne(newsModel.news_no);
                viewModels.NewsModel = newsModel;
                ViewBag.NextAction = "EditActivityInfoUpdate";
                ViewBag.NextName = "修改";
            }
            return View(viewModels);
        }

        [HttpPost]
        public ActionResult EditActivityInfoInsert()
        {
            if (Request["news_type"] != null)
            {
                NewsModel model = new NewsModel();
                model.news_title = Request["news_title"];
                model.news_date = DateTime.Parse(Request["news_date"]);
                model.news_type = Request["news_type"];
                model.activity_id = int.Parse(Request["activity_id"]);
                model.manager_id = Request["manager_id"];
                model.content = Request["content"];
                newsService.InsertOne(model);
            }
            return Redirect("B_NewsList");
        }

        [HttpPost]
        public ActionResult EditActivityInfoUpdate()
        {
            NewsModel model = new NewsModel();
            model.news_no = int.Parse(Request["news_no"]);
            model.news_title = Request["news_title"];
            model.news_date = DateTime.Parse(Request["news_date"]);
            model.activity_id = int.Parse(Request["activity_id"]);
            model.manager_id = Request["manager_id"];
            model.content = Request["content"];
            newsService.UpdateOne(model);
            return Redirect("B_NewsList");
        }

        /*新增新聞訊息*/
        [HttpGet]
        public ActionResult EditNewsInfo()
        {
            NewsModel model;
            if (Request["Id"] == null)
            {
                model = new NewsModel();
                ViewBag.NextAction = "Insert";
                ViewBag.NextName = "新增";
                ViewBag.NextNewsType = "";
            }
            else {
                model = new NewsModel();
                ViewBag.NextAction = "Insert";
                ViewBag.NextName = "修改";
                ViewBag.NextNewsType = "checked";
            }
            return View(model);
        }
    }
}