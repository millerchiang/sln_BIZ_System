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

        public int cluster_no { get; set; }         //聚落編號 (Default 0，代表沒有聚落)
        public string is_public { get; set; }       //聚落公開 (公開：1；私人：0)
        public string user_id { get; set; }         //公司id (有值: 公司訊息 ; 空值 : 非公司訊息)

        public string msg_member { get; set; }       //成員
        public DateTime create_time { get; set; }    //建立時間

        //UerInfo 公司名稱(中文)
        public string company { get; set; }
        public int rpy_cnt { get; set; } //回覆數
        public string is_read { get; set; }     // 0 :未讀 , 1:已讀

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
    }

    
}