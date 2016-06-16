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
    public class PasswordController : Controller
    {

        public PasswordService passwordService;
        public Password_ViewModel passwordViewModel;
        public PasswordController()
        {
            passwordService = new PasswordService();
            passwordViewModel = new Password_ViewModel();
            ViewBag.Form = "Manager";
        }

        // GET: Password
        public ActionResult EditPasswd()
        {
            return View();
        }

        //修改密碼
        public ActionResult PasswordInsertUpdate(string old_pw , string new_pw)
        {
            string current_id = "";
            string current_manager_id = Request.Cookies["ManagerInfo"]["manager_id"]; // 取 manager_id 的 cookie
            string current_user_id = ""; // "12345678"; // 取 user_id 的 cookie
            string errMsg = "修改成功";
            if (!string.IsNullOrEmpty(current_manager_id))
            {
                current_id = current_manager_id;
                if (passwordService.getManagerPassword(current_id).Equals(old_pw))
                {
                    if(!passwordService.UpdateManagerPassword(current_id, new_pw))
                    {
                        errMsg = "修改失敗";
                    }
                }
                else
                {
                    errMsg = "輸入的舊密碼不正確";
                }
            }

            if (!string.IsNullOrEmpty(current_user_id))
            {
                current_id = current_user_id;
                if (passwordService.getUserPassword(current_id).Equals(old_pw))
                {
                    if(passwordService.UpdateUserPassword(current_id, new_pw))
                    {
                        errMsg = "修改失敗";
                    }
                }
                else
                {
                    errMsg = "輸入的舊密碼不正確";
                }
            }

            TempData["pw_errMsg"] = errMsg;

            return Redirect("EditPasswd");
        }

        public ActionResult ForgetPassword()
        {
            return View();
        }

        //忘記密碼 只有前端有
        public ActionResult ReSetPassword(string user_id , string email)
        {
            string errMsg = "修改成功";
            UserInfoModel md = passwordService.SelectOneByIdEmail(user_id, email);
            if (md != null)
            {
                string new_pw = MailHelper.sendForgetPassword(md.email , Request.Url.Host, Request.Url.Port);
                bool isUpdateSuccess = passwordService.UpdateUserPassword(md.user_id, new_pw);
                if (!isUpdateSuccess)
                {
                    errMsg = "更新失敗";
                }
            }
            else
            {
                errMsg = "輸入的資料不正確";
            }

            TempData["fp_errMsg"] = errMsg;

            return Redirect("ForgetPassword");
        }
    }
}