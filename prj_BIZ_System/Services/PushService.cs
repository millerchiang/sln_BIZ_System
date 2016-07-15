using prj_BIZ_System.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Npgsql;
using IBatisNet.DataMapper;
using IBatisNet.DataMapper.Configuration;
using System.Collections;

namespace prj_BIZ_System.Services
{
    public class PushService : _BaseService
    {
        public IList<PushSampleModel> getPushSampleAll(int? grp_id)
        {
            var param = new PushSampleModel { grp_id = grp_id };
            return mapper.QueryForList<PushSampleModel>("Push.SelectPushSample", param);
        }

        public void PushSampleInsertOne(PushSampleModel model)
        {
            var result = mapper.Insert("Push.InsertPushSample", model);
            return ;
        }

        public bool PushSampleUpdateOne(PushSampleModel model)
        {
            return mapper.Update("Push.UpdatePushSample", model) > 0;
        }

        public bool PushSampleDeleteOne(int sample_id)
        {
            var param = new PushSampleModel { sample_id = sample_id };
            return mapper.Delete("Push.DeletePushSample", param) > 0;
        }

        public IList<ActivityInfoModel> getActivityInfoListAfterNow()
        {
            DateTime today = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);

            var param = new ActivityInfoModel { endtime = today };
            IList<ActivityInfoModel> result = mapper.QueryForList<ActivityInfoModel>("Push.SelectActivityAfterNow", param);
            return result;
        }

        public int getPushListCountBySampleId(int sample_id)
        {
            var param = new PushListModel { sample_id = sample_id };
            int userCount = (int)mapper.QueryForObject("Push.SelectPushListCountBySampleId", param );
            return userCount ;
        }

        public IList<PushListModel> getPushListByCondition(string push_type , string push_name,int? grp_id)
        {
            var param = new PushListModel { push_type = push_type, push_name = push_name, grp_id = grp_id };
            return mapper.QueryForList<PushListModel>("Push.getPushListByCondition", param);
        }

        public PushListModel getPushListOne(int? push_id)
        {
            var param = new PushListModel { push_id = push_id };
            return mapper.QueryForObject<PushListModel>("Push.SelectPushListOne", param);
        }

        public void PushListInsertOne(PushListModel model)
        {
            mapper.Insert("Push.InsertPushList", model);
        }

        public void PushListUpdateOne(PushListModel model)
        {
            mapper.Update("Push.UpdatePushList", model);
        }

        public bool DeletePushListOne(int? push_id)
        {
            var param = new PushListModel { push_id = push_id };
            return mapper.Delete("Push.DeletePushList", param) > 0 ;
        }

        public object MobileDeviceInfoInsertOne(MobileDeviceInfoModel model)
        {
            object result = mapper.Insert("Push.InsertSelectMobileDeviceInfo", model);
            return result;
        }
    }
}