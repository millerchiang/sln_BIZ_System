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
        public IList<ActivityInfoModel> GetActivityInfo()
        {
            return activityService.GetActivityInfoList();
        }

        [HttpGet]
        public IList<NewsModel> GetNewsInfo()
        {
            return activityService.GetNewsAll();
        }
    }
}
