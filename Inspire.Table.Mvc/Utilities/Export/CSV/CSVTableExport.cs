namespace Inspire.Table.Mvc.Utilities.Export.CSV
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using CsvHelper;
    using Inspire.Core.Infrastructure.Attribute;
    using Inspire.Table.Mvc.Helpers;
    using Inspire.Utilities.Extensions;

    public class CsvTableExport : TableExport
    {
        protected override byte[] Export<T>(
            IEnumerable<T> table,
            IEnumerable<KeyValuePair<PropertyInfo, TableOptionsAttribute>> tableColumns)
        {
            using (var memoryStream = new MemoryStream())
            {
                using (var streamWriter = new StreamWriter(memoryStream, Encoding.UTF8))
                {
                    using (var csvWriter = new CsvWriter(streamWriter, CultureInfo.InstalledUICulture))
                    {
                        csvWriter.Configuration.Delimiter = ExportConfiguration.CsvDelimeter;

                        foreach (var header in tableColumns.Select(item => item.Value))
                        {
                            csvWriter.WriteField(header.Title);
                        }

                        csvWriter.NextRecord();

                        foreach (var obj in table)
                        {
                            foreach (var tableColumn in tableColumns)
                            {
                                var val = tableColumn.Key.GetPropertyValue(obj, tableColumn.Value);
                                csvWriter.WriteField(
                                    val != null
                                        ? string.Format(
                                            tableColumn.Value.Format.IsNotNullOrEmpty()
                                                ? "{0:" + tableColumn.Value.Format + "}"
                                                : "{0}",
                                            val)
                                        : string.Empty);
                            }

                            csvWriter.NextRecord();
                        }
                    }
                }

                return memoryStream.ToArray();
            }
        }
    }
}