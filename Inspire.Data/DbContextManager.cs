namespace Inspire.Data
{
    using System.Data.Common;

    using Inspire.Core.Infrastructure.Context;
    using Inspire.Core.Infrastructure.RequestData;
    using Inspire.Core.Infrastructure.TransactionManager;
    using Inspire.Core.Infrastructure.TransactionManager.Connection;
    using Inspire.Core.Infrastructure.TransactionManager.Transaction;
    using Inspire.Data.DbContextManager.Connection;
    using Inspire.Data.DbContextManager.Transaction;

    public abstract class DbContextManager<TDbConnection, TDbTransaction> : IDbContextManager
        where TDbConnection : DbConnection, new()
        where TDbTransaction : DbTransaction
    {
        private readonly IConnectionContext connectionContext;

        protected DbContextManager(IAisContext context, IConnectionContext connectionContext)
        {
            Context = context;
            this.connectionContext = connectionContext;
        }

        public IAisContext Context { get; }

        public IAisConnection NewConnection(IRequestData contextData = null)
        {
            var connection = new AisConnection<TDbConnection>(Context);

            connectionContext.SetContext(contextData);

            return connection;
        }

        public IAisTransaction NewTransaction(IRequestData contextData = null)
        {
            var transaction = new AisTransaction<TDbConnection, TDbTransaction>(Context);

            connectionContext.SetContext(contextData);

            return transaction;
        }
    }
}