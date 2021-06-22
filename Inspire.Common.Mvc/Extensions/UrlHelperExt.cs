namespace Inspire.Common.Mvc.Extensions
{
    using System;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Routing;

    using Inspire.Common.Mvc.Helpers;

    public static class UrlHelperExt
    {
        public static string AbsoluteAction(
            this UrlHelper url,
            string action,
            string controller,
            object routeValues = null)
        {
            var actionUrl = routeValues is RouteValueDictionary routeValueDictionary
                ? url.Action(action, controller, routeValueDictionary)
                : url.Action(action, controller, routeValues);

            var requestUrl = url.RequestContext.HttpContext.Request.Url;
            return $"{requestUrl.Scheme}://{requestUrl.Authority}{actionUrl}";
        }

        public static string Content(this UrlHelper urlHelper, string contentPath, bool toAbsolute = false)
        {
            var path = urlHelper.Content(contentPath);
            var url = new Uri(HttpContext.Current.Request.Url, path);

            return toAbsolute ? url.AbsoluteUri : path;
        }

        public static string DynamicAction(
            this UrlHelper url,
            string action,
            Type controller,
            object routeValues = null,
            bool absoluteUrl = false)
        {
            var route = CustomHelpers.AddAreaRouteValue(controller, routeValues);
            var controllerName = controller.GetControllerName();
            return absoluteUrl
                ? url.AbsoluteAction(action, controllerName, route)
                : url.Action(action, controllerName, route);
        }
    }
}