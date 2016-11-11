using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace prj_BIZ_System.Models
{

    public class MsgModel
    {
        public long msg_no { get; set; }             //私人訊息編號
        public string creater_id { get; set; }       //建立者帳號
        public string msg_title { get; set; }        //訊息標題
        public string msg_content { get; set; }      //訊息內容

        public int? cluster_no { get; set; }         //聚落編號 (Default 0，代表沒有聚落)
        public string is_public { get; set; }       //聚落公開 (公開：1；私人：0)
        public string user_id { get; set; }         //公司id (有值: 公司訊息 ; 空值 : 非公司訊息)

        public string msg_member { get; set; }       //成員
        public DateTime create_time { get; set; }    //建立時間

        //UerInfo 公司名稱(中文)
        public string company { get; set; }
        public int rpy_cnt { get; set; } //回覆數
        public string is_read { get; set; }     // 0 :未讀 , 1:已讀

        public string cluster_user { get; set; } //聚落使用者的id
        public string[] msg_members { get; set; }       //成員陣列
    }

    /*
    public class MsgPrivateModel
    {
        public long msg_no { get; set; }             //私人訊息編號
        public string creater_id { get; set; }       //建立者帳號
        public string msg_title { get; set; }        //訊息標題
        public string msg_content { get; set; }      //訊息內容
        public string msg_member { get; set; }       //成員
        public DateTime create_time { get; set; }    //建立時間

        //UerInfo 公司名稱(中文)
        public string company { get; set; }
        public int rpy_cnt { get; set; } //回覆數 
    }
    */

    public partial class MsgFileModel
    {
        public long msg_file_no { get; set; }        //私人訊息附件編號
        public long msg_no { get; set; }             //私人訊息編號
        public string msg_file_site { get; set; }    //附件檔案位置
        public DateTime create_time { get; set; }    //建立時間
    }

    public class MsgReplyModel
    {
        public long msg_reply_no { get; set; }       //私人訊息回覆編號
        public long msg_no { get; set; }             //私人訊息編號
        public string msg_reply { get; set; }        //回覆者
        public string reply_content { get; set; }    //回覆內容
        public DateTime create_time { get; set; }    //建立時間

        //UerInfo 公司名稱(中文)
        public string company { get; set; }
        //logo_img  公司logo圖
        public string logo_img { get; set; }

        public List<MsgReplyFileModel> msg_reply_file { get; set; } //回覆時的附件
    }

    public class MsgReplyFileModel
    {
        public long msg_reply_file_no { get; set; }        //訊息回覆附件編號
        public long msg_reply_no { get; set; }             //訊息回覆編號
        public string msg_reply_file_site { get; set; }    //訊息回覆附件檔案位置
        public DateTime create_time { get; set; }    //建立時間
    }



    public class MsgPushModel
    {
        public long   msg_no { get; set; }           //私人訊息編號         (必傳)
        public string msg_title { get; set; }        //訊息主題             (必傳)
        public string msg_content { get; set; }      //訊息內文             (必傳)
        public string reply_user_id { get; set; }    //回覆者id
        public string company { get; set; }          //回覆者名稱           (必傳)
        public string company_en { get; set; }       //回覆者英文名稱       (必傳)
        //public string company_send { get; set; }     //發送訊息時的名稱
        public long   msg_reply_no { get; set; }     //私人訊息回覆編號     (必傳) //手機端判斷依據
        public string reply_content { get; set; }    //訊息回覆的內容       (必傳) //手機端判斷依據
        public string device_id { get; set; }       /*MobileDeviceInfo 的 裝置識別碼*/
        public string device_os { get; set; }       /*MobileDeviceInfo 的 裝置作業系統*/
    }
}