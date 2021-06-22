namespace Inspire.Data.Repositories
{
    using System;
    using System.Data.Common;

    using Inspire.Core.Infrastructure.Cache;
    using Inspire.Core.Infrastructure.Context;
    using Inspire.Core.Infrastructure.Logger;
    using Inspire.Data.Utilities;

    public class CacheRepository : BaseRepository
    {
        private readonly ICacheService cacheService;
        private readonly ILogger logger;

        protected CacheRepository(IAisContext context, ICacheService cacheService, ILogger logger)
            : base(context)
        {
            this.cacheService = cacheService;
            this.logger = logger;
        }

        protected T GetOrSetCache<T>(DbCommand command, Func<DbCommand, T> func)
        {
            var key = command.GetLog();
            return cacheService.GetOrSetCache(key, () => func(command));
        }

        protected void Clear()
        {
            cacheService.Clear();
        }
    }
}