﻿using prj_BIZ_System.Models;
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

        //ActivityInfoModel******************************************************************************//

        public IList<ActivityInfoModel> GetActivityInfoList()
        {
            return mapper.QueryForList<ActivityInfoModel>("ActivityInfo.SelectActivityAll", null); 
        }

        public ActivityInfoModel GetActivityInfoOne(int activity_id)
        {
            return (ActivityInfoModel)mapper.QueryForObject("ActivityInfo.SelectActivityOne", activity_id);
        }

        public void ActivityInfoInsertOne(ActivityInfoModel activityInfoModel)
        {
            mapper.Insert("ActivityInfo.InsertActivityOne", activityInfoModel);
        }

        public int ActivityInfoDelectOne(int activity_id)
        {
             Object obj = mapper.Delete("ActivityInfo.DeleteActivityOne", activity_id);
            return (int)obj;
        }

        public int ActivityInfoUpdateOne(ActivityInfoModel activityInfoModel)
        {
           Object obj =  mapper.Update("ActivityInfo.UpdateActivityOne", activityInfoModel);
            return (int)obj;
        }

        /*搜尋出user_id和company*/
        public IList<UserInfoToIdAndCpModel> GetUserInfoToIdandCp()
        {
            return mapper.QueryForList<UserInfoToIdAndCpModel>("ActivityInfo.SelectUserInfoToIdandCp", null);
        }


        //NewsModel******************************************************************************//

        public IList<NewsModel> GetNewsAll()
        {
            return mapper.QueryForList<NewsModel>("ActivityInfo.SelectNewsAll", null);
        }

        public NewsModel GetNewsOne(int news_no)
        {
            NewsModel newsModel = new NewsModel();
            newsModel.news_no = news_no;
            return (NewsModel)mapper.QueryForObject("ActivityInfo.SelectNewsOne", newsModel);
        }

        public void NewsInsertOne(NewsModel newsModel)
        {
            mapper.Insert("ActivityInfo.InsertNewsOne", newsModel);
        }

        public void NewsUpdateOne(NewsModel newsModel)
        {
            mapper.Update("ActivityInfo.UpdateNewsOne", newsModel);
        }

        public void NewsDeleteOne(int news_no)
        {
            mapper.Delete("ActivityInfo.DeleteNewsOne", news_no);
        }



    }

}