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
        public UserInfoModel CheckUserInfo(UserInfoModel userInfoModel)
        {
            return userService.ChkUserInfoOne(userInfoModel.user_id, userInfoModel.user_pw);
        }

        [HttpPost]
        public object UserInfoInsert(UserInfoModel userInfoModel, string sort_id)
        {
            bool isInsertSuccess = insertEnterpriseId(userInfoModel, sort_id);
            object userInfoId = null;
            if (isInsertSuccess)
            {
                userInfoId = insertUserInfoAndSendEmail(userInfoModel);
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
            bool isInsertUserSort = true;
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
                    isInsertUserSort = false;
                }
            }
            else
            {
                isInsertUserSort = false;
            }

            return isInsertUserSort;
        }

        private object insertUserInfoAndSendEmail(UserInfoModel userInfoModel)
        {
            object userInfoId = userService.UserInfoInsertOne(userInfoModel);
            if (userInfoId != null)
            {
                SendAccountMailValidate(userInfoId, userInfoModel.user_id, userInfoModel.email);
                return userInfoId;
            }
            else
            {
                return null;
            }
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
