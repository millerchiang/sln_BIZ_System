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

        public IList<ActivityInfoModel> GetActivityInfoList(int? grp_id, DateTime? endtime)
        {
            ActivityInfoModel param = new ActivityInfoModel();
            param.grp_id = grp_id;
            if (endtime!=null)
            {
                param.endtime = (DateTime)endtime;
            }
            return mapper.QueryForList<ActivityInfoModel>("ActivityInfo.SelectActivityAll", param);
        }

        public IList<ActivityInfoModel> GetActivityInfoListNotStart(int? grp_id)
        {
            ActivityInfoModel param = new ActivityInfoModel() { grp_id = grp_id };
            return mapper.QueryForList<ActivityInfoModel>("ActivityInfo.SelectActivityNotStart", param);
        }

        public IList<ActivityInfoModel> GetActivityInfoListLimit(int limit)
        {
            return mapper.QueryForList<ActivityInfoModel>("ActivityInfo.SelectActivityLimit", limit);
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
        public IList<NewsModel> GetNewsAll(string manager_id, string news_style)
        {

            NewsModel param = new NewsModel() { manager_id = manager_id, news_style= news_style };
            return mapper.QueryForList<NewsModel>("ActivityInfo.SelectNewsAll", param);
        }

        public IList<NewsModel> GetNewsLimit(int limit )
        {
            return mapper.QueryForList<NewsModel>("ActivityInfo.SelectNewsLimit", limit);
        }

        public IList<NewsModel> GetNews0Limit(int limit)
        {
            return mapper.QueryForList<NewsModel>("ActivityInfo.SelectNews0Limit", limit);
        }
        public IList<NewsModel> GetNews1Limit(int limit)
        {
            return mapper.QueryForList<NewsModel>("ActivityInfo.SelectNews1Limit", limit);
        }

        public IList<NewsModel> GetNewsLimit_e(int limit)
        {
            return mapper.QueryForList<NewsModel>("ActivityInfo.SelectNewsLimit_e", limit);
        }

        public IList<NewsModel> GetNews0Limit_e(int limit)
        {
            return mapper.QueryForList<NewsModel>("ActivityInfo.SelectNews0Limit_e", limit);
        }
        public IList<NewsModel> GetNews1Limit_e(int limit)
        {
            return mapper.QueryForList<NewsModel>("ActivityInfo.SelectNews1Limit_e", limit);
        }


        public IList<NewsModel> GetNewsType(string news_type,int? grp_id,string news_style)
        {
            NewsModel param = new NewsModel() { news_type = news_type, grp_id = grp_id, news_style= news_style };
            return mapper.QueryForList<NewsModel>("ActivityInfo.SelectNewsType", param);
        }

        public IList<NewsModel> GetNewsTypeView(string news_type, int? grp_id, string news_style)
        {
            NewsModel param = new NewsModel() { news_type = news_type, grp_id = grp_id, news_style = news_style };
            return mapper.QueryForList<NewsModel>("ActivityInfo.SelectNewsTypeView", param);
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

        //--------------問卷----------------------------------------------------
        public QuestionnaireModel GetQuestionnaireOne(int activity_id,string buyer_id,string seller_id)
        {
            QuestionnaireModel param = new QuestionnaireModel() { activity_id = activity_id,buyer_id=buyer_id,seller_id=seller_id };
            return (QuestionnaireModel)mapper.QueryForObject("ActivityInfo.SelectQuestionnaire", param);
        }

        public IList<QuestionnaireModel> GetQuestionnaireList(int activity_id, string buyer_id)
        {
            QuestionnaireModel param = new QuestionnaireModel() { activity_id = activity_id, buyer_id = buyer_id };
            return mapper.QueryForList<QuestionnaireModel>("ActivityInfo.SelectQuestionnaireList", param);
        }

        public void QuestionnaireInsertOne(QuestionnaireModel questionnaireModel)
        {
            mapper.Insert("ActivityInfo.InsertQuestionnaire", questionnaireModel);
        }

        public void QuestionnaireDeleteOne(int activity_id, string buyer_id, string seller_id)
        {
            QuestionnaireModel param = new QuestionnaireModel() { activity_id = activity_id, buyer_id = buyer_id, seller_id = seller_id };
            mapper.Delete("ActivityInfo.DeleteQuestionnaire", param);
        }

        public void QuestionnaireUpdateOne(QuestionnaireModel questionnaireModel)
        {
            mapper.Update("ActivityInfo.UpdateQuestionnaire", questionnaireModel);
        }


        //UserInfoToIdAndCpModel******************************************************************************//
        /*搜尋出user_id和company*/
        public IList<UserInfoToIdAndCpModel> GetUserInfoToIdandCp()
        {
            return mapper.QueryForList<UserInfoToIdAndCpModel>("ActivityInfo.SelectUserInfoToIdandCp", null);
        }

        //BuyerInfoModel******************************************************************************//

        public IList<BuyerInfoModel> GetBuyerInfoAll(int? grp_id, DateTime? endtime, string activity_name, string company)
        {
            if (activity_name != null) activity_name = activity_name.ToUpper();
            if (company != null) company = company.ToUpper();
            BuyerInfoModel param = new BuyerInfoModel() { grp_id = grp_id, endtime= endtime, activity_name= activity_name, company = company };
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

        public BuyerInfoModel GetBuyerDataByActivityWithIdOne(int activity_id, string buyer_id)
        {
            BuyerInfoModel param = new BuyerInfoModel() { activity_id = activity_id, buyer_id = buyer_id };
            return (BuyerInfoModel)mapper.QueryForObject("ActivityInfo.SelectBuyerDataByActivityWithId", param);
        }

        public object BuyerInfoInsertOne(BuyerInfoModel buyerInfoModel)
        {
            return mapper.Insert("ActivityInfo.InsertBuyerInfoOne", buyerInfoModel);
        }

        public bool BuyerInfoUpdateOne(BuyerInfoModel buyerInfoModel)
        {
            if (buyerInfoModel.annual_turnover_1y_ago == null)
                buyerInfoModel.annual_turnover_1y_ago = 0;
            if (buyerInfoModel.annual_turnover_2y_ago == null)
                buyerInfoModel.annual_turnover_2y_ago = 0;
            if (buyerInfoModel.annual_turnover_3y_ago == null)
                buyerInfoModel.annual_turnover_3y_ago = 0;
            if (buyerInfoModel.estimated_purchasing_now == null)
                buyerInfoModel.estimated_purchasing_now = 0;
            if (buyerInfoModel.estimated_purchasing_1y_ago == null)
                buyerInfoModel.estimated_purchasing_1y_ago = 0;
            if (buyerInfoModel.estimated_purchasing_2y_ago == null)
                buyerInfoModel.estimated_purchasing_2y_ago = 0;

            return mapper.Update("ActivityInfo.UpdateBuyerInfoOne", buyerInfoModel) >0;
        }

        public void BuyerInfoDeleteOne(int serial_no)
        {
            BuyerInfoModel param = new BuyerInfoModel() { serial_no = serial_no };
            mapper.Delete("ActivityInfo.DeleteBuyerInfoOne", param);
        }

        public IList<ActivityRegisterModel> GetSellerInfoActivity(int activity_id)
        {
            ActivityRegisterModel param = new ActivityRegisterModel() { activity_id = activity_id };
            return mapper.QueryForList<ActivityRegisterModel>("ActivityInfo.SelectSellerInfoActivity", param);
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

        public IList<ActivityRegisterModel> GetActivityCheckAllByConditionWithId(int? activity_id, string company, string starttime, string endtime, int? grp_id, string manager_check)
        {
            DateTime startDate;
            DateTime endDate;

            if (starttime == null || starttime == "")
                starttime = "0001/01/01";
            startDate = Convert.ToDateTime(starttime);
            if (endtime == null || endtime == "")
                endtime = "9999/12/30";
            endDate = Convert.ToDateTime(endtime);
            int activity_id_for_reg = (activity_id == null) ? 0 : (int)activity_id;
            ActivityRegisterModel param = new ActivityRegisterModel { activity_id = activity_id_for_reg, company = company, starttime = startDate, endtime = endDate, grp_id = grp_id, manager_check= manager_check};
            return mapper.QueryForList<ActivityRegisterModel>("ActivityInfo.SelectActivityCheckAll", param);
        }

        public ActivityRegisterModel GetActivityRegisterOne(int register_id)
        {
            ActivityRegisterModel param = new ActivityRegisterModel { register_id = register_id };
            return (ActivityRegisterModel)mapper.QueryForObject("ActivityInfo.SelectActivityRegisterOne", param);
        }

        public IList<ActivityRegisterModel> GetActivityRegisterList(int activity_id)
        {
            ActivityRegisterModel param = new ActivityRegisterModel { activity_id = activity_id };
            return mapper.QueryForList<ActivityRegisterModel>("ActivityInfo.SelectActivityRegisterList", param);
        }

        public IList<ActivityRegisterModel> GetARCheckPassList(int activity_id)
        {
            ActivityRegisterModel param = new ActivityRegisterModel { activity_id = activity_id };
            return mapper.QueryForList<ActivityRegisterModel>("ActivityInfo.SelectARCheckPassList", param);
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

        public ActivityRegisterModel ActivityRegisterChkMailInfo(int register_id)
        {
            ActivityRegisterModel param = new ActivityRegisterModel { register_id = register_id};
            return mapper.QueryForObject<ActivityRegisterModel>("ActivityInfo.ActivityRegisterChkMailInfo", param);
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

        public ActivityRegisterModel SelectSellerRegisterInfo(int activity_id , string user_id)
        {
            var param = new ActivityRegisterModel() { activity_id = activity_id , user_id = user_id };
            return mapper.QueryForObject<ActivityRegisterModel>("Match.SelectSellerRegisterInfo" , param);
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

        /*顯示所有的活動照片*/
        public IList<ActivityPhotoModel> getAllPhoto(string manager_id)
        {
            ActivityPhotoModel param = new ActivityPhotoModel() { manager_id = manager_id };
            return mapper.QueryForList<ActivityPhotoModel>("ActivityInfo.SelectPhotoListByManagerId", param);
        }
        /*顯示所有的活動照片*/
        public IList<ActivityPhotoModel> getActivityViewPhoto()
        {
//            ActivityPhotoModel param = new ActivityPhotoModel() {};
            return mapper.QueryForList<ActivityPhotoModel>("ActivityInfo.SelectActivityViewPhotoList", null);
        }
        public ActivityPhotoModel getPhotoOne(int? photo_id)
        {
            ActivityPhotoModel param = new ActivityPhotoModel() { photo_id = photo_id };
            return mapper.QueryForObject<ActivityPhotoModel>("ActivityInfo.SelectPhotoByPhotoId", param);
        }

        /*假刪除活動照片*/
        public bool PhotoListDeleteFake(int[] del_photos)
        {
            if (del_photos != null)
            {
                foreach (int del_photo in del_photos)
                {
                    var tempModel = new ActivityPhotoModel { photo_id = del_photo };
                    mapper.Update("ActivityInfo.DeletePhotoListByPhotoIdFake", tempModel);
                }
            }
            return true;
        }
        /*新增活動照片*/
        public object insertPhotoList(ActivityPhotoModel param)
        {
            //param.user_id = user_id;
            param.deleted = "1";
            return mapper.Insert("ActivityInfo.InsertPhotoList", param);
        }
        /*修改活動照片*/
        public int updatePhotoList(ActivityPhotoModel param)
        {
            //param.user_id = user_id;
            param.deleted = "1";
            return mapper.Update("ActivityInfo.UpdatePhotoList", param);
        }


        /*顯示所有的Banner照片*/
        public IList<BannerPhotoModel> getAllBanner(string manager_id)
        {
            BannerPhotoModel param = new BannerPhotoModel() { manager_id = manager_id };
            return mapper.QueryForList<BannerPhotoModel>("ActivityInfo.SelectBannerListByManagerId", param);
        }
        /*顯示所有的Banner照片*/
        public IList<BannerPhotoModel> getBannerViewPhoto()
        {
            //            ActivityPhotoModel param = new ActivityPhotoModel() {};
            return mapper.QueryForList<BannerPhotoModel>("ActivityInfo.SelectBannerViewPhotoList", null);
        }
        public BannerPhotoModel getBannerOne(int? photo_id)
        {
            BannerPhotoModel param = new BannerPhotoModel() { photo_id = photo_id };
            return mapper.QueryForObject<BannerPhotoModel>("ActivityInfo.SelectBannerByPhotoId", param);
        }

        /*新增Banner照片*/
        public object insertBannerList(BannerPhotoModel param)
        {
            //param.user_id = user_id;
            param.deleted = "1";
            return mapper.Insert("ActivityInfo.InsertBannerList", param);
        }
        /*修改Banner照片*/
        public int updateBannerList(BannerPhotoModel param)
        {
            //param.user_id = user_id;
            param.deleted = "1";
            return mapper.Update("ActivityInfo.UpdateBannerList", param);
        }

        /*假刪除Banner照片*/
        public bool BannerListDeleteFake(int[] del_photos)
        {
            if (del_photos != null)
            {
                foreach (int del_photo in del_photos)
                {
                    var tempModel = new BannerPhotoModel { photo_id = del_photo };
                    mapper.Update("ActivityInfo.DeleteBannerListByPhotoIdFake", tempModel);
                }
            }
            return true;
        }

        public bool BannerListUpdateActive(int[] view_photos)
        {
            if (view_photos != null)
            {
                foreach (int view_photo in view_photos)
                {
                    var tempModel = new BannerPhotoModel { photo_id = view_photo };
                    mapper.Update("ActivityInfo.UpdateBannerActiveByPhotoId", tempModel);
                }
            }
            return true;
        }

    }

}