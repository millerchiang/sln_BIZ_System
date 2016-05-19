using prj_BIZ_System.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Npgsql;
using IBatisNet.DataMapper;
using IBatisNet.DataMapper.Configuration;

namespace prj_BIZ_System.Services
{
    public class ActivityService : _BaseService
    {

        /* select方法*/
        public IList<ActivityInfoModel> GetActivityInfoList()
        {
            return mapper.QueryForList<ActivityInfoModel>("ActivityInfo.SelectAll", null); 
        }

        public ActivityInfoModel GetActivityInfoOne(int activity_id)
        {
            return (ActivityInfoModel)mapper.QueryForObject("ActivityInfo.SelectOne", activity_id);
        }

        /* insert方法*/
        public void ActivityInfoInsertOne(ActivityInfoModel activityInfoModel)
        {
            mapper.Insert("ActivityInfo.InsertOne", activityInfoModel);
        }

        /* delect方法*/
        public int ActivityInfoDelectOne(int activity_id)
        {
             Object obj = mapper.Delete("ActivityInfo.DeleteOne", activity_id);
            return (int)obj;
        }

        /*update方法*/
        public int ActivityInfoUpdateOne(ActivityInfoModel activityInfoModel)
        {
           Object obj =  mapper.Update("ActivityInfo.UpdateOne", activityInfoModel);
            return (int)obj;
        }

        /*搜尋出user_id和company*/
        public IList<UserInfoToIdAndCpModel> GetUserInfoToIdandCp()
        {
            return mapper.QueryForList<UserInfoToIdAndCpModel>("ActivityInfo.SelectUserInfoToIdandCp", null);
        }


    }

}