namespace Inspire.Core.Infrastructure.RequestData
{
    using System;

    public interface IRequestData
    {
        string Host { get; }

        string Address { get; }

        string Browser { get; }

        Guid? UserId { get; }

        string UserName { get; }

        Guid LanguageId { get; }
    }
}