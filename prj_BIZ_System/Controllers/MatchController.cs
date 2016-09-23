using prj_BIZ_System.Services;
using prj_BIZ_System.Models;
using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Collections;
using System.Collections.Generic;
using prj_BIZ_System.Extensions;

namespace prj_BIZ_System.Controllers
{
    public class MatchController : _BaseController
    {
        public ActivityService activityService;
        public MatchService matchService;
        public Match_ViewModel matchModel;

        public MatchController()
        {
            activityService = new ActivityService();
            matchService = new MatchService();
            matchModel = new Match_ViewModel();
        }


        #region 帳戶管理(賣家與買主)
        public ActionResult AccountManage()
        {

            if (Request.Cookies["UserInfo"] == null)
                return Redirect("~/Home/Login");

            matchModel.activityregisterList = matchService.GetSellerAccountPassActivity(Request.Cookies["UserInfo"]["user_id"]);
            matchModel.activityinfoList = matchService.GetAccountNotRegisterActivity(Request.Cookies["UserInfo"]["user_id"]);
            matchModel.buyerinfoList = matchService.GetUserWhenActivityBuyer(Request.Cookies["UserInfo"]["user_id"]);

            return View(matchModel);
        }
        #endregion

        #region 是否媒合買家
        [HttpGet]
        public ActionResult WhetherMetchBuyer()
        {
            if (Request.Cookies["UserInfo"] == null)
                return Redirect("~/Home/Login");

            int activity_id = int.Parse(Request["activity_id"]);

            ActivityInfoModel activityInfoModel = activityService.GetActivityInfoOne(activity_id);

            //matchModel.matchmakingNeedList = matchService.GetCertainActivitySellerCheckBuyerList(activity_id, Request.Cookies["UserInfo"]["user_id"]);
            //matchModel.matchmakingAllList = matchService.GetCertainActivitySellerCheckBuyerList(activity_id, Request.Cookies["UserInfo"]["user_id"]);

            if (activityInfoModel.matchmaking_select.Equals("0") && activityInfoModel.seller_select.Equals("1"))
            {
                return Redirect("SellerBusinessMatch?activity_id=" + activity_id);
            }
            else
            {
                return Redirect("MatchTimeArrangeSeller?activity_id=" + activity_id);
            }
        }
        #endregion

        #region 商務對接(賣家)
        [HttpGet]
        public ActionResult SellerBusinessMatch()
        {
            if (Request.Cookies["UserInfo"] == null)
                return Redirect("~/Home/Login");

            ViewBag.Action = "EditSellerMatchBuyerToInsertUpdate";
            matchModel.buyerinfoList = matchService.GetSellerMatchToBuyerNameAndNeedList(int.Parse(Request["activity_id"]));
            string[] buyerArray = matchService.GetCertainActivitySellerCheckBuyerList(int.Parse(Request["activity_id"]), Request.Cookies["UserInfo"]["user_id"]).Select(model => model.buyer_id).ToArray();


            foreach (BuyerInfoModel model in matchModel.buyerinfoList)
            {
                if (buyerArray.Contains(model.buyer_id))
                {
                    model.Ischeck = true;
                }
            }

            if (buyerArray.Any())
            {
                ViewBag.PageType = "Edit";
                ViewBag.SubmitName = "修改";
            }
            else {
                ViewBag.PageType = "Create";
                ViewBag.SubmitName = "新增";
            }

            return View(matchModel);
        }

        [HttpPost]
        public ActionResult EditSellerMatchBuyerToInsertUpdate(MatchmakingAllModel matchmakingAllModel, IList<string> buyer_id)
        {
            if (Request.Cookies["UserInfo"] == null)
                return Redirect("~/Home/Login");

            matchmakingAllModel.seller_id = Request.Cookies["UserInfo"]["user_id"];

            var oldSellerneedList = matchService.GetMatchmakingSellerneedList(matchmakingAllModel.activity_id, matchmakingAllModel.seller_id).Select(model => new { model.serial_no, model.buyer_id }).ToList();
            IList<string> oldBuyer_id = new List<string>();

            if (oldSellerneedList.Count == 0)
            {
                foreach (string id in buyer_id)
                {
                    matchmakingAllModel.buyer_id = id;
                    matchService.MatchmakingSellerneedInsertOne(matchmakingAllModel);
                }
            }
            else
            {
                foreach (var oldsellerneed in oldSellerneedList)
                {
                    if (buyer_id == null)
                    {
                        buyer_id = new List<string>();
                    }

                    if (!buyer_id.Contains(oldsellerneed.buyer_id))
                    {
                        matchService.MatchmakingSellerneedDelete(oldsellerneed.serial_no);
                    }

                    oldBuyer_id.Add(oldsellerneed.buyer_id);
                }
                var buyerArray = buyer_id.Except(oldBuyer_id);//取差集

                foreach (string except_id in buyerArray)
                {
                    matchmakingAllModel.buyer_id = except_id;
                    matchService.MatchmakingSellerneedInsertOne(matchmakingAllModel);
                }
            }

            return Redirect("SellerBusinessMatch?activity_id=" + matchmakingAllModel.activity_id);
        }
        #endregion

        #region 是否媒合賣家
        public ActionResult WhetherMetchSeller()
        {
            if (Request.Cookies["UserInfo"] == null)
                return Redirect("~/Home/Login");

            int activity_id = int.Parse(Request["activity_id"]);

            ActivityInfoModel activityInfoModel = activityService.GetActivityInfoOne(activity_id);

            //matchModel.matchmakingNeedList = matchService.GetCertainActivityBuyerCheckSellerList(activity_id, Request.Cookies["UserInfo"]["user_id"]);
            //matchModel.matchmakingAllList = matchService.GetCertainActivityBuyerCheckSellerList(activity_id, Request.Cookies["UserInfo"]["user_id"]);

            if (activityInfoModel.matchmaking_select.Equals("0") && activityInfoModel.seller_select.Equals("1"))
            {
                return Redirect("BuyerBusinessMatch?activity_id=" + activity_id);
            }
            else {
                return Redirect("MatchTimeArrangeBuyer?activity_id=" + activity_id);
            }

        }
        #endregion

        #region 商務對接(買家)
        [HttpGet]
        public ActionResult BuyerBusinessMatch()
        {
            if (Request.Cookies["UserInfo"] == null)
                return Redirect("~/Home/Login");

            //ViewBag.Action = "EditBuyerMatchSellerToInsert";
            ViewBag.Action = "EditBuyerMatchSellerToInsertUpdate";

            //matchModel.activityregisterList = matchService.GetBuyerMatchToSellerName(int.Parse(Request["activity_id"]));
            //matchModel.matchmakingNeedList = matchService.GetBuyerForActivityMatchSellerList(int.Parse(Request["activity_id"]), Request.Cookies["UserInfo"]["user_id"], "");
            matchModel.activityregisterList = matchService.GetCertainActivityHaveCheckSellerNameList(int.Parse(Request["activity_id"]));
            string[] sellerArray = matchService.GetCertainActivityBuyerCheckSellerList(int.Parse(Request["activity_id"]), Request.Cookies["UserInfo"]["user_id"]).Select(model => model.seller_id).ToArray();

            foreach(ActivityRegisterModel model in matchModel.activityregisterList)
            {
                if (sellerArray.Contains(model.user_id))
                {
                    model.Ischeck = true;
                }
            }

            if (sellerArray.Any())
            {
                ViewBag.PageType = "Edit";
                ViewBag.SubmitName = "修改";
            }
            else
            {
                ViewBag.PageType = "Create";
                ViewBag.SubmitName = "新增";
            }

            return View(matchModel);
        }

        [HttpPost]
        public ActionResult EditBuyerMatchSellerToInsertUpdate(MatchmakingAllModel matchmakingAllModel, IList<string> seller_id)
        {
            if (Request.Cookies["UserInfo"] == null)
                return Redirect("~/Home/Login");

            matchmakingAllModel.buyer_id = Request.Cookies["UserInfo"]["user_id"];

            var oldBuyerneedList = matchService.GetMatchmakingBuyerneedList(matchmakingAllModel.activity_id, matchmakingAllModel.buyer_id).Select(model => new { model.serial_no, model.seller_id }).ToList();
            IList<string> oldSeller_id = new List<string>();

            if (oldBuyerneedList.Count == 0)
            {
                foreach (string id in seller_id)
                {
                    matchmakingAllModel.seller_id = id;
                    matchService.MatchmakingBuyerneedInsertOne(matchmakingAllModel);
                }
            }
            else
            {
                foreach(var oldbuyerneed in oldBuyerneedList)
                {
                    if(seller_id == null)
                    {
                        seller_id = new List<string>();
                    }

                    if (!seller_id.Contains(oldbuyerneed.seller_id))
                    {
                        matchService.MatchmakingBuyerneedDelete(oldbuyerneed.serial_no);
                    }

                    oldSeller_id.Add(oldbuyerneed.seller_id);
                }
                var sellerArray = seller_id.Except(oldSeller_id);//取差集

                foreach(string except_id in sellerArray)
                {
                    matchmakingAllModel.seller_id = except_id;
                    matchService.MatchmakingBuyerneedInsertOne(matchmakingAllModel);
                }
            }

            return Redirect("BuyerBusinessMatch?activity_id=" + matchmakingAllModel.activity_id);
        }
        #endregion

        #region 媒合時程安排(賣家)
        public ActionResult MatchTimeArrangeSeller()
        {
            if (Request.Cookies["UserInfo"] == null)
                return Redirect("~/Home/Login");

            matchModel.matchmakingAllList = matchService.GetSellerForActivityMatchBuyerList(int.Parse(Request["activity_id"]), Request.Cookies["UserInfo"]["user_id"]);
            matchModel.matchmakingScheduleList = matchService.GetWhenUserIsSellerMatchMakingDataList(int.Parse(Request["activity_id"]), Request.Cookies["UserInfo"]["user_id"]);
            return View(matchModel);
        }
        #endregion

        #region 媒合時程安排(買家)
        public ActionResult MatchTimeArrangeBuyer()
        {
            if (Request.Cookies["UserInfo"] == null)
                return Redirect("~/Home/Login");

            matchModel.matchmakingAllList = matchService.GetBuyerForActivityMatchSellerList(int.Parse(Request["activity_id"]), Request.Cookies["UserInfo"]["user_id"]);
            matchModel.matchmakingScheduleList = matchService.GetWhenUserIsBuyerMatchMakingDataList(int.Parse(Request["activity_id"]), Request.Cookies["UserInfo"]["user_id"]);
            return View(matchModel);
        }
        #endregion

    }
}