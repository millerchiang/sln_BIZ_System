using prj_BIZ_System.Models;
using prj_BIZ_System.Services;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace prj_BIZ_System.Controllers
{
    public class ActivityController : Controller
    {
        public ActivityService activityService;

        public ActivityController()
        {
            activityService = new ActivityService();
        }

        public ActionResult ActivityList()
        {
            ViewBag.Message = "Your application description page.";
            IList<ActivityInfoModel> activityInfoModels = activityService.GetActivityInfoList();
            ViewData["list"] = activityInfoModels;

            return View();
        }

        public ActionResult ActivityInsert()
        {
            if (Request["activity_name"] != null)
            {
                ActivityInfoModel model = new ActivityInfoModel();
                model.manager_id = Request.Cookies["UserInfo"]["user_id"];
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
                activityService.ActivityInfoInsertOne(model);
            }
            return Redirect("ActivityList");
        }

        [HttpGet]
        public ActionResult EditActivity()
        {
            ActivityInfoModel model = null;

            if (Request["Id"] != null)
            {
                int activity_id = int.Parse(Request["Id"]);
                model = activityService.GetActivityInfoOne(activity_id);
                ViewBag.user = model;
            }
            
            return View(model);
        }



        public ActionResult DeleteActivity()
        {
            ActivityInfoModel model = new ActivityInfoModel();
            model.activity_id = int.Parse(Request["Id"]);
            activityService.ActivityInfoDelectOne(model.activity_id);
            return Redirect("ActivityList");
        }

        [HttpPost]
        public ActionResult ActivityUpdate(ActivityInfoModel model)
        {

            model.activity_id = int.Parse(Request["activity_id"]);
            model.manager_id = Request.Cookies["UserInfo"]["user_id"];
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
            model.update_time = DateTime.Now;
            activityService.ActivityInfoUpdateOne(model);
            return Redirect("ActivityList");

//            return Content("修改失敗");
        }




    }
}