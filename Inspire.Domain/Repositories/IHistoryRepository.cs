namespace Inspire.Domain.Repositories
{
    using System;
    using System.Collections.Generic;

    using Action = Inspire.Model.History.Action;

    public interface IHistoryRepository
    {
        List<Action> GetObjectHistory(Guid? objectId, Guid objectType, Guid language);
    }
}