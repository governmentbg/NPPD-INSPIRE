namespace Inspire.Infrastructure.Utilities
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    ///     Utilities that use reflection.
    /// </summary>
    public static class ReflectionUtils
    {
        /// <summary>
        ///     Get caller class name
        /// </summary>
        public static string CallClassName
        {
            get
            {
                var declaringType = new StackTrace().GetFrame(1).GetMethod().DeclaringType;
                return declaringType != null ? declaringType.FullName : string.Empty;
            }
        }

        /// <summary>
        ///     Trims all the string/String props of an Object.
        /// </summary>
        /// <typeparam name="T">The type of the object</typeparam>
        /// <param name="obj"></param>
        public static void TrimObjectProps<T>(T obj)
            where T : class
        {
            MapObjectStrings(obj, v => v.Trim());
        }

        public static T UpperObjectProps<T>(T obj)
            where T : class
        {
            MapObjectStrings(obj, v => v.ToUpper());
            return obj;
        }

        public static void MapObjectStrings<T>(T obj, Func<string, string> eval)
            where T : class
        {
            if (obj == null)
            {
                return;
            }

            var stringProperties = GetStringProperties(obj);

            foreach (var stringProperty in stringProperties)
            {
                var val = stringProperty.GetValue(obj, null) as string;
                if (!string.IsNullOrEmpty(val))
                {
                    stringProperty.SetValue(obj, eval(val), null);
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
                            var trim = string.Format("%{0}%", val.Replace(' ', '%'));
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
        public static bool HasNotNullProperty<T>(T obj)
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

        public static object GetPropValue(this object src, string propName)
        {
            if (string.IsNullOrWhiteSpace(propName))
            {
                throw new ArgumentException("Pass a valid property name, not an empty string.");
            }

            if (src.GetType().GetProperties().All(p => p.Name != propName))
            {
                throw new ArgumentException("No property named \"" + propName + "\", was found in object.");
            }

            return src.GetType().GetProperty(propName).GetValue(src, null);
        }

        public static void SetNestedPropertyValue(this object target, string compoundProperty, object value)
        {
            if (string.IsNullOrWhiteSpace(compoundProperty))
            {
                throw new ArgumentException("Pass a valid property name, not an empty string.");
            }

            var bits = compoundProperty.Split('.');
            for (var i = 0; i < bits.Length - 1; i++)
            {
                var propertyToGet = target.GetType().GetProperty(bits[i]);
                target = propertyToGet.GetValue(target, null);
            }

            var propertyToSet = target.GetType().GetProperty(bits.Last());
            propertyToSet.SetValue(target, value, null);
        }

        public static object GetNestedPropertyValue(this object src, string compoundProperty)
        {
            if (string.IsNullOrWhiteSpace(compoundProperty))
            {
                throw new ArgumentException("Pass a valid property name, not an empty string.");
            }

            var bits = compoundProperty.Split('.');
            for (var i = 0; i < bits.Length - 1; i++)
            {
                var propertyToGet = src.GetType().GetProperty(bits[i]);
                src = propertyToGet.GetValue(src, null);
            }

            return src.GetType().GetProperty(bits.Last()).GetValue(src, null);
        }

        private static IEnumerable<PropertyInfo> GetStringProperties<T>(T obj)
        {
            // Get only read/write string properties
            return obj.GetType().GetProperties()
                      .Where(p => p.PropertyType == typeof(string) && p.CanRead && p.CanWrite);
        }
    }
}