namespace Inspire.Data.DbContextManager.Transaction
{
    using System.Data.Common;

    using Inspire.Core.Infrastructure.Context;
    using Inspire.Core.Infrastructure.TransactionManager.Transaction;
    using Inspire.Data.DbContextManager.Connection;

    public class AisTransaction<TDbConnection, TDbTransaction> : AisConnection<TDbConnection>, IAisTransaction
        where TDbConnection : DbConnection, new()
        where TDbTransaction : DbTransaction
    {
        public AisTransaction(IAisContext context)
            : base(context)
        {
            Context.Transaction = null;
            var transaction = Context.Connection.BeginTransaction();
            Context.Transaction = transaction is ContextTransaction<TDbTransaction>
                ? transaction
                : new ContextTransaction<TDbTransaction>(transaction as TDbTransaction);
        }

        protected ContextTransaction<TDbTransaction> Transaction =>
            Context.Transaction as ContextTransaction<TDbTransaction>;

        public void Commit()
        {
            Transaction.InnerTransaction.Commit();
        }

        public void Rollback()
        {
            Transaction.InnerTransaction.Rollback();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Transaction.InnerTransaction?.Dispose();

                if (Context.Transaction != null)
                {
                    Context.Transaction.Dispose();
                    Context.Transaction = null;
                }
            }

            base.Dispose(true);
        }
    }
}