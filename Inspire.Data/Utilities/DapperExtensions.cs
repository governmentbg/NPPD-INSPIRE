namespace Inspire.Data.Utilities
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using System.Linq;
    using System.Reflection;

    using Core.Infrastructure.Attribute;

    using Dapper;

    public static class DapperExtensions
    {
        public static IEnumerable<T> Query<T>(this DbConnection cnn, string command)
        {
            return cnn.Query<T>(command, commandType: CommandType.StoredProcedure);
        }

        public static IEnumerable<T> Query<T>(this DbConnection cnn, string command, object parameters, CommandType commandType = CommandType.StoredProcedure)
        {
            return SqlMapper.Query<T>(cnn, command, parameters.ToPropertyDictionary(), commandType: commandType);
        }

        public static IEnumerable<T> Query<T>(this DbConnection cnn, string command, Dictionary<string, object> parameters, CommandType commandType = CommandType.StoredProcedure)
        {
            return SqlMapper.Query<T>(cnn, command, parameters, commandType: commandType);
        }

        public static int Execute(this DbConnection cnn, string sql, CommandType commandType = CommandType.StoredProcedure, object parameters = null)
        {
            return cnn.Execute(sql, parameters?.ToPropertyDictionary(), commandType: commandType);
        }

        public static SqlMapper.GridReader QueryMultiple(this DbConnection cnn, string command, object parameters = null)
        {
            return cnn.QueryMultiple(command, parameters?.ToPropertyDictionary(), commandType: CommandType.StoredProcedure);
        }

        public static IEnumerable<TReturn> Query<TFirst, TSecond, TReturn>(this IDbConnection cnn, string sql, Func<TFirst, TSecond, TReturn> map, object parameters = null, IDbTransaction transaction = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null, CommandType? commandType = CommandType.StoredProcedure)
        {
            return SqlMapper.Query(cnn, sql, map, parameters?.ToPropertyDictionary(), transaction, buffered, splitOn, commandTimeout, commandType);
        }

        public static IEnumerable<TReturn> Query<TFirst, TSecond, TThird, TReturn>(this IDbConnection cnn, string sql, Func<TFirst, TSecond, TThird, TReturn> map, object parameters = null, IDbTransaction transaction = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null, CommandType? commandType = CommandType.StoredProcedure)
        {
            return SqlMapper.Query(cnn, sql, map, parameters?.ToPropertyDictionary(), transaction, buffered, splitOn, commandTimeout, commandType);
        }

        public static IEnumerable<TReturn> Query<TFirst, TSecond, TThird, TFourth, TReturn>(this IDbConnection cnn, string sql, Func<TFirst, TSecond, TThird, TFourth, TReturn> map, object parameters = null, IDbTransaction transaction = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null, CommandType? commandType = CommandType.StoredProcedure)
        {
            return SqlMapper.Query(cnn, sql, map, parameters?.ToPropertyDictionary(), transaction, buffered, splitOn, commandTimeout, commandType);
        }

        public static IEnumerable<TReturn> Query<TFirst, TSecond, TThird, TFourth, TFifth, TReturn>(this IDbConnection cnn, string sql, Func<TFirst, TSecond, TThird, TFourth, TFifth, TReturn> map, object parameters = null, IDbTransaction transaction = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null, CommandType? commandType = CommandType.StoredProcedure)
        {
            return SqlMapper.Query(cnn, sql, map, parameters?.ToPropertyDictionary(), transaction, buffered, splitOn, commandTimeout, commandType);
        }

        public static IEnumerable<TReturn> Query<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn>(this IDbConnection cnn, string sql, Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn> map, object parameters = null, IDbTransaction transaction = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null, CommandType? commandType = CommandType.StoredProcedure)
        {
            return SqlMapper.Query(cnn, sql, map, parameters?.ToPropertyDictionary(), transaction, buffered, splitOn, commandTimeout, commandType);
        }

        public static IEnumerable<TReturn> Query<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn>(this IDbConnection cnn, string sql, Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn> map, object parameters = null, IDbTransaction transaction = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null, CommandType? commandType = CommandType.StoredProcedure)
        {
            return SqlMapper.Query(cnn, sql, map, parameters?.ToPropertyDictionary(), transaction, buffered, splitOn, commandTimeout, commandType);
        }

        public static IEnumerable<TReturn> Query<TReturn>(this IDbConnection cnn, string sql, Type[] types, Func<object[], TReturn> map, object parameters = null, IDbTransaction transaction = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null, CommandType? commandType = null)
        {
            return SqlMapper.Query(cnn, sql, types, map, parameters?.ToPropertyDictionary(), transaction, buffered, splitOn, commandTimeout, commandType);
        }

        public static void SetTypeMap<T>(Dictionary<string, string> columnNames)
            where T : class
        {
            SqlMapper.SetTypeMap(
                typeof(T),
                new CustomPropertyTypeMap(
                    typeof(T),
                    (type, name) => type.GetProperty(
                        columnNames.ContainsKey(name)
                            ? columnNames[name]
                            : name,
                        BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance)));
        }

        private static Dictionary<string, object> ToPropertyDictionary(this object obj)
        {
            var dictionary = new Dictionary<string, object>();

            if (obj != null)
            {
                var properties = ReflectionUtils.GetPropertyInfoCache(obj.GetType());
                foreach (var propertyInfo in properties.Where(pr => pr.GetCustomAttributes(typeof(Ignore), true).Length <= 0))
                {
                    if (propertyInfo.PropertyType != typeof(List<KeyValuePair<string, string>>) &&
                        propertyInfo.CanRead &&
                        propertyInfo.GetIndexParameters().Length == 0)
                    {
                        dictionary[$"p{propertyInfo.Name}".ToLower()] = propertyInfo.GetValue(obj, null);
                    }
                }
            }

            return dictionary;
        }
    }
}