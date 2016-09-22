using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using prj_BIZ_System.WebService.Model;

namespace prj_BIZ_System.Models
{

    public class Manager_Activity_ViewModel
    {
        public BuyerInfoModel buyerinfo { get; set; }
        public UserInfoToIdAndCpModel userinfotoidandcp { get; set; }
        public UserInfoModel userinfo { get; set; }
        public ActivityInfoModel activityinfo { get; set; }
        public NewsModel news { get; set; }
        public ActivityRegisterModel activityregister { get; set; }
        public IList<UserInfoModel> userinfoList { get; set; }
        public IList<ActivityInfoModel> activityinfoList { get; set; }
        public IList<NewsModel> newsList { get; set; }
        public IList<ActivityRegisterModel> activityregisterList { get; set; }
        public IList<EnterpriseSortListModel> enterprisesortList { get; set; }
        public IList<EnterpriseSortModel> usersortList { get; set; }
        public IList<BuyerInfoModel> buyerinfoList { get; set; }
        public IList<UserInfoToIdAndCpModel> userinfotoidandcpList { get; set; }
    }

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
        public CompanySortModel companysort { get; set; }
        public EnterpriseSortModel usersort { get; set; }
        public ProductListModel product { get; set; }
        public CatalogListModel catalog { get; set; }
        public VideoListModel videolog { get; set; }
        public IList<UserInfoModel> userinfoList { get; set; }
        public IList<EnterpriseSortListModel> enterprisesortList { get; set; }
        public IList<EnterpriseSortModel> usersortList { get; set; }
        public IList<CompanySortModel> companysortList { get; set; }
        public IList<ProductListModel> productsortList { get; set; }
        public IList<CatalogListModel> cataloglistList { get; set; }
        public IList<VideoListModel> videolistList { get; set; }
    }

    public class Index_ViewModel
    {
        public IList<CatalogListModel> cataloglistList { get; set; }
        public IList<VideoListModel> videolistList { get; set; }
        public IList<UserInfoModel> userinfoList { get; set; }
        public IList<EnterpriseSortListModel> enterprisesortList { get; set; }
        public IList<ActivityInfoModel> activityinfoList { get; set; }
        public IList<NewsModel> newsList { get; set; }
        public NewsModel news { get; set; }

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
        public SchedulePeriodSetModel SchedulePeriodSet { get; set; }
        public MatchmakingScheduleModel matchmakingSchedule { get; set; }
        public IList<ActivityRegisterModel> activityregisterList { get; set; }
        public IList<ActivityInfoModel> activityinfoList { get; set; }
        public IList<BuyerInfoModel> buyerinfoList { get; set; }
        //public IList<MatchmakingNeedModel> matchmakingNeedList { get; set; }
        public IList<MatchmakingAllModel> matchmakingAllList { get; set; }
        public IList<SchedulePeriodSetModel> schedulePeriodSetList { get; set; }
        public IList<MatchmakingScheduleModel> matchmakingScheduleList { get; set; }

        public string[] activityRegisterSellerCompany { get; set; }
        public string[] matchMakingScheduleSellerCompany { get; set; }
        public string[] matchMakingScheduleSellerId { get; set; }
        public Dictionary<string, IList<string>> sellerCompanyNamereply1Dic = new Dictionary<string, IList<string>>();
        public Dictionary<string, IList<string>> sellerCompanyNamereply0Dic = new Dictionary<string, IList<string>>();
    }

    public class Manager_ViewModel
    {
        public ManagerInfoModel managerInfo { get; set; }
        public GroupModel group { get; set; }
        public IList<ManagerInfoModel> managerInfoList { get; set; }
        public IList<GroupModel> groupList { get; set; }
    }

    public class Password_ViewModel
    {

    }

    public class Message_ViewModel
    {
        public MsgModel msgPrivate { get; set; }
        public MsgFileModel msgPrivateFile { get; set; }
        public MsgReplyModel msgPrivateReply { get; set; }
        public IList<MsgModel> msgPrivateList { get; set; }
        public IList<MsgFileModel> msgPrivateFileList { get; set; }
        public IList<MsgReplyModel> msgPrivateReplyList { get; set; }
    }

    public class Cluster_ViewModel
    {
        public ClusterModel cluster { get; set; }
        public ClusterInfoModel clusterInfo { get; set; }
        public ClusterFileModel clusterFile { get; set; }
        public ClusterMemberModel clusterMember { get; set; }
        public IList<ClusterModel> clusterList { get; set; }
        public IList<ClusterInfoModel> clusterInfoList { get; set; }
        public IList<ClusterFileModel> clusterFileList { get; set; }
        public IList<ClusterMemberModel> clusterMemberList { get; set; }
        public IList<ClusterInfo> clusterWebServiceInfoList { get; set; }

    }

    public class Sales_ViewModel
    {
        public SalesInfoModel salesInfo { get; set; }
        public IList<SalesInfoModel> salesInfoList { get; set; }
    }
}