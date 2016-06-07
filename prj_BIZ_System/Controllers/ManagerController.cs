using Newtonsoft.Json;
using prj_BIZ_System.Models;
using prj_BIZ_System.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

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

        #region ManagerInfo 帳號管理
        // GET: ManagerInfo
        public ActionResult ManagerInfo(int? where_grp_id , string where_manager_id)
        {
            managerViewModel.groupList = managerService.getAllGroup();
            managerViewModel.managerInfoList = managerService.getManagerInfoByCondition(where_grp_id, where_manager_id);
            ViewBag.Where_GroupId = where_grp_id;
            ViewBag.Where_ManagerId = where_manager_id;
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
            //非真的刪 , 只是停用
            bool isDelSuccess = managerService.ManagerInfoDisableOne(manager_id);
            return Json(isDelSuccess, JsonRequestBehavior.AllowGet);
        }



        #endregion

        #region Group 群組管理

        // GET: Group
        public ActionResult Group()
        {
            managerViewModel.groupList = managerService.getAllGroup();

            return View(managerViewModel);
        }

        public ActionResult GetGroupDetail(int grp_id)
        {
            managerViewModel.groupList = managerService.getAllGroup();
            GroupModel gp_model = managerService.GroupSelectOne(grp_id);
            Dictionary<string, object> result = new Dictionary<string, object>();
            Dictionary<string, string> limitsDict = new Dictionary<string, string>();

            if (gp_model != null)
            {
                limitsDict = new JavaScriptSerializer().Deserialize<Dictionary<string, string>>(gp_model.limit);
            }

            IList<ManagerInfoModel> manager_model_list = managerService.getManagerInfoByGrpId(grp_id);
            result.Add("memangers", manager_model_list);
            result.Add("limits", limitsDict);

            return Json( result , JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GroupInsertUpdate(
              int? grp_id   , string grp_name   , string user    , string activity 
            , string push   , string news       , string manager      , string statistic
        )
        {
            Dictionary<string, string> limits = new Dictionary<string, string>();
            limits.Add("user", user);
            limits.Add("activity", activity);
            limits.Add("push", push);
            limits.Add("news", news);
            limits.Add("manager", manager);
            limits.Add("statistic", statistic);

            if(grp_id == null)
            {
                managerService.GroupInsertOne(grp_name , limits);
                return Redirect("Group");
            }
            else
            {
                bool isUpdateSuccess = managerService.GroupUpdateOne(grp_name, limits);
                return Json(isUpdateSuccess , JsonRequestBehavior.AllowGet);
            }
        }

        #endregion
    }
}