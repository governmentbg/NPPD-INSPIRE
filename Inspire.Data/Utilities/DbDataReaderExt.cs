namespace Inspire.Utilities.Extensions
{
    using System.Data.Common;

    public static class DbDataReaderExt
    {
        /// <summary>
        ///     Gets the value of the specified column as a type, given the column name.
        /// </summary>
        /// <typeparam name="T">The expected type of the column being retrieved.</typeparam>
        /// <param name="reader">The reader from which to retrieve the column.</param>
        /// <param name="columnName">The name of the column to be retrieved.</param>
        /// <returns>The returned type object.</returns>
        public static T GetFieldValue<T>(this DbDataReader reader, string columnName)
        {
            var index = reader.GetOrdinal(columnName);
            return reader.IsDBNull(index)
                ? default(T)
                : reader.GetFieldValue<T>(index);
        }
    }
}