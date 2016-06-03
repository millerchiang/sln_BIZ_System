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

        //ActivityInfoModel
        public IList<ActivityInfoModel> GetAccountNotRegisterActivity(string user_id)
        {
            ActivityRegisterModel param = new ActivityRegisterModel() { user_id = user_id };
            return mapper.QueryForList<ActivityInfoModel>("Match.SelectAccountNotRegisterActivity", param);

        }

        //BuyerInfoModel
        public IList<BuyerInfoModel> GetSellerMatchToBuyerNameAndNeed(int activity_id)
        {
            BuyerInfoModel param = new BuyerInfoModel() { activity_id = activity_id };
            return mapper.QueryForList<BuyerInfoModel>("Match.SelectSellerMatchToBuyerNameAndNeed", param);
        }

        public IList<BuyerInfoModel> GetBuyerAccountPassActivity(string buyer_id)
        {
            BuyerInfoModel param = new BuyerInfoModel() {buyer_id = buyer_id };
            return mapper.QueryForList<BuyerInfoModel>("Match.SelectBuyerAccountPassActivity", param);
        }

        //MatchmakingNeedModel
        public void MatchmakingNeedInsertOne(MatchmakingNeedModel matchmakingNeedModel)
        {
            mapper.Insert("Match.InsertMatchmakingNeedOne", matchmakingNeedModel);
        }

    }
}