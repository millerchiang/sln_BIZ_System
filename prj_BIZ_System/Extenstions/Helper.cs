using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using prj_BIZ_System.App_Start;
using System.Text;
using System.Web.Mvc.Ajax;

namespace System.Web.Mvc.Html
{
    public static class MyHtmlHelperExtensions
    {
        public static MvcHtmlString InsertHtmlTagActionLink(this AjaxHelper ajaxHelper, string linkText, string actionName, string controllerName, AjaxOptions ajaxOptions)
        {
            var lnk = ajaxHelper.ActionLink("[replaceme]", actionName, controllerName, ajaxOptions);
            return MvcHtmlString.Create(lnk.ToString().Replace("[replaceme]", linkText));
        }

        public static MvcHtmlString Test(this HtmlHelper htmlHelper, string linkText, string actionName, string controllerName, AjaxOptions ajaxOptions)
        {
            var lnk = htmlHelper.ActionLink("[replaceme]", actionName, controllerName, ajaxOptions);
            return MvcHtmlString.Create(lnk.ToString().Replace("[replaceme]", linkText));
        }

        public static MvcHtmlString PagesList<T>(this HtmlHelper htmlHelper, PageList<T> pages) where T:class
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<ul class='pagelist'>");
            if(pages.currentPage > 1)
            {
                sb.Append("<li><a href = '' > &lt;</a></li>");
            }
            for(var i = 0; i < pages.maxPage; i++)
            {
                sb.Append("<li><a href = '' >"+(i+1)+"</a></li>");
            }
            if(pages.maxPage > 1 && pages.currentPage != pages.maxPage)
            {
                sb.Append("<li><a href = '' > &gt;</a></li>");
            }
            sb.Append("</ul>");
            //var lnk = htmlHelper.ActionLink("[replaceme]", actionName, controllerName, ajaxOptions);
            return MvcHtmlString.Create(sb.ToString().Replace("[pageszz]", ""));
        }
    }
}