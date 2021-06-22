namespace Inspire.Common.Mvc.Filters
{
    using System;
    using System.Linq;
    using System.Net.Http;
    using System.Web.Helpers;
    using System.Web.Mvc;

    public class AntiForgeryTokenFilter : IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationContext filterContext)
        {
            if (IsHttpPostRequest(filterContext) && !SkipCsrfCheck(filterContext))
            {
                var httpContext = filterContext.HttpContext;
                if (httpContext.Request.IsAjaxRequest()
                    && httpContext.Request.Headers != null
                    && httpContext.Request.Headers.Keys.Cast<string>().Any(
                        key => key.Equals(
                            "__RequestVerificationToken",
                            StringComparison.InvariantCultureIgnoreCase)))
                {
                    var cookie = httpContext.Request.Cookies[AntiForgeryConfig.CookieName];
                    AntiForgery.Validate(
                        cookie != null ? cookie.Value : null,
                        httpContext.Request.Headers["__RequestVerificationToken"]);
                }
                else
                {
                    AntiForgery.Validate();
                }
            }
        }

        private static bool IsHttpPostRequest(AuthorizationContext filterContext)
        {
            return filterContext.RequestContext.HttpContext.Request.HttpMethod == HttpMethod.Post.ToString();
        }

        private static bool SkipCsrfCheck(AuthorizationContext filterContext)
        {
            return filterContext.ActionDescriptor.GetCustomAttributes(typeof(SkipCsrfCheckAttribute), false).Any()

                   // a post request that does not require authentication is not "dangerous" - usually the POST verb
                   // is put on AllowAnonymous actions to allow posting bigger forms ( e.g Registration);
                   || filterContext.ActionDescriptor.GetCustomAttributes(typeof(AllowAnonymousAttribute), false).Any();
        }
    }
}