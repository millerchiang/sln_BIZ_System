using prj_BIZ_System.Models;
using prj_BIZ_System.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Script.Serialization;
using WebApiContrib.ModelBinders;

namespace prj_BIZ_System.WebService
{
    [MvcStyleBinding]
    public class ActivityController : ApiController
    {
        ActivityService activityService = new ActivityService();

        [HttpGet]
        public IList<NewsModel> GetNewsInfo()
        {
            return activityService.GetNewsAll(null);
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

        [HttpGet]
        public ActivityRegisterModel GetUserActivityRegister(int activity_id, string user_id)
        {
            return activityService.GetActivityRegisterSelectOne(activity_id, user_id);
        }

        [HttpPost]
        public object ActivityRegister(ActivityRegisterModel activityRegisterModel)
        {
            object activityRegisterId = null;
            activityRegisterModel.manager_check = "0";
            activityRegisterModel.create_time = DateTime.Now;
            try
            {
                activityRegisterId = activityService.ActivityRegisterInserOne(activityRegisterModel);
            }
            catch (Exception ex)
            {

            }
            return activityRegisterId;
        }

        [HttpPost]
        public int modifyActivityRegister(ActivityRegisterModel activityRegisterModel)
        {
            return activityService.ActivityRegisterUpdateOne(activityRegisterModel);
        }

        [HttpPost]
        public int cancelActivityRegister(int activity_id, string user_id)
        {
            return activityService.ActivityRegisterDeleteOne(activity_id, user_id);
        }
    }
}
