using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace prj_BIZ_System.Models
{

    public class Activity_ViewModel
    {
        public ActivityInfoModel activityinfo { get; set; }
        public ActivityRegisterModel activityregister { get; set; }
        public NewsModel news { get; set; }
        public BuyerInfoModel buyerinfo { get; set; }
        public UserInfoToIdAndCpModel userinfotoidandcp { get; set; }
        public UserInfoModel userinfo { get; set; }
        public IList<ActivityInfoModel> activityinfoList { get; set; }
        public IList<ActivityRegisterModel> activityregisterList { get; set; }
        public IList<NewsModel> newsList { get; set; }
        public IList<BuyerInfoModel> buyerinfoList { get; set; }
        public IList<UserInfoToIdAndCpModel> userinfotoidandcpList { get; set; }
        public IList<UserInfoModel> userinfoList { get; set; }
    }

    public class User_ViewModel
    {
        public UserInfoModel userinfo { get; set; }
        public EnterpriseSortModel enterprisesort { get; set; }
        public UserSortModel usersort { get; set; }
        public ProductListModel product { get; set; }
        public CatalogListModel catalog { get; set; }
        public IList<UserInfoModel> userinfoList { get; set; }
        public IList<EnterpriseSortModel> enterprisesortList { get; set; }
        public IList<UserSortModel> usersortList { get; set; }
        public IList<ProductListModel> productsortList { get; set; }
        public IList<CatalogListModel> cataloglistList { get; set; }
    }

    public class Index_ViewModel
    {
        public IList<UserInfoModel> userinfoList { get; set; }
        public IList<EnterpriseSortModel> enterprisesortList { get; set; }
        public IList<ActivityInfoModel> activityinfoList { get; set; }
        public IList<NewsModel> newsList { get; set; }

    }

}