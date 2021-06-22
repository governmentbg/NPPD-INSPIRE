namespace Inspire.Core.Infrastructure.Configuration
{
    using System.Configuration;

    public class AuthenticationSection : ConfigurationSection
    {
        public static AuthenticationSection AllowUsers =>
            ConfigurationManager.GetSection("authenticationSection") as AuthenticationSection;

        [ConfigurationProperty("", IsDefaultCollection = true)]
        public UserCollection Users => this[string.Empty] as UserCollection;
    }
}