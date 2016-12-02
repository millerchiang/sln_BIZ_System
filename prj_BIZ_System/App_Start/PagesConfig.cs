using prj_BIZ_System.App_Start;
using prj_BIZ_System.Controllers;
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

        public static IList<T> Pages<T>(this IList<T> modelList , HttpRequestBase req , ControllerBase ctrl,int pageNum) where T : class
        {

            PageList<T> page = new PageList<T>();
            int current_page = 0 ;
            if (req["currentPage" + ((_BaseController)ctrl).pageSeqCount] == null) {
                current_page = 1;
                page.pageSeqIndex = ((_BaseController)ctrl).pageSeqCount++;
            }
            else
            {
                current_page = Int32.Parse(req["currentPage" + (((_BaseController)ctrl).pageSeqCount++)]);
            }
            page.maxPage = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(modelList.Count()) / pageNum));
            page.currentPage = Math.Min(page.maxPage,current_page);
            page.datalist = modelList.Skip<T>((current_page-1)*pageNum).Take<T>(pageNum).ToList<T>();
            page.pageNum = pageNum;
            StringBuilder sb = new StringBuilder("?");
            int i_temp = 0;
            page.paramDict = new Dictionary<string, object>();
            if (req.QueryString != null)
            {
                foreach (var k in req.QueryString.AllKeys)
                {
                    if(k != null)
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
            }
            page.querystring = sb.ToString();
            page.maxCount = modelList.Count();
            page.prevCounts = (page.currentPage-1)*page.pageNum;

            List<PageList<T>> pageList;
            if (ctrl.ViewData["PageList"] == null)
            {
                pageList = new List<PageList<T>>() { page };
            }
            else
            {
                pageList = (List<PageList<T>>)(ctrl.ViewData["PageList"]);
                pageList.Add(page);
            }

            ctrl.ViewData["PageList"] = pageList ;
            return page.datalist;
        }

        public static MvcHtmlString PagesList<T>(this HtmlHelper htmlHelper, List<PageList<T>> pagesList, int index = 0) where T : class
        {
            HttpRequest req = HttpContext.Current.Request;
            string[] urls = req.RawUrl.Split('?');
            string url = urls[0];
            StringBuilder paramStr = new StringBuilder("?");
            StringBuilder otherCurrStr = new StringBuilder("");
            int i_temp = 0;
            PageList<T> pages = pagesList[index];
            int totalSeq = pagesList.Count;

            Dictionary<int, int> otherPages = new Dictionary<int, int>();

            if (pages.paramDict != null)
            {
                foreach (KeyValuePair<string, object> kvp in pages.paramDict.Where(kvpp => !kvpp.Key.Equals("currentPage"+ index)))
                {
                    if (kvp.Key.Contains("currentPage"))
                    {
                        otherCurrStr.Append(kvp.Key + "=" + kvp.Value +"&");
                    }
                    else
                    {
                        if (i_temp > 0)
                        {
                            paramStr.Append("&");
                        }
                        paramStr.Append(kvp.Key + "=" + kvp.Value);
                        i_temp++;
                    }
                }
            }

            if (otherCurrStr.Length == 0)
            {
                for (int c=0;c< totalSeq;c++)
                {
                    if (c != index)
                    {
                        otherCurrStr.Append("currentPage" + c + "=1" + "&");
                    }
                }
            }

            StringBuilder sb = new StringBuilder();

            sb.Append("<p style='padding: 10px 0'>");
            if (pages.maxCount > 0)
            {
                sb.Append(LanguageResource.User.lb_total_results + pages.maxCount);
            }
            else
            {
                sb.Append(LanguageResource.User.lb_nodata);
            }
            sb.Append("</p>");

            if (pages.maxPage > 1)
            {
                sb.Append("<ul class='pagelist'>");
                if (pages.currentPage > 1)
                {
                    string prevPage = Math.Max(0, pages.currentPage - 1).ToString();
                    string paramS1 = paramStr + ((i_temp > 0) ? "&" : "");
                    string partCurrent = "currentPage" + index + "=" + prevPage;
                    sb.Append("<li><a href = '" + (url + paramS1 + otherCurrStr + partCurrent) + "' > &lt;</a></li>");
                }

                int start = Math.Max(0, (pages.currentPage-1) - 5);
                int end = Math.Min(pages.maxPage, (pages.currentPage-1) + 5);

                for (var i = start; i < end; i++)
                {
                    string somePage = (i + 1).ToString();
                    string paramS2 = paramStr + ((i_temp > 0) ? "&" : "");
                    string partCurrent = "currentPage" + index + "=" + somePage;
                    sb.Append("<li><a " + (i == (pages.currentPage - 1) ? "class='active'" : "") + " href = '" + (url + paramS2 + otherCurrStr + partCurrent) + "' >" + (i + 1) + "</a></li>");
                }
                if (pages.maxPage > 1 && pages.currentPage != pages.maxPage)
                {
                    string nextPage = Math.Min(pages.maxPage, pages.currentPage + 1).ToString();
                    string paramS3 = paramStr + ((i_temp > 0) ? "&" : "");
                    string partCurrent = "currentPage" + index + "=" + nextPage;
                    sb.Append("<li><a href = '" + (url + paramS3 + otherCurrStr + partCurrent ) + "' > &gt;</a></li>");
                }
                sb.Append("</ul>");
            }
                    
            return MvcHtmlString.Create(sb.ToString());
        }


        public static MvcHtmlString PagesListX<T>(this HtmlHelper htmlHelper, List<PageList<T>> pagesList, Dictionary<string, string> customSetting , int index = 0) where T : class
        {
            HttpRequest req = HttpContext.Current.Request;
            string[] urls = req.RawUrl.Split('?');
            string url = urls[0];
            StringBuilder paramStr = new StringBuilder("?");
            StringBuilder otherCurrStr = new StringBuilder("");
            int i_temp = 0;
            PageList<T> pages = pagesList[index];
            int totalSeq = pagesList.Count;

            Dictionary<int, int> otherPages = new Dictionary<int, int>();

            Dictionary<string,string> customeParam = customSetting;

            if (pages.paramDict != null)
            {
                foreach (KeyValuePair<string, object> kvp in pages.paramDict.Where(kvpp => !kvpp.Key.Equals("currentPage" + index)))
                {
                    if (kvp.Key.Contains("currentPage"))
                    {
                        otherCurrStr.Append(kvp.Key + "=" + (customeParam!=null && customeParam.ContainsKey(kvp.Key)? customeParam[kvp.Key] :kvp.Value) + "&");
                    }
                    else
                    {
                        if (i_temp > 0)
                        {
                            paramStr.Append("&");
                        }
                        paramStr.Append(kvp.Key + "=" + (customeParam != null && customeParam.ContainsKey(kvp.Key) ? customeParam[kvp.Key] : kvp.Value));
                        i_temp++;
                    }
                }
            }

            if (otherCurrStr.Length == 0)
            {
                for (int c = 0; c < totalSeq; c++)
                {
                    if (c != index)
                    {
                        otherCurrStr.Append("currentPage" + c + "=1" + "&");
                    }
                }
            }

            if (i_temp == 0 && customeParam !=null)
            {
                foreach(KeyValuePair<string, string> kvp in customeParam)
                {
                    if (i_temp > 0)
                    {
                        paramStr.Append("&");
                    }
                    paramStr.Append(kvp.Key + "=" +  kvp.Value);
                    i_temp++;
                }
            }

            StringBuilder sb = new StringBuilder();

            sb.Append("<p style='padding: 10px 0'>");
            if (pages.maxCount > 0)
            {
                sb.Append(LanguageResource.User.lb_total_results + pages.maxCount);
            }
            else
            {
                sb.Append(LanguageResource.User.lb_nodata);
            }
            sb.Append("</p>");

            if (pages.maxPage > 1)
            {
                sb.Append("<ul class='pagelist'>");
                if (pages.currentPage > 1)
                {
                    string prevPage = Math.Max(0, pages.currentPage - 1).ToString();
                    string paramS1 = paramStr + ((i_temp > 0) ? "&" : "");
                    string partCurrent = "currentPage" + index + "=" + prevPage;
                    sb.Append("<li><a href = '" + (url + paramS1 + otherCurrStr + partCurrent) + "' > &lt;</a></li>");
                }

                int start = Math.Max(0, (pages.currentPage - 1) - 5);
                int end = Math.Min(pages.maxPage, (pages.currentPage - 1) + 5);

                for (var i = start; i < end; i++)
                {
                    string somePage = (i + 1).ToString();
                    string paramS2 = paramStr + ((i_temp > 0) ? "&" : "");
                    string partCurrent = "currentPage" + index + "=" + somePage;
                    sb.Append("<li><a " + (i == (pages.currentPage - 1) ? "class='active'" : "") + " href = '" + (url + paramS2 + otherCurrStr + partCurrent) + "' >" + (i + 1) + "</a></li>");
                }
                if (pages.maxPage > 1 && pages.currentPage != pages.maxPage)
                {
                    string nextPage = Math.Min(pages.maxPage, pages.currentPage + 1).ToString();
                    string paramS3 = paramStr + ((i_temp > 0) ? "&" : "");
                    string partCurrent = "currentPage" + index + "=" + nextPage;
                    sb.Append("<li><a href = '" + (url + paramS3 + otherCurrStr + partCurrent) + "' > &gt;</a></li>");
                }
                sb.Append("</ul>");
            }

            return MvcHtmlString.Create(sb.ToString());
        }
    }

    public class PageList<T>
    {
        public int currentPage { get; set; }
        public int prevCounts { get; set; }
        public int pageNum { get; set; }
        public int pageSeqIndex { get; set; }
        public int maxCount { get; set; }
        public int maxPage { get; set; }
        public Dictionary<string, object> paramDict { get; set; }
        public string querystring { get; set; }
        public List<T> datalist { get; set; }
    }
}