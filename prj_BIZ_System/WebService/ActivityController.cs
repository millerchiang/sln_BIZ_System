using prj_BIZ_System.Models;
using prj_BIZ_System.Services;
using prj_BIZ_System.WebService.Model;
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
        public ActivityInfo GetActivityInfoById(int id)
        {
            ActivityInfoModel activityInfoModel = activityService.GetActivityInfoOne(id);
            ActivityInfo activityInfo = new ActivityInfo {
                activity_id = activityInfoModel.activity_id,
                manager_id = activityInfoModel.manager_id,
                activity_type = activityInfoModel.activity_type,
                activity_name = activityInfoModel.activity_name,
                starttime = activityInfoModel.starttime.ToString("yyyy-MM-dd HH:mm"),
                endtime = activityInfoModel.endtime.ToString("yyyy-MM-dd HH:mm"),
                addr = activityInfoModel.addr,
                organizer = activityInfoModel.organizer,
                name = activityInfoModel.name,
                phone = activityInfoModel.phone,
                email = activityInfoModel.email,
                activity_name_en = activityInfoModel.activity_name_en,
                addr_en = activityInfoModel.addr_en,
                organizer_en = activityInfoModel.organizer_en
            };

            return activityInfo;
        }

        [HttpGet]
        public IList<EnterpriseSortAndListModel> GetEnterpriseSortByUserId(string user_id)
        {
            return activityService.GetEnterpriseSortAndListOne(user_id);
        }

        [HttpGet]
        public ActivityRegister GetUserActivityRegister(int activity_id, string user_id)
        {
            ActivityRegisterModel activityRegisterModel = activityService.GetActivityRegisterSelectOne(activity_id, user_id);
            if (activityRegisterModel == null) return null;

            ActivityInfoModel activityInfoModel = activityService.GetActivityInfoOne(activity_id);
            return new ActivityRegister
            {
                activity_name = activityInfoModel.activity_name,
                starttime = activityInfoModel.starttime.ToString("yyyy-MM-dd HH:mm"),
                endtime = activityInfoModel.endtime.ToString("yyyy-MM-dd HH:mm"),
                addr = activityInfoModel.addr,
                user_id = activityRegisterModel.user_id,
                quantity = activityRegisterModel.quantity,
                name_a = activityRegisterModel.name_a,
                title_a = activityRegisterModel.title_a,
                name_b = activityRegisterModel.name_b,
                title_b = activityRegisterModel.title_b,
                telephone = activityRegisterModel.telephone,
                phone = activityRegisterModel.phone,
                email = activityRegisterModel.email,
                manager_check = activityRegisterModel.manager_check,
            };
        }

        [HttpPost]
        public HttpResponseMessage ActivityRegister(ActivityRegisterModel activityRegisterModel)
        {
            ActivityRegisterModel hasActivityRegister = activityService.GetActivityRegisterSelectOne(activityRegisterModel.activity_id, activityRegisterModel.user_id);
            var message = ""; 
            if (hasActivityRegister != null)
            {
                message = string.Format("register has exist");
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, message);
            }
            else
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
                    message = string.Format("insert fail");
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, message);
                }
                return Request.CreateResponse(HttpStatusCode.OK, activityRegisterId);
            }
        }

        [HttpPost]
        public int ModifyActivityRegister(ActivityRegisterModel activityRegisterModel)
        {
            return activityService.ActivityRegisterUpdateOne(activityRegisterModel);
        }

        [HttpPost]
        public int CancelActivityRegister(int activity_id, string user_id)
        {
            return activityService.ActivityRegisterDeleteOne(activity_id, user_id);
        }
    }
}
