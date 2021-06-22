namespace Inspire.Table.Mvc.Utilities.Export.XLS
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Web.Mvc;

    using Inspire.Core.Infrastructure.Attribute;
    using Inspire.Core.Infrastructure.ResourceManager;
    using Inspire.Table.Mvc.Helpers;
    using Inspire.Utilities.Extensions;

    using NPOI.HSSF.UserModel;
    using NPOI.HSSF.Util;
    using NPOI.SS.UserModel;
    using NPOI.SS.Util;

    public class XlsTableExport : TableExport
    {
        private readonly IResourceManager resource = DependencyResolver.Current.GetService<IResourceManager>();

        protected override byte[] Export<T>(IEnumerable<T> table, IEnumerable<KeyValuePair<PropertyInfo, TableOptionsAttribute>> tableColumns)
        {
            using (var ms = new MemoryStream())
            {
                var workbook = new HSSFWorkbook();
                var sheet = workbook.CreateSheet();

                var rowIndex = 0;
                var colIndex = 0;
                var colCount = tableColumns.Count();

                var headerRow = sheet.CreateRow(0);
                var headerRowCell = headerRow.CreateCell(colIndex);
                headerRowCell.SetCellValue($"{resource?.Get("CreateDate")} {DateTime.Now.ToString(CultureInfo.CurrentUICulture)}".Trim());
                sheet.AddMergedRegion(new CellRangeAddress(rowIndex, rowIndex, colIndex, colCount - 1));

                var style = workbook.CreateCellStyle();
                style.Alignment = HorizontalAlignment.Right;
                headerRowCell.CellStyle = style;

                var tableHeaderRow = sheet.CreateRow(1);

                // Creating styles
                var headerCellStyle = workbook.CreateCellStyle();

                var palette = workbook.GetCustomPalette();
                palette.SetColorAtIndex(HSSFColor.Lavender.Index, ExportConfiguration.HeaderBackgroundColor.R, ExportConfiguration.HeaderBackgroundColor.G, ExportConfiguration.HeaderBackgroundColor.B);
                headerCellStyle.FillBackgroundColor = HSSFColor.Lavender.Index;
                headerCellStyle.FillPattern = FillPattern.SolidForeground;

                var doubleCellStyle = workbook.CreateCellStyle();
                doubleCellStyle.DataFormat = HSSFDataFormat.GetBuiltinFormat("0.00000");

                var intCellStyle = workbook.CreateCellStyle();
                intCellStyle.DataFormat = HSSFDataFormat.GetBuiltinFormat("0");

                // Creates the header cells
                foreach (var tableColumn in tableColumns.Select(item => item.Value))
                {
                    var headerCell = tableHeaderRow.CreateCell(colIndex);
                    headerCell.SetCellValue(tableColumn.Title);

                    // Add Style to Cell
                    headerCell.CellStyle = headerCellStyle;

                    colIndex++;
                }

                if (table != null)
                {
                    // Writes the cells
                    foreach (var obj in table)
                    {
                        colIndex = 0;
                        rowIndex++;
                        var dataRow = sheet.CreateRow(rowIndex);

                        foreach (var tableColumn in tableColumns)
                        {
                            var val = tableColumn.Key.GetPropertyValue(obj, tableColumn.Value);
                            if (val != null)
                            {
                                var cell = dataRow.CreateCell(colIndex);

                                if (val is double)
                                {
                                    cell.SetCellType(CellType.Numeric);

                                    cell.SetCellValue((double)val);
                                    cell.CellStyle = doubleCellStyle;
                                }
                                else if (val is short || val is int || val is long)
                                {
                                    cell.SetCellType(CellType.Numeric);

                                    cell.SetCellValue(Convert.ToInt64(val));
                                    cell.CellStyle = intCellStyle;
                                }
                                else
                                {
                                    cell.SetCellValue(string.Format(tableColumn.Value.Format.IsNotNullOrEmpty() ? "{0:" + tableColumn.Value.Format + "}" : "{0}", val));
                                }
                            }

                            colIndex++;
                        }
                    }
                }

                // Auto size the columns
                for (var i = 0; i < colCount; i++)
                {
                    sheet.AutoSizeColumn(i);
                }

                // Create the stream
                workbook.Write(ms);

                return ms.ToArray();
            }
        }
    }
}