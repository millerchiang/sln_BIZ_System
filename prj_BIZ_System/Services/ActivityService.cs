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
            return mapper.QueryForList<ActivityInfoModel>("ActivityInfo.SelectActivityAll", null); 
        }

        public ActivityInfoModel GetActivityInfoOne(int activity_id)
        {
            ActivityInfoModel param = new ActivityInfoModel() { activity_id = activity_id };
            return (ActivityInfoModel)mapper.QueryForObject("ActivityInfo.SelectActivityOne", param);
        }

        public void ActivityInfoInsertOne(ActivityInfoModel activityInfoModel)
        {
            mapper.Insert("ActivityInfo.InsertActivityOne", activityInfoModel);
        }

        public int ActivityInfoDelectOne(int activity_id)
        {
            ActivityInfoModel param = new ActivityInfoModel() { activity_id = activity_id };
            Object obj = mapper.Delete("ActivityInfo.DeleteActivityOne", param);
            return (int)obj;
        }

        public int ActivityInfoUpdateOne(ActivityInfoModel activityInfoModel)
        {
           Object obj =  mapper.Update("ActivityInfo.UpdateActivityOne", activityInfoModel);
            return (int)obj;
        }

        //NewsModel******************************************************************************//

        public IList<NewsModel> GetNewsAll()
        {
            return mapper.QueryForList<NewsModel>("ActivityInfo.SelectNewsAll", null);
        }

        public NewsModel GetNewsOne(int news_no)
        {
            NewsModel param = new NewsModel() { news_no = news_no };
            return (NewsModel)mapper.QueryForObject("ActivityInfo.SelectNewsOne", param);
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
            NewsModel param = new NewsModel() { news_no = news_no };
            mapper.Delete("ActivityInfo.DeleteNewsOne", param);
        }

        //UserInfoToIdAndCpModel******************************************************************************//
        /*搜尋出user_id和company*/
        public IList<UserInfoToIdAndCpModel> GetUserInfoToIdandCp()
        {
            return mapper.QueryForList<UserInfoToIdAndCpModel>("ActivityInfo.SelectUserInfoToIdandCp", null);
        }

        //BuyerInfoModel******************************************************************************//
        
        public IList<BuyerInfoModel> GetBuyerInfoAll()
        {
            return mapper.QueryForList<BuyerInfoModel>("ActivityInfo.SelectBuyerInfoAll", null);
        }

        public BuyerInfoModel GetBuyerInfoOne(int serial_no)
        {
            BuyerInfoModel param = new BuyerInfoModel() {serial_no = serial_no };
            return (BuyerInfoModel)mapper.QueryForObject("ActivityInfo.SelectBuyerInfoOne", param);
        }

        public void BuyerInfoInsertOne(BuyerInfoModel buyerInfoModel)
        {
            mapper.Insert("ActivityInfo.InsertBuyerInfoOne", buyerInfoModel);
        }

        public void BuyerInfoUpdateOne(BuyerInfoModel buyerInfoModel)
        {
            mapper.Update("ActivityInfo.UpdateBuyerInfoOne", buyerInfoModel);
        }

        public void BuyerInfoDeleteOne(int serial_no)
        {
            BuyerInfoModel param = new BuyerInfoModel() { serial_no = serial_no };
            mapper.Delete("ActivityInfo.DeleteBuyerInfoOne", param);
        }

        //EnterpriseSortAndListModel******************************************************************************//
        public IList<EnterpriseSortAndListModel> GetEnterpriseSortAndListAll()
        {
            return mapper.QueryForList<EnterpriseSortAndListModel>
               ("ActivityInfo.SelectEnterpriseAll", null);
        }

        public IList<EnterpriseSortAndListModel> GetEnterpriseSortAndListOne(string user_id)
        {
            EnterpriseSortAndListModel param = new EnterpriseSortAndListModel() {user_id = user_id };
            return mapper.QueryForList<EnterpriseSortAndListModel>("ActivityInfo.SelectEnterpriseByUserId", param);
        }

        //ActivityRegisterModel******************************************************************************//
        public void ActivityRegisterInserOne(ActivityRegisterModel activityRegisterModel)
        {
            mapper.Insert("ActivityInfo.InsertActivityRegisterOne", activityRegisterModel);
        }

        public IList<ActivityRegisterModel> GetActivityCheckAllByCondition(string activity_name, string company)
        {
            ActivityRegisterModel param = new ActivityRegisterModel { activity_name = activity_name , company = company };
            return mapper.QueryForList<ActivityRegisterModel>("ActivityInfo.SelectActivityCheckAll", param);
        }

        public void ActivityRegisterUpdateOne(ActivityRegisterModel activityRegisterModel)
        {
            mapper.Update("ActivityInfo.UpdateActivityRegisterOne", activityRegisterModel);
        }

        //ActivityProductSelectModel******************************************************************************//
        public void ActivityProductInsertOne(ActivityProductSelectModel activityProductSelectModel)
        {
            mapper.Insert("ActivityInfo.InsertActivityProductOne", activityProductSelectModel);
        }

        //ActivityCatalogSelectModel******************************************************************************//
        public void ActivityCatalogInsertOne(ActivityCatalogSelectModel activityCatalogSelectModel)
        {
            mapper.Insert("ActivityInfo.InsertActivityCatalogOne", activityCatalogSelectModel);
        }
    }

}