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
        private const int notFoundIndex = 999;

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
            matchModel.buyerinfoList = matchService.GetUserWhenActivityBuyer(Request.Cookies["UserInfo"]["user_id"]);

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
            matchModel.buyerinfoList = matchService.GetSellerMatchToBuyerNameAndNeedList(int.Parse(Request["activity_id"]));
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
            //matchModel.activityregisterList = matchService.GetBuyerMatchToSellerName(int.Parse(Request["activity_id"]));
            matchModel.matchmakingNeedList = matchService.GetBuyerForActivityMatchSellerList(int.Parse(Request["activity_id"]), Request.Cookies["UserInfo"]["user_id"], "");
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
            matchModel.matchmakingNeedList = matchService.GetBuyerForActivityMatchSellerList(int.Parse(Request["activity_id"]), Request.Cookies["UserInfo"]["user_id"], "");
            return View(matchModel);
        }
        #endregion

        #region 媒合時程表時間設定新增與刪除
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
        public ActionResult StoreMatchTimeInterval(SchedulePeriodSetModel schedulePeriodSetModel, int[] old_period_sn, DateTime[] old_time_start, DateTime[] old_time_end)
        {
            SchedulePeriodSetModel model = new SchedulePeriodSetModel();
            if (old_period_sn != null)
            {
                for (int i = 0; i < old_period_sn.Length; i++)
                {
                    model.period_sn = old_period_sn[i];
                    model.time_start = old_time_start[i];
                    model.time_end = old_time_end[i];
                    matchService.MatchTimeIntervalUpdateOne(model);
                }
            }

            if ((schedulePeriodSetModel.time_start.ToString() != "0001/1/1 上午 12:00:00")
                || (schedulePeriodSetModel.time_end.ToString() != "0001/1/1 上午 12:00:00"))
            {
                matchService.MatchTimeIntervalInsert(schedulePeriodSetModel);
            }

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

        #region 媒合時程大表列表
        [HttpGet]
        public ActionResult MatchScheduleList()
        {
            //const int notFoundIndex = 999;
            ViewBag.Action = "StoreMatchData";

            IList<MatchmakingNeedModel> CheckIs1List = matchService.GetCertainActivityWithBuyerReplyAllList
                (int.Parse(Request["activity_id"]), "1");
            IList<MatchmakingNeedModel> CheckIs0List = matchService.GetCertainActivityWithBuyerReplyAllList
                (int.Parse(Request["activity_id"]), "");
            ISet<string> buyerReply1Set = new HashSet<string>();
            ISet<string> buyerReply0Set = new HashSet<string>();

            /*列出某活動所有買主*/
            matchModel.buyerinfoList = matchService.GetSellerMatchToBuyerNameAndNeedList(int.Parse(Request["activity_id"]));
            /*列出某活動媒合時段*/
            matchModel.schedulePeriodSetList = matchService.GetActivityMatchTimeIntervalList(int.Parse(Request["activity_id"]));
            /*取得設定時間的活動編號*/
            matchModel.SchedulePeriodSet = new SchedulePeriodSetModel();
            matchModel.SchedulePeriodSet.activity_id = int.Parse(Request["activity_id"]);

            /*列出某活動有審核的賣家*/
            matchModel.activityregisterList = matchService.GetCertainActivityHaveCheckSellerNameList(int.Parse(Request["activity_id"]));

            /*列出某活動的媒合大表資料*/
            matchModel.matchmakingScheduleList = matchService.GetCertainActivityMatchMakingDataList(int.Parse(Request["activity_id"]));

            /*列出某活動的時間區段輸入媒合的賣家*/
            int i = notFoundIndex, j = notFoundIndex;//i是時段, j是買主

            matchModel.matchMakingScheduleSellerCompany = Enumerable.Repeat(String.Empty, matchModel.buyerinfoList.Count * matchModel.schedulePeriodSetList.Count).ToArray();

            foreach (MatchmakingScheduleModel matchmakingScheduleModel in matchModel.matchmakingScheduleList)
            {
                foreach (SchedulePeriodSetModel schedulePeriodSetModel in matchModel.schedulePeriodSetList)
                {
                    if (schedulePeriodSetModel.period_sn == matchmakingScheduleModel.period_sn)
                    {
                        i = matchModel.schedulePeriodSetList.IndexOf(schedulePeriodSetModel);
                    }
                }

                foreach (BuyerInfoModel buyerInfoModel in matchModel.buyerinfoList)
                {
                    if (buyerInfoModel.buyer_id == matchmakingScheduleModel.buyer_id)
                    {
                        j = matchModel.buyerinfoList.IndexOf(buyerInfoModel);
                    }
                }

                if ((i != notFoundIndex) && (j != notFoundIndex))
                {
                    matchModel.matchMakingScheduleSellerCompany[i * matchModel.buyerinfoList.Count + j] = matchmakingScheduleModel.company;
                }
            }

            /*列出雙方有媒合意願的賣家*/
            foreach (MatchmakingNeedModel model in CheckIs1List)
            {
                buyerReply1Set.Add(model.buyer_id);
            }

            foreach (string buyer in buyerReply1Set)
            {
                matchModel.sellerCompanyNamereply1Dic[buyer] = new List<string>();
            }

            foreach (MatchmakingNeedModel model in CheckIs1List)
            {
                matchModel.sellerCompanyNamereply1Dic[model.buyer_id].Add(model.company);
            }

            /*列出有媒合意願的賣家*/
            foreach (MatchmakingNeedModel model in CheckIs0List)
            {
                buyerReply0Set.Add(model.buyer_id);
            }

            foreach (string buyer in buyerReply0Set)
            {
                matchModel.sellerCompanyNamereply0Dic[buyer] = new List<string>();
            }

            foreach (MatchmakingNeedModel model in CheckIs0List)
            {
                matchModel.sellerCompanyNamereply0Dic[model.buyer_id].Add(model.company);
            }

            /*刪除某時段,媒合大表中的相同時段資料刪除*/
            ISet<int> schedulePeriodSn = new HashSet<int>();
            ISet<int> matchmakingPeriodSn = new HashSet<int>();
            foreach (SchedulePeriodSetModel schedulePeriodSetModel in matchModel.schedulePeriodSetList)
            {
                schedulePeriodSn.Add(schedulePeriodSetModel.period_sn);
            }

            foreach(MatchmakingScheduleModel matchmakingScheduleModel in matchModel.matchmakingScheduleList)
            {
                matchmakingPeriodSn.Add(matchmakingScheduleModel.period_sn);
            }

            var periodSns = from n1 in matchmakingPeriodSn
                     where schedulePeriodSn.Contains(n1) == false
                     select n1;
            if(periodSns.Count() != 0)
            {
                foreach (int periodSn in periodSns)
                {
                    matchService.MatchkingDataByActivityWithPeriodDelete(int.Parse(Request["activity_id"]), periodSn);
                }
            }

            return View(matchModel);
        }
        #endregion

        #region 媒合時程大表新增修改刪除
        [HttpPost]
        public ActionResult StoreMatchData(int[] period_sn, int activity_id, string[] buyer_id, string[] seller_id)
        {
            /*列出某活動的媒合大表資料*/
            matchModel.matchmakingScheduleList = matchService.GetCertainActivityMatchMakingDataList(activity_id);
            /*列出某活動所有買主*/
            matchModel.buyerinfoList = matchService.GetSellerMatchToBuyerNameAndNeedList(activity_id);
            /*列出某活動媒合時段*/
            matchModel.schedulePeriodSetList = matchService.GetActivityMatchTimeIntervalList(activity_id);
            /*列出某活動的時間區段輸入媒合的賣家*/
            int i = notFoundIndex, j = notFoundIndex;//i是時段, j是買主

            matchModel.matchMakingScheduleSellerCompany = Enumerable.Repeat(String.Empty, buyer_id.Length * period_sn.Length).ToArray();

            foreach (MatchmakingScheduleModel model in matchModel.matchmakingScheduleList)
            {
                foreach (SchedulePeriodSetModel schedulePeriodSetModel in matchModel.schedulePeriodSetList)
                {
                    if (schedulePeriodSetModel.period_sn == model.period_sn)
                    {
                        i = matchModel.schedulePeriodSetList.IndexOf(schedulePeriodSetModel);
                    }
                }

                foreach (BuyerInfoModel buyerInfoModel in matchModel.buyerinfoList)
                {
                    if (buyerInfoModel.buyer_id == model.buyer_id)
                    {
                        j = matchModel.buyerinfoList.IndexOf(buyerInfoModel);
                    }
                }

                if ((i != notFoundIndex) && (j != notFoundIndex))
                {
                    matchModel.matchMakingScheduleSellerCompany[i * matchModel.buyerinfoList.Count + j] = model.company;
                }

            }
            MatchmakingScheduleModel matchmakingScheduleModel = new MatchmakingScheduleModel();
            matchmakingScheduleModel.activity_id = activity_id;
            matchmakingScheduleModel.create_time = DateTime.Now;

            for (int x = 0; x < period_sn.Length * buyer_id.Length; x++)
            {

                if (matchModel.matchMakingScheduleSellerCompany[x] == "" && seller_id[x].Length != 0)
                {   
                    matchmakingScheduleModel.period_sn = period_sn[x / buyer_id.Length];
                    matchmakingScheduleModel.buyer_id = buyer_id[x % buyer_id.Length];
                    matchmakingScheduleModel.seller_id = seller_id[x];
                    matchService.CertainTimeMatchSellerInsert(matchmakingScheduleModel);
                }
                else if (matchModel.matchMakingScheduleSellerCompany[x] != "" && seller_id[x] != "")
                {
                    matchmakingScheduleModel.period_sn = period_sn[x / buyer_id.Length];
                    matchmakingScheduleModel.buyer_id = buyer_id[x % buyer_id.Length];
                    matchmakingScheduleModel.seller_id = seller_id[x];
                    matchService.CertainActivityMatchkingDataUpdate(matchmakingScheduleModel);
                }
                else if (matchModel.matchMakingScheduleSellerCompany[x].Length != 0 && seller_id[x] == "")
                {
                    matchmakingScheduleModel.period_sn = period_sn[x / buyer_id.Length];
                    matchmakingScheduleModel.buyer_id = buyer_id[x % buyer_id.Length];
                    matchmakingScheduleModel.seller_id = seller_id[x];
                    matchService.CertainActivityMatchkingDataDelete(matchmakingScheduleModel);

                }
            }

            return Redirect("MatchScheduleList?activity_id=" + activity_id);
        }
        #endregion


    }
}