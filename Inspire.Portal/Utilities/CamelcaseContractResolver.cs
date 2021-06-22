namespace Inspire.Portal.Utilities
{
    using System.Linq;
    using Newtonsoft.Json.Serialization;

    public class CamelCaseContractResolver : DefaultContractResolver
    {
        protected override string ResolvePropertyName(string propertyName)
        {
            return string.Join(
                " ",
                propertyName
                    .Split()
                    .Select(i => char.ToLower(i[0]) + i.Substring(1)));
        }
    }
}