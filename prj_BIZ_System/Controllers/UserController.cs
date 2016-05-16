using prj_BIZ_System.Models;
using prj_BIZ_System.Services;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace prj_BIZ_System.Controllers
{

    public class UserController : Controller
    {

        public UserService userService;

        public UserController()
        {
            userService = new UserService();
        }

        public ActionResult UserList()
        {
            IList<UserInfoModel> userInfoModels = userService.GetUserInfoList();
            ViewData["list"] = userInfoModels;
            return View();
        }



        public ActionResult UserInsert()
        {

            if (Request["user_id"] != null)
            {
                UserInfoModel model = new UserInfoModel();
                model.user_id = Request["user_id"];
                model.user_pw = Request["user_pw"];
                model.enterprise_type = Request["enterprise_type"];
                model.company = Request["company"];
                //            model.endtime = DateTime.Parse(Request["endtime"]);
                model.company_en = Request["company_en"];
                model.leader = Request["leader"];
                model.addr = Request["addr"];
                model.leader_en = Request["leader_en"];
                model.addr_en = Request["addr_en"];
                model.contact = Request["contact"];
                model.contact_en = Request["contact_en"];
                model.phone = Request["phone"];
                model.email = Request["email"];
                model.capital = int.Parse(Request["capital"]);
                model.revenue = Request["revenue"];
                model.website = Request["website"];
                model.info = Request["info"];
                model.info_en = Request["info_en"];
                userService.UserInfoInsertOne(model);
            }
            return Redirect("../Home/Index");
//            return Redirect("UserList");
        }

        [HttpGet]
        public ActionResult Register()
        {
            UserInfoModel model = null;

            if (Request["user_id"] != null)
            {
                string user_id = Request["user_id"];
                model = userService.GeUserInfoOne(user_id);
                ViewBag.user = model;
            }

            IList<EnterpriseSortModel> userSortModels = userService.GetSortList();
            ViewData["sortlist"] = userSortModels;

            return View(model);
        }



        public ActionResult DeleteUser()
        {
            UserInfoModel model = new UserInfoModel();
            model.user_id = Request["user_id"];
            userService.UserInfoDelectOne(model.user_id);
            return Redirect("UserList");
        }

        [HttpPost]
        public ActionResult UserUpdate(UserInfoModel model)
        {

//            model.user_id = Request["user_id"];
            model.user_pw = Request["user_pw"];
            model.enterprise_type = Request["enterprise_type"];
            model.company = Request["company"];
            //            model.endtime = DateTime.Parse(Request["endtime"]);
            model.company_en = Request["company_en"];
            model.leader = Request["leader"];
            model.addr = Request["addr"];
            model.leader_en = Request["leader_en"];
            model.addr_en = Request["addr_en"];
            model.contact = Request["contact"];
            model.contact_en = Request["contact_en"];
            model.phone = Request["phone"];
            model.email = Request["email"];
            model.capital = int.Parse(Request["capital"]);
            model.revenue = Request["revenue"];
            model.website = Request["website"];
            model.info = Request["info"];
            model.info_en = Request["info_en"];
            model.update_time = DateTime.Now;
            userService.UserInfoUpdateOne(model);
            return Redirect("../Home/Index");
        }
    }
}