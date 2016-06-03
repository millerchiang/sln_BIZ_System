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
                return null;
            }
            return userService.UserInfoInsertOne(userInfoModel); 
        }

        [HttpGet]
        public IList<EnterpriseSortListModel> GetSortList()
        {
            return userService.GetSortList();
        }

    }
}
