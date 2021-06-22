namespace Inspire.Table.Mvc.Utilities.Export
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using Inspire.Core.Infrastructure.Attribute;

    using Inspire.Table.Mvc.Helpers;
    using Inspire.Utilities.Extensions;

    public abstract class TableExport
    {
        public virtual byte[] Export<T>(IEnumerable<T> data, string firstLevelName = null, string secondLevelName = null)
            where T : class
        {
            if (data.IsNullOrEmpty())
            {
                return null;
            }

            var tableColumns = SearchTableHelpers.GetPropertiesTableData(data, firstLevelName, secondLevelName)
                                                 .Where(item => (!item.Item1.IsHidden || item.Item1.ShouldExport) && item.Item1.Title.IsNotNullOrEmpty())
                                                 .Select(item => new KeyValuePair<PropertyInfo, TableOptionsAttribute>(item.Item3, item.Item1));
            if (tableColumns.IsNullOrEmpty())
            {
                return null;
            }

            return Export(data, tableColumns);
        }

        protected abstract byte[] Export<T>(IEnumerable<T> table, IEnumerable<KeyValuePair<PropertyInfo, TableOptionsAttribute>> tableColumns)
            where T : class;
    }
}