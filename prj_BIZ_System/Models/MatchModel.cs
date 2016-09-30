using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace prj_BIZ_System.Models
{
    public class MatchmakingNeedModel
    {
        public int serial_no { get; set; }      //流水號
        public int activity_id { get; set; }    //活動編號
        public string seller_id { get; set; }   //參加廠商帳號
        public string buyer_id { get; set; }    //對接買主帳號
        public string buyer_reply { get; set; } //買主回覆狀態

        public string company { get; set; }     //UserInfoToIdAndCpModel 的 公司名稱
    }

    public class MatchmakingAllModel
    {
        public int serial_no { get; set; }      //流水號
        public int activity_id { get; set; }    //活動編號
        public string seller_id { get; set; }   //參加廠商帳號
        public string buyer_id { get; set; }    //對接買主帳號

        public string company { get; set; }     //UserInfoToIdAndCpModel 的 公司名稱
        public string company_en { get; set; }     //UserInfoToIdAndCpModel 的 公司名稱

        public string IsBothOrBuyer { get; set; } //是雙方媒合資料或買方媒合資料
    }






    public class MatchmakingScheduleModel
    {
        public int serial_no { get; set; }        /*流水號*/
        public int activity_id { get; set; }      /*活動編號*/
        public int period_sn { get; set; }        /*時段流水號*/
        public string buyer_id { get; set; }        /*買家id*/
        public string seller_id { get; set; }     /*媒合賣家id*/
        public DateTime create_time { get; set; } /*建立日期*/
        public DateTime update_time { get; set; } /*更新日期*/

        public string company { get; set; }     //UserInfoToIdAndCpModel 的 公司名稱
        public string company_en { get; set; }     //UserInfoToIdAndCpModel 的 公司名稱
    }

    public class SchedulePeriodSetModel
    {
        public int period_sn { get; set; }        /*時段流水號*/
        public int activity_id { get; set; }      /*活動編號*/
        public DateTime time_start { get; set; }  /*時間起*/
        public DateTime time_end { get; set; }    /*時間迄*/
    }

}