using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace prj_BIZ_System.Models
{
    public class PushListModel
    {
        public int? push_id { get; set; }              /* 推播編號   */
        public string manager_id { get; set; }           /* 管理者帳號 */
        public string push_type { get; set; }         /* 推播類型   */
        public string push_name { get; set; }         /* 推播名稱   */
        public string push_objects { get; set; }      /* 推播對象   */
        public int  activity_id { get; set; }         /* 活動編號   */
        public DateTime push_date { get; set; }       /* 發送日期   */
        public int sample_id { get; set; }            /* 範本編號   */
        public DateTime create_time { get; set; }     /* 建立時間   */
        public DateTime update_time { get; set; }     /* 修改時間   */

        public string sample_title { get; set; }    /*PushSampleModel 的 範本標題*/
    }

    public class PushSampleModel
    {
        public int sample_id { get; set; }          /* 範本編號 */
        public string create_id { get; set; }       /* 建立帳號 */
        public string sample_title { get; set; }    /* 範本標題 */
        public string content { get; set; }          /* 內容     */
        public DateTime create_time { get; set; }   /* 建立時間 */
        public DateTime update_time { get; set; }   /* 更新時間 */
    }
}