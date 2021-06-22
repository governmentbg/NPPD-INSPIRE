namespace Inspire.Domain.Services
{
    using System;
    using System.Collections.Generic;

    using Action = Inspire.Model.History.Action;

    public interface IHistoryService
    {
        List<Action> GetObjectHistory(Guid? objectId, Guid objectType);
    }
}