using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace prj_BIZ_System.Models
{
    public class UserInfoModel  //user_info
    {
        public int id { get; set; }//流水號
        public string user_id { get; set; }//使用者帳號 
        public string user_pw { get; set; }//使用者密碼
        public string id_enable { get; set; }//帳號有效
        public string enterprise_type { get; set; }//企業類型
        public string enterprise_type_en { get; set; }//企業類型
        public string company { get; set; }//公司名稱(中文)
        public string leader { get; set; }//代表人(中文)
        public string addr { get; set; }// 公司地址(中文)
        public string contact { get; set; }// 聯絡人姓名(中文)
        public string phone { get; set; }//聯絡電話
        public string email { get; set; }//電子郵件
        public long capital { get; set; }//資本額(單位：千)
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

        //額外加的
        public string device_id { get; set; }       /*MobileDeviceInfo 的 裝置識別碼*/
        public string device_os { get; set; }       /*MobileDeviceInfo 的 裝置作業系統*/

    }

    public class EnterpriseSortListModel //enterprise_sort_list
    {
        public int sort_id { get; set; }//產業別流水號 
        public string enterprise_sort_id { get; set; }//產業別編號
        public string enterprise_sort_name { get; set; }//產業別名稱
        public string enterprise_sort_name_en { get; set; }//產業別名稱
    }

    public class EnterpriseSortModel  //enterprise_sort
    {
        public string user_id { get; set; }//使用者帳號
        public int sort_id { get; set; }//產業別流水號 
        public string enterprise_sort_id { get; set; }//產業別編號
        public string enterprise_sort_name { get; set; }//產業別名稱
        public string enterprise_sort_name_en { get; set; }//產業別名稱
    }

    public class CompanySortModel  //company_sort
    {
        public string user_id { get; set; }//使用者帳號
        public string company { get; set; }//公司中文名稱
        public string company_en { get; set; }//公司英文名稱
        public int sort_id { get; set; }//產業別流水號 
    }

    public class ProductListModel
    {
        public int? product_id { get; set; } //P+流水號9碼
        public string user_id { get; set; } //使用者帳號  (國內企業輸入統編為會員帳號；國外會員自訂)
        public string product_category { get; set; } //類別(中文)
        public string product_name { get; set; } //產品名稱(中文)
        public string product_info { get; set; } //產品資料(中文)
        public string model_no { get; set; } //商品型號(中文)
        public string patent_or_winners { get; set; } //商品專利/得獎(中文)
        public string specifications_or_other { get; set; } //商品規格/其他(中文)
        public string product_category_en { get; set; } //類別(英文)
        public string product_name_en { get; set; } //產品名稱(英文)
        public string product_info_en { get; set; } //產品資料(英文)
        public string model_no_en { get; set; } //商品型號(英文)
        public string patent_or_winners_en { get; set; } //商品專利/得獎(英文)
        public string specifications_or_other_en { get; set; } //商品規格/其他(英文)
        public string product_pic_site { get; set; } //商品圖片位置
        public string deleted { get; set; } //是否刪除  (0：刪除；1：未刪除)

        public string company { get; set; } //公司名稱
        public string company_en { get; set; } //公司名稱(英文)
    }
    
    public class CatalogListModel
    {
        public int catalog_no { get; set; } //型錄編號
        public string user_id { get; set; } //使用者帳號
        public string catalog_name { get; set; } //型錄名稱
        public string cover_file { get; set; } //公司型錄封面位置
        public string catalog_file { get; set; } //公司型錄檔案位置
        public string deleted { get; set; } //是否刪除(0：刪除；1：未刪除)

        public string company { get; set; } // userInfo表內 的 公司名稱(中文)
        public string company_en { get; set; } // userInfo表內 的 公司名稱(中文)
    }

    public class VideoListModel
    {
        public int video_no { get; set; } //影音編號
        public string user_id { get; set; } //使用者帳號
        public string video_name { get; set; } //影音型錄名稱
        public string youtube_site { get; set; } //youtube影音網址

        public string company { get; set; } // userInfo表內 的 公司名稱(中文)
        public string company_en { get; set; } // userInfo表內 的 公司名稱(中文)
    }

    public class ActiveVideoModel
    {
        public int video_no { get; set; } //影音編號
    }

    public class SalesInfoModel
    {
        public string sales_id { get; set; }        //業務帳號
        public string user_id { get; set; }         //所屬企業帳號
        public string sales_name { get; set; }      //業務姓名
        public string sales_pw { get; set; }        //業務密碼
        public string id_enable { get; set; }       //帳號有效 0：無效(未驗證前) 1：有效(驗證完成)
        public string phone { get; set; }           //聯絡電話
        public string email { get; set; }           //電子郵件
        public string limit { get; set; }           //權限 (0：關閉；1：開放)
        public DateTime? create_time { get; set; }   //建立時間
        public DateTime? update_time { get; set; }  //修改時間

        public string company { get; set; }         //公司名稱
        public string company_en { get; set; }      //公司英文名稱

        //額外加的
        public string device_id { get; set; }       /*MobileDeviceInfo 的 裝置識別碼*/
        public string device_os { get; set; }       /*MobileDeviceInfo 的 裝置作業系統*/
    }

    }