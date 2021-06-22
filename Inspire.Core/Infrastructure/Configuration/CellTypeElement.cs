namespace Inspire.Core.Infrastructure.Configuration
{
    using System;
    using System.Configuration;

    public class CellTypeElement : ConfigurationElement
    {
        [ConfigurationProperty("id", IsRequired = true)]
        public Guid Id => Guid.Parse(this["id"].ToString());

        [ConfigurationProperty("name", IsRequired = true)]
        public string Name => this["name"]?.ToString();

        [ConfigurationProperty("regex")]
        public string Regex => this["regex"]?.ToString();

        [ConfigurationProperty("format")]
        public string Format => this["format"]?.ToString();
    }
}