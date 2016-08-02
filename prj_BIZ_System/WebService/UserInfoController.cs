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
using prj_BIZ_System.WebService.Model;

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

        [HttpGet]
        public UserEnterpriseInfo UserInfo(string user_id)
        {
            if (user_id == null) return null;
            string[] enterprise_type = new string[]
            {
                "國內企業",
                "國外企業"
            };
            string[] revenue = new string[] 
            {
                "500萬以下", "501-1000萬", "1001-1500萬",
                "1501-3000萬", "3001-5000萬", "5000萬-1億",
                "一億以上"
            };
            UserEnterpriseInfo userEnterpriseInfo = new UserEnterpriseInfo();
            userEnterpriseInfo.userinfo = userService.GeUserInfoOne(user_id);
            userEnterpriseInfo.userinfo.user_pw = null;
            int enterprise_typeNum = int.Parse(userEnterpriseInfo.userinfo.enterprise_type);
            int revenueNum = int.Parse(userEnterpriseInfo.userinfo.revenue);
            userEnterpriseInfo.userinfo.enterprise_type = enterprise_type[enterprise_typeNum];
            userEnterpriseInfo.userinfo.revenue = revenue[revenueNum];
            userEnterpriseInfo.usersortList = userService.SelectUserSortByUserId(user_id);
            return userEnterpriseInfo;
        }

        [HttpPost]
        public object UserInfoInsert(UserInfoModel userInfoModel, string sort_id)
        {
            userInfoModel.id_enable = "0";
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
            string errMsg = "200";
            UserInfoModel md = passwordService.SelectOneByIdEmail(user_id, email);
            if (md != null)
            {
                string new_pw = MailHelper.sendForgetPassword(md.email);
                bool isUpdateSuccess = passwordService.UpdateUserPassword(md.user_id, new_pw);
                if (!isUpdateSuccess)
                {
                    errMsg = "304";
                    return errMsg;
                }
            }
            else
            {
                errMsg = "400";
                return errMsg;
            }

            return errMsg;
        }

        [HttpGet]
        public IList<EnterpriseSortListModel> GetSortList()
        {
            return userService.GetSortList();
        }

        [HttpGet]
        public IList<CompanySortModel> GetCompanySortById(string sort_id)
        {
            return userService.SelectUserSortBySortId(int.Parse(sort_id), "");
        }

        [HttpGet]
        public IList<CompanySortModel> GetCompanySortByName(string company_name)
        {
            return userService.SelectUserKw(company_name).Select(
                userInfoModel =>
                new CompanySortModel
                {
                    user_id = userInfoModel.user_id,
                    company = userInfoModel.company,
                    company_en = userInfoModel.company_en,
                }
            ).ToList();
        }
    }
}
