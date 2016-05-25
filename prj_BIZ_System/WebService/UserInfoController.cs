using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using prj_BIZ_System.Services;
using System.Web.Script.Serialization;
using prj_BIZ_System.Models;

namespace prj_BIZ_System.WebService
{
    public class UserInfoController : ApiController
    {
        private UserService userService = new UserService();

        [HttpPost]
        public string CheckUserInfo([FromBody]UserInfoModel userInfoModel)
        {
            return new JavaScriptSerializer().Serialize(userService.ChkUserInfoOne(userInfoModel.user_id, userInfoModel.user_pw));
        }

        [HttpPost]
        public string UserInfoInsert([FromBody]UserInfoModel userInfoModel)
        {
            return new JavaScriptSerializer().Serialize(userService.UserInfoInsertOne(userInfoModel)); 
        }

    }
}
