using prj_BIZ_System.App_Start;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace prj_BIZ_System.App_Start
{
    public static class PagesConfig
    {
        public static int pageNum = 10;

        public static IList<T> Pages<T>(this IList<T> modelList , HttpRequestBase req , ControllerBase ctrl) where T : class
        {
            int current_page = req["currentPage"] == null ? 1 : Int32.Parse(req["currentPage"]);
            PageList<T> page = new PageList<T>();
            page.datalist = modelList.Skip<T>((current_page - 1) * pageNum).Take<T>(pageNum).ToList<T>();
            page.pageNum = pageNum;
            page.currentPage = current_page;
            StringBuilder sb = new StringBuilder("?");
            int i_temp = 0;
            page.paramDict = new Dictionary<string, object>();
            if (req.QueryString != null)
            {
                foreach (var k in req.QueryString.AllKeys)
                {
                    page.paramDict.Add(k, req.QueryString[k]);

                    if (i_temp > 0)
                    {
                        sb.Append("&");
                    }
                    sb.Append(k + "=" + req.QueryString[k]);
                    i_temp++;
                }
            }
            page.querystring = sb.ToString();
            page.maxCount = modelList.Count();
            page.maxPage = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(modelList.Count()) / pageNum));
            ctrl.ViewBag.Pages = page;
            return page.datalist;
        }

        public static MvcHtmlString PagesList<T>(this HtmlHelper htmlHelper, PageList<T> pages) where T : class
        {
            HttpRequest req = HttpContext.Current.Request;
            string url = req.RawUrl.Split('?')[0];

            StringBuilder paramStr = new StringBuilder("?");
            int i_temp = 0;
            if (pages.paramDict != null)
            {
                foreach (KeyValuePair<string, object> kvp in pages.paramDict.Where(kvpp => !kvpp.Key.Equals("currentPage")))
                {
                    if (i_temp > 0)
                    {
                        paramStr.Append("&");
                    }
                    paramStr.Append(kvp.Key + "=" + kvp.Value);
                    i_temp++;
                }
            }

            StringBuilder sb = new StringBuilder();
            sb.Append("<ul class='pagelist'>");
            if (pages.currentPage > 1)
            {
                string prevPage = Math.Max(0, pages.currentPage - 1).ToString();
                string paramS1 = paramStr + ((i_temp > 0) ? "&" : "") + "currentPage=" + prevPage;
                sb.Append("<li><a href = '" + (url + paramS1) + "' > &lt;</a></li>");
            }

            int start = Math.Max(0, pages.currentPage - 5);
            int end = Math.Min(pages.maxPage, pages.currentPage + 5);

            for (var i = start; i < end; i++)
            {
                string somePage = (i + 1).ToString();
                string paramS2 = paramStr + ((i_temp > 0) ? "&" : "") + "currentPage=" + somePage;
                sb.Append("<li><a href = '" + (url + paramS2) + "' >" + (i + 1) + "</a></li>");
            }
            if (pages.maxPage > 1 && pages.currentPage != pages.maxPage)
            {
                string nextPage = Math.Min(pages.maxPage, pages.currentPage + 1).ToString();
                string paramS3 = paramStr + ((i_temp > 0) ? "&" : "") + "currentPage=" + nextPage;
                sb.Append("<li><a href = '" + (url + paramS3) + "' > &gt;</a></li>");
            }
            sb.Append("</ul>");
            return MvcHtmlString.Create(sb.ToString());
        }

    }

    public class PageList<T>
    {
        public int currentPage { get; set; }
        public int pageNum { get; set; }
        public int maxCount { get; set; }
        public int maxPage { get; set; }
        public Dictionary<string, object> paramDict { get; set; }
        public string querystring { get; set; }
        public List<T> datalist { get; set; }
    }
}