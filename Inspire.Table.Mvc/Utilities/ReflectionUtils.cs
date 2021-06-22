namespace Inspire.Table.Mvc.Utilities
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Inspire.Utilities.Extensions;

    /// <summary>
    ///     Utilities that use reflection.
    /// </summary>
    public static class ReflectionUtils
    {
        /// <summary>
        ///     Trims all the string/String props of an Object.
        /// </summary>
        /// <typeparam name="T">The type of the object</typeparam>
        /// <param name="obj"></param>
        public static void TrimObjectProps<T>(ref T obj)
        {
            foreach (var prop in obj.GetType().GetProperties())
            {
                try
                {
                    if (prop.PropertyType.Name.ToLower() == "string")
                    {
                        var val = (string)prop.GetValue(obj, null);
                        if (!string.IsNullOrEmpty(val))
                        {
                            var trim = val.Trim();
                            prop.SetValue(obj, trim.IsNullOrEmptyOrWhiteSpace() ? null : trim, null);
                        }
                    }
                }
                catch
                {
                }
            }
        }

        public static void TrimObjectPropsDashes<T>(ref T obj)
        {
            foreach (var prop in obj.GetType().GetProperties())
            {
                try
                {
                    if (prop.PropertyType.Name.ToLower() == "string")
                    {
                        var val = (string)prop.GetValue(obj, null);
                        if (!string.IsNullOrEmpty(val))
                        {
                            var trim = val;

                            if (val.Contains("- "))
                            {
                                trim = val.Replace("- ", "-");
                            }

                            if (val.Contains(" -"))
                            {
                                trim = val.Replace(" -", "-");
                            }

                            if (val.Contains(" - "))
                            {
                                trim = val.Replace(" - ", "-");
                            }

                            prop.SetValue(obj, trim, null);
                        }
                    }
                }
                catch
                {
                }
            }
        }

        /// <summary>
        ///     Adds search extensions all the string/String props of an Object.
        /// </summary>
        /// <typeparam name="T">The type of the object</typeparam>
        /// <param name="obj"></param>
        public static void ExtendSearchObjectProps<T>(ref T obj)
        {
            foreach (var prop in obj.GetType().GetProperties())
            {
                try
                {
                    if (prop.PropertyType.Name.ToLower() == "string")
                    {
                        var val = (string)prop.GetValue(obj, null);
                        if (!string.IsNullOrEmpty(val))
                        {
                            var trim = string.Format("%{0}%", val.Replace(' ', '%')).Trim();
                            prop.SetValue(obj, trim, null);
                        }
                    }
                }
                catch
                {
                }
            }
        }

        /// <summary>
        ///     Checks if given object has properties that are not null
        /// </summary>
        /// <typeparam name="T">The type of the object</typeparam>
        /// <param name="obj"></param>
        /// <returns>True if obj has property with value different from null, Flase otherwise</returns>
        public static bool HasNonNullProperty<T>(T obj)
        {
            return typeof(T).GetProperties()
                            .Select(prop => prop.GetValue(obj, null))
                            .Any(val => val != null);
        }

        /// <summary>
        ///     Checks if given object has properties that are not null
        /// </summary>
        /// <typeparam name="T">The type of the object</typeparam>
        /// <param name="obj"></param>
        /// <returns>True if obj has property with value different from null, Flase otherwise</returns>
        public static bool HasNotNullOrEmptyProperty<T>(T obj)
        {
            return typeof(T).GetProperties()
                            .Select(prop => prop.GetValue(obj, null))
                            .Any(val => val is string ? !string.IsNullOrEmpty(val as string) : val != null);
        }

        /// <summary>
        ///     Filters the list by a given filter.
        /// </summary>
        /// <typeparam name="T">The type of the object</typeparam>
        /// <param name="filter"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        public static List<T> FilterList<T>(T filter, List<T> list)
        {
            return FilterList(filter, list, new HashSet<string>());
        }

        /// <summary>
        ///     Filters the list by a given filter.
        /// </summary>
        /// <typeparam name="T">The type of the object</typeparam>
        /// <param name="filter"></param>
        /// <param name="list"></param>
        /// <param name="blackSet"></param>
        /// <returns></returns>
        public static List<T> FilterList<T>(T filter, List<T> list, HashSet<string> blackSet)
        {
            var filteredList = new List<T>();

            var isFilterEmpty = 0;

            list.ForEach(
                item =>
                {
                    var filterMatch = true;
                    var enteredField = false;
                    foreach (var tempPropertyInfo in typeof(T).GetProperties())
                    {
                        var propertyInfo = tempPropertyInfo;
                        if (blackSet == null || !blackSet.Contains(propertyInfo.Name))
                        {
                            var val = propertyInfo.GetValue(item, null);
                            var filterVal = propertyInfo.GetValue(filter, null);

                            if (val != null && filterVal != null)
                            {
                                enteredField = true;

                                filterMatch = filterMatch && val.ToString().IndexOf(
                                                  filterVal.ToString(),
                                                  StringComparison.OrdinalIgnoreCase) >= 0;

                                if (!filterMatch)
                                {
                                    break;
                                }
                            }
                        }
                    }

                    if (enteredField)
                    {
                        if (filterMatch)
                        {
                            filteredList.Add(item);
                        }
                    }
                    else
                    {
                        isFilterEmpty++;
                    }
                });

            if (list.Count - isFilterEmpty == 0)
            {
                return list;
            }

            return filteredList;
        }
    }
}