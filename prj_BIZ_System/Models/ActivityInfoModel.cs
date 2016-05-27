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

    public class BuyerInfoModel
    {
        public int serial_no { get; set; }//流水號
        public int activity_no { get; set; }//活動編號
        public string buyer_id { get; set; }//買主帳號
        public string buyer_need { get; set; }//買主媒合需求
    }

    public class UserInfoToIdAndCpModel
    {
        public string user_id { get; set; }//使用者帳號
        public string company { get; set; }//公司名稱
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
        public string catalog_file { get; set; }//公司型錄檔案位置 (企業型錄，使用者自行上傳)
        public string manager_check { get; set; }//後台審核 (0：不通過；1：通過)
        public string user_info { get; set; }//公司簡介(中文) (預設與用戶資訊相同)
        public string user_info_en { get; set; }//公司簡介(英文) (預設與用戶資訊相同)
        public DateTime create_time { get; set; }//建立時間
        public DateTime update_time { get; set; }//修改時間
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
    }

    public class EnterpriseSortAndListModel
    {
        public string user_id { get; set; }//使用者帳號
        public int sort_id { get; set; }//產業別流水號
        public string enterprise_sort_id { get; set; }//產業別編號
        public string enterprise_sort_name { get; set; }//產業別名稱
    }

    public class ActivityProductSelectModel
    {
        public int serial_no { get; set; }//流水號
        public string user_id { get; set; }//使用者帳號
        public int activity_id { get; set; }//活動編號
        public int product_id { get; set; }//產品編號
    }

    public class ActivityCatalogSelectModel
    {
        public int serial_no { get; set; }//流水號
        public string user_id { get; set; }//使用者帳號
        public int activity_id { get; set; }//活動編號
        public int catalog_no { get; set; }//型錄編號
    }

    /*
        public class News_BNList_ViewModel
        {
            public IList<NewsModel> NewsList { get; set; }
        }

        public class News_EAInfo_ViewModel
        {
            public IList<ActivityInfoModel> ActivityInfoList { get; set; }
            public NewsModel NewsModel { get; set; }
        }
    */


}