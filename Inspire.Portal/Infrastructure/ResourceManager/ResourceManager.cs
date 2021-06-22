namespace Inspire.Portal.Infrastructure.ResourceManager
{
    using Inspire.Core.Infrastructure.ResourceManager;
    using Inspire.Portal.App_GlobalResources;

    public class ResourceManager : IResourceManager
    {
        private readonly System.Resources.ResourceManager manager =
            new System.Resources.ResourceManager(typeof(Resource));

        public string Get(string key)
        {
            return manager.GetString(key);
        }
    }
}