namespace Inspire.Data.Context
{
    using System.Data.Common;

    using Inspire.Core.Infrastructure.Context;

    public class AisContext : IAisContext
    {
        public AisContext(string connectionString)
        {
            ConnectionString = connectionString;
        }

        public string ConnectionString { get; }

        public DbConnection Connection { get; set; }

        public DbTransaction Transaction { get; set; }
    }
}