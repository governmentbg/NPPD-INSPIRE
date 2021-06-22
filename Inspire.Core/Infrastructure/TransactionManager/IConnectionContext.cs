namespace Inspire.Core.Infrastructure.TransactionManager
{
    using Inspire.Core.Infrastructure.RequestData;

    public interface IConnectionContext
    {
        void SetContext(IRequestData contextData = null);
    }
}