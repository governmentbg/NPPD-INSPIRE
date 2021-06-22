namespace Inspire.Repository
{
    using System;

    using Inspire.Core.Infrastructure.Context;
    using Inspire.Data.Repositories;
    using Inspire.Data.Utilities;
    using Inspire.Domain.Repositories;
    using Inspire.Repository.Utilities;

    public class LogRepository : BaseRepository, ILogRepository
    {
        public LogRepository(IAisContext context)
            : base(context)
        {
        }

        public void Insert(Guid systemId, string address, string description)
        {
            using (var command = Context.Connection.GenerateCommand(
                "ais.ins_serviceinvokelog",
                new
                {
                    systemid = systemId,
                    ipaddress = address,
                    description
                }))
            {
                command.ExecuteNonQuerySafety();
            }
        }
    }
}