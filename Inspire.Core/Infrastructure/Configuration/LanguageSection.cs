namespace Inspire.Core.Infrastructure.Configuration
{
    using System.Configuration;

    public class LanguageSection : ConfigurationSection
    {
        public static LanguageSection Instance => ConfigurationManager.GetSection("languages") as LanguageSection;

        [ConfigurationProperty("", IsDefaultCollection = true)]
        public LanguageCollection Languages => this[string.Empty] as LanguageCollection;
    }
}