namespace Inspire.Table.Mvc.Helpers
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Reflection;
    using System.Threading;
    using System.Web;

    using Inspire.Core.Infrastructure.Attribute;

    using Inspire.Utilities.Extensions;

    using iTextSharp.text.html;

    public static class SearchTableHelpers
    {
        public static Dictionary<Type, List<Tuple<TableOptionsAttribute, Type, PropertyInfo>>> TableDataCache
        {
            get
            {
                var user = HttpContext.Current.User != null ? "User" : string.Empty;
                var keyByCulture = $"TableDataCache_{Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName}_{user}";
                var cache = HttpContext.Current.Session[keyByCulture] as Dictionary<Type, List<Tuple<TableOptionsAttribute, Type, PropertyInfo>>>;
                if (cache == null)
                {
                    HttpContext.Current.Session[keyByCulture] = new Dictionary<Type, List<Tuple<TableOptionsAttribute, Type, PropertyInfo>>>();
                }

                return (Dictionary<Type, List<Tuple<TableOptionsAttribute, Type, PropertyInfo>>>)HttpContext.Current.Session[keyByCulture];
            }
        }

        public static List<Tuple<TableOptionsAttribute, Type, PropertyInfo>> GetPropertiesTableData<T>(IEnumerable<T> list, string firstLevelName = null, string secondLevelName = null)
            where T : class
        {
            var type = typeof(T);
            if (TableDataCache.ContainsKey(type))
            {
                return TableDataCache[type];
            }

            var hasDynamicColumns = false;
            var result = new List<Tuple<TableOptionsAttribute, Type, PropertyInfo>>();
            foreach (var propertyInfo in type.GetProperties())
            {
                if (propertyInfo.Name.IsNullOrEmpty())
                {
                    continue;
                }

                TableOptionsAttribute tableOptionsAttribute = null;
                var tableAttribute = propertyInfo.GetCustomAttributes(typeof(TableOptionsAttribute), false);
                if (tableAttribute.Length > 0)
                {
                    tableOptionsAttribute = tableAttribute[0] as TableOptionsAttribute;
                }

                var displayNameAttributes = propertyInfo.GetCustomAttributes(typeof(DisplayNameAttribute), false);
                if (displayNameAttributes.Length > 0)
                {
                    tableOptionsAttribute = tableOptionsAttribute ?? new TableOptionsAttribute();
                    tableOptionsAttribute.Title = ((DisplayNameAttribute)displayNameAttributes[0]).DisplayName;
                }

                var displayAttributes = propertyInfo.GetCustomAttributes(typeof(DisplayAttribute), false);
                if (displayAttributes.Length > 0)
                {
                    tableOptionsAttribute = tableOptionsAttribute ?? new TableOptionsAttribute();

                    // Get display attribute and check if it equals to "FirstLevel" or "Second" level. If so - replace title with its value
                    var displayAttribute = (DisplayAttribute)displayAttributes[0];
                    var displayAttributeName = displayAttribute.Name;
                    var displayAttributeNameValue = displayAttribute.GetName();

                    if (displayAttributeName == "FirstLevel" && firstLevelName.IsNotNullOrEmpty())
                    {
                        tableOptionsAttribute.Title = firstLevelName;
                    }
                    else if (displayAttributeName == "SecondLevel" && secondLevelName.IsNotNullOrEmpty())
                    {
                        tableOptionsAttribute.Title = secondLevelName;
                    }
                    else
                    {
                        tableOptionsAttribute.Title = displayAttributeNameValue;
                    }
                }

                if (tableOptionsAttribute != null)
                {
                    if (tableOptionsAttribute.IsOnlyForSignedUsers && HttpContext.Current.User == null)
                    {
                        continue;
                    }

                    if (propertyInfo.PropertyType.IsGenericType && propertyInfo.PropertyType.GetGenericTypeDefinition() == typeof(Dictionary<,>))
                    {
                        hasDynamicColumns = true;
                        var data = list?.FirstOrDefault();
                        var dictionary = data != null
                            ? propertyInfo.GetValue(data) as IDictionary
                            : null;
                        if (dictionary != null)
                        {
                            var dictionaryType = dictionary.GetType();
                            var keyType = dictionaryType.GetGenericArguments()[0];
                            var valueType = dictionaryType.GetGenericArguments()[1];
                            foreach (DictionaryEntry entry in dictionary)
                            {
                                tableOptionsAttribute = new TableOptionsAttribute
                                {
                                    Key = entry.Key.ToString(),
                                    Name = keyType == typeof(string)
                                        ? $"{propertyInfo.Name}['{entry.Key}']"
                                        : $"{propertyInfo.Name}[{entry.Key}]",
                                    Title = entry.Key.ToString(),
                                    Format = tableOptionsAttribute?.Format,
                                    CssClass = tableOptionsAttribute?.CssClass
                                };

                                result.Add(new Tuple<TableOptionsAttribute, Type, PropertyInfo>(tableOptionsAttribute, valueType, propertyInfo));
                            }
                        }
                    }
                    else
                    {
                        tableOptionsAttribute.Name = propertyInfo.Name;
                        result.Add(new Tuple<TableOptionsAttribute, Type, PropertyInfo>(tableOptionsAttribute, propertyInfo.PropertyType, propertyInfo));
                    }
                }
            }

            if (result.IsNotNullOrEmpty())
            {
                result = result.OrderBy(item => item.Item1.Order).ToList();
            }

            if (!hasDynamicColumns)
            {
                TableDataCache[type] = result;
            }

            return result;
        }

        public static object GetPropertyValue(this PropertyInfo prop, object obj, TableOptionsAttribute attr)
        {
            if (obj == null)
            {
                return null;
            }

            var value = prop.GetValue(obj, null);
            if (prop.PropertyType.IsGenericType && prop.PropertyType.GetGenericTypeDefinition() == typeof(Dictionary<,>))
            {
                return prop.PropertyType.GetProperty("Item")?.GetValue(value, new object[] { attr.Key });
            }

            return value;
        }

        public static string GetPropertyClientTemplate(Tuple<TableOptionsAttribute, Type, PropertyInfo> data, string getTemplateFunction = "#= getClientTemplateById('{0}', data)#")
        {
            if (data.Item1.ClientHtmlTemplate.IsNotNullOrEmpty())
            {
                return data.Item1.ClientHtmlTemplate;
            }

            if (data.Item1.ClientHtmlTemplateId.IsNotNullOrEmpty())
            {
                return string.Format(getTemplateFunction, data.Item1.ClientHtmlTemplateId);
            }

            // https://msdn.microsoft.com/en-us/library/az4se3k1(v=vs.90).aspx
            if (data.Item2 == typeof(DateTime) || data.Item2 == typeof(DateTime?))
            {
                data.Item1.Format = data.Item1.Format ?? "d";
                return string.Format("#= {0} ? kendo.toString(kendo.parseDate({0}), '{1}') : '' #", data.Item1.Name, data.Item1.Format);
            }

            if ((data.Item2 == typeof(bool) || data.Item2 == typeof(bool?)) && data.Item1.IsCheckbox)
            {
                return string.Format("<input type='checkbox' name='{0}' #= {0} ? checked='checked':'' # class='{1} checkItem-js' {2}/>", data.Item1.Name, data.Item1.CssClass ?? string.Empty, data.Item1.DataAttributes);
            }

            return string.Format("#= {0} !== undefined && {0} !== null ? kendo.toString({0}, '{1}') : '' #", data.Item1.Name, data.Item1.Format?.Replace("#", "\\#"));
        }
    }
}