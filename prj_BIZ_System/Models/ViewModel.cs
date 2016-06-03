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
        public EnterpriseSortAndListModel enterprisesortandlist { get; set; }
        public ProductListModel product { get; set; }
        public CatalogListModel catalog { get; set; }
        public ActivityProductSelectModel activityproductselect { get; set; }
        public ActivityCatalogSelectModel activitycatalogselect { get; set; }
        public IList<ActivityInfoModel> activityinfoList { get; set; }
        public IList<ActivityRegisterModel> activityregisterList { get; set; }
        public IList<NewsModel> newsList { get; set; }
        public IList<BuyerInfoModel> buyerinfoList { get; set; }
        public IList<UserInfoToIdAndCpModel> userinfotoidandcpList { get; set; }
        public IList<UserInfoModel> userinfoList { get; set; }
        public IList<EnterpriseSortAndListModel> enterprisesortandlistList { get; set; }
        public IList<ProductListModel> productsortList { get; set; }
        public IList<CatalogListModel> cataloglistList { get; set; }
        public IList<ActivityProductSelectModel> activityproductselectList { get; set; }
        public IList<ActivityCatalogSelectModel> activitycatalogselectList { get; set; }
    }

    public class User_ViewModel
    {
        public UserInfoModel userinfo { get; set; }
        public EnterpriseSortListModel enterprisesort { get; set; }
        public EnterpriseSortModel usersort { get; set; }
        public ProductListModel product { get; set; }
        public CatalogListModel catalog { get; set; }
        public IList<UserInfoModel> userinfoList { get; set; }
        public IList<EnterpriseSortListModel> enterprisesortList { get; set; }
        public IList<EnterpriseSortModel> usersortList { get; set; }
        public IList<ProductListModel> productsortList { get; set; }
        public IList<CatalogListModel> cataloglistList { get; set; }
    }

    public class Index_ViewModel
    {
        public IList<UserInfoModel> userinfoList { get; set; }
        public IList<EnterpriseSortListModel> enterprisesortList { get; set; }
        public IList<ActivityInfoModel> activityinfoList { get; set; }
        public IList<NewsModel> newsList { get; set; }

    }

    public class Push_ViewModel
    {
        public PushListModel pushList { get; set; }
        public PushSampleModel pushSample { get; set; }
        public IList<PushListModel> pushListList { get; set; }
        public IList<PushSampleModel> pushSampleList { get; set; }
        public IList<ActivityInfoModel> activityinfoList { get; set; }
    }

    public class Match_ViewModel
    {
        public IList<ActivityRegisterModel> activityregisterList { get; set; }
        public IList<ActivityInfoModel> activityinfoList { get; set; }
        public IList<BuyerInfoModel> buyerinfoList { get; set; }
    }
}