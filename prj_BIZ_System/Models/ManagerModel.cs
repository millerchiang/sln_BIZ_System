using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace prj_BIZ_System.Models
{
    public class ManagerInfoModel
    {
        public string manager_id { get; set; }      //管理者帳號
        public string manager_pw { get; set; }      //管理者密碼
        public string enable { get; set; }          //帳號有效
        public string name { get; set; }            //姓名
        public string phone { get; set; }           //電話
        public string email { get; set; }           //電子郵件
        public string create_manager { get; set; }  //建立人
        public int?   grp_id { get; set; }          //群組編號
        public DateTime create_time { get; set; }   //建立時間
        public DateTime update_time { get; set; }   //資料更新時間

        public string grp_name { get; set; }             //群組名稱
    }

    public class GroupModel
    {
        public int grp_id { get; set; }             //群組id
        public string grp_name { get; set; }        //群組名稱
        public string limit { get; set; }            //權限
        public DateTime create_time { get; set; }   //建立時間
        public DateTime update_time { get; set; }   //更新時間
    }
}