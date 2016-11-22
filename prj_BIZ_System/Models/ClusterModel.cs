using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace prj_BIZ_System.Models
{

    public class ClusterModel
    {
        public string user_id { get; set; }      /*建立成員帳號*/
        public string manager_id { get; set; }      /*管理成員帳號*/
        public int? cluster_no { get; set; }        /*聚落編號*/
        public string cluster_name { get; set; }        /*聚落名稱*/
        public string cluster_info { get; set; }        /*聚落簡介*/
        public string enable { get; set; }        /*聚落是否成立 0：不成立；1：成立*/
        public string cluster_enable { get; set; }        /*是否接受邀請 0：不接受；1：接受*/
        public string limit { get; set; }        /*業務會員權限 ps.業務會員權限以逗點分隔*/
        public string is_public { get; set; }      /*是否公開*/
    }


    public class ClusterInfoModel
    {
        public int? cluster_no { get; set; }        /*聚落編號*/
        public string user_id { get; set; }      /*建立者帳號*/
        public string manager_id { get; set; }      /*管理者帳號*/
        public string cluster_name { get; set; }        /*聚落名稱*/
        public string cluster_info { get; set; }        /*聚落簡介*/
        public DateTime create_time { get; set; } /*建立日期*/
        public DateTime update_time { get; set; } /*更新日期*/
        public string enable { get; set; }        /*聚落是否成立 0：不成立；1：成立*/

        public string is_public { get; set; }      /*是否公開*/
        public double file_limit { get; set; }      /*文件總量限制*/
    }

    public class ClusterDetailModel
    {
        public ClusterDetailModel()
        {

        }

        public ClusterDetailModel(ClusterInfoModel clusterInfoModel)
        {
            cluster_no = clusterInfoModel.cluster_no;
            cluster_name = clusterInfoModel.cluster_name;
            cluster_info = clusterInfoModel.cluster_info;
            enable = clusterInfoModel.enable;

            user_id = clusterInfoModel.user_id;
            manager_id = clusterInfoModel.manager_id;
        }

        public int? cluster_no { get; set; }        /*聚落編號*/
        public string creator_name { get; set; }      /*建立者公司*/
        public string creator_name_en { get; set; }      /*建立者公司*/
        public string cluster_name { get; set; }        /*聚落名稱*/
        public string cluster_members { get; set; }   /*聚落成員*/
        public string cluster_members_en { get; set; }   /*聚落成員*/
        public string cluster_members_id { get; set; }   /*聚落成員*/
        public string cluster_info { get; set; }        /*聚落簡介*/
        public DateTime cluster_create_time { get; set; }        /*聚落成立時間*/
        public DateTime member_invite_time { get; set; }        /*聚落成員邀請時間*/
        public string enable { get; set; }        /*聚落是否成立 0：不成立；1：成立*/

        public string user_id { get; set; }      /*建立成員帳號*/
        public string manager_id { get; set; }      /*管理成員帳號*/

    }

    public class ClusterFileModel
    {
        public int cluster_file_no { get; set; }        /*聚落附件編號*/
        public int cluster_no { get; set; }        /*聚落編號*/
        public string user_id { get; set; }      /*上傳成員帳號*/
        public string cluster_file_site { get; set; }        /*附件檔案位置*/
        public DateTime create_time { get; set; } /*建立日期*/
        public string deleted { get; set; }        /*是否刪除 0：刪除；1：未刪除*/
        public double file_size { get; set; }        /*檔案大小*/

    }

    public class ClusterMemberModel
    {
        public int cluster_member_no { get; set; }        /*聚落成員編號*/
        public int? cluster_no { get; set; }        /*聚落編號*/
        public string user_id { get; set; }      /*成員帳號*/
        public string cluster_enable { get; set; }        /*是否接受邀請 0：不接受；1：接受*/
        public DateTime create_time { get; set; } /*建立日期*/
        public string limit { get; set; }        /*業務會員權限 ps.業務會員權限以逗點分隔*/
        public string company { get; set; }        /*成員名稱*/

    }
}