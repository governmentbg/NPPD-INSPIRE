namespace Inspire.Core.Infrastructure.TransactionManager
{
    using Inspire.Core.Infrastructure.RequestData;
    using Inspire.Core.Infrastructure.TransactionManager.Connection;
    using Inspire.Core.Infrastructure.TransactionManager.Transaction;

    public interface IDbContextManager
    {
        IAisConnection NewConnection(IRequestData contextData = null);

        IAisTransaction NewTransaction(IRequestData contextData = null);
    }
}