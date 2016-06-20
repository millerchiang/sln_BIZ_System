using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using prj_BIZ_System.App_Start;
using System.Text;
using System.Web.Mvc.Ajax;

/*
namespace System.Web.Mvc.Html
{
    public static class MyHtmlHelperExtensions
    {

        public static MvcHtmlString PagesList<T>(this HtmlHelper htmlHelper, PageList<T> pages) where T:class
        {

            HttpRequest req = HttpContext.Current.Request;
            string url = HttpContext.Current.Request.RawUrl.Split('?')[0];


            StringBuilder paramStr = new StringBuilder("?");
            int i_temp = 0;
            if (pages.paramDict != null)
            {
                foreach(KeyValuePair<string,object> kvp in pages.paramDict.Where( kvpp => !kvpp.Key.Equals("currentPage")))
                {
                    if (i_temp > 0)
                    {
                        paramStr.Append("&");
                    }
                    paramStr.Append(kvp.Key +"="+kvp.Value);
                    i_temp++;
                }
            }

            StringBuilder sb = new StringBuilder();
            sb.Append("<ul class='pagelist'>");
            if(pages.currentPage > 1)
            {
                string prevPage = Math.Max(0, pages.currentPage - 1).ToString();
                string paramS1 = paramStr + ((i_temp > 0) ? "&" : "") + "currentPage=" + prevPage;
                sb.Append("<li><a href = '" + (url + paramS1) + "' > &lt;</a></li>");
            }
            for(var i = 0; i < pages.maxPage; i++)
            {
                string somePage = (i+1).ToString();
                string paramS2 = paramStr + ((i_temp > 0) ? "&" : "") + "currentPage=" + somePage;
                sb.Append("<li><a href = '" + (url + paramS2) + "' >" + (i+1)+"</a></li>");
            }
            if(pages.maxPage > 1 && pages.currentPage != pages.maxPage)
            {
                string nextPage = Math.Min(pages.maxPage, pages.currentPage + 1).ToString();
                string paramS3 = paramStr + ((i_temp > 0) ? "&" : "") + "currentPage=" + nextPage;
                sb.Append("<li><a href = '"+(url+ paramS3) +"' > &gt;</a></li>");
            }
            sb.Append("</ul>");
            //var lnk = htmlHelper.ActionLink("[replaceme]", actionName, controllerName, ajaxOptions);
            return MvcHtmlString.Create(sb.ToString().Replace("[pageszz]", ""));
        }
    }
}

    */