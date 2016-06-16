using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace prj_BIZ_System.App_Start
{

    public class UtilConfig<T>
    {
        public static int pageNum = 10;
        public PageList<T> dataPages(List<T> modelList , int current_page , string querystring)
        {
            PageList<T> page = new PageList<T>();
            page.datalist = modelList.Skip<T>((current_page - 1) * pageNum).Take<T>(pageNum).ToList<T>();
            page.pageNum = pageNum;
            page.currentPage = current_page;
            page.maxCount = modelList.Count();
            page.maxPage = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(modelList.Count()) / pageNum));
            return page;
        }

    }

    public class PageList<T>
    {
        public int currentPage { get; set; }
        public int pageNum { get; set; }
        public int maxCount { get; set; }
        public int maxPage { get; set; }
        public List<T> datalist { get; set; }
    }

}