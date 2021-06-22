namespace Inspire.Data.Utilities.DynamicCommand
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using System.Linq;
    using System.Reflection;

    using Inspire.Utilities.Extensions;

    public static class DynamicCommandGenerator
    {
        public static void FillDbCommandParameters(
            this DbConnection cnn,
            DbCommand dbCommand,
            object obj,
            Dictionary<string, object> additionalParameters = null)
        {
            try
            {
                foreach (var dbParameter in dbCommand.Parameters.Cast<DbParameter>().Where(
                    p => p.Direction != ParameterDirection.Output && p.Direction != ParameterDirection.ReturnValue))
                {
                    var name = dbParameter.ParameterName.TrimStart(':');
                    dbParameter.Value = additionalParameters != null && additionalParameters.ContainsKey(name)
                        ? additionalParameters[name]
                        : GetValueForName(obj, name) ?? DBNull.Value;

                    // Change default TimeSpan convert type. The default is "Interval"
                    if (dbParameter.Value is TimeSpan && dbParameter.DbType != DbType.Time)
                    {
                        dbParameter.DbType = DbType.Time;
                    }

                    if (dbParameter.Value is int && dbParameter.DbType == DbType.Boolean)
                    {
                        dbParameter.Value = (int)dbParameter.Value == 1;
                    }
                }
            }
            catch (MissingFieldException e)
            {
                throw new TargetParameterCountException(
                    $"Missing parameter part value for {e.Message} in procedure {dbCommand}",
                    e);
            }
        }

        private static object GetValueForName(object obj, string name)
        {
            try
            {
                if (obj == null)
                {
                    return null;
                }

                name = name.Remove(0, 1); //// Removes the leading "p" in the parameter Mapex specific
                var match = name.Split('_');

                var tempValue = GetPropertyValueRec(obj, match.First(), match.Skip(1));
                if (tempValue == null)
                {
                    return null;
                }

                return PrepareForDb(tempValue);
            }
            catch (MissingFieldException)
            {
                throw new MissingFieldException(name.Replace('_', '.'));
            }
        }

        private static object PrepareForDb(object value)
        {
            if (value == null)
            {
                return null;
            }

            var type = value.GetType();
            if (type.IsEnumOrNullableEnum())
            {
                var nullableEnumUnderlyingType = Nullable.GetUnderlyingType(type);

                if (nullableEnumUnderlyingType == null)
                {
                    return (int)Convert.ChangeType(value, Enum.GetUnderlyingType(type));
                }

                return (int?)Convert.ChangeType(value, Enum.GetUnderlyingType(Nullable.GetUnderlyingType(type)));
            }

            if (value is IEnumerable<string> stringCollection)
            {
                return stringCollection.Select(item => item.IsNullOrEmpty() ? null : item).ToArray();
            }

            if (value is string str)
            {
                return str.IsNullOrEmpty() ? null : str;
            }

            return value;
        }

        private static object GetPropertyValueRec(object obj, string head, IEnumerable<string> tail)
        {
            var pi = ReflectionUtils.GetPropertyInfoCache(obj.GetType())
                                    .FirstOrDefault(p => p.Name.Equals(head, StringComparison.OrdinalIgnoreCase));
            if (pi == null)
            {
                return null;
            }

            var innerObj = pi.GetValue(obj, null);
            if (innerObj == null || !tail.Any())
            {
                return innerObj;
            }

            return GetPropertyValueRec(innerObj, tail.First(), tail.Skip(1));
        }

        private static bool IsEnumOrNullableEnum(this Type t)
        {
            var u = Nullable.GetUnderlyingType(t);
            return t.IsEnum || (u != null && u.IsEnum);
        }
    }
}