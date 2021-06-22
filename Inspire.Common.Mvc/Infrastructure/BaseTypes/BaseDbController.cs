namespace Inspire.Common.Mvc.Infrastructure.BaseTypes
{
    using AutoMapper;

    using Inspire.Core.Infrastructure.Logger;
    using Inspire.Core.Infrastructure.TransactionManager;

    public abstract class BaseDbController : BaseController
    {
        protected readonly IDbContextManager ContextManager;

        protected BaseDbController(ILogger logger, IMapper mapper, IDbContextManager contextManager)
            : base(logger, mapper)
        {
            ContextManager = contextManager;
        }
    }
}