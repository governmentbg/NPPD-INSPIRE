namespace Inspire.Domain.Repositories
{
    using System;
    using System.Collections.Generic;
    using Inspire.Model.Group;
    using Inspire.Portal.Areas.Admin.Models.Queries;

    public interface IGroupRepository
    {
        void Upsert(Group model);

        Group Get(Guid id, Guid? languageId = null);

        List<GroupTableModel> Search(GroupQueryModel query, Guid languageId);
    }
}
