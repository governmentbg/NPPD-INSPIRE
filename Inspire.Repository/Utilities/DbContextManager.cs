namespace Inspire.Repository.Utilities
{
    using Inspire.Core.Infrastructure.Context;
    using Inspire.Core.Infrastructure.TransactionManager;
    using Inspire.Data;

    using Npgsql;

    public class DbContextManager : DbContextManager<NpgsqlConnection, NpgsqlTransaction>
    {
        public DbContextManager(IAisContext context, IConnectionContext connectionContext)
            : base(context, connectionContext)
        {
        }
    }
}