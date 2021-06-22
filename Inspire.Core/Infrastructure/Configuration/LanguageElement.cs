namespace Inspire.Core.Infrastructure.Configuration
{
    using System;
    using System.Configuration;
    using System.Globalization;

    public class LanguageElement : ConfigurationElement
    {
        [ConfigurationProperty("culture", IsRequired = true)]
        public CultureInfo Culture => new CultureInfo(this["culture"].ToString());

        [ConfigurationProperty("id", IsRequired = true)]
        public Guid Id => Guid.Parse(this["id"].ToString());

        [ConfigurationProperty("required")]
        public bool Required => this["required"] is bool && (bool)this["required"];
    }
}