namespace Inspire.Core.Infrastructure.Configuration
{
    using System.Configuration;

    [ConfigurationCollection(
        typeof(UserElement),
        AddItemName = "user",
        CollectionType = ConfigurationElementCollectionType.BasicMap)]
    public class UserCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new UserElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((UserElement)element).UserName;
        }
    }
}