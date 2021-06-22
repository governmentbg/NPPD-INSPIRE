namespace Inspire.Data.Utilities
{
    using System;
    using System.Collections;
    using System.Data;
    using System.Data.Common;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;

    using Inspire.Core.Infrastructure.Logger;
    using Inspire.Utilities.Exception;
    using Inspire.Utilities.Extensions;

    public static class DbExtension
    {
        private const string CustomDBErrorCode = "P0001";

        private const string DBErrorMessageToReplace = "P0001:";

        public static ILogger Logger { get; set; }

        public static void ExecuteNonQuerySafety(this DbCommand command)
        {
            string logDbData = null;

            try
            {
                command.ValidateCommandConnection();
                logDbData = command.LogExecuteDbData();

                LogExecutionTime(
                    () =>
                    {
                        command.Prepare();
                        command.ExecuteNonQuery();
                    },
                    command.CommandText);
            }
            catch (DbException dbException)
            {
                dbException.CatchDbException(logDbData ?? command.CreateDbCommandLog());
            }
        }

        public static object ExecuteScalarSafety(this DbCommand command)
        {
            string logDbData = null;

            try
            {
                command.ValidateCommandConnection();
                logDbData = command.LogExecuteDbData();

                return LogExecutionTime(
                    () =>
                    {
                        command.Prepare();
                        return command.ExecuteScalar();
                    },
                    command.CommandText);
            }
            catch (DbException dbException)
            {
                dbException.CatchDbException(logDbData ?? command.CreateDbCommandLog());
            }

            return null;
        }

        public static DbDataReader ExecuteReaderSafety(this DbCommand command)
        {
            string logDbData = null;

            try
            {
                command.ValidateCommandConnection();
                logDbData = command.LogExecuteDbData();

                return LogExecutionTime(
                    () =>
                    {
                        command.Prepare();
                        return command.ExecuteReader();
                    },
                    command.CommandText);
            }
            catch (DbException dbException)
            {
                dbException.CatchDbException(logDbData ?? command.CreateDbCommandLog());
            }

            return null;
        }

        public static void FillSafety(this DbDataAdapter dbDataAdapter, DataSet dataSet)
        {
            string logDbData = null;
            var command = dbDataAdapter.SelectCommand;

            try
            {
                command.ValidateCommandConnection();
                logDbData = command.LogExecuteDbData();

                LogExecutionTime(
                    () =>
                    {
                        command.Prepare();
                        dbDataAdapter.Fill(dataSet);
                    },
                    command.CommandText);
            }
            catch (DbException dbException)
            {
                dbException.CatchDbException(logDbData ?? command.CreateDbCommandLog());
            }
        }

        public static void FillSafety(this DbDataAdapter dbDataAdapter, DataTable dataTable)
        {
            string logDbData = null;
            var command = dbDataAdapter.SelectCommand;

            try
            {
                command.ValidateCommandConnection();
                logDbData = command.LogExecuteDbData();

                LogExecutionTime(
                    () =>
                    {
                        command.Prepare();
                        dbDataAdapter.Fill(dataTable);
                    },
                    command.CommandText);
            }
            catch (DbException dbException)
            {
                dbException.CatchDbException(logDbData ?? command.CreateDbCommandLog());
            }
        }

        public static string GetLog(this DbCommand command)
        {
            if (command == null)
            {
                throw new ArgumentNullException("command");
            }

            var logParameters = command.Parameters.Cast<DbParameter>()
                                       .Where(
                                           dbParameter =>
                                               dbParameter.Direction != ParameterDirection.Output &&
                                               dbParameter.Direction != ParameterDirection.ReturnValue)
                                       .Select(item => $"{item.ParameterName} := {GetFormatValueByType(item)}");

            return $"{command.CommandText}({string.Join(", ", logParameters)});";
        }

        public static T LogExecutionTime<T>(Func<T> function, string actionName = null)
            where T : class
        {
            var begin = DateTime.UtcNow;
            try
            {
                return function.Invoke();
            }
            finally
            {
                var end = DateTime.UtcNow;
                Logger?.Debug($"Execution time {actionName}: {(end - begin).TotalMilliseconds:N0} ms.".Trim());
            }
        }

        public static void LogExecutionTime(Action action, string actionName = null)
        {
            var begin = DateTime.UtcNow;
            action.Invoke();
            var end = DateTime.UtcNow;
            Logger?.Debug($"Execution time {actionName.ToUpper()}: {(end - begin).TotalMilliseconds:N0} ms.".Trim());
        }

        private static void CatchDbException(this Exception dbException, string commandLog = null)
        {
            throw CreateDbException(dbException, commandLog);
        }

        private static Exception CreateDbException(this Exception dbException, string commandLog = null)
        {
            string customDbErrorCode = null;
            var dbExceptionType = dbException.GetType();
            var dbErrorMessage = "Db execution error";

            // Try to parce custom db exception
            if (dbExceptionType.ToString().Contains("OracleException"))
            {
                var tempCommandLog = commandLog != null ? new StringBuilder(commandLog) : new StringBuilder();
                tempCommandLog.AppendLine();
                tempCommandLog.AppendLine(
                    $"DataSource: {dbExceptionType.GetProperty("DataSource")?.GetValue(dbException, null)}");
                tempCommandLog.AppendLine();
                tempCommandLog.AppendLine(
                    $"Procedure: {dbExceptionType.GetProperty("Procedure")?.GetValue(dbException, null)}");
                commandLog = tempCommandLog.ToString();

                var oracleDbExceptionNumber =
                    Convert.ToInt32(dbExceptionType.GetProperty("Number").GetValue(dbException, null));
                customDbErrorCode = oracleDbExceptionNumber.ToString(CultureInfo.InvariantCulture);

                if (oracleDbExceptionNumber >= 20000 && oracleDbExceptionNumber <= 20999)
                {
                    var oracleRegex = $"(?<=ORA-{oracleDbExceptionNumber}:)(.*\n?)(?=ORA)";
                    var regex = new Regex(oracleRegex);
                    return new UserException(regex.Match(dbException.Message).Value.Trim());
                }
            }
            else if (dbExceptionType.ToString().Contains("Npgsql.PostgresException"))
            {
                var tempCommandLog = commandLog != null ? new StringBuilder(commandLog) : new StringBuilder();
                tempCommandLog.AppendLine();
                tempCommandLog.AppendLine(
                    $"ErrorSql: {(dbExceptionType.GetProperty("ErrorSql") != null ? dbExceptionType.GetProperty("ErrorSql")?.GetValue(dbException, null) : string.Empty)}");
                commandLog = tempCommandLog.ToString();

                customDbErrorCode = dbExceptionType.GetProperty("Code") != null
                    ? dbExceptionType.GetProperty("Code")?.GetValue(dbException, null).ToString()
                    : string.Empty;
                if (customDbErrorCode == CustomDBErrorCode)
                {
                    return new UserException(dbException.Message.Replace(DBErrorMessageToReplace, string.Empty).Trim());
                }
            }

            return new UserDbException(dbErrorMessage, dbException, customDbErrorCode, commandLog);
        }

        private static string CreateDbCommandLog(this DbCommand command)
        {
            if (command == null)
            {
                throw new ArgumentNullException("command");
            }

            var builder = new StringBuilder(
                command.Connection == null
                    ? "Connection is null!"
                    : $"ConnectionString: {command.Connection.ConnectionString}");
            builder.AppendLine();
            builder.AppendLine("Execute statement:");
            builder.AppendLine();
            builder.AppendLine(command.GetLog());

            return builder.ToString();
        }

        private static void ValidateCommandConnection(this DbCommand command)
        {
            if (command == null)
            {
                throw new ArgumentNullException("command");
            }

            if (command.Connection == null)
            {
                throw new UserDbException(
                    $"Connection is null for DB command '{command.CommandText}'. CallStack: {Environment.StackTrace}");
            }

            // Prepare command parameters
            foreach (var parameter in command.Parameters.Cast<DbParameter>())
            {
                // Replace input parameter null value with DBNull.Value
                if ((parameter.Direction == ParameterDirection.Input ||
                     parameter.Direction == ParameterDirection.InputOutput) && parameter.Value == null)
                {
                    parameter.Value = DBNull.Value;
                }
            }
        }

        private static string GetFormatValueByType(object value)
        {
            if (value == null || value == DBNull.Value)
            {
                return "null";
            }

            if (value is string || value is Guid)
            {
                return $"'{value}'";
            }

            if (value.GetType().IsEnum)
            {
                return value.GetHashCode().ToString();
            }

            if (value is DateTime date)
            {
                return $"'{date:yyyy-MM-dd hh:mm:ss.fff}'";
            }

            return value.ToString();
        }

        private static string GetFormatValueByType(IDataParameter parameter)
        {
            var dbTypeName = string.Empty;
            if (parameter.GetType().Name.Equals("NpgsqlParameter", StringComparison.InvariantCultureIgnoreCase))
            {
                var type = parameter.GetType();
                var postgresType = type.GetProperty("PostgresType")?.GetValue(parameter, null);
                if (postgresType != null)
                {
                    dbTypeName = postgresType.GetType().GetProperty("Name")?.GetValue(postgresType, null) as string;
                    if (dbTypeName.IsNotNullOrEmpty())
                    {
                        dbTypeName = $"::{dbTypeName}";
                    }
                }
            }

            var value = parameter.Value;
            if (value is IEnumerable enumerable && !(enumerable is string))
            {
                var blob = enumerable as byte[];
                if (blob.IsNotNullOrEmpty())
                {
                    return $"\'[Blob with length: {blob.Length}]\'{dbTypeName}";
                }

                var values = (from object a in enumerable select GetFormatValueByType(a)).ToList();
                return $"array[{string.Join(", ", values)}]{dbTypeName}";
            }

            return $"{GetFormatValueByType(value)}{dbTypeName}";
        }

        private static string LogExecuteDbData(this DbCommand command)
        {
            if (Logger == null || (bool)!Logger?.LogLevel.HasFlag(LogLevel.Debug))
            {
                return null;
            }

            var logDbData = command.CreateDbCommandLog();
            Logger?.Debug(logDbData);

            return logDbData;
        }
    }
}