namespace Inspire.Common.Mvc.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Routing;

    public static class RouteValueDictionaryExt
    {
        public static RouteValueDictionary Extend(
            this RouteValueDictionary destination,
            IEnumerable<KeyValuePair<string, object>> source)
        {
            foreach (var srcElement in source.ToList())
            {
                if (destination.ContainsKey(srcElement.Key))
                {
                    if (!srcElement.Key.Equals("data-val", StringComparison.InvariantCultureIgnoreCase))
                    {
                        destination[srcElement.Key] += " " + srcElement.Value;
                    }
                }
                else
                {
                    destination[srcElement.Key] = srcElement.Value;
                }
            }

            return destination;
        }
    }
}