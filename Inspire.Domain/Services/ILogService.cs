namespace Inspire.Domain.Services
{
    using System;

    public interface ILogService
    {
        void Insert(Guid systemId, string description);
    }
}