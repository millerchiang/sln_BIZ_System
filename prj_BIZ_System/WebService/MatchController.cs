using prj_BIZ_System.Models;
using prj_BIZ_System.Services;
using prj_BIZ_System.WebService.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebApiContrib.ModelBinders;

namespace prj_BIZ_System.WebService
{
    [MvcStyleBinding]
    public class MatchController : ApiController
    {
        MatchService matchService = new MatchService();

        [HttpGet]
        public IList<AccountActivity> GetAccountActivity(string user_id)
        {
            if (user_id == null) return null;

            List<AccountActivity> accountActivitys = matchService.GetSellerAccountPassActivity(user_id).Select(
                activityRegister =>
                new AccountActivity
                {
                    activity_id = activityRegister.activity_id,
                    activity_name = activityRegister.activity_name,
                    is_buyer = "0"
                }
            ).ToList();

            accountActivitys.AddRange(matchService.GetUserWhenActivityBuyer(user_id).Select(
                buyerInfo =>
                new AccountActivity
                {
                    activity_id = buyerInfo.activity_id,
                    activity_name = buyerInfo.activity_name,
                    is_buyer = "1"
                }
            ).ToList());
            return accountActivitys;
        }

        [HttpGet]
        public IList<AccountActivity> GetRecommedActivity(string user_id)
        {
            if (user_id == null) return null;

            List<AccountActivity> recommedActivitys = matchService.GetAccountNotRegisterActivity(user_id).Select(
                activityInfo =>
                new AccountActivity
                {
                    activity_id = activityInfo.activity_id,
                    activity_name = activityInfo.activity_name,
                    is_buyer = "0"
                }
            ).ToList();
            return recommedActivitys;
        }

        [HttpGet]
        public IList<Buyer> GetBuyerForMatch(int activity_id)
        {
            return matchService.GetSellerMatchToBuyerNameAndNeedList(activity_id).Select(
                buyerInfo => new Buyer
                {
                    activity_id = buyerInfo.activity_id,
                    buyer_id = buyerInfo.buyer_id,
                    company = buyerInfo.company,
                    buyer_need = buyerInfo.buyer_need
                }
            ).ToList();
        }

        //[HttpGet]
        //public IList<Seller> GetSellerForMatch(int activity_id, string user_id)
        //{
        //    return matchService.GetBuyerForActivityMatchSellerList(activity_id, user_id, "").Select(
        //        matchmakingNeed => new Seller
        //        {
        //            activity_id = matchmakingNeed.activity_id,
        //            seller_id = matchmakingNeed.seller_id,
        //            company = matchmakingNeed.company
        //        }
        //    ).ToList();
        //}

        [HttpGet]
        public SellerNeed GetBuyerForSellerCheck(int activity_id, string user_id)
        {
            if (user_id == null) return null;
            SellerNeed sellerNeed = new SellerNeed();
            sellerNeed.seller_check =
            matchService.GetSellerForActivityMatchBuyerList(activity_id, user_id).Select(
                matchmakingNeed =>
                new Buyer
                {
                    buyer_id = matchmakingNeed.buyer_id,
                    company = matchmakingNeed.company
                }
            ).ToList();

            sellerNeed.manager_schedule = matchService.GetWhenUserIsSellerMatchMakingDataList(activity_id, user_id).Select(
                matchmakingNeed =>
                new Buyer
                {
                    buyer_id = matchmakingNeed.buyer_id,
                    company = matchmakingNeed.company
                }
            ).ToList();

            return sellerNeed;
        }

        //[HttpGet]
        //public BuyerNeed GetSellerForBuyerCheck(int activity_id, string user_id)
        //{
        //    if (user_id == null) return null;
        //    BuyerNeed buyerNeed = new BuyerNeed();
        //    buyerNeed.buyer_check =
        //    matchService.GetBuyerForActivityMatchSellerList(activity_id, user_id, "1").Select(
        //        matchmakingNeed =>
        //        new Seller
        //        {
        //            seller_id = matchmakingNeed.buyer_id,
        //            company = matchmakingNeed.company,
        //        }
        //    ).ToList();

        //    buyerNeed.manager_schedule = matchService.GetWhenUserIsBuyerMatchMakingDataList(activity_id, user_id).Select(
        //        matchmakingNeed =>
        //        new Seller
        //        {
        //            seller_id = matchmakingNeed.buyer_id,
        //            company = matchmakingNeed.company
        //        }
        //    ).ToList();

        //    return buyerNeed;
        //}

        //[HttpPost]
        //public object SellerMatchToBuyer(int activity_id, string seller_id, string buyer_id)
        //{
        //    string[] buyer_ids = buyer_id.Split(',');

        //    MatchmakingNeedModel matchmakingNeedModel = new MatchmakingNeedModel();
        //    matchmakingNeedModel.activity_id = activity_id;
        //    matchmakingNeedModel.seller_id = seller_id;
        //    matchmakingNeedModel.buyer_reply = "0";

        //    object matchmakingNeedId = null;
        //    foreach (string id in buyer_ids)
        //    {
        //        matchmakingNeedModel.buyer_id = id;
        //        matchmakingNeedId = matchService.MatchmakingNeedInsertOne(matchmakingNeedModel);
        //    }

        //    return matchmakingNeedId;
        //}

        //[HttpPost]
        //public int BuyerMatchToSeller(int activity_id, string buyer_id, string seller_id)
        //{
        //    string[] seller_ids = seller_id.Split(',');

        //    MatchmakingNeedModel matchmakingNeedModel = new MatchmakingNeedModel();
        //    matchmakingNeedModel.activity_id = activity_id;
        //    matchmakingNeedModel.buyer_id = buyer_id;

        //    int matchmakingNeedUpDateRow = 0;
        //    foreach (string id in seller_ids)
        //    {
        //        matchmakingNeedModel.seller_id = id;
        //        matchmakingNeedUpDateRow += matchService.MatchmakingNeedUpdateOne(matchmakingNeedModel);
        //    }

        //    return matchmakingNeedUpDateRow;
        //}


    }
}
