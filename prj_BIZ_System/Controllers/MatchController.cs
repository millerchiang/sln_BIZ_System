using prj_BIZ_System.Services;
using prj_BIZ_System.Models;
using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Collections;
using System.Collections.Generic;

namespace prj_BIZ_System.Controllers
{
    public class MatchController : Controller
    {
        public MatchService matchService;
        public Match_ViewModel matchModel;

        public MatchController()
        {
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

            //matchModel.matchmakingNeedList = matchService.GetCertainActivitySellerCheckBuyerList(activity_id, Request.Cookies["UserInfo"]["user_id"]);
            matchModel.matchmakingAllList = matchService.GetCertainActivitySellerCheckBuyerList(activity_id, Request.Cookies["UserInfo"]["user_id"]);


            if (matchModel.matchmakingAllList.Any())
            {
                return Redirect("MatchTimeArrangeSeller?activity_id=" + activity_id);
            }
            else {
                return Redirect("SellerBusinessMatch?activity_id=" + activity_id);
            }

        }
        #endregion

        #region 商務對接(賣家)
        [HttpGet]
        public ActionResult SellerBusinessMatch()
        {
            if (Request.Cookies["UserInfo"] == null)
                return Redirect("~/Home/Login");

            ViewBag.Action = "EditSellerMatchBuyerToInsert";
            matchModel.buyerinfoList = matchService.GetSellerMatchToBuyerNameAndNeedList(int.Parse(Request["activity_id"]));
            return View(matchModel);
        }

        [HttpPost]
        public ActionResult EditSellerMatchBuyerToInsert(MatchmakingAllModel matchmakingAllModel, string[] buyer_id)
        {
            if (Request.Cookies["UserInfo"] == null)
                return Redirect("~/Home/Login");

            matchmakingAllModel.seller_id = Request.Cookies["UserInfo"]["user_id"];
            //matchmakingNeedModel.buyer_reply = "0";

            foreach (string id in buyer_id)
            {
                matchmakingAllModel.buyer_id = id;
                matchService.MatchmakingSellerneedInsertOne(matchmakingAllModel);
            }

            return Redirect("MatchTimeArrangeSeller?activity_id=" + matchmakingAllModel.activity_id);
        }
        #endregion

        #region 是否媒合賣家
        public ActionResult WhetherMetchSeller()
        {
            if (Request.Cookies["UserInfo"] == null)
                return Redirect("~/Home/Login");

            int activity_id = int.Parse(Request["activity_id"]);

            //matchModel.matchmakingNeedList = matchService.GetCertainActivityBuyerCheckSellerList(activity_id, Request.Cookies["UserInfo"]["user_id"]);
            matchModel.matchmakingAllList = matchService.GetCertainActivityBuyerCheckSellerList(activity_id, Request.Cookies["UserInfo"]["user_id"]);
            if (matchModel.matchmakingAllList.Any())
            {
                return Redirect("MatchTimeArrangeBuyer?activity_id=" + activity_id);
            }
            else {
                return Redirect("BuyerBusinessMatch?activity_id=" + activity_id);
            }

        }
        #endregion

        #region 商務對接(買家)
        [HttpGet]
        public ActionResult BuyerBusinessMatch()
        {
            if (Request.Cookies["UserInfo"] == null)
                return Redirect("~/Home/Login");

            ViewBag.Action = "EditBuyerMatchSellerToInsert";
            //matchModel.activityregisterList = matchService.GetBuyerMatchToSellerName(int.Parse(Request["activity_id"]));
            //matchModel.matchmakingNeedList = matchService.GetBuyerForActivityMatchSellerList(int.Parse(Request["activity_id"]), Request.Cookies["UserInfo"]["user_id"], "");
            matchModel.activityregisterList = matchService.GetCertainActivityHaveCheckSellerNameList(int.Parse(Request["activity_id"]));

            return View(matchModel);
        }

        [HttpPost]
        public ActionResult EditBuyerMatchSellerToInsert(MatchmakingAllModel matchmakingNeedModel, string[] seller_id)
        {
            if (Request.Cookies["UserInfo"] == null)
                return Redirect("~/Home/Login");

            matchmakingNeedModel.buyer_id = Request.Cookies["UserInfo"]["user_id"];
            foreach (string id in seller_id)
            {
                matchmakingNeedModel.seller_id = id;
                //matchService.MatchmakingNeedUpdateOne(matchmakingNeedModel);
                matchService.MatchmakingBuyerneedInsertOne(matchmakingNeedModel);
            }
            return Content("成功送出想媒合的賣家");
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