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
using prj_BIZ_System.Extensions;

namespace prj_BIZ_System.WebService
{
    [MvcStyleBinding]
    public class MatchController : ApiController
    {
        MatchService matchService = new MatchService();

        [HttpGet]
        public object GetAccountActivity(string user_id)
        {
            if (user_id == null) return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "user id is null");

            var accountActivitys = matchService.GetSellerAccountPassActivity(user_id).Select(
                activityRegister =>
                new 
                {
                    activityRegister.activity_id,
                    activityRegister.activity_name,
                    is_buyer = "0",
                    activityRegister.seller_select
                }
            ).ToList();

            accountActivitys.AddRange(matchService.GetUserWhenActivityBuyer(user_id).Select(
                buyerInfo =>
                new 
                {
                    buyerInfo.activity_id,
                    buyerInfo.activity_name,
                    is_buyer = "1",
                    buyerInfo.seller_select
                }
            ).ToList());
            return Request.CreateResponse(HttpStatusCode.OK, accountActivitys);
        }

        [HttpGet]
        public object GetRecommedActivity(string user_id)
        {
            if (user_id.IsNullOrEmpty()) return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "activity id is null");

            var recommedActivitys = matchService.GetAccountNotRegisterActivity(user_id).Select(
                activityInfo =>
                new 
                {
                    activityInfo.activity_id,
                    activityInfo.activity_name,
                    is_buyer = "0"
                }
            ).ToList();
            return Request.CreateResponse(HttpStatusCode.OK, recommedActivitys);
        }

        #region 取得活動媒合買家
        [HttpGet]
        public object GetBuyerForMatch(int activity_id)
        {
            if (activity_id == null) return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "activity id is null");
            var buyerForMatch = matchService.GetSellerMatchToBuyerNameAndNeedList(activity_id)
                .Select(
                buyerInfo => 
                new
                {
                    buyerInfo.activity_id,
                    buyerInfo.buyer_id,
                    buyerInfo.company,
                    buyerInfo.buyer_need
                }
            ).ToList();
            return Request.CreateResponse(HttpStatusCode.OK, buyerForMatch);
        }
        #endregion

        #region 取得活動媒合賣家
        [HttpGet]
        public object GetSellerForMatch(int activity_id)
        {
            if (activity_id == null) return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "activity id is null");
            var sellerForMatch = matchService.GetCertainActivityHaveCheckSellerNameList(activity_id)
                .Select(
                activityRegisterModel => 
                new 
                {
                    activityRegisterModel.activity_id,
                    seller_id = activityRegisterModel.user_id,
                    activityRegisterModel.company
                }
            ).ToList();
            return Request.CreateResponse(HttpStatusCode.OK, sellerForMatch);
        }
        #endregion

        [HttpGet]
        public object GetBuyerForSellerCheck(int activity_id, string user_id)
        {
            if (activity_id == null || user_id.IsNullOrEmpty()) return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "activity id is null");
            Dictionary<string, dynamic> sellerNeed = new Dictionary<string, dynamic>();
            sellerNeed["check"] = matchService.GetCertainActivitySellerCheckBuyerList(activity_id, user_id)
                                .Select(
                                    matchmakingNeed =>
                                    new 
                                    {
                                        matchmakingNeed.buyer_id,
                                        matchmakingNeed.company
                                    }
                                ).ToList();
            sellerNeed["schedule"] = new List<dynamic>();
            //sellerNeed.schedule = matchService.GetWhenUserIsSellerMatchMakingDataList(activity_id, user_id).Select(
            //    matchmakingNeed =>
            //    new Buyer
            //    {
            //        buyer_id = matchmakingNeed.buyer_id,
            //        company = matchmakingNeed.company
            //    }
            //).ToList();
            return Request.CreateResponse(HttpStatusCode.OK, sellerNeed);
        }

        [HttpGet]
        public object GetSellerForBuyerCheck(int activity_id, string user_id)
        {
            if (activity_id == null || user_id.IsNullOrEmpty()) return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "activity id is null");
            Dictionary<string, dynamic> buyerNeed = new Dictionary<string, dynamic>();
            buyerNeed["check"] = matchService.GetCertainActivityBuyerCheckSellerList(activity_id, user_id)
                                .Select(
                                    matchmakingNeed =>
                                    new 
                                    {
                                        matchmakingNeed.seller_id,
                                        matchmakingNeed.company,
                                    }
                                ).ToList();
            buyerNeed["schedule"] = new List<dynamic>();
            //buyerNeed.manager_schedule = matchService.GetWhenUserIsBuyerMatchMakingDataList(activity_id, user_id).Select(
            //    matchmakingNeed =>
            //    new Seller
            //    {
            //        seller_id = matchmakingNeed.buyer_id,
            //        company = matchmakingNeed.company
            //    }
            //).ToList();
            return Request.CreateResponse(HttpStatusCode.OK, buyerNeed);
        }

        [HttpPost]
        public object SellerMatchToBuyer(int activity_id, string seller_id, string buyer_id)
        {
            if (activity_id == null || seller_id.IsNullOrEmpty())
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "id is null");

            object matchmakingNeedId = null;

            string[] buyer_ids = buyer_id != null ? buyer_id.Split(',') : new string[] { "" };
            var oldSellerneedList = matchService.getMatchmakingSellerneedList(activity_id, seller_id)
                                                .Select(model => new { model.serial_no, model.buyer_id })
                                                .ToList();

            MatchmakingAllModel matchmakingAllModel = new MatchmakingAllModel();
            matchmakingAllModel.activity_id = activity_id;
            matchmakingAllModel.seller_id = seller_id;

            if (oldSellerneedList.Count == 0)
            {
                foreach (string id in buyer_ids)
                {
                    matchmakingAllModel.buyer_id = id;
                    matchmakingNeedId = matchService.MatchmakingSellerneedInsertOne(matchmakingAllModel);
                }
            }
            else
            {
                oldSellerneedList.ForEach(oldSeller =>
                {
                    if (!buyer_ids.Contains(oldSeller.buyer_id))
                    {
                        matchService.MatchmakingSellerneedDelete(oldSeller.serial_no);
                    }
                });
                var oldBuyerId = oldSellerneedList.Select(oldSellerneed => oldSellerneed.buyer_id)
                                                  .ToArray();
                var buyerExcept = buyer_ids.Except(oldBuyerId);
                if(!buyerExcept.Contains(""))
                {
                    buyerExcept.ForEach(buyerId =>
                    {
                        matchmakingAllModel.buyer_id = buyerId;
                        matchmakingNeedId = matchService.MatchmakingSellerneedInsertOne(matchmakingAllModel);
                    });
                }
            }
            return Request.CreateResponse(HttpStatusCode.OK, matchmakingNeedId);
        }

        [HttpPost]
        public object BuyerMatchToSeller(int activity_id, string buyer_id, string seller_id)
        {
            if (activity_id == null || seller_id.IsNullOrEmpty() || buyer_id.IsNullOrEmpty())
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "id is null");

            string[] seller_ids = seller_id.Split(',');

            MatchmakingAllModel matchmakingAllModel = new MatchmakingAllModel();
            matchmakingAllModel.activity_id = activity_id;
            matchmakingAllModel.buyer_id = buyer_id;

            object matchmakingNeedId = null;
            foreach (string id in seller_ids)
            {
                matchmakingAllModel.seller_id = id;
                matchmakingNeedId = matchService.MatchmakingBuyerneedInsertOne(matchmakingAllModel);
            }
            return Request.CreateResponse(HttpStatusCode.OK, matchmakingNeedId);
        }
    }
}
