using System.Web;
using System.Web.Optimization;

namespace prj_BIZ_System
{
    public class BundleConfig
    {
        // 如需「搭配」的詳細資訊，請瀏覽 http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            bundles.Add(new ScriptBundle("~/bundles/ckeditor").Include(
                        "~/Scripts/ckeditor/ckeditor.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryUI").Include(
                        "~/Scripts/jqueryUI/jquery-ui.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/datetimepicker").Include(
                        "~/Scripts/datetimepicker/jquery-ui-timepicker-zh-TW.js",
                        "~/Scripts/datetimepicker/jquery-ui-sliderAccess.js",
                        "~/Scripts/datetimepicker/jquery-ui-timepicker-addon.js",
                        "~/Scripts/datetimepicker/datepicker-zh-TW.js"));
            
            // 使用開發版本的 Modernizr 進行開發並學習。然後，當您
            // 準備好實際執行時，請使用 http://modernizr.com 上的建置工具，只選擇您需要的測試。
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/site.css"));

            bundles.Add(new StyleBundle("~/Content/css/timepicker").Include(
                      "~/Content/jquery-ui-timepicker-addon.css",
                      "~/Content/jquery-ui.css"
                      ));

            bundles.Add(new StyleBundle("~/stylesheets/css").Include(
                      "~/stylesheets/screen.css"
                      ));

            bundles.Add(new StyleBundle("~/stylesheets/css/sys").Include(
                      "~/stylesheets/sys.css"
                      ));
        }
    }
}
