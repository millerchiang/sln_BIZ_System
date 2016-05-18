using prj_BIZ_System.Models;
using prj_BIZ_System.Services;
using prj_BIZ_System.ViewModels;
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace prj_BIZ_System.Controllers
{

    public class UserController : Controller
    {

        public UserService userService;
        public User_ViewModel userModel;

        public UserController()
        {
            userService = new UserService();
            userModel = new User_ViewModel();
        }

        public ActionResult UserList()
        {
            userModel.userinfoList = userService.GetUserInfoList();
            ViewData["list"] = userModel.userinfoList;
            return View();
        }



        public ActionResult UserInsert()
        {

            if (Request["user_id"] != null)
            {
                UserInfoModel model = userModel.userinfo;
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

            string name = Request["company"];
            if (name == "")
                name = Request["company_en"];
            return Redirect("~/Home/Verification?name=" + name + "&email=" + Request["email"]);
        }

        [HttpGet]
        public ActionResult Register()
        {
            string user_id = Request["user_id"];
            if (Request["user_id"] != null) //修改
            {
                userModel.userinfo = userService.GeUserInfoOne(user_id);
                ViewBag.user = userModel.userinfo;
                userModel.usersortList = userService.SelectUserSortByUserId(userModel.userinfo.user_id);
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                ViewBag.userSortList = serializer.Serialize(userModel.usersortList);
                ViewBag.nextAction = "UserUpdate";
            }
            else //新增
            {
                ViewBag.nextAction = "UserInsert";
            }

            userModel.enterprisesortList = userService.GetSortList();
//            urViewModel.enterpriseSortModel = userService.GetSortList();
            ViewData["sortlist"] = userModel.enterprisesortList;

            return View(userModel.userinfo);
        }



        public ActionResult DeleteUser()
        {
            UserInfoModel model = new UserInfoModel();
            model.user_id = Request["user_id"];
            userService.UserInfoDelectOne(model.user_id);
            return Redirect("UserList");
        }

        [HttpPost]
        public ActionResult UserUpdate(UserInfoModel model , int[] sort_id)
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
            bool refreshResult = userService.RefreshUserSort(model.user_id,sort_id);
            //            return Redirect("../Home/Index");
            string name = Request["company"];
            if (name == "")
                name = Request["company_en"];
            return Redirect("../Home/Verification?name=" + name + "&email=" + Request["email"]);
        }
    }
}