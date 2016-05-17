using prj_BIZ_System.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace prj_BIZ_System.ViewModels
{
    /*新聞網頁的新聞列表Model*/
    public class News_BNList_ViewModel
    {
        public IList<NewsModel> NewsList { get; set; }
    }

    /*新聞網頁的活動新聞Model*/
    public class News_EAInfo_ViewModel
    {
        public IList<ActivityInfoModel> ActivityInfoList { get; set; }
        public NewsModel NewsModel { get; set; }
    }
}