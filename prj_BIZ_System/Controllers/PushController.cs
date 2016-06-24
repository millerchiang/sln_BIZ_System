using prj_BIZ_System.App_Start;
using prj_BIZ_System.Models;
using prj_BIZ_System.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace prj_BIZ_System.Controllers
{
    public class PushController : Controller
    {
        public PushService pushService;
        public Push_ViewModel pushViewModel;
        public ManagerService managerService;
        public PushController()
        {
            pushService = new PushService();
            pushViewModel = new Push_ViewModel();
            managerService = new ManagerService();
            ViewBag.Form = "Manager";

        }

        // GET: Push
        public ActionResult SearchPushList(string push_type,string push_name)
        {
            string manager_id = null;
            int? grp_id = null;
            if (Request.Cookies["ManagerInfo"]["push"] == "2")
            {
                manager_id = Request.Cookies["ManagerInfo"]["manager_id"];
                grp_id = managerService.getManagerGroup(Request.Cookies["ManagerInfo"]["manager_id"]);
            }


            IList<PushListModel> result = pushService.getPushListByCondition(push_type, push_name, grp_id).Pages(Request,this,10);
            ViewBag.Where_PushType = push_type;
            ViewBag.Where_PushName = push_name;
            return View(ViewBag.Pages.datalist);
        }

        // GET: Push
        public ActionResult SearchPushListCount(int sample_id)
        {
            int count = pushService.getPushListCountBySampleId(sample_id);
            return Json(count,JsonRequestBehavior.AllowGet);
        }

        public ActionResult EditPushList(int? push_id)
        {

            string manager_id = null;
            int? grp_id = null;
            if (Request.Cookies["ManagerInfo"]["push"] == "2")
            {
                manager_id = Request.Cookies["ManagerInfo"]["manager_id"];
                grp_id = managerService.getManagerGroup(Request.Cookies["ManagerInfo"]["manager_id"]);
            }

            //var manager_id = "admin"; //管理者編號 
            //PushTypeEnum push_type = new PushTypeEnum();
            //ViewBag.push_type = push_type;
            pushViewModel.activityinfoList = pushService.getActivityInfoListAfterNow();
            pushViewModel.pushSampleList = pushService.getPushSampleAll(grp_id);
            ViewBag.Action = "EditPushListInsertUpdate";

            if (push_id == null)
            {
                //正常是要從使用者
                pushViewModel.pushList = new PushListModel();
                pushViewModel.pushList.push_date = DateTime.Now;
                pushViewModel.pushList.push_objects = "0";
                ViewBag.SubmitName = "新增";
                ViewBag.PageType = "Create";
            }
            else
            {
                pushViewModel.pushList = pushService.getPushListOne(push_id);
                ViewBag.SubmitName = "修改";
                ViewBag.PageType = "Edit";
            }
            return View(pushViewModel);
        }

        [HttpPost]
        public ActionResult EditPushListInsertUpdate(PushListModel model)
        {
            if (model.push_id == null)
            {
                model.manager_id = "admin"; //之後要從 Cookie抓
                pushService.PushListInsertOne(model);
            }
            else
            {
                pushService.PushListUpdateOne(model);
            }
            return Redirect("SearchPushList");
        }

        public ActionResult DeletePushList(int? push_id)
        {
            bool isDelSuccess = pushService.DeletePushListOne(push_id);
            return Redirect("SearchPushList");
        }

        [HttpPost]
        public ActionResult PushListInsertUpdateJson()
        {
            return View();
        }

        public ActionResult EditPushSample()
        {
            string manager_id = null;
            int? grp_id = null;
            if (Request.Cookies["ManagerInfo"]["push"] == "2")
            {
                manager_id = Request.Cookies["ManagerInfo"]["manager_id"];
                grp_id = managerService.getManagerGroup(Request.Cookies["ManagerInfo"]["manager_id"]);
            }
            pushViewModel.pushSampleList = pushService.getPushSampleAll(grp_id).Pages(Request,this,10);
            return View(pushViewModel);
        }

        /* Json */
        public ActionResult EditPushSampleJson(string action)
        {
            string manager_id = null;
            int? grp_id = null;
            if (Request.Cookies["ManagerInfo"]["push"] == "2")
            {
                manager_id = Request.Cookies["ManagerInfo"]["manager_id"];
                grp_id = managerService.getManagerGroup(Request.Cookies["ManagerInfo"]["manager_id"]);
            }
            pushViewModel.pushSampleList = pushService.getPushSampleAll(grp_id);
            return Json(pushViewModel, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult PushSampleInsertUpdate(string pagetype , PushSampleModel model )
        {
            model.create_id = Request.Cookies["ManagerInfo"]["manager_id"];  // Request.Cookies["user_id"]
            if ("Insert".Equals(pagetype))
            {
                pushService.PushSampleInsertOne(model);
                return Redirect("EditPushSample");
            }
            else if ("Update".Equals(pagetype))
            {
                bool isUpdateSuccess = pushService.PushSampleUpdateOne(model);
                return Json(isUpdateSuccess);
            }
                return Redirect("EditPushSample");
        }

        public ActionResult DeletePushSampleJson(int sample_id)
        {
            bool isDelSuccess = pushService.PushSampleDeleteOne(sample_id);
            return Json(isDelSuccess, JsonRequestBehavior.AllowGet);
        }
    }
}