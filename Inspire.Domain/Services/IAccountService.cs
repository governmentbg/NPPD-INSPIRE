namespace Inspire.Domain.Services
{
    using System;

    using Inspire.Model.User;

    public interface IAccountService
    {
        User GetByUserName(string userName);

        Guid LogLoginAction(Guid id, string userEmail);
    }
}