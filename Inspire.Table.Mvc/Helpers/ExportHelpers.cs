namespace Inspire.Table.Mvc.Helpers
{
    using System;
    using System.Text;
    using System.Web.Mvc;

    using Inspire.Common.Mvc.Helpers;

    using Inspire.Table.Mvc.Utilities.Export;

    public static class ExportHelpers
    {
        public static MvcHtmlString ExportLinks(this HtmlHelper helper)
        {
            var stringBuilder = new StringBuilder();
            foreach (var type in ExportConfiguration.ExportTypes)
            {
                if (!Enum.TryParse(type, true, out ExportFactory.ExportType exportType))
                {
                    continue;
                }

                var html = string.Format(
                    "<a class='option {3} export-js' title='{0}' href='#' data-controller='{1}' data-action='Export' data-area='{2}' data-type='{0}'>" +
                    "<svg class='icon currentColor1'><use xlink:href='#dl'></use></svg>{0}</a>" +
                    "</a>",
                    type.ToUpper(),
                    helper.ViewContext.Controller.GetControllerName(),
                    CustomHelpers.GetControllerArea(helper.ViewContext.Controller),
                    ExportFactory.GetCssStyleByType(exportType));
                stringBuilder.Append(html);
            }

            return new MvcHtmlString(stringBuilder.ToString());
        }
    }
}