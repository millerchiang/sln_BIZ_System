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

        [HttpPost]
        public UserInfoModel CheckUserInfo(string user_id, string user_pw)
        {
            return userService.ChkUserInfoOne(user_id, user_pw);
        }

        [HttpPost]
        public object UserInfoInsert(UserInfoModel userInfoModel, string sort_id)
        {
            string errorInfo;
            //int emailCode = checkEmail(userInfoModel.email, out errorInfo);
            //if (checkEmail(userInfoModel.email, out errorInfo) == 200)
            //{
            //    return true;
            //}


            bool isInsertSuccess = insertEnterpriseId(userInfoModel, sort_id);
            object userInfoId = null;
            if (isInsertSuccess)
            {
                userInfoId = userService.UserInfoInsertOne(userInfoModel);
            }
            if (isInsertSuccess == true && userInfoId != null)
            {
                return userInfoId;
            }
            else
            {
                return null;
            }
        }

        private bool insertEnterpriseId(UserInfoModel userInfoModel, string sort_id)
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
                    userService.RefreshUserSort(userInfoModel.user_id, enterprise_sort_id);
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
            var ip = HttpContext.Current.Request.Url.Host;
            var port = HttpContext.Current.Request.Url.Port;
            MailHelper.sendAccountMailValidate(id, user_id, email, ip, port);
        }

        [HttpGet]
        public IList<EnterpriseSortListModel> GetSortList()
        {
            return userService.GetSortList();
        }

    }
}
