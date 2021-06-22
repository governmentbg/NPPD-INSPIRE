namespace Inspire.Repository.Utilities
{
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using System.Linq;

    using Inspire.Core.Infrastructure.Cache;
    using Inspire.Data.DbContextManager.Connection;
    using Inspire.Data.Utilities.DynamicCommand;
    using Inspire.Utilities.Extensions;

    using Npgsql;

    internal static class NpgsqlCommandFactory
    {
        public static ICacheService Cache { get; set; }

        public static NpgsqlCommand GenerateCommand(
            this DbConnection connection,
            string commandText,
            object data = null,
            Dictionary<string, object> additionalParameters = null,
            CommandType commandType = CommandType.StoredProcedure)
        {
            var command = connection.GetOrInsertCommandCache(commandText, commandType);
            command.Connection.FillDbCommandParameters(command, data, additionalParameters);

            return command;
        }

        private static NpgsqlCommand GetOrInsertCommandCache(
            this DbConnection connection,
            string commandText,
            CommandType commandType = CommandType.StoredProcedure)
        {
            var npgsqlConnection = connection is ContextConnection<NpgsqlConnection> contextConnection
                ? contextConnection.InnerConnection
                : connection as NpgsqlConnection;

            var command = new NpgsqlCommand
                          {
                              CommandType = commandType,
                              Connection = npgsqlConnection,
                              CommandText = commandText
                          };
            if (Cache?.ContainsKey(commandText) == true)
            {
                var parameters = Cache.GetValue<IEnumerable<NpgsqlParameter>>(commandText);
                if (parameters.IsNotNullOrEmpty())
                {
                    command.Parameters.AddRange(parameters.ToArray());
                }
            }
            else
            {
                NpgsqlCommandBuilder.DeriveParameters(command);
                var parameters = command.Parameters
                                        .Select(
                                            item =>
                                            {
                                                var temp = item.Clone();
                                                temp.Collection = null;
                                                temp.Value = null;

                                                return temp;
                                            });
                Cache?.SetValue(commandText, parameters);
            }

            return command;
        }
    }
}