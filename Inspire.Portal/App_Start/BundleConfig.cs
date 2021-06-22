namespace Inspire.Portal
{
    using System.Web;
    using System.Web.Optimization;

    using Inspire.Common.Mvc.Bundle;

    using Microsoft.Ajax.Utilities;

    public class BundleConfig
    {
        private static readonly string JqueryVersion = "3.5.1";

        private static readonly string KendoVersion = "2021.1.119";

        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.IgnoreList.Clear();
            BundleTable.Bundles.ResetAll();
            BundleTable.EnableOptimizations = !HttpContext.Current.IsDebuggingEnabled;

            RegisterContentBundles(bundles);
            RegisterScriptBundles(bundles);

            bundles.ForEach(b => b.Orderer = new SyncBundleOrder());
        }

        private static void RegisterContentBundles(BundleCollection bundles)
        {
            bundles.Add(
                new CustomStyleBundle("~/Bundles/Styles/styles")
                    .Include("~/Content/normalize.css")
                    .Include("~/Content/font-awesome.min.css")
                    .Include("~/Content/magnific-popup.css")
                    .Include("~/Content/perfect-scrollbar.css")
                    .Include("~/Content/owl.carousel.min.css")
                    .Include($"~/Content/kendo/{KendoVersion}/kendo.common.min.css")
                    .Include($"~/Content/kendo/{KendoVersion}/kendo.office365.min.css")
                    .Include($"~/Content/kendo/{KendoVersion}/kendo.office365.mobile.min.css")
                    .Include("~/Content/kendo.custom.css")
                    .Include("~/Content/style.css")
                    .Include("~/Content/style.custom.css"));
        }

        private static void RegisterScriptBundles(BundleCollection bundles)
        {
            bundles.Add(
                new CustomScriptBundle("~/Bundles/Scripts/common")
                    .Include(
                        $"~/Scripts/jquery-{JqueryVersion}.min.js",
                        "~/Scripts/jquery.unobtrusive-ajax.min.js",
                        "~/Scripts/jquery.validate.min.js",
                        "~/Scripts/jquery.validate.unobtrusive.min.js",
                        "~/Scripts/jquery.dirtyforms.min.js",
                        "~/Scripts/globalize.0.1.3/globalize.js",
                        "~/Scripts/validate.globalize.js",
                        $"~/Scripts/kendo/{KendoVersion}/jszip.min.js",
                        $"~/Scripts/kendo/{KendoVersion}/kendo.all.min.js",
                        $"~/Scripts/kendo/{KendoVersion}/kendo.aspnetmvc.min.js",
                        $"~/Scripts/kendo/{KendoVersion}/kendo.timezones.min.js",
                        "~/Scripts/kendo.modernizr.custom.js",
                        "~/Scripts/jquery.magnific-popup.min.js"));

            foreach (var cultureName in Global.AllowCultures)
            {
                bundles.Add(
                    new CustomScriptBundle($"~/Bundles/Scripts/culture_{cultureName}")
                        .Include(
                            $"~/Scripts/globalize.0.1.3/cultures/globalize.culture.{cultureName}.js",
                            $"~/Scripts/validate.messages.{cultureName}.js",
                            $"~/Scripts/kendo/{KendoVersion}/cultures/kendo.culture.{cultureName}.min.js",
                            $"~/Scripts/kendo/{KendoVersion}/messages/kendo.messages.{cultureName}.min.js",
                            "~/Scripts/Utilities/Core.js",
                            "~/Scripts/gdpr.js",
                            "~/Scripts/Utilities/Resources.js"));
            }

            bundles.Add(
                new CustomScriptBundle("~/Bundles/Scripts/layout")
                    .Include(
                        "~/Scripts/owl.carousel.min.js",
                        "~/Scripts/jquery.touchSwipe.min.js",
                        "~/Scripts/perfect-scrollbar.min.js",
                        "~/Scripts/core.js",
                        "~/Scripts/Utilities/Notification.js",
                        "~/Scripts/Utilities/SearchTable.js",
                        "~/Scripts/cachesvg.js",
                        "~/Areas/Admin/Scripts/Controllers/History/History.js"));
        }
    }
}