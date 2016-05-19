using prj_BIZ_System.Models;
using prj_BIZ_System.Services;
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
            return View(userModel);
        }

        [HttpGet]
        public ActionResult Register()
        {
            userModel.enterprisesortList = userService.GetSortList();
            ViewBag.Action = "UserInsertUpdate";


            if (Request["user_id"] == null) //新增
            {
                userModel.userinfo = new UserInfoModel();
                ViewBag.PageType = "Create";
                ViewBag.SubmitName = "確定送出";
                Response.Cookies["UserInfo"]["edit"] = "Add";

            }
            else //修改
            {
                userModel.userinfo = userService.GeUserInfoOne(Request["user_id"]);
                ViewBag.user = userModel.userinfo;
                userModel.usersortList = userService.SelectUserSortByUserId(userModel.userinfo.user_id);
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                ViewBag.userSortList = serializer.Serialize(userModel.usersortList);
                ViewBag.PageType = "Edit";
                ViewBag.SubmitName = "修改";
                Response.Cookies["UserInfo"]["edit"] = "Update";
            }
            return View(userModel);
        }



        public ActionResult DeleteUser()
        {
            userService.UserInfoDelectOne(Request["user_id"]);
            return Redirect("UserList");
        }

        [HttpPost]
        public ActionResult UserInsertUpdate(UserInfoModel model , int[] sort_id)
        {
            if (Request.Cookies["UserInfo"]["edit"] == "Add")//新增
            {
                userService.UserInfoInsertOne(model);
            }
            else //修改
            {
                model.update_time = DateTime.Now;
                userService.UserInfoUpdateOne(model);
            }

            bool refreshResult = userService.RefreshUserSort(model.user_id,sort_id);
            string name = Request["company"];
            if (name == "")
                name = Request["company_en"];
            return Redirect("../Home/Verification?name=" + name + "&email=" + Request["email"]);
        }
    }
}