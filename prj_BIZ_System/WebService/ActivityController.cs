using prj_BIZ_System.Models;
using prj_BIZ_System.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Script.Serialization;

namespace prj_BIZ_System.WebService
{
    public class ActivityController : ApiController
    {
        ActivityService activityService = new ActivityService();

        [HttpGet]
        public IList<NewsModel> GetNewsInfo()
        {
            return activityService.GetNewsAll();
        }

        [HttpGet]
        public ActivityInfoModel GetActivityInfoById(int id)
        {
            return activityService.GetActivityInfoOne(id);
        }

        [HttpGet]
        public IList<EnterpriseSortAndListModel> GetEnterpriseSortByUserId(string user_id)
        {
            return activityService.GetEnterpriseSortAndListOne(user_id);
        }

        //[HttpPost]
        //public IList<NewsModel> ActivityRegister(UserInfoModel userInfoModel)
        //{

        //    return activityService.GetNewsAll();
        //}
    }
}
