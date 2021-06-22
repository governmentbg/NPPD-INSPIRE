namespace Inspire.Common.Mvc.Helpers
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Mvc.Html;
    using System.Web.Optimization;
    using System.Web.Routing;

    using Inspire.Common.Mvc.Bundle;
    using Inspire.Common.Mvc.Enums;
    using Inspire.Utilities.Encryption;
    using Inspire.Utilities.Extensions;

    public static class CustomHelpers
    {
        private const string Аreas = "Areas";

        public static string GetDefaultUrlByCulture(this HtmlHelper helper)
        {
            return
                $"{VirtualPathUtility.ToAbsolute("~/")}{Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName.ToLower()}";
        }

        public static IHtmlString DynamicBundle(this HtmlHelper helper, params string[] files)
        {
            if (files.IsNullOrEmpty())
            {
                return null;
            }

            var bundleName = EncryptionTools.Hash(string.Join(";", files));
            var isJs = files.First().EndsWith(".js", StringComparison.InvariantCultureIgnoreCase);
            if (isJs)
            {
                bundleName = bundleName.StartsWith("~/") ? bundleName : $"~/Bundles/Scripts/{bundleName}";
            }
            else
            {
                bundleName = bundleName.StartsWith("~/") ? bundleName : $"~/Bundles/Styles/{bundleName}";
            }

            if (BundleTable.Bundles.All(item => item.Path != bundleName)
                && files.IsNotNullOrEmpty())
            {
                var filePaths = files
                                .Where(file => File.Exists(helper.ViewContext.HttpContext.Server.MapPath(file)))
                                .ToArray();
                if (filePaths.IsNotNullOrEmpty())
                {
                    var newBundle = isJs
                        ? new CustomScriptBundle(bundleName) as Bundle
                        : new CustomStyleBundle(bundleName);
                    newBundle.Include(filePaths);
                    newBundle.Orderer = new SyncBundleOrder();

                    BundleTable.Bundles.Add(newBundle);
                }
            }

            var bundle = BundleTable.Bundles.SingleOrDefault(item => item.Path == bundleName);
            if (bundle == null)
            {
                return null;
            }

            return isJs
                ? Scripts.Render(bundle.Path)
                : Styles.Render(bundle.Path);
        }

        public static HtmlString RenderNotification(this HtmlHelper htmlHelper)
        {
            var messages = new StringBuilder();
            foreach (var messageType in Enum.GetNames(typeof(MessageType)))
            {
                var message = htmlHelper.ViewContext.ViewData.ContainsKey(messageType)
                    ? htmlHelper.ViewContext.ViewData[messageType]
                    : htmlHelper.ViewContext.TempData.ContainsKey(messageType)
                        ? htmlHelper.ViewContext.TempData[messageType]
                        : null;

                if (message != null)
                {
                    message = message.ToString().Replace("'", " ");
                    messages.Append(
                        string.Format(
                            "notification.displayMessage('{0}', '{1}');",
                            message,
                            messageType.ToLowerInvariant()));
                }
            }

            return MvcHtmlString.Create(
                messages.Length > 0
                    ? @"<script>$(document).ready(function () {" + messages + "  }); </script>"
                    : string.Empty);
        }

        public static MvcHtmlString DynamicActionLink(
            this HtmlHelper url,
            string text,
            string action,
            Type controller,
            object routeValues = null,
            object htmlAttributes = null)
        {
            var route = AddAreaRouteValue(controller, routeValues);
            return url.ActionLink(
                text,
                action,
                GetControllerName(controller),
                route,
                htmlAttributes != null ? new RouteValueDictionary(htmlAttributes) : null);
        }

        public static MvcHtmlString DynamicAction(
            this HtmlHelper url,
            string action,
            Type controller,
            object routeValues = null)
        {
            var route = AddAreaRouteValue(controller, routeValues);

            if (controller.BaseType != null && !route.ContainsKey("controller"))
            {
                route.Add("controller", url.ViewContext.Controller.GetType());
            }

            return url.Action(action, GetControllerName(controller), route);
        }

        public static void RenderDynamicAction(
            this HtmlHelper url,
            string action,
            Type controller,
            object routeValues = null)
        {
            var route = AddAreaRouteValue(controller, routeValues);

            if (controller.BaseType != null && !route.ContainsKey("controller"))
            {
                route.Add("controller", url.ViewContext.Controller.GetType());
            }

            url.RenderAction(action, GetControllerName(controller), route);
        }

        public static string GetControllerArea(IController controller)
        {
            return GetControllerArea(controller.GetType());
        }

        public static string GetControllerArea(Type controllerType)
        {
            var ns = controllerType.Namespace;
            if (ns != null && ns.Contains(Аreas))
            {
                var firstDotIndex = ns.IndexOf(Аreas, StringComparison.Ordinal) + Аreas.Length + 1;
                var secondDotIndex = ns.IndexOf('.', firstDotIndex + 1);
                return ns.Substring(firstDotIndex, secondDotIndex - firstDotIndex);
            }

            return null;
        }

        public static string GetControllerName(this IController controller)
        {
            return GetControllerName(controller.GetType());
        }

        public static string GetControllerName(this Type controller)
        {
            return controller.Name.Replace("Controller", string.Empty);
        }

        public static RouteValueDictionary AddAreaRouteValue(Type controller, object routeValues)
        {
            var route = (routeValues is RouteValueDictionary
                ? (RouteValueDictionary)routeValues
                : routeValues != null
                    ? new RouteValueDictionary(routeValues)
                    : null) ?? new RouteValueDictionary();
            if (!route.Keys.Any(key => key.Equals("area", StringComparison.InvariantCultureIgnoreCase)))
            {
                var area = GetControllerArea(controller);
                route.Add("area", area);
            }

            // http://stackoverflow.com/questions/20331054/asp-net-mvc-html-action-html-actionlink-and-culture-specific-datetime
            if (route.IsNotNullOrEmpty())
            {
                for (var i = 0; i < route.Count; i++)
                {
                    var elem = route.ElementAt(i);
                    if (elem.Value is DateTime)
                    {
                        route[elem.Key] = elem.Value.ToString();
                    }
                }
            }

            return route;
        }
    }
}