namespace Inspire.Common.Mvc.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Routing;
    using System.Web.WebPages;

    public static class HtmlHelperExt
    {
        public static MvcHtmlString Script(this HtmlHelper htmlHelper, Func<object, HelperResult> template)
        {
            htmlHelper.ViewContext.HttpContext.Items["_script_" + Guid.NewGuid()] = template;
            return MvcHtmlString.Empty;
        }

        public static IHtmlString RenderPartialViewScripts(this HtmlHelper htmlHelper)
        {
            foreach (var key in htmlHelper.ViewContext.HttpContext.Items.Keys)
            {
                if (key.ToString().StartsWith("_script_"))
                {
                    if (htmlHelper.ViewContext.HttpContext.Items[key] is Func<object, HelperResult> template)
                    {
                        htmlHelper.ViewContext.Writer.Write(template(null));
                    }
                }
            }

            return MvcHtmlString.Empty;
        }

        public static RouteValueDictionary MergeHtmlAttributes(this HtmlHelper helper, params object[] htmlAttributes)
        {
            return htmlAttributes.Where(item => item != null)
                                 .Select(
                                     item => item is IDictionary<string, object> objects
                                         ? new RouteValueDictionary(objects)
                                         : new RouteValueDictionary(item))
                                 .Aggregate(
                                     new RouteValueDictionary(),
                                     (current, routeValue) => current.Extend(routeValue));
        }

        public static IDictionary<string, object> MergeHtmlAttributes(
            this HtmlHelper helper,
            object htmlAttributesObject,
            object defaultHtmlAttributesObject)
        {
            var concatKeys = new[] { "class" };

            var htmlAttributesDict = htmlAttributesObject as IDictionary<string, object>;
            var defaultHtmlAttributesDict = defaultHtmlAttributesObject as IDictionary<string, object>;

            var htmlAttributes = htmlAttributesDict != null
                ? new RouteValueDictionary(htmlAttributesDict)
                : HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributesObject);
            var defaultHtmlAttributes = defaultHtmlAttributesDict != null
                ? new RouteValueDictionary(defaultHtmlAttributesDict)
                : HtmlHelper.AnonymousObjectToHtmlAttributes(defaultHtmlAttributesObject);

            foreach (var item in htmlAttributes)
            {
                if (concatKeys.Contains(item.Key))
                {
                    defaultHtmlAttributes[item.Key] = defaultHtmlAttributes[item.Key] != null
                        ? $"{defaultHtmlAttributes[item.Key]} {item.Value}"
                        : item.Value;
                }
                else
                {
                    defaultHtmlAttributes[item.Key] = item.Value;
                }
            }

            return defaultHtmlAttributes;
        }
    }
}