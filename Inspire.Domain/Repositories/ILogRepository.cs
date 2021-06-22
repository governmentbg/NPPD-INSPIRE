namespace Inspire.Domain.Repositories
{
    using System;

    public interface ILogRepository
    {
        void Insert(Guid systemId, string address, string description);
    }
}