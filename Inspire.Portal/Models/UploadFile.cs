namespace Inspire.Portal.Models
{
    using System.Collections.Generic;

    using Kendo.Mvc.Infrastructure;

    public class UploadFile : Kendo.Mvc.UI.UploadFile
    {
        private const long DefaultSize = 0L;

        private readonly string defaultExtension = string.Empty;

        private readonly string defaultName = string.Empty;

        public string Url { get; set; }

        public string Description { get; set; }

        protected override void Serialize(IDictionary<string, object> json)
        {
            FluentDictionary.For(json)
                            .Add("name", Name, defaultName)
                            .Add("size", Size, DefaultSize)
                            .Add("extension", Extension, defaultExtension)
                            .Add("url", Url, string.Empty)
                            .Add("description", Description, string.Empty);
        }
    }
}