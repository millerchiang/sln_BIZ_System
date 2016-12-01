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
        public QuestionnaireModel questionnaire { get; set; }
        public IList<QuestionnaireModel> questionnaireList { get; set; }
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
        public IList<ActivityPhotoModel> activityphotoList { get; set; }
        public IList<BannerPhotoModel> bannerphotoList { get; set; }
        public NewsModel news { get; set; }
        public ActivityPhotoModel activityphoto { get; set; }
        public IList<BuyerInfoModel> buyerinfoList { get; set; }

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
        public SchedulePeriodSetModel schedulePeriodSet { get; set; }
        public MatchmakingScheduleModel matchmakingSchedule { get; set; }
        public IList<ActivityRegisterModel> activityregisterList { get; set; }
        public IList<ActivityRegisterModel> activityregisterList_2 { get; set; }
        public IList<ActivityInfoModel> activityinfoList { get; set; }
        public IList<BuyerInfoModel> buyerinfoList { get; set; }
        public IList<BuyerInfoModel> buyerinfoList_2 { get; set; }
        //public IList<MatchmakingNeedModel> matchmakingNeedList { get; set; }
        public IList<MatchmakingAllModel> matchmakingAllList { get; set; }
        public IList<SchedulePeriodSetModel> schedulePeriodSetList { get; set; }
        public IList<MatchmakingScheduleModel> matchmakingScheduleList { get; set; }

        public string[] activityRegisterSellerCompany { get; set; }//舊版
        public string[] matchMakingScheduleSellerCompany { get; set; }
        public string[] matchMakingScheduleSellerId { get; set; }
        public string[] bothWithbuyerMergeSellerCompany { get; set; }

        public Dictionary<string, IList<string>> sellerCompanyNamereply1Dic = new Dictionary<string, IList<string>>();
        public Dictionary<string, IList<string>> sellerCompanyNamereply0Dic = new Dictionary<string, IList<string>>();


        public IList<MatchmakingAllModel> matchmakingBothList { get; set; }
        public IList<MatchmakingAllModel> matchmakingBuyerList { get; set; }
        public IList<MatchmakingAllModel> matchmakingSellerList { get; set; }

        public List<List<Tuple<string,string,string>>> matchBothForbuyer_idList { get; set; }
        public List<List<Tuple<string,string,string>>> matchSellerCompanyDatamergeList { get; set; }
        public List<string[]> matchBuyerForbuyer_idList { get; set; }
        public List<string[]> matchSellerForbuyer_idList { get; set; }
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
        public MsgReplyFileModel msgReplyFile { get; set; }
        public IList<MsgModel> msgPrivateList { get; set; }
        public IList<MsgModel> msgPrivateList2 { get; set; }
        public IList<MsgFileModel> msgPrivateFileList { get; set; }
        public IList<MsgReplyModel> msgPrivateReplyList { get; set; }
        public IList<MsgReplyFileModel> msgReplyFileList { get; set; }
        public IList<IList<MsgModel>> msgLists { get; set; }

        public UserInfoModel userinfo { get; set; }
        public IList<SalesInfoModel> salesInfoList { get; set; }
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
        public IList<ClusterDetailModel> clusterWebServiceInfoList { get; set; }

    }

    public class Sales_ViewModel
    {
        public SalesInfoModel salesInfo { get; set; }
        public IList<SalesInfoModel> salesInfoList { get; set; }
    }


    public class SalesPermission_ViewModel
    {
        //Read
        public string company { get; set; }         //企業資料庫權限
        public string video { get; set; }           //影音型錄權限
        public string sales { get; set; }           //業務管理權限
        public string message { get; set; }         //訊息管理權限

        public List<SalesInfoModel> unCompanySalesList { get; set; }         //企業資料庫權限不可共同管理成員
        public List<SalesInfoModel> unVideoSalesList { get; set; }           //影音型錄權限不可共同管理成員
        public List<SalesInfoModel> unSalesSalesList { get; set; }           //業務管理權限不可共同管理成員
        public List<SalesInfoModel> unMessageSalesList { get; set; }         //訊息管理權限不可共同管理成員

        public List<SalesInfoModel> companySalesList { get; set; }         //企業資料庫權限可共同管理成員
        public List<SalesInfoModel> videoSalesList { get; set; }           //影音型錄權限可共同管理成員
        public List<SalesInfoModel> salesSalesList { get; set; }           //業務管理權限可共同管理成員
        public List<SalesInfoModel> messageSalesList { get; set; }         //訊息管理權限可共同管理成員

        //Write
        public List<string> unCompanySalesIds { get; set; }
        public List<string> companySalesIds { get; set; }
        public List<string> unVideoSalesIds { get; set; }
        public List<string> videoSalesIds { get; set; }
        public List<string> unSalesSalesIds { get; set; }
        public List<string> salesSalesIds { get; set; }
        public List<string> unMessageSalesIds { get; set; }
        public List<string> messageSalesIds { get; set; }
    }
}