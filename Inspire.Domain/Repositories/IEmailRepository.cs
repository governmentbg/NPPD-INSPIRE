namespace Inspire.Domain.Repositories
{
    using System;

    using Inspire.Model.Email;

    public interface IEmailRepository
    {
        Guid Insert(Email model);

        byte[] GetMailContentById(Guid mailContentId);
    }
}