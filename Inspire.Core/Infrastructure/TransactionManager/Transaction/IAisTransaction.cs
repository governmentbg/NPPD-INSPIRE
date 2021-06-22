namespace Inspire.Core.Infrastructure.TransactionManager.Transaction
{
    using Inspire.Core.Infrastructure.TransactionManager.Connection;

    public interface IAisTransaction : IAisConnection
    {
        void Commit();

        void Rollback();
    }
}