namespace Inspire.Portal.Infrastructure.AuthenticationProvider
{
    public interface IAuthenticationProvider
    {
        System.Web.Mvc.ModelStateDictionary Login(string userName, string password, bool createPersistentCookie = false);

        void SignOut();
    }
}