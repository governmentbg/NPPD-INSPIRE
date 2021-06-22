namespace Inspire.Data.DbContextManager.Connection
{
    using System;
    using System.Data;
    using System.Data.Common;

    using Inspire.Core.Infrastructure.Context;
    using Inspire.Core.Infrastructure.TransactionManager.Connection;

    public class AisConnection<TDbConnection> : IAisConnection
        where TDbConnection : DbConnection, new()
    {
        protected readonly IAisContext Context;

        public AisConnection(IAisContext context)
        {
            Context = context;
            Context.Connection = new ContextConnection<TDbConnection>(Context);
            if (Context.Connection.State != ConnectionState.Open)
            {
                Context.Connection.Close();
                Context.Connection.Open();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (Context != null && Context.Connection != null)
                {
                    Context.Connection.Close();
                    Context.Connection = null;
                }
            }
        }
    }
}