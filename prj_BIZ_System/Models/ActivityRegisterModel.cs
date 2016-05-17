using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace prj_BIZ_System.Models
{
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
}