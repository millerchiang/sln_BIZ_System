using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace prj_BIZ_System.Models
{
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
}