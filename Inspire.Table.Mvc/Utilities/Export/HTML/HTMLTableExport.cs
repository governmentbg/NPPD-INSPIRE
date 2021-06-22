namespace Inspire.Table.Mvc.Utilities.Export.HTML
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Web.Mvc;

    using Inspire.Core.Infrastructure.Attribute;
    using Inspire.Core.Infrastructure.ResourceManager;
    using Inspire.Table.Mvc.Helpers;
    using Inspire.Utilities.Extensions;

    public class HTMLTableExport : TableExport
    {
        private readonly IResourceManager resource = DependencyResolver.Current.GetService<IResourceManager>();

        protected override byte[] Export<T>(IEnumerable<T> table, IEnumerable<KeyValuePair<PropertyInfo, TableOptionsAttribute>> tableColumns)
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("<html>");
            stringBuilder.AppendLine("<head><meta charset='UTF-8'></head>");
            var createDateTitle = resource?.Get("CreateDate");
            if (createDateTitle.IsNotNullOrEmpty())
            {
                stringBuilder.AppendLine($"<h3>{createDateTitle} {DateTime.Now}</h3>");
            }

            stringBuilder.AppendLine("<table>");
            stringBuilder.AppendLine("<thead>");
            stringBuilder.AppendLine("<tr>");

            foreach (var column in tableColumns.Select(item => item.Value))
            {
                stringBuilder.AppendLine("<th>");
                stringBuilder.Append(column.Title);
                stringBuilder.AppendLine("</th>");
            }

            stringBuilder.AppendLine("</thead>");
            stringBuilder.AppendLine("<tbody>");

            foreach (var obj in table)
            {
                stringBuilder.AppendLine("<tr>");

                foreach (var tableColumn in tableColumns)
                {
                    var val = tableColumn.Key.GetPropertyValue(obj, tableColumn.Value);
                    stringBuilder.AppendLine("<td>");
                    stringBuilder.Append(val);
                    stringBuilder.AppendLine("</td>");
                }

                stringBuilder.AppendLine("</tr>");
            }

            stringBuilder.AppendLine("</tbody>");
            stringBuilder.AppendLine("</table>");
            stringBuilder.AppendLine("</html>");

            return Encoding.UTF8.GetBytes(stringBuilder.ToString());
        }
    }
}