using prj_BIZ_System.Models;
using prj_BIZ_System.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace prj_BIZ_System.Controllers
{
    public class ManagerController : Controller
    {
        public ManagerService managerService;
        public Manager_ViewModel managerViewModel;
        public ManagerController()
        {
            managerService = new ManagerService();
            managerViewModel = new Manager_ViewModel();
        }

        // GET: ManagerInfo
        public ActionResult ManagerInfo(int? grp_id , string manager_id)
        {
            managerViewModel.groupList = managerService.getAllGroup();
            managerViewModel.managerInfoList = managerService.getManagerInfoByCondition(grp_id, manager_id);
            ViewBag.Where_GroupId = grp_id;
            ViewBag.Where_ManagerId = manager_id;
            return View(managerViewModel);
        }


        [HttpPost]
        public ActionResult ManagerInfoInsertUpdate(string pagetype, ManagerInfoModel model)
        {
            model.create_manager = "admin";  // Request.Cookies["UserInfo"]["manager_id"]
            if ("Insert".Equals(pagetype))
            {
                managerService.ManagerInfoInsertOne(model);
                return Redirect("ManagerInfo");
            }
            else if ("Update".Equals(pagetype))
            {
                bool isUpdateSuccess = managerService.ManagerInfoUpdateOne(model);
                return Json(isUpdateSuccess);
            }
            return Redirect("ManagerInfo");
        }

        public ActionResult DeleteManagerInfoJson(string manager_id)
        {
            bool isDelSuccess = managerService.ManagerInfoDeleteOne(manager_id);
            return Json(isDelSuccess, JsonRequestBehavior.AllowGet);
        }

        // GET: Group
        public ActionResult Group()
        {
            return View();
        }
    }
}