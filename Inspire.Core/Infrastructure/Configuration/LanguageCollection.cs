namespace Inspire.Core.Infrastructure.Configuration
{
    using System.Configuration;

    [ConfigurationCollection(
        typeof(LanguageElement),
        AddItemName = "language",
        CollectionType = ConfigurationElementCollectionType.BasicMap)]
    public class LanguageCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new LanguageElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((LanguageElement)element).Culture;
        }
    }
}