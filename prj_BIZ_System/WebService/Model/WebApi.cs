﻿using prj_BIZ_System.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace prj_BIZ_System.WebService.Model
{
    public class UserEnterpriseInfo
    {
        public UserInfoModel userinfo { get; set; }
        public IList<EnterpriseSortModel> usersortList { get; set; }
    }

    public class ActivityRegister
    {
        public string activity_name { get; set; }//活動名稱(中文)
        public DateTime starttime { get; set; }//活動時間(起) yyyy/mm/dd hh:mm
        public DateTime endtime { get; set; }//活動時間(迄) yyyy/mm/dd hh:mm
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

    public class AccountActivity
    {
        public int activity_id { get; set; }//活動編號
        public string activity_name { get; set; }//ActivityInfoModel 的 活動名稱(中文)
        public string is_buyer { get; set; }//是否為活動買家(1:買家,0:賣家)
    }

    public class Buyer
    {
        public int activity_id { get; set; }//活動編號
        public string buyer_id { get; set; }//買主帳號
        public string buyer_need { get; set; }//買主媒合需求
        public string company { get; set; }//UserInfoModel 的 公司名稱(中文)
    }

    public class SellerNeed
    {
        public IList<Buyer> seller_check { get; set; }//賣家有意願洽談買家
        public IList<Buyer> manager_schedule { get; set; }//主辦方排定買家
    }

    public class Seller
    {
        public int activity_id { get; set; }//活動編號
        public string seller_id { get; set; }//賣家帳號
        public string company { get; set; }//UserInfoModel 的 公司名稱(中文)
    }

    public class BuyerNeed
    {
        public IList<Seller> buyer_check { get; set; }//買家有意願洽談賣家
        public IList<Seller> manager_schedule { get; set; }//主辦方排定賣家
    }

    public class MsgPrivate
    {
        public long msg_no { get; set; }             //私人訊息編號
        public string msg_title { get; set; }        //訊息標題
        public string create_time { get; set; }    //建立時間
        //UerInfo 公司名稱(中文)
        public string company { get; set; }
    }


}