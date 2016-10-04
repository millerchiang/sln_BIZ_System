using prj_BIZ_System.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace prj_BIZ_System.WebService.Model
{
    public class UserInfo  //user_info
    {
        public UserInfo(UserInfoModel userInfoModel)
        {
            user_id = userInfoModel.user_id;//使用者帳號 
            user_pw = userInfoModel.user_pw;//使用者密碼
            id_enable = userInfoModel.id_enable;//帳號有效
            enterprise_type = userInfoModel.enterprise_type;//企業類型
            company = userInfoModel.company;//公司名稱(中文)
            leader = userInfoModel.leader;//代表人(中文)
            addr = userInfoModel.addr;// 公司地址(中文)
            contact = userInfoModel.contact;// 聯絡人姓名(中文)
            phone = userInfoModel.phone;//聯絡電話
            email = userInfoModel.email;//電子郵件
            capital = userInfoModel.capital;//資本額(單位：千)
            revenue = userInfoModel.revenue;//營業額
            website = userInfoModel.website;//企業網址
            info = userInfoModel.info;//企業簡介(中文)
            company_en = userInfoModel.company_en;//公司名稱(英文)
            leader_en = userInfoModel.leader_en;//代表人(英文)
            addr_en = userInfoModel.addr_en;//公司地址(英文)
            contact_en = userInfoModel.contact_en;//聯絡人姓名(英文)
            info_en = userInfoModel.info_en;//企業簡介(英文)
        }

        public string user_id { get; set; }//使用者帳號 
        public string user_pw { get; set; }//使用者密碼
        public string id_enable { get; set; }//帳號有效
        public string enterprise_type { get; set; }//企業類型
        public string enterprise_type_en { get; set; }//企業類型
        public string company { get; set; }//公司名稱(中文)
        public string leader { get; set; }//代表人(中文)
        public string addr { get; set; }// 公司地址(中文)
        public string contact { get; set; }// 聯絡人姓名(中文)
        public string phone { get; set; }//聯絡電話
        public string email { get; set; }//電子郵件
        public long capital { get; set; }//資本額(單位：千)
        public string revenue { get; set; }//營業額
        public string revenue_en { get; set; }//營業額
        public string website { get; set; }//企業網址
        public string info { get; set; }//企業簡介(中文)
        public string company_en { get; set; }//公司名稱(英文)
        public string leader_en { get; set; }//代表人(英文)
        public string addr_en { get; set; }//公司地址(英文)
        public string contact_en { get; set; }//聯絡人姓名(英文)
        public string info_en { get; set; }//企業簡介(英文)
        public string activity_id_buyer { get; set; }//企業簡介(英文)
    }

    public class UserEnterpriseInfo
    {
        public UserInfo userinfo { get; set; }
        public IList<EnterpriseSortModel> usersortList { get; set; }
        public IList<ProductListModel> productsortList { get; set; }
        public IList<CatalogListModel> cataloglistList { get; set; }
        public IList<Video> videolistList { get; set; }
    }

    public class News
    {
        public int news_no { get; set; }//新聞編號流水號
        public string news_type { get; set; }//新聞類型
        public string news_title { get; set; }//新聞標題
        public int activity_id { get; set; }//活動編號
        public string starttime { get; set; }//ActivityInfoModel 的 活動開始時間
    }

    public class ActivityInfo
    {
        public int activity_id { get; set; }//年份+流水號(四碼)EX: 20160001
        public string manager_id { get; set; }//建立者帳號 (建立本活動的管理者帳號)
        public string activity_type { get; set; }//活動類型 (0：商洽會；1：拓銷會)
        public string activity_name { get; set; }//活動名稱(中文)
        public string starttime { get; set; }//活動時間(起) yyyy/mm/dd hh:mm
        public string endtime { get; set; }//活動時間(迄) yyyy/mm/dd hh:mm
        public string addr { get; set; }//活動地點(中文)
        public string organizer { get; set; }//主辦單位(中文)
        public string name { get; set; }//主要聯絡人
        public string phone { get; set; }//手機號碼
        public string email { get; set; }//電子郵件
        public string activity_name_en { get; set; }//活動名稱(英文)
        public string addr_en { get; set; }//活動地點(英文)
        public string organizer_en { get; set; }//主辦單位(英文)
    }

    public class ActivityRegister
    {
        public string activity_name { get; set; }//活動名稱(中文)
        public string starttime { get; set; }//活動時間(起) yyyy/mm/dd hh:mm
        public string endtime { get; set; }//活動時間(迄) yyyy/mm/dd hh:mm
        public string addr { get; set; }//活動地點(中文)

        public string user_id { get; set; }//會員帳號
        public int quantity { get; set; }//與會人數
        public string name_a { get; set; }//與會人姓名
        public string title_a { get; set; }//與會人職稱
        public string name_b { get; set; }//主要聯絡人
        public string title_b { get; set; }//主要聯絡人職稱
        public string telephone { get; set; }//連絡電話
        public string phone { get; set; }//手機號碼
        public string email { get; set; }//電子郵件
        public string manager_check { get; set; }//後台審核 (0：不通過；1：通過)
    }

    public class MsgPrivate
    {
        public long msg_no { get; set; }             //私人訊息編號
        public string msg_title { get; set; }        //訊息標題
        public string msg_member { get; set; }       //成員
        public string msg_content { get; set; }      //訊息內容
        public string msg_file { get; set; }         //訊息附件
        public string create_time { get; set; }      //建立時間
        //UerInfo 公司名稱(中文)
        public string company { get; set; }
        public string is_read { get; set; }
    }

    public class MsgPrivateReply
    {
        public string reply_content { get; set; }    //回覆內容
        public string create_time { get; set; }    //建立時間

        //UerInfo 公司名稱(中文)
        public string company { get; set; }
    }

    public class MessageContent
    {
        public MsgPrivate msgPrivate { get; set; }
        public IList<MsgPrivateReply> msgPrivateReplyList { get; set; }
    }

    public class MessageReplys
    {
        public IList<MsgPrivateReply> msgPrivateReplyList { get; set; }
    }

    public class Video
    {
        public int video_no { get; set; } //影音編號
        public string video_name { get; set; } //影音型錄名稱
        public string youtube_site { get; set; } //youtube影音網址
    }

    public class Cluster
    {
        public int? cluster_no { get; set; }        /*聚落編號*/
        public string cluster_name { get; set; }        /*聚落名稱*/
        public string cluster_members { get; set; }   /*聚落成員*/
    }

    public class ClusterInfo
    {
        public ClusterInfo()
        {

        }

        public ClusterInfo(ClusterInfoModel clusterInfoModel)
        {
            cluster_no = clusterInfoModel.cluster_no;
            cluster_name = clusterInfoModel.cluster_name;
            cluster_info = clusterInfoModel.cluster_info;        
            enable = clusterInfoModel.enable;        
        }

        public int? cluster_no { get; set; }        /*聚落編號*/
        public string creator_name { get; set; }      /*建立者公司*/
        public string creator_name_en { get; set; }      /*建立者公司*/
        public string cluster_name { get; set; }        /*聚落名稱*/
        public string cluster_members { get; set; }   /*聚落成員*/
        public string cluster_members_en { get; set; }   /*聚落成員*/
        public string cluster_info { get; set; }        /*聚落簡介*/
        public DateTime cluster_create_time { get; set; }        /*聚落成立時間*/
        public DateTime member_invite_time { get; set; }        /*聚落成員邀請時間*/
        public string enable { get; set; }        /*聚落是否成立 0：不成立；1：成立*/
    }
}