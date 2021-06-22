namespace Inspire.Core.Infrastructure.Repository
{
    using Inspire.Core.Infrastructure.Context;

    public interface IRepository
    {
        IAisContext Context { get; }
    }
}