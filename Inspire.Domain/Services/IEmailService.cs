namespace Inspire.Domain.Services
{
    using System;

    using Inspire.Model.Email;

    public interface IEmailService
    {
        Guid Insert(Email model);

        byte[] GetMailContentById(Guid mailContentId);
    }
}