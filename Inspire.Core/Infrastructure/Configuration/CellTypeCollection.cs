namespace Inspire.Core.Infrastructure.Configuration
{
    using System.Configuration;

    [ConfigurationCollection(
        typeof(CellTypeElement),
        AddItemName = "cellType",
        CollectionType = ConfigurationElementCollectionType.BasicMap)]
    public class CellTypeCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new CellTypeElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((CellTypeElement)element).Name;
        }
    }
}