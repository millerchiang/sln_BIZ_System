using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace prj_BIZ_System.Models
{
    public class UserInfoModel  //user_info
    {
        public string user_id { get; set; }//使用者帳號 
        public string user_pw { get; set; }//使用者密碼
        public string id_enable { get; set; }//帳號有效
        public string enterprise_type { get; set; }//企業類型
        public string company { get; set; }//公司名稱(中文)
        public string leader { get; set; }//代表人(中文)
        public string addr { get; set; }// 公司地址(中文)
        public string contact { get; set; }// 聯絡人姓名(中文)
        public string phone { get; set; }//聯絡電話
        public string email { get; set; }//電子郵件
        public int capital { get; set; }//資本額(單位：萬)
        public string revenue { get; set; }//營業額
        public string website { get; set; }//企業網址
        public string info { get; set; }//企業簡介(中文)
        public DateTime create_time { get; set; }//建立時間
        public DateTime update_time { get; set; }//修改時間
        public DateTime last_login_time { get; set; }//最後登入時間
        public string logo_img { get; set; }//公司logo圖(企業logo，使用者自行上傳)
        public string company_en { get; set; }//公司名稱(英文)
        public string leader_en { get; set; }//代表人(英文)
        public string addr_en { get; set; }//公司地址(英文)
        public string contact_en { get; set; }//聯絡人姓名(英文)
        public string info_en { get; set; }//企業簡介(英文)

    }

    public class EnterpriseSortModel //enterprise_sort_list
    {
        public int sort_id { get; set; }//產業別流水號 
        public string enterprise_sort_id { get; set; }//產業別編號
        public string enterprise_sort_name { get; set; }//產業別名稱
    }

    public class UserSortModel  //enterprise_sort
    {
        public string user_id { get; set; }//使用者帳號
        public int sort_id { get; set; }//產業別流水號 
    }

    public class User_ViewModel
    {
        public UserInfoModel userinfo { get; set; }
        public EnterpriseSortModel enterprisesort { get; set; }
        public UserSortModel usersort { get; set; }
        public IList<UserInfoModel> userinfoList { get; set; }
        public IList<EnterpriseSortModel> enterprisesortList { get; set; }
        public IList<UserSortModel> usersortList { get; set; }
    }


}