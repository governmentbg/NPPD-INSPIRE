namespace Inspire.Portal
{
    using System.Web.Mvc;

    using Inspire.Common.Mvc.Filters;
    using Inspire.Common.Mvc.Filters.CustomAuthorize;
    using Inspire.Portal.Infrastructure.Attribute;

    using CustomErrorHandler = Inspire.Portal.Filters.CustomErrorHandler;

    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new CustomErrorHandler());
            filters.Add(new DisableCacheAttribute());
            filters.Add(new MessagesFilter());
            filters.Add(new CustomAuthorizeAttribute());

            // Security filters
            filters.Add(new AntiForgeryTokenFilter());

            // Localization filter
            filters.Add(new LocalizationFilterAttribute());
        }
    }
}