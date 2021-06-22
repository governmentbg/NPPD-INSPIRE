namespace Inspire.Data.DbContextManager.Transaction
{
    using System.Data;
    using System.Data.Common;

    public class ContextTransaction<TDbTransaction> : DbTransaction
        where TDbTransaction : DbTransaction
    {
        public ContextTransaction(TDbTransaction transaction)
        {
            InnerTransaction = transaction;
        }

        public TDbTransaction InnerTransaction { get; }

        public override IsolationLevel IsolationLevel => InnerTransaction.IsolationLevel;

        protected override DbConnection DbConnection => InnerTransaction.Connection;

        public override void Commit()
        {
        }

        public override void Rollback()
        {
        }

        protected override void Dispose(bool disposing)
        {
        }
    }
}