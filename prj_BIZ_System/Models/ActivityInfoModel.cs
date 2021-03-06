﻿using System;
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

        public int? grp_id { get; set; }//群組
    }

    public class BuyerInfoModel
    {
        public int serial_no { get; set; }//流水號
        public int activity_id { get; set; }//活動編號
        public string buyer_id { get; set; }//買主帳號
        public string buyer_need { get; set; }//買主媒合需求
        public string buyer_need_en { get; set; }//買主媒合需求(英)
        public int? annual_turnover_3y_ago { get; set; }//年營業額(前三年)
        public int? annual_turnover_2y_ago { get; set; }//年營業額(前二年)
        public int? annual_turnover_1y_ago { get; set; }//年營業額(前一年)
        public int? estimated_purchasing_2y_ago { get; set; }//預估採購金額(前兩年)
        public int? estimated_purchasing_1y_ago { get; set; }//預估採購金額(前一年)
        public int? estimated_purchasing_now { get; set; }//預估採購金額(今年)
        public string distribution { get; set; }//採購範圍
        public string distribution_en { get; set; }//採購範圍(英)
        public string items { get; set; }//採購項目
        public string items_en { get; set; }//採購項目(英)
        public DateTime create_time { get; set; }//建立時間
        public DateTime update_time { get; set; }//修改時間

        public string company { get; set; }//UserInfoModel 的 公司名稱(中文)
        public string activity_name { get; set; }//ActivityInfoModel 的 活動名稱(中文)
        public string company_en { get; set; }//UserInfoModel 的 公司名稱(英文)
        public string activity_name_en { get; set; }//ActivityInfoModel 的 活動名稱(英文)
        public string manager_id { get; set; }//ActivityInfoModel 建立者帳號)
        public string user_id { get; set; }//UserInfoModel 的 使用者帳號
        public string seller_select { get; set; }//ActivityInfoModel 的 是否顯示商務對接  (0：否；1：是)
        public string matchmaking_select { get; set; }//是否顯示媒合時程 (0：否；1：是)

        public int? grp_id { get; set; }//群組
        public DateTime starttime { get; set; }//活動時間(起) yyyy/mm/dd hh:mm
        public DateTime? endtime { get; set; }//活動時間(迄) yyyy/mm/dd hh:mm

        private bool ischeck = false;
        public bool Ischeck { get; set; }//判斷買主是否被勾選
    }

    public class UserInfoToIdAndCpModel
    {
        public string user_id { get; set; }//使用者帳號
        public string company { get; set; }//公司名稱
        public string company_en { get; set; }//公司名稱
    }

    public class QuestionnaireModel
    {
        public int activity_id { get; set; }//活動編號
        public string buyer_id { get; set; }//買主帳號
        public string buyer_name { get; set; }//買主名稱中文
        public string buyer_name_en { get; set; }//買主名稱英文
        public string seller_id { get; set; }//賣家帳號
        public string seller_name { get; set; }//賣家名稱中文
        public string seller_name_en { get; set; }//賣家名稱英文
        public string question_1 { get; set; }//問題一選項回覆
        public string question_1_1 { get; set; }//訂單成交金額
        public string question_1_2 { get; set; }//訂單預估成交金額
        public string question_1_2_other { get; set; }//訂單預估成交金額_其他
        public string question_1_4 { get; set; }//其他
        public string question_2 { get; set; }//問題二回覆
        public DateTime create_time { get; set; }//建立時間
        public DateTime update_time { get; set; }//修改時間
    }



    public class ActivityRegisterModel
    {
        public int register_id { get; set; }//報名編號流水號
        public int activity_id { get; set; }//活動編號
        public string user_id { get; set; }//會員帳號
        public int quantity { get; set; }//與會人數
        public string name_a { get; set; }//與會人姓名
        public string title_a { get; set; }//與會人職稱
        public string name_b { get; set; }//主要聯絡人
        public string title_b { get; set; }//主要聯絡人職稱
        public string telephone { get; set; }//連絡電話
        public string phone { get; set; }//手機號碼
        public string email { get; set; }//電子郵件
//        public string catalog_file { get; set; }//公司型錄檔案位置 (企業型錄，使用者自行上傳)
        public string manager_check { get; set; }//後台審核 (0：不通過；1：通過)
        public string user_info { get; set; }//公司簡介(中文) (預設與用戶資訊相同)
        public string user_info_en { get; set; }//公司簡介(英文) (預設與用戶資訊相同)
        public DateTime create_time { get; set; }//建立時間
        public DateTime update_time { get; set; }//修改時間

        public string activity_name { get; set; }//ActivityInfoModel 的 活動名稱(中文)
        public string activity_name_en { get; set; }
        public string manager_id { get; set; }//ActivityInfoModel 建立者帳號)
        public string addr { get; set; }//ActivityInfoModel 活動地點)
        public string addr_en { get; set; }//ActivityInfoModel 活動地點)
        public DateTime starttime { get; set; }//ActivityInfoModel 的 活動開始時間
        public DateTime endtime { get; set; }//ActivityInfoModel 的 活動結束時間
        public string seller_select { get; set; }//ActivityInfoModel 的 是否顯示商務對接  (0：否；1：是)
        public string matchmaking_select { get; set; }//ActivityInfoModel 的 是否顯示媒合時程 (0：否；1：是)
        public string company { get; set; }//UserInfoToIdAndCpModel 的 公司名稱
        public string company_en { get; set; }//UserInfoToIdAndCpModel 的 公司英文名稱
        public string buyer_need { get; set; }//BuyerInfoModel 的 買主媒合需求
        public int? grp_id { get; set; }//群組
        private bool ischeck = false;
        public bool Ischeck { get; set; }//判斷賣家是否被勾選
    }

    public class NewsModel
    {
        public int news_no { get; set; }//新聞編號流水號
        public string manager_id { get; set; }//建立帳號
        public string news_title { get; set; }//新聞標題
        public DateTime news_date { get; set; }//發布日期
        public string news_type { get; set; }//新聞類型
        public int activity_id { get; set; }//活動編號
        public string website { get; set; }//網址
        public string content { get; set; }//內容
        public DateTime create_time { get; set; }//建立時間
        public DateTime update_time { get; set; }//修改時間
        public string news_style { get; set; }//新聞語系

        public string activity_name { get; set; }//ActivityInfoModel 的 活動名稱(中文)
        public int? grp_id { get; set; }//群組

        public DateTime starttime { get; set; }// 活動開始時間

    }

    public class EnterpriseSortAndListModel
    {
        public string user_id { get; set; }//使用者帳號
        public int sort_id { get; set; }//產業別流水號
        public string enterprise_sort_id { get; set; }//產業別編號
        public string enterprise_sort_name { get; set; }//產業別名稱
        public string enterprise_sort_name_en { get; set; }//產業別名稱(英文)
        public string enterprise_sort_id_b { get; set; }//產業別編號
        public string enterprise_sort_name_b { get; set; }//產業別名稱
        public string enterprise_sort_name_en_b { get; set; }//產業別名稱(英文)
    }

    public class ActivityProductSelectModel
    {
        public int serial_no { get; set; }//流水號
        public string user_id { get; set; }//使用者帳號
        public int activity_id { get; set; }//活動編號
        public int product_id { get; set; }//產品編號

        public string product_name { get; set; } //ProductListModel 的 產品名稱(中文)
        public string product_info { get; set; } //ProductListModel 的 產品簡介(中文)
    }

    public class ActivityCatalogSelectModel
    {
        public int serial_no { get; set; }//流水號
        public string user_id { get; set; }//使用者帳號
        public int activity_id { get; set; }//活動編號
        public int catalog_no { get; set; }//型錄編號
        public string catalog_name { get; set; }//型錄編號
        public string cover_file { get; set; } //CatalogListModel 的 公司型錄封面位置
        public string catalog_file { get; set; } //CatalogListModel 的 公司型錄檔案位置
    }

    public class ActivityPhotoModel
    {
        public int? photo_id { get; set; } //流水號
        public string manager_id { get; set; } //管理者帳號
        public DateTime photo_time { get; set; }//照片顯示時間
        public string photo_brief { get; set; } //照片簡介(中文)
        public string photo_brief_en { get; set; } //照片簡介(英文)
        public string photo_info { get; set; } //照片說明(中文)
        public string photo_info_en { get; set; } //照片說明(英文)
        public string photo_pic_site { get; set; } //活動圖片位置
        public string active { get; set; } //顯示於網頁(0：不顯示；1：顯示)
        public string deleted { get; set; } //是否刪除(0：刪除；1：未刪除)
    }

    public class BannerPhotoModel
    {
        public int? photo_id { get; set; } //流水號
        public string manager_id { get; set; } //管理者帳號
        public string photo_pic_site { get; set; } //Banner圖片位置
        public string active { get; set; } //顯示於網頁(0：不顯示；1：顯示)
        public string deleted { get; set; } //是否刪除(0：刪除；1：未刪除)
        public string hlink { get; set; } //超連結網址
    }

}