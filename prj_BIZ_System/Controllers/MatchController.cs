using prj_BIZ_System.Services;
using prj_BIZ_System.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

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


        #region 帳戶管理(賣主與買主)
        public ActionResult AccountManage()
        {
            matchModel.activityregisterList = matchService.GetSellerAccountPassActivity(Request.Cookies["UserInfo"]["user_id"]);
            matchModel.activityinfoList = matchService.GetAccountNotRegisterActivity(Request.Cookies["UserInfo"]["user_id"]);
            matchModel.buyerinfoList = matchService.GetBuyerAccountPassActivity(Request.Cookies["UserInfo"]["user_id"]);
            return View(matchModel);
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
    }
}