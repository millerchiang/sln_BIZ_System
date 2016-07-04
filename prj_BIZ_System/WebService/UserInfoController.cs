using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using prj_BIZ_System.Models;
using prj_BIZ_System.Services;
using System.Web.Script.Serialization;
using WebApiContrib.ModelBinders;
using prj_BIZ_System.App_Start;
using System.Web;

namespace prj_BIZ_System.WebService
{
    [MvcStyleBinding]
    public class UserInfoController : ApiController
    {
        private UserService userService = new UserService();
        private PasswordService passwordService = new PasswordService();

        [HttpPost]
        public UserInfoModel CheckUserInfo(string user_id, string user_pw)
        {
            return userService.ChkUserInfoOne(user_id, user_pw);
        }

        [HttpPost]
        public int ModifyUserInfo(UserInfoModel userInfoModel, string sort_id)
        {
            userInfoModel.id_enable = "1";
            bool isInsertSuccess = insertEnterpriseId(userInfoModel.user_id, sort_id);
            return isInsertSuccess == true ? userService.UserInfoUpdateOne(userInfoModel) : 0;
        }

        [HttpPost]
        public object UserInfoInsert(UserInfoModel userInfoModel, string sort_id)
        {
            string errorInfo;
            int emailCode = MailHelper.checkEmail(userInfoModel.email, out errorInfo);

            if (!errorInfo.Equals("")) return "Email fail";

            bool isInsertSuccess = insertEnterpriseId(userInfoModel.user_id, sort_id);
            object userInfoId = null;
            if (isInsertSuccess)
            {
                try
                {
                    userInfoId = userService.UserInfoInsertOne(userInfoModel);
                    if (userInfoId != null)
                    {
                        SendAccountMailValidate(userInfoId, userInfoModel.user_id, userInfoModel.email);
                    }
                }
                catch (Exception ex)
                {
                    if (ex.Message.Contains("user_info_pkey"))
                    {
                        return "Account already exists";
                    }
                }
            }
            if (isInsertSuccess == true && userInfoId != null)
            {
                return userInfoId;
            }
            else
            {
                return "Userinfo insert fail";
            }
        }

        private bool insertEnterpriseId(string user_id, string sort_id)
        {
            bool isInsertSuccess = true;
            if (sort_id != null)
            {
                string[] strings = sort_id.Split(',');
                int[] enterprise_sort_id = new int[strings.Length];

                for (int i = 0; i < strings.Length; i++)
                {
                    enterprise_sort_id[i] = int.Parse(strings[i]);
                }
                try
                {
                    userService.RefreshUserSort(user_id, enterprise_sort_id);
                }
                catch (Exception ex)
                {
                    isInsertSuccess = false;
                }
            }
            else
            {
                isInsertSuccess = false;
            }

            return isInsertSuccess;
        }

        [HttpPost]
        public void SendAccountMailValidate(object id, string user_id, string email)
        {

            MailHelper.sendAccountMailValidate(id, user_id, email);
        }

        [HttpPost]
        public string ReSetPassword(string user_id, string email)
        {

            string errMsg = "新的註冊密碼通知信已寄出，請至你註冊填寫的信箱收取!!";
            UserInfoModel md = passwordService.SelectOneByIdEmail(user_id, email);
            if (md != null)
            {
                string new_pw = MailHelper.sendForgetPassword(md.email);
                bool isUpdateSuccess = passwordService.UpdateUserPassword(md.user_id, new_pw);
                if (!isUpdateSuccess)
                {
                    errMsg = "新的註冊密碼通知信更新失敗，請重新操作!!";
                    return errMsg;
                }
            }
            else
            {
                errMsg = "輸入的資料不正確，請重新操作!!";
                return errMsg;
            }

            return errMsg;
        }

        [HttpGet]
        public IList<EnterpriseSortListModel> GetSortList()
        {
            return userService.GetSortList();
        }

    }
}
