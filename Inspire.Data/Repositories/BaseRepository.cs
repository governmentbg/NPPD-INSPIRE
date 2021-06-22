namespace Inspire.Data.Repositories
{
    using Inspire.Core.Infrastructure.Context;
    using Inspire.Core.Infrastructure.Repository;

    public class BaseRepository : IRepository
    {
        public BaseRepository(IAisContext context)
        {
            Context = context;
        }

        public IAisContext Context { get; }
    }
}