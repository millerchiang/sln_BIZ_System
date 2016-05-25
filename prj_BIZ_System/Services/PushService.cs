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
        public IList<PushSampleModel> getPushSampleAll()
        {
            return mapper.QueryForList<PushSampleModel>("Push.SelectPushSample", null);
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
    }
}