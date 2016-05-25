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

        public PushController()
        {
            pushService = new PushService();
            pushViewModel = new Push_ViewModel();
        }

        // GET: Push
        public ActionResult SearchPushList()
        {
            return View();
        }

        public ActionResult EditPushList()
        {
            return View();
        }

        [HttpPost]
        public ActionResult PushListInsertUpdateJson()
        {
            return View();
        }

        public ActionResult EditPushSample()
        {
            return View();
        }

        /* Json */
        public ActionResult EditPushSampleJson(string action)
        {
            pushViewModel.pushSampleList = pushService.getPushSampleAll();
            return Json(pushViewModel, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult PushSampleInsertUpdate(string pagetype , PushSampleModel model )
        {
            model.create_id = "12345678";  // Request.Cookies["UserInfo"]["user_id"]
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