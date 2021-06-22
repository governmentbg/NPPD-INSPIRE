namespace Inspire.Domain.Repositories
{
    using System;
    using System.Collections.Generic;

    using Inspire.Model.Nomenclature;
    using Inspire.Model.Role;

    public interface IRoleRepository
    {
        IEnumerable<Role> Search(RoleQuery query);

        IEnumerable<Nomenclature> GetAllActivities();

        Role Get(Guid id, Guid userId);

        Guid Upsert(Role role);

        List<Guid> GetUserRoles(Guid id);

        void Delete(Guid id);
    }
}