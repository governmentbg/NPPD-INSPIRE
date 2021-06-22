namespace Inspire.Common.Mvc.Extensions
{
    using System.Diagnostics;
    using System.Reflection;
    using System.Web;
    using System.Web.WebPages;

    public static class WebViewPageExt
    {
        public static FileVersionInfo GetApplicationVersion(this WebPageRenderingBase webViewPage)
        {
            var version = webViewPage.Session != null
                ? webViewPage.Context.Application["Version"] as FileVersionInfo
                : null;
            if (version == null)
            {
                var executingAssembly = GetWebEntryAssembly();
                version = FileVersionInfo.GetVersionInfo(executingAssembly.Location);
                if (webViewPage.Session != null)
                {
                    webViewPage.Context.Application.Lock();
                    webViewPage.Context.Application["Version"] = version;
                    webViewPage.Context.Application.UnLock();
                }
            }

            return version;
        }

        private static Assembly GetWebEntryAssembly()
        {
            if (HttpContext.Current == null || HttpContext.Current.ApplicationInstance == null)
            {
                return null;
            }

            var type = HttpContext.Current.ApplicationInstance.GetType();
            while (type != null && type.Namespace == "ASP")
            {
                type = type.BaseType;
            }

            return type == null ? null : type.Assembly;
        }
    }
}