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

        public IList<ActivityInfoModel> GetActivityInfoList(int? grp_id)
        {
            ActivityInfoModel param = new ActivityInfoModel() { grp_id = grp_id };
            return mapper.QueryForList<ActivityInfoModel>("ActivityInfo.SelectActivityAll", param);
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
            Object obj = mapper.Update("ActivityInfo.UpdateActivityOne", activityInfoModel);
            return (int)obj;
        }

        //NewsModel******************************************************************************//
        public IList<NewsModel> GetNewsAll(string manager_id)
        {

            NewsModel param = new NewsModel() { manager_id = manager_id };
            return mapper.QueryForList<NewsModel>("ActivityInfo.SelectNewsAll", param);
        }


        public IList<NewsModel> GetNewsType(string news_type,int? grp_id)
        {
            NewsModel param = new NewsModel() { news_type = news_type, grp_id = grp_id };
            return mapper.QueryForList<NewsModel>("ActivityInfo.SelectNewsType", param);
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

        public IList<BuyerInfoModel> GetBuyerInfoAll(int? grp_id)
        {
            BuyerInfoModel param = new BuyerInfoModel() { grp_id = grp_id };
            return mapper.QueryForList<BuyerInfoModel>("ActivityInfo.SelectBuyerInfoAll", param);
        }

        public IList<BuyerInfoModel> GetBuyerInfoActivity(int activity_id)
        {
            BuyerInfoModel param = new BuyerInfoModel() { activity_id = activity_id };
            return mapper.QueryForList<BuyerInfoModel>("ActivityInfo.SelectBuyerInfoActivity", param);
        }

        public BuyerInfoModel GetBuyerInfoOne(int serial_no)
        {
            BuyerInfoModel param = new BuyerInfoModel() { serial_no = serial_no };
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
            EnterpriseSortAndListModel param = new EnterpriseSortAndListModel() { user_id = user_id };
            return mapper.QueryForList<EnterpriseSortAndListModel>("ActivityInfo.SelectEnterpriseByUserId", param);
        }

        //ActivityRegisterModel******************************************************************************//

        public IList<ActivityRegisterModel> GetActivityCheckAllByCondition(string activity_name, string company,string starttime, string endtime,int? grp_id)
        {
            DateTime startDate;
            DateTime endDate;

            if (starttime == null || starttime == "")
                starttime = "0001/01/01";
            startDate = Convert.ToDateTime(starttime);
            if (endtime == null || endtime == "")
                endtime = "9999/12/30";
            endDate = Convert.ToDateTime(endtime);

            ActivityRegisterModel param = new ActivityRegisterModel { activity_name = activity_name, company = company, starttime = startDate, endtime = endDate, grp_id = grp_id };
            return mapper.QueryForList<ActivityRegisterModel>("ActivityInfo.SelectActivityCheckAll", param);
        }

        public ActivityRegisterModel GetActivityRegisterOne(int register_id)
        {
            ActivityRegisterModel param = new ActivityRegisterModel { register_id = register_id };
            return (ActivityRegisterModel)mapper.QueryForObject("ActivityInfo.SelectActivityRegisterOne", param);
        }

        public ActivityRegisterModel GetActivityRegisterSelectOne(int activity_id,string user_id)
        {
            ActivityRegisterModel param = new ActivityRegisterModel { activity_id = activity_id, user_id = user_id };
            return (ActivityRegisterModel)mapper.QueryForObject("ActivityInfo.SelectActivityRegisterSelectOne", param);
        }

        public object ActivityRegisterInserOne(ActivityRegisterModel activityRegisterModel)
        {
            return mapper.Insert("ActivityInfo.InsertActivityRegisterOne", activityRegisterModel);
        }

        public void ActivityRegisterUpdateOneChk(ActivityRegisterModel activityRegisterModel)
        {
            mapper.Update("ActivityInfo.UpdateActivityRegisterOneChk", activityRegisterModel);
        }

        public int ActivityRegisterUpdateOne(ActivityRegisterModel activityRegisterModel)
        {
            return mapper.Update("ActivityInfo.UpdateActivityRegisterOne", activityRegisterModel);
        }

        public int ActivityRegisterDeleteOne(int activity_id, string user_id)
        {
            ActivityRegisterModel param = new ActivityRegisterModel { activity_id = activity_id, user_id = user_id };
            return mapper.Update("ActivityInfo.DeleteActivityRegisterOne", param);
        }

        //ActivityProductSelectModel******************************************************************************//
        public void ActivityProductDeleteOne(int activity_id,string user_id)
        {
            ActivityProductSelectModel param = new ActivityProductSelectModel { activity_id = activity_id, user_id = user_id };
            mapper.Delete("ActivityInfo.DeleteActivityProductOne", param);
        }

        public void ActivityProductInsertOne(ActivityProductSelectModel activityProductSelectModel)
        {
            mapper.Insert("ActivityInfo.InsertActivityProductOne", activityProductSelectModel);
        }

        public IList<ActivityProductSelectModel> GetActivityProductSelectList(string user_id, int activity_id)
        {
            ActivityProductSelectModel param = new ActivityProductSelectModel() { user_id = user_id, activity_id = activity_id };
            return mapper.QueryForList<ActivityProductSelectModel>("ActivityInfo.SelectActivityProductByCondition", param);
        }

        //ActivityCatalogSelectModel******************************************************************************//
        public void ActivityCatalogDeleteOne(int activity_id, string user_id)
        {
            ActivityCatalogSelectModel param = new ActivityCatalogSelectModel { activity_id = activity_id, user_id = user_id };
            mapper.Delete("ActivityInfo.DeleteActivityCatalogOne", param);
        }

        public void ActivityCatalogInsertOne(ActivityCatalogSelectModel activityCatalogSelectModel)
        {
            mapper.Insert("ActivityInfo.InsertActivityCatalogOne", activityCatalogSelectModel);
        }

        public IList<ActivityCatalogSelectModel> GetActivityCatalogSelectList(string user_id, int activity_id)
        {
            ActivityCatalogSelectModel param = new ActivityCatalogSelectModel() { user_id = user_id, activity_id = activity_id };
            return mapper.QueryForList<ActivityCatalogSelectModel>("ActivityInfo.SelectActivityCatalogByCondition", param);
        }
    }

}