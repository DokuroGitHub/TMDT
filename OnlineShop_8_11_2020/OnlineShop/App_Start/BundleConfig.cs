using System.Web;
using System.Web.Optimization;

namespace OnlineShop
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jscore").Include(
                    "~/assets/client/js/jquery-1.12.4.min.js",
                    "~/assets/client/js/jquery-ui.js",
                    "~/assets/client/js/bootstrap.min.js",
                    "~/assets/client/js/move-top.js",
                    "~/assets/client/js/easing.js",
                    "~/assets/client/js/startstop-slider.js"));

            bundles.Add(new ScriptBundle("~/bundles/js_baseController").Include(
                    "~/assets/client/js/controller/baseController.js"));

             //không nhận link thư mục, chỉ nhận link asp.net ~
             bundles.Add(new StyleBundle("~/bundles/core").Include(
                    "~/assets/client/css/bootstrap.css",
                    "~/assets/client/css/bootstrap-social.css",
                    "~/assets/client/css/font-awesome.min.css",
                    "~/assets/client/css/bootstrap-theme.css",
                    "~/assets/client/css/jquery-ui.css",
                    "~/assets/client/css/style.css",
                    "~/assets/client/css/slider.css"));

            BundleTable.EnableOptimizations = true;
        }
    }
}
