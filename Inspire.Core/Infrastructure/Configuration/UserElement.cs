namespace Inspire.Core.Infrastructure.Configuration
{
    using System.Configuration;

    public class UserElement : ConfigurationElement
    {
        [ConfigurationProperty("username", IsRequired = true)]
        public string UserName => this["username"] as string;

        [ConfigurationProperty("password", IsRequired = true)]
        public string Password => this["password"] as string;
    }
}