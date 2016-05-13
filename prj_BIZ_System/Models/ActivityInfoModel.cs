using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace prj_BIZ_System.Models
{
    public class ActivityInfoModel
    {
        public int activity_id { get; set; }//年份+流水號(四碼)EX: 20160001
        public string manager_id { get; set; }//建立者帳號 (建立本活動的管理者帳號)
        public string activity_type { get; set; }//活動類型 (0：商洽會；1：拓銷會)
        public string activity_name { get; set; }//活動名稱(中文)
        public DateTime starttime { get; set; }//活動時間(起) yyyy/mm/dd hh:mm
        public DateTime endtime { get; set; }//活動時間(迄) yyyy/mm/dd hh:mm
        public string addr { get; set; }//活動地點(中文)
        public string organizer { get; set; }//主辦單位(中文)
        public string name { get; set; }//主要聯絡人
        public string phone { get; set; }//手機號碼
        public string email { get; set; }//電子郵件
        public string seller_select { get; set; }//是否顯示商務對接  (0：否；1：是)
        public string matchmaking_select { get; set; }//是否顯示媒合時程 (0：否；1：是)
        public string activity_name_en { get; set; }//活動名稱(英文)
        public string addr_en { get; set; }//活動地點(英文)
        public string organizer_en { get; set; }//主辦單位(英文)
        public DateTime create_time { get; set; }//建立時間
        public DateTime update_time { get; set; }//修改時間

    }
}