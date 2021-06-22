namespace Inspire.Services
{
    using System;
    using System.Collections.Generic;

    using AutoMapper;

    using Inspire.Core.Infrastructure.RequestData;
    using Inspire.Domain.Repositories;
    using Inspire.Domain.Services;
    using Inspire.Model.Nomenclature;
    using Inspire.Model.Role;

    public class RoleService : BaseService, IRoleService
    {
        private readonly IRoleRepository roleRepository;

        public RoleService(IMapper mapper, IRequestData requestData, IRoleRepository roleRepository)
            : base(mapper, requestData)
        {
            this.roleRepository = roleRepository;
        }

        public IEnumerable<Role> Search(RoleQuery query)
        {
            return roleRepository.Search(query);
        }

        public IEnumerable<Nomenclature> GetAllActivities()
        {
            return roleRepository.GetAllActivities();
        }

        public Role Get(Guid id)
        {
            return roleRepository.Get(id, RequestData.UserId.Value);
        }

        public Guid Upsert(Role role)
        {
            return roleRepository.Upsert(role);
        }

        public List<Guid> GetUserRoles(Guid id)
        {
            return roleRepository.GetUserRoles(id);
        }

        public void Delete(Guid id)
        {
            roleRepository.Delete(id);
        }
    }
}