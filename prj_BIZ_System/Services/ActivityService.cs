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

        //ActivityInfoModel******************************************************************************//

        public IList<ActivityInfoModel> GetActivityInfoList()
        {
            return mapper.QueryForList<ActivityInfoModel>("ActivityInfo.SelectAll", null); 
        }

        public ActivityInfoModel GetActivityInfoOne(int activity_id)
        {
            return (ActivityInfoModel)mapper.QueryForObject("ActivityInfo.SelectOne", activity_id);
        }

        public void ActivityInfoInsertOne(ActivityInfoModel activityInfoModel)
        {
            mapper.Insert("ActivityInfo.InsertOne", activityInfoModel);
        }

        public int ActivityInfoDelectOne(int activity_id)
        {
             Object obj = mapper.Delete("ActivityInfo.DeleteOne", activity_id);
            return (int)obj;
        }

        public int ActivityInfoUpdateOne(ActivityInfoModel activityInfoModel)
        {
           Object obj =  mapper.Update("ActivityInfo.UpdateOne", activityInfoModel);
            return (int)obj;
        }


        //NewsModel******************************************************************************//

        public IList<NewsModel> GetNewsAll()
        {
            return mapper.QueryForList<NewsModel>("News.NewsSelectAll", null);
        }

        public NewsModel GetNewsOne(int news_no)
        {
            NewsModel newsModel = new NewsModel();
            newsModel.news_no = news_no;
            return (NewsModel)mapper.QueryForObject("News.NewsSelectOne", newsModel);
        }

        public void NewsInsertOne(NewsModel newsModel)
        {
            mapper.Insert("News.NewsInsertOne", newsModel);
        }

        public void NewsUpdateOne(NewsModel newsModel)
        {
            mapper.Update("News.NewsUpdateOne", newsModel);
        }

        public void NewsDeleteOne(int news_no)
        {
            mapper.Delete("News.NewsDeleteOne", news_no);
        }



    }

}