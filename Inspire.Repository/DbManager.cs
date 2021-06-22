namespace Inspire.Repository
{
    using Inspire.Core.Infrastructure.Cache;
    using Inspire.Core.Infrastructure.Logger;
    using Inspire.Data.Utilities;
    using Inspire.Domain.Repositories;
    using Inspire.Repository.Utilities;

    public class DbManager : IDbManager
    {
        private readonly ICacheService cacheService;
        private readonly ILogger logger;

        public DbManager(ILogger logger, ICacheService cacheService)
        {
            this.logger = logger;
            this.cacheService = cacheService;
        }

        public void Init()
        {
            DbExtension.Logger = logger;
            NpgsqlCommandFactory.Cache = cacheService;
        }
    }
}