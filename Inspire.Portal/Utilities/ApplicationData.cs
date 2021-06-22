namespace Inspire.Portal.Utilities
{
    using System.IO;
    using System.Threading;
    using System.Web;

    using Newtonsoft.Json.Linq;

    public static class ApplicationData
    {
        private static JObject geonetworkSearchSuggestConfig;

        public static JObject GeonetworkInspireThemes =>
            JObject.Parse(ReadGeonetworkConfigFile("InspireThemes.json"));

        internal static JObject GeonetworkSearchSuggestConfig
        {
            get
            {
                if (geonetworkSearchSuggestConfig == null)
                {
                    var autocompleteConfig = JObject.Parse(ReadGeonetworkConfigFile("SearchAutocompleteConfig.json"));

                    var functionScore = JObject.Parse(ReadGeonetworkConfigFile("SearchScoreConfig.json"));
                    functionScore.Merge(
                        autocompleteConfig,
                        new JsonMergeSettings { MergeArrayHandling = MergeArrayHandling.Union });

                    geonetworkSearchSuggestConfig =
                        new JObject { { "query", new JObject { { "function_score", functionScore } } } };
                }

                return geonetworkSearchSuggestConfig;
            }
        }

        internal static JObject GeonetworkSearchLeadThemesConfig =>
            JObject.Parse(ReadGeonetworkConfigFile("SearchLeadThemesConfig.json"));

        public static string Translate(this JToken data)
        {
            return (data[$"lang{Thread.CurrentThread.CurrentCulture.ThreeLetterISOLanguageName.ToLower()}"] ?? data["default"])?.Value<string>();
        }

        internal static string ReadGeonetworkConfigFile(string fileName)
        {
            return File.ReadAllText(
                Path.Combine(HttpContext.Current.Request.PhysicalApplicationPath, $"App_Data/Geonetwork/{fileName}"));
        }
    }
}