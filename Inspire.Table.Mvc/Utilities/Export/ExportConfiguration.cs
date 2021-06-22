namespace Inspire.Table.Mvc.Utilities.Export
{
    using System.Configuration;
    using System.Drawing;

    public static class ExportConfiguration
    {
        public static string FontPath => ConfigurationManager.AppSettings.Get("ExportFontPath");

        public static int FontSize => int.Parse(ConfigurationManager.AppSettings.Get("ExportFontSize"));

        public static Color HeaderFontColor => ColorTranslator.FromHtml(ConfigurationManager.AppSettings.Get("ExportHeaderFontColor"));

        public static Color HeaderBackgroundColor => ColorTranslator.FromHtml(ConfigurationManager.AppSettings.Get("ExportHeaderBackgroundColor"));

        public static string CsvDelimeter => ConfigurationManager.AppSettings.Get("ExportCsvDelimeter");

        public static string[] ExportTypes => ConfigurationManager.AppSettings.Get("ExportTypes").Split(',');
    }
}