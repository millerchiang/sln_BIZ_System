using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace prj_BIZ_System.Models
{
  
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
    }

    public partial class MsgPrivateFileModel
    {
        public long msg_file_no { get; set; }        //私人訊息附件編號
        public long msg_no { get; set; }             //私人訊息編號
        public string msg_file_site { get; set; }    //附件檔案位置
        public DateTime create_time { get; set; }    //建立時間
    }

    public class MsgPrivateReplyModel
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