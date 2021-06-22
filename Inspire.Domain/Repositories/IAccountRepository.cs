namespace Inspire.Domain.Repositories
{
    using System;

    using Inspire.Model.User;

    public interface IAccountRepository
    {
        User GetByUserName(string userName);

        Guid LogLoginAction(Guid id, string userEmail, string ipAddress);
    }
}