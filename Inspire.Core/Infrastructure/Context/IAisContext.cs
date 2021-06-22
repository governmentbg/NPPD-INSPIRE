namespace Inspire.Core.Infrastructure.Context
{
    using System.Data.Common;

    public interface IAisContext
    {
        string ConnectionString { get; }

        DbConnection Connection { get; set; }

        DbTransaction Transaction { get; set; }
    }
}