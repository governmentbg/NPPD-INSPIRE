namespace Inspire.Services
{
    using System;
    using System.Collections.Generic;
    using AutoMapper;
    using Inspire.Core.Infrastructure.RequestData;
    using Inspire.Domain.Repositories;
    using Inspire.Domain.Services;
    using Inspire.Model.Group;
    using Inspire.Portal.Areas.Admin.Models.Queries;

    public class GroupService : BaseService, IGroupService
    {
        private readonly IGroupRepository groupRepository;

        public GroupService(IMapper mapper, IRequestData requestData, IGroupRepository groupRepository)
            : base(mapper, requestData)
        {
           this.groupRepository = groupRepository;
        }

        public Group Get(Guid id, Guid? languageId = null)
        {
            return groupRepository.Get(id, languageId);
        }

        public List<GroupTableModel> Search(GroupQueryModel query)
        {
            return groupRepository.Search(query, RequestData.LanguageId);
        }

        public void Upsert(Group model)
        {
            groupRepository.Upsert(model);
        }
    }
}
