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
            if (user_id.IsNullOrEmpty()) return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "user id is null");

            DateTime dateNow = DateTime.Now;
            var accountActivitys = matchService.GetSellerAccountPassActivity(user_id)
                .Where(act => ((TimeSpan)(act.endtime - dateNow)).TotalHours > 0)
                .Select(
                ar =>
                new 
                {
                    ar.activity_id,
                    ar.activity_name,
                    ar.activity_name_en,
                    is_buyer = "0",
                    ar.seller_select,
                    ar.matchmaking_select
                }
            ).ToList();

            accountActivitys.AddRange(matchService.GetUserWhenActivityBuyer(user_id)
                .Where(act => ((TimeSpan)(act.endtime - dateNow)).TotalHours > 0)
                .Select(
                bi =>
                new 
                {
                    bi.activity_id,
                    bi.activity_name,
                    bi.activity_name_en,
                    is_buyer = "1",
                    bi.seller_select,
                    bi.matchmaking_select
                }
            ).ToList());
            return Request.CreateResponse(HttpStatusCode.OK, accountActivitys);
        }

        [HttpGet]
        public object GetAccountActivityEnd(string user_id)
        {
            if (user_id.IsNullOrEmpty()) return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "user id is null");

            DateTime dateNow = DateTime.Now;
            var accountActivitys = matchService.GetSellerAccountPassActivity(user_id)
                .Where(act => ((TimeSpan)(act.endtime - dateNow)).TotalHours <= 0)
                .Select(
                ar =>
                new 
                {
                    ar.activity_id,
                    ar.activity_name,
                    ar.activity_name_en,
                    is_buyer = "0",
                    ar.seller_select,
                    ar.matchmaking_select
                }
            ).ToList();

            accountActivitys.AddRange(matchService.GetUserWhenActivityBuyer(user_id)
                .Where(act => ((TimeSpan)(act.endtime - dateNow)).TotalHours <= 0)
                .Select(
                bi =>
                new 
                {
                    bi.activity_id,
                    bi.activity_name,
                    bi.activity_name_en,
                    is_buyer = "1",
                    bi.seller_select,
                    bi.matchmaking_select
                }
            ).ToList());
            return Request.CreateResponse(HttpStatusCode.OK, accountActivitys);
        }

        [HttpGet]
        public object GetRecommedActivity(string user_id)
        {
            if (user_id.IsNullOrEmpty()) return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "activity id is null");

            DateTime dateNow = DateTime.Now;
            var recommedActivitys = matchService.GetAccountNotRegisterActivity(user_id)
                .Where(act => ((TimeSpan)(act.starttime - dateNow)).TotalHours > 24)
                .Select(
                ai =>
                new 
                {
                    ai.activity_id,
                    ai.activity_name,
                    ai.activity_name_en,
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
                bi => 
                new
                {
                    bi.activity_id,
                    bi.buyer_id,
                    bi.company,
                    bi.company_en,
                    bi.buyer_need,
                    bi.buyer_need_en
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
                    activityRegisterModel.company,
                    activityRegisterModel.company_en
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
                                    mn =>
                                    new 
                                    {
                                        mn.buyer_id,
                                        mn.company,
                                        mn.company_en
                                    }
                                ).ToList();
            sellerNeed["schedule"] = new List<dynamic>();
            sellerNeed["schedule"] = matchService.GetWhenUserIsSellerMatchMakingDataList(activity_id, user_id).Select(
                mn =>
                new 
                {
                    mn.buyer_id,
                    mn.company,
                    mn.company_en
                }
            ).ToList();
            return Request.CreateResponse(HttpStatusCode.OK, sellerNeed);
        }

        [HttpGet]
        public object GetSellerForBuyerCheck(int activity_id, string user_id)
        {
            if (activity_id == null || user_id.IsNullOrEmpty()) return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "activity id is null");
            Dictionary<string, dynamic> buyerNeed = new Dictionary<string, dynamic>();
            buyerNeed["check"] = matchService.GetCertainActivityBuyerCheckSellerList(activity_id, user_id)
                                .Select(
                                    mn =>
                                    new 
                                    {
                                        mn.seller_id,
                                        mn.company,
                                        mn.company_en
                                    }
                                ).ToList();
            buyerNeed["schedule"] = new List<dynamic>();
            buyerNeed["schedule"] = matchService.GetWhenUserIsBuyerMatchMakingDataList(activity_id, user_id).Select(
                mn =>
                new 
                {
                    mn.seller_id,
                    mn.company,
                    mn.company_en
                }
            ).ToList();
            return Request.CreateResponse(HttpStatusCode.OK, buyerNeed);
        }

        [HttpPost]
        public object SellerMatchToBuyer(int activity_id, string seller_id, string buyer_id)
        {
            if (activity_id == null || seller_id.IsNullOrEmpty())
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "id is null");

            string[] buyer_ids = buyer_id != null ? buyer_id.Split(',') : new string[] { "" };
            var oldSellerneedList = matchService.GetMatchmakingSellerneedList(activity_id, seller_id)
                                                .GetSelectList(model => new { model.serial_no, model.buyer_id });

            MatchmakingAllModel matchmakingAllModel = new MatchmakingAllModel();
            matchmakingAllModel.activity_id = activity_id;
            matchmakingAllModel.seller_id = seller_id;

            int matchmakingNeedId = 0;
            if (oldSellerneedList.Count == 0 && buyer_ids[0] != "")
            {
                foreach (string id in buyer_ids)
                {
                    matchmakingAllModel.buyer_id = id;
                    try
                    {
                        matchmakingNeedId = matchService.MatchmakingSellerneedInsertOne(matchmakingAllModel);
                    }
                    catch (Exception ex)
                    {
                        return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "insert fail");
                    }
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
                var buyerExcept = buyer_ids.Except(oldBuyerId).ToList();
                if(buyerExcept.Any() && buyerExcept[0] != "")
                {
                    foreach(string buyerId in buyerExcept)
                    {
                        matchmakingAllModel.buyer_id = buyerId;
                        try
                        {
                            matchmakingNeedId = matchService.MatchmakingSellerneedInsertOne(matchmakingAllModel);
                        }
                        catch (Exception ex)
                        {
                            return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "insert fail");
                        }
                    }
                }
            }
            return Request.CreateResponse(HttpStatusCode.OK, matchmakingNeedId);
        }

        [HttpPost]
        public object BuyerMatchToSeller(int activity_id, string buyer_id, string seller_id)
        {
            if (activity_id == null || buyer_id.IsNullOrEmpty())
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "id is null");

            string[] seller_ids = seller_id != null ? seller_id.Split(',') : new string[] { "" };
            var oldBuyerneedList = matchService.GetMatchmakingBuyerneedList(activity_id, buyer_id)
                                                .GetSelectList(model => new { model.serial_no, model.seller_id });

            MatchmakingAllModel matchmakingAllModel = new MatchmakingAllModel();
            matchmakingAllModel.activity_id = activity_id;
            matchmakingAllModel.buyer_id = buyer_id;

            int matchmakingNeedId = 0;
            if (oldBuyerneedList.Count == 0 && seller_ids[0] != "")
            {
                foreach (string id in seller_ids)
                {
                    matchmakingAllModel.seller_id = id;
                    try
                    {
                        matchmakingNeedId = matchService.MatchmakingBuyerneedInsertOne(matchmakingAllModel);
                    }
                    catch (Exception ex)
                    {
                        return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "insert fail");
                    }
                }
            }
            else
            {
                oldBuyerneedList.ForEach(oldSeller =>
                {
                    if (!seller_ids.Contains(oldSeller.seller_id))
                    {
                        matchService.MatchmakingBuyerneedDelete(oldSeller.serial_no);
                    }
                });
                var oldSellerId = oldBuyerneedList.Select(oldSellerneed => oldSellerneed.seller_id)
                                                  .ToArray();
                var sellerExcept = seller_ids.Except(oldSellerId).ToList();
                if (sellerExcept.Any() && sellerExcept[0] != "")
                {
                    foreach(string sellerId in sellerExcept)
                    {
                        matchmakingAllModel.seller_id = sellerId;
                        try
                        {
                            matchmakingNeedId = matchService.MatchmakingBuyerneedInsertOne(matchmakingAllModel);
                        }
                        catch (Exception ex)
                        {
                            return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "insert fail");
                        }
                    }
                }
            }
            return Request.CreateResponse(HttpStatusCode.OK, matchmakingNeedId);
        }
    }
}
