namespace Inspire.Table.Mvc.Utilities.Export.XLSX
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;
    using System.Web.Mvc;

    using Inspire.Core.Infrastructure.Attribute;
    using Inspire.Core.Infrastructure.ResourceManager;
    using Inspire.Table.Mvc.Helpers;
    using Inspire.Utilities.Extensions;

    using OfficeOpenXml;
    using OfficeOpenXml.Style;

    public class XlsxTableExport : TableExport
    {
        private readonly IResourceManager resource = DependencyResolver.Current.GetService<IResourceManager>();

        protected override byte[] Export<T>(
            IEnumerable<T> table,
            IEnumerable<KeyValuePair<PropertyInfo, TableOptionsAttribute>> tableColumns)
        {
            using (var p = new ExcelPackage())
            {
                var tableName = "export";
                p.Workbook.Worksheets.Add(tableName);
                var ws = p.Workbook.Worksheets[1];
                ws.Name = tableName;

                var rowIndex = 1;
                var colIndex = 1;

                var keyValuePairs = tableColumns as KeyValuePair<PropertyInfo, TableOptionsAttribute>[] ??
                                    tableColumns.ToArray();
                var headerCell = ws.Cells[rowIndex, colIndex, rowIndex, keyValuePairs.Length];
                headerCell.Merge = true;
                headerCell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                headerCell.Value = $"{resource?.Get("CreateDate")} {DateTime.Now.ToString(CultureInfo.CurrentUICulture)}".Trim();
                rowIndex++;

                foreach (var tableColumn in keyValuePairs.Select(item => item.Value))
                {
                    var cell = ws.Cells[rowIndex, colIndex];

                    cell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    cell.Style.Fill.BackgroundColor.SetColor(ExportConfiguration.HeaderBackgroundColor);
                    cell.Style.Font.Color.SetColor(ExportConfiguration.HeaderFontColor);

                    cell.Value = tableColumn.Title;
                    colIndex++;
                }

                foreach (var obj in table)
                {
                    colIndex = 1;
                    rowIndex++;

                    foreach (var tableColumn in keyValuePairs)
                    {
                        var val = tableColumn.Key.GetPropertyValue(obj, tableColumn.Value);
                        if (val != null)
                        {
                            var cell = ws.Cells[rowIndex, colIndex];
                            if (val is double || val is short || val is int || val is long)
                            {
                                cell.Value = val;
                            }
                            else
                            {
                                cell.Value = string.Format(
                                    tableColumn.Value.Format.IsNotNullOrEmpty()
                                        ? "{0:" + tableColumn.Value.Format + "}"
                                        : "{0}",
                                    val);
                            }
                        }

                        colIndex++;
                    }
                }

                return p.GetAsByteArray();
            }
        }
    }
}