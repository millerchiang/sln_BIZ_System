using prj_BIZ_System.Services;
using prj_BIZ_System.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Collections;

namespace prj_BIZ_System.Controllers
{
    public class MatchController : Controller
    {
        public MatchService matchService;
        public Match_ViewModel matchModel;
        ArrayList searchActivityBuyerList = new ArrayList();

        public MatchController()
        {
            matchService = new MatchService();
            matchModel = new Match_ViewModel();
        }


        #region 帳戶管理(賣家與買主)
        public ActionResult AccountManage()
        {

            matchModel.activityregisterList = matchService.GetSellerAccountPassActivity(Request.Cookies["UserInfo"]["user_id"]);
            matchModel.activityinfoList = matchService.GetAccountNotRegisterActivity(Request.Cookies["UserInfo"]["user_id"]);
            matchModel.buyerinfoList = matchService.GetBuyerAccountPassActivity(Request.Cookies["UserInfo"]["user_id"]);
          
            return View(matchModel);
        }
        #endregion

        #region 是否媒合買家
        [HttpGet]
        public ActionResult WhetherMetchBuyer()
        {
            int activity_id = int.Parse(Request["activity_id"]);

            matchModel.matchmakingNeedList = matchService.GetCertainActivitySellerCheckBuyerList(activity_id, Request.Cookies["UserInfo"]["user_id"]);

            if (matchModel.matchmakingNeedList.Any())
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
            ViewBag.Action = "EditSellerMatchBuyerToInsert";
            matchModel.buyerinfoList = matchService.GetSellerMatchToBuyerNameAndNeed(int.Parse(Request["activity_id"]));
            return View(matchModel);
        }

        [HttpPost]
        public ActionResult EditSellerMatchBuyerToInsert(MatchmakingNeedModel matchmakingNeedModel, string[] buyer_id)
        {
            matchmakingNeedModel.seller_id = Request.Cookies["UserInfo"]["user_id"];
            matchmakingNeedModel.buyer_reply = "0";

            foreach (string id in buyer_id)
            {
                matchmakingNeedModel.buyer_id = id;
                matchService.MatchmakingNeedInsertOne(matchmakingNeedModel);
            }

            return Content("成功送出想媒合的買家");
        }
        #endregion

        #region 是否媒合賣家
        public ActionResult WhetherMetchSeller()
        {
            int activity_id = int.Parse(Request["activity_id"]);

            matchModel.matchmakingNeedList = matchService.GetCertainActivityBuyerCheckSellerList(activity_id, Request.Cookies["UserInfo"]["user_id"]);

            if (matchModel.matchmakingNeedList.Any())
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
            ViewBag.Action = "EditBuyerMatchSellerToInsert";
            matchModel.activityregisterList = matchService.GetBuyerMatchToSellerName(int.Parse(Request["activity_id"]));
            return View(matchModel);
        }

        [HttpPost]
        public ActionResult EditBuyerMatchSellerToInsert(MatchmakingNeedModel matchmakingNeedModel, string[] seller_id)
        {
            matchmakingNeedModel.buyer_id = Request.Cookies["UserInfo"]["user_id"];
            foreach (string id in seller_id)
            {
                matchmakingNeedModel.seller_id = id;
                matchService.MatchmakingNeedUpdateOne(matchmakingNeedModel);
            }
            return Content("成功送出想媒合的賣家");
        }
        #endregion

        #region 媒合時程安排(賣家)
        public ActionResult MatchTimeArrangeSeller()
        {
            matchModel.matchmakingNeedList = matchService.GetSellerForActivityMatchBuyerList(int.Parse(Request["activity_id"]), Request.Cookies["UserInfo"]["user_id"]);
            return View(matchModel);     
        }
        #endregion

        #region 媒合時程安排(買家)
        public ActionResult MatchTimeArrangeBuyer()
        {
            matchModel.matchmakingNeedList = matchService.GetBuyerForActivityMatchSellerList(int.Parse(Request["activity_id"]), Request.Cookies["UserInfo"]["user_id"]);
            return View(matchModel);     
        }
        #endregion

        #region 媒合時程表時間設定
        [HttpGet]
        public ActionResult MatchScheduleTime()
        {
            ViewBag.Action = "StoreMatchTimeInterval";
            matchModel.schedulePeriodSetList = matchService.GetActivityMatchTimeIntervalList(int.Parse(Request["activity_id"]));
            matchModel.SchedulePeriodSet = new SchedulePeriodSetModel();
            matchModel.SchedulePeriodSet.activity_id = int.Parse(Request["activity_id"]);
            return View(matchModel);
        }

        [HttpPost]
        public ActionResult StoreMatchTimeInterval(SchedulePeriodSetModel schedulePeriodSetModel)
        {
            
            matchService.MatchTimeIntervalInsert(schedulePeriodSetModel);
            return Redirect("MatchScheduleTime?activity_id=" + schedulePeriodSetModel.activity_id);
        }

        [HttpGet]
        public ActionResult MatchTimeIntervalDelect()
        {
            int activity_id = int.Parse(Request["activity_id"]);
            matchService.MatchTimeIntervalDeleteOne(int.Parse(Request["period_sn"]));
            return Redirect("MatchScheduleTime?activity_id=" + activity_id);
        }
        #endregion





    }
}