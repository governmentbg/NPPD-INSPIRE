namespace Inspire.Table.Mvc.Utilities.Export
{
    using System;

    using Inspire.Table.Mvc.Utilities.Export.CSV;

    using Inspire.Table.Mvc.Utilities.Export.HTML;

    using Inspire.Table.Mvc.Utilities.Export.PDF;
    using Inspire.Table.Mvc.Utilities.Export.XLS;
    using Inspire.Table.Mvc.Utilities.Export.XLSX;
    using Inspire.Table.Mvc.Utilities.Export.Xml;

    public static class ExportFactory
    {
        public enum ExportType
        {
            Pdf = 1,
            Xlsx = 2,
            Xls = 3,
            Csv = 4,
            Xml = 5,
            Html = 6
        }

        public static TableExport CreateExport(ExportType type)
        {
            switch (type)
            {
                case ExportType.Pdf:
                    return new PdfTableExport();
                case ExportType.Csv:
                    return new CsvTableExport();
                case ExportType.Xls:
                    return new XlsTableExport();
                case ExportType.Xlsx:
                    return new XlsxTableExport();
                case ExportType.Xml:
                    return new XmlTableExport();
                case ExportType.Html:
                    return new HTMLTableExport();
                default:
                    throw new ArgumentOutOfRangeException("type", type, null);
            }
        }

        public static string GetFileExtensionByType(ExportType type)
        {
            switch (type)
            {
                case ExportType.Pdf:
                    return "pdf";
                case ExportType.Xlsx:
                    return "xlsx";
                case ExportType.Xls:
                    return "xls";
                case ExportType.Csv:
                    return "csv";
                case ExportType.Xml:
                    return "xml";
                case ExportType.Html:
                    return "html";
                default:
                    throw new ArgumentOutOfRangeException("type", type, null);
            }
        }

        public static string GetCssStyleByType(ExportType type)
        {
            switch (type)
            {
                case ExportType.Pdf:
                    return "bgr2";
                case ExportType.Csv:
                    return "bgr4";
                case ExportType.Xml:
                    return "bgr3";
                case ExportType.Html:
                    return "bgr0";
                default:
                    return "bgr1";
            }
        }
    }
}