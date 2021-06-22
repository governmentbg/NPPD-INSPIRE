namespace Inspire.Repository.Utilities
{
    using Inspire.Core.Infrastructure.Context;
    using Inspire.Core.Infrastructure.RequestData;
    using Inspire.Core.Infrastructure.TransactionManager;
    using Inspire.Data.Repositories;
    using Inspire.Data.Utilities;

    public class ConnectionContext : BaseRepository, IConnectionContext
    {
        private readonly IRequestData requestData;

        public ConnectionContext(IAisContext context, IRequestData requestData)
            : base(context)
        {
            this.requestData = requestData;
        }

        public void SetContext(IRequestData contextData = null)
        {
            contextData = contextData ?? requestData;
            using (var command = Context.Connection.GenerateCommand(
                "admdata.aux_set_transaction_context",
                new
                {
                    host = contextData.Host,
                    ipaddress = contextData.Address,
                    username = contextData.UserName,
                    userid = contextData.UserId,
                    reason = string.Empty
                }))
            {
                command.ExecuteNonQuerySafety();
            }
        }
    }
}