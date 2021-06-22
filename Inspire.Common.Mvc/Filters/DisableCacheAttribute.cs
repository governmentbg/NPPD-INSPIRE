namespace Inspire.Common.Mvc.Filters
{
    using System;
    using System.Web;
    using System.Web.Mvc;

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class DisableCacheAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext.HttpContext.Request.IsSecureConnection &&
                string.Equals(
                    filterContext.HttpContext.Request.Browser.Browser,
                    "IE",
                    StringComparison.OrdinalIgnoreCase) &&
                filterContext.HttpContext.Request.Browser.MajorVersion < 9)
            {
                filterContext.HttpContext.Response.ClearHeaders();
                filterContext.HttpContext.Response.AddHeader("cache-control", "no-store, no-cache, must-revalidate");
            }
            else
            {
                filterContext.HttpContext.Response.Cache.SetNoStore();
                filterContext.HttpContext.Response.Cache.SetCacheability(HttpCacheability.NoCache);
                filterContext.HttpContext.Response.Cache.SetRevalidation(HttpCacheRevalidation.AllCaches);
                filterContext.HttpContext.Response.Cache.SetExpires(DateTime.UtcNow.AddDays(-1));
                filterContext.HttpContext.Response.Cache.SetValidUntilExpires(false);
                filterContext.HttpContext.Response.AddHeader("X-UA-Compatible", "IE=Edge");
            }
        }
    }
}