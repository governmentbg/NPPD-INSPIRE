namespace Inspire.Portal.Services.UserMailService
{
    using Inspire.Model.User;

    public interface IUserMailService
    {
        void SendChangePasswordMail(User user);

        void SendCompleteRegistrationEmail(User user);
    }
}