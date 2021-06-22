namespace Inspire.Data.DbContextManager.Connection
{
    using System.Data;
    using System.Data.Common;

    using Inspire.Core.Infrastructure.Context;

    public class ContextConnection<TDbConnection> : DbConnection
        where TDbConnection : DbConnection, new()
    {
        public readonly TDbConnection InnerConnection;
        private readonly IAisContext context;

        public ContextConnection(IAisContext context)
        {
            this.context = context;
            InnerConnection = new TDbConnection { ConnectionString = context.ConnectionString };
        }

        public override string ConnectionString
        {
            get => InnerConnection.ConnectionString;

            set => InnerConnection.ConnectionString = value;
        }

        public override int ConnectionTimeout => InnerConnection.ConnectionTimeout;

        public override string Database => InnerConnection.Database;

        public override ConnectionState State => InnerConnection.State;

        public override string DataSource => InnerConnection.DataSource;

        public override string ServerVersion => InnerConnection.ServerVersion;

        public override void ChangeDatabase(string databaseName)
        {
            InnerConnection.ChangeDatabase(databaseName);
        }

        public override void Close()
        {
            if (InnerConnection != null && InnerConnection.State != ConnectionState.Closed)
            {
                InnerConnection.Close();
            }
        }

        public override DataTable GetSchema()
        {
            return InnerConnection.GetSchema();
        }

        public override DataTable GetSchema(string collectionName)
        {
            return InnerConnection.GetSchema(collectionName);
        }

        public override DataTable GetSchema(string collectionName, string[] restrictionValues)
        {
            return InnerConnection.GetSchema(collectionName, restrictionValues);
        }

        public override void Open()
        {
            InnerConnection.Open();
        }

        protected override DbTransaction BeginDbTransaction(IsolationLevel isolationLevel)
        {
            return context.Transaction ?? InnerConnection.BeginTransaction(isolationLevel);
        }

        protected override DbCommand CreateDbCommand()
        {
            return InnerConnection.CreateCommand();
        }

        protected override void Dispose(bool disposing)
        {
        }
    }
}