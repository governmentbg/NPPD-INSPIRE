namespace Inspire.Common.Mvc.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;

    using Inspire.Core.Infrastructure.ResourceManager;
    using Inspire.Model.Nomenclature;

    public static class ListExt
    {
        private static readonly IResourceManager Resource = DependencyResolver.Current.GetService<IResourceManager>();

        public static List<TModel> AddDefaultValue<TModel>(this List<TModel> list, bool chooseType = true)
            where TModel : Nomenclature, new()
        {
            var name = chooseType
                ? Resource?.Get("Choose") ?? "Choose"
                : Resource?.Get("All") ?? "All";

            if (list?.Any(i => i.Name?.Equals(name, StringComparison.InvariantCultureIgnoreCase) == true) == true)
            {
                return list;
            }

            var item = Activator.CreateInstance<TModel>();
            item.Name = name;
            list?.Insert(0, item);

            return list;
        }

        public static List<KeyValuePair<string, string>> AddDefaultValue(this List<KeyValuePair<string, string>> list, bool chooseType = true)
        {
            var name = chooseType
                ? Resource?.Get("Choose") ?? "Choose"
                : Resource?.Get("All") ?? "All";

            if (list?.Any(i => i.Value?.Equals(name, StringComparison.InvariantCultureIgnoreCase) == true) == true)
            {
                return list;
            }

            list?.Insert(0, new KeyValuePair<string, string>(null, name));

            return list;
        }

        public static List<SelectListItem> AddDefaultValue(this List<SelectListItem> list, bool chooseType = true)
        {
            var name = chooseType
                ? Resource?.Get("Choose") ?? "Choose"
                : Resource?.Get("All") ?? "All";

            if (list?.Any(i => i.Text?.Equals(name, StringComparison.InvariantCultureIgnoreCase) == true) == true)
            {
                return list;
            }

            list?.Insert(0, new SelectListItem { Text = name });

            return list;
        }

        public static List<SelectListItem> AsSelectItemList<TModel>(this IEnumerable<TModel> list, Func<TModel, bool> predicate = null)
            where TModel : Nomenclature, new()
        {
            return list
                   .Select(
                       i => new SelectListItem
                       {
                           Selected = predicate != null && predicate(i),
                           Value = i.Id?.ToString() ?? string.Empty,
                           Text = i.Name
                       })
                   .ToList();
        }
    }
}