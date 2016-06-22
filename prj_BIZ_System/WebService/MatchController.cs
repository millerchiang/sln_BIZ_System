using prj_BIZ_System.Models;
using prj_BIZ_System.Services;
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
        public IList<ActivityRegisterModel> GetManagerCheckActivity(string user_id)
        {
            return matchService.GetSellerAccountPassActivity(user_id);
        }

        [HttpGet]
        public IList<BuyerInfoModel> GetUserWhenActivityBuyer(string user_id)
        {
            return matchService.GetUserWhenActivityBuyer(user_id);
        }

        [HttpGet]
        public IList<ActivityInfoModel> GetRecommedActivity(string user_id)
        {
            return matchService.GetAccountNotRegisterActivity(user_id);
        }

        [HttpGet]
        public IList<BuyerInfoModel> GetBuyerForMatch(int activity_id)
        {
            return matchService.GetSellerMatchToBuyerNameAndNeedList(activity_id);
        }

        [HttpGet]
        public IList<MatchmakingNeedModel> GetSellerForMatch(int activity_id, string user_id)
        {
            return matchService.GetBuyerForActivityMatchSellerList(activity_id, user_id, "");
        }

        [HttpGet]
        public IList<MatchmakingNeedModel> GetSellerCheckBuyerList(int activity_id, string user_id)
        {
            return matchService.GetCertainActivitySellerCheckBuyerList(activity_id, user_id);
        }

        [HttpGet]
        public IList<MatchmakingNeedModel> GetBuyerCheckSellerList(int activity_id, string user_id)
        {
            return matchService.GetCertainActivityBuyerCheckSellerList(activity_id, user_id);
        }

    }
}
