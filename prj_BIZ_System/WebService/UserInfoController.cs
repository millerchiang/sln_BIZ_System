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
        public string CheckUserInfo([FromBody]string userinfo)
        {
            string[] userinfoArray = userinfo.Split(',');
            return new JavaScriptSerializer().Serialize(userService.ChkUserInfoOne(userinfoArray[0], userinfoArray[1]));
        }

        [HttpPost]
        public string UserInfoInsert([FromBody]UserInfoModel userInfoModel)
        {
            object result = userService.UserInfoInsertOne(userInfoModel);
            return result.ToString();
        }

        // GET: api/UserInfo
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/UserInfo/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/UserInfo
        public void Post([FromBody]string value)
        {

        }

        // PUT: api/UserInfo/5
        public void Put(int id, [FromBody]string value)
        {

        }

        // DELETE: api/UserInfo/5
        public void Delete(int id)
        {
        }
    }
}
