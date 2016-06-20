using prj_BIZ_System.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace prj_BIZ_System.Services
{
    public class MatchService : _BaseService
    {
        //ActivityRegisterModel
        public IList<ActivityRegisterModel> GetSellerAccountPassActivity(string user_id)
        {
            ActivityRegisterModel param = new ActivityRegisterModel() { user_id = user_id};
            return mapper.QueryForList<ActivityRegisterModel>("Match.SelectSellerAccountPassActivity", param);
        }

        public IList<ActivityRegisterModel> GetCertainActivityHaveCheckSellerNameList(int activity_id)
        {
            ActivityRegisterModel param = new ActivityRegisterModel() { activity_id = activity_id };
            return mapper.QueryForList<ActivityRegisterModel>("Match.SelectCertainActivityHaveCheckSellerName", param);
        }

        public IList<ActivityRegisterModel> GetSellerJoinThoseActivityList(string user_id)
        {
            ActivityRegisterModel param = new ActivityRegisterModel() { user_id = user_id };
            return mapper.QueryForList<ActivityRegisterModel>("Match.SelectSellerJoinThoseActivity", param);
        }

        //ActivityInfoModel
        public IList<ActivityInfoModel> GetAccountNotRegisterActivity(string user_id)
        {
            ActivityRegisterModel param = new ActivityRegisterModel() { user_id = user_id };
            return mapper.QueryForList<ActivityInfoModel>("Match.SelectAccountNotRegisterActivity", param);

        }

        //BuyerInfoModel
        public IList<BuyerInfoModel> GetSellerMatchToBuyerNameAndNeedList(int activity_id)
        {
            BuyerInfoModel param = new BuyerInfoModel() { activity_id = activity_id};
            return mapper.QueryForList<BuyerInfoModel>("Match.SelectSellerMatchToBuyerNameAndNeed", param);
        }

        public IList<BuyerInfoModel> GetBuyerAccountPassActivity(string buyer_id)
        {
            BuyerInfoModel param = new BuyerInfoModel() {buyer_id = buyer_id };
            return mapper.QueryForList<BuyerInfoModel>("Match.SelectBuyerAccountPassActivity", param);
        }

        public IList<BuyerInfoModel> GetBuyerListAllList(int activity_id)
        {
            BuyerInfoModel param = new BuyerInfoModel() { activity_id = activity_id };
            return mapper.QueryForList<BuyerInfoModel>("Match.SelectBuyerListAll", param);
        }

        //MatchmakingNeedModel
        public void MatchmakingNeedInsertOne(MatchmakingNeedModel matchmakingNeedModel)
        {
            mapper.Insert("Match.InsertMatchmakingNeedOne", matchmakingNeedModel);
        }

        public void MatchmakingNeedUpdateOne(MatchmakingNeedModel matchmakingNeedModel)
        {
            mapper.Update("Match.UpdateMatchmakingNeedOne", matchmakingNeedModel);
        }

        public IList<MatchmakingNeedModel> GetSellerForActivityMatchBuyerList(int activity_id, string user_id)
        {
            MatchmakingNeedModel param = new MatchmakingNeedModel() {activity_id = activity_id, seller_id = user_id};
            return mapper.QueryForList<MatchmakingNeedModel>("Match.SelectSellerForActivityMatchBuyer", param);
        }

        public IList<MatchmakingNeedModel> GetBuyerForActivityMatchSellerList(int activity_id, string user_id, string buyer_reply)
        {
            MatchmakingNeedModel param = new MatchmakingNeedModel() { activity_id = activity_id, buyer_id = user_id, buyer_reply = buyer_reply };
            return mapper.QueryForList<MatchmakingNeedModel>("Match.SelectBuyerForActivityMatchSeller", param);
        }

        public IList<MatchmakingNeedModel> GetCertainActivitySellerCheckBuyerList(int activity_id, string user_id)
        {
            MatchmakingNeedModel param = new MatchmakingNeedModel() { activity_id = activity_id, seller_id = user_id };
            return mapper.QueryForList<MatchmakingNeedModel>("Match.SelectCertainActivitySellerCheckBuyer", param);
        }

        public IList<MatchmakingNeedModel> GetCertainActivityBuyerCheckSellerList(int activity_id, string user_id)
        {
            MatchmakingNeedModel param = new MatchmakingNeedModel() { activity_id = activity_id, buyer_id = user_id };
            return mapper.QueryForList<MatchmakingNeedModel>("Match.SelectCertainActivityBuyerCheckSeller", param);
        }

        public IList<MatchmakingNeedModel> GetCertainActivityWithBuyerReplyAllList(int activity_id, string buyer_reply)
        {
            MatchmakingNeedModel param = new MatchmakingNeedModel() { activity_id = activity_id, buyer_reply = buyer_reply };
            return mapper.QueryForList<MatchmakingNeedModel>("Match.SelectCertainActivityWithBuyerReplyAll", param);
        }

        //SchedulePeriodSetModel
        public void MatchTimeIntervalInsert(SchedulePeriodSetModel schedulePeriodSetModel)
        {
            mapper.Insert("Match.InsertMatchTimeInterval", schedulePeriodSetModel);
        }

        public IList<SchedulePeriodSetModel> GetActivityMatchTimeIntervalList(int activity_id)
        {
            SchedulePeriodSetModel param = new SchedulePeriodSetModel() { activity_id = activity_id};
            return mapper.QueryForList<SchedulePeriodSetModel>("Match.SelectActivityMatchTimeInterval",param);
        }

        public void MatchTimeIntervalDeleteOne(int period_sn)
        {
            SchedulePeriodSetModel param = new SchedulePeriodSetModel() {period_sn = period_sn };
            mapper.Delete("Match.DeleteMatchTimeInterval", param);
        }

        public void MatchTimeIntervalUpdateOne(SchedulePeriodSetModel schedulePeriodSetModel)
        {
            mapper.Update("Match.UpdateMatchTimeInterval", schedulePeriodSetModel);
        }

        //MatchmakingScheduleModel
        public void CertainTimeMatchSellerInsert(MatchmakingScheduleModel matchmakingScheduleModel)
        {
            mapper.Insert("Match.InsertCertainTimeMatchSeller", matchmakingScheduleModel);
        }

        public IList<MatchmakingScheduleModel> GetCertainActivityMatchMakingDataList(int activity_id)
        {
            MatchmakingScheduleModel param = new MatchmakingScheduleModel() { activity_id = activity_id};
            return mapper.QueryForList<MatchmakingScheduleModel>("Match.SelectCertainActivityMatchMakingData", param);
        }

        public void CertainActivityMatchkingDataUpdate(MatchmakingScheduleModel matchmakingScheduleModel)
        {
            mapper.Update("Match.UpdateCertainActivityMatchkingData", matchmakingScheduleModel);
        }

        public void CertainActivityMatchkingDataDelete(MatchmakingScheduleModel matchmakingScheduleModel)
        {
            mapper.Delete("Match.DeleteCertainActivityMatchkingData", matchmakingScheduleModel);
        }

    }
}