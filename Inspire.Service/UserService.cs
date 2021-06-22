namespace Inspire.Services
{
    using System;
    using System.Collections.Generic;

    using AutoMapper;

    using Inspire.Core.Infrastructure.RequestData;
    using Inspire.Domain.Repositories;
    using Inspire.Domain.Services;
    using Inspire.Model.Nomenclature;
    using Inspire.Model.User;
    using Inspire.Utilities.Enums;

    public class UserService : BaseService, IUserService
    {
        private readonly IAttachmentRepository attachmentRepository;
        private readonly IUserRepository userRepository;

        public UserService(
            IMapper mapper,
            IRequestData requestData,
            IUserRepository userRepository,
            IAttachmentRepository attachmentRepository)
            : base(mapper, requestData)
        {
            this.userRepository = userRepository;
            this.attachmentRepository = attachmentRepository;
        }

        public IEnumerable<User> Search(UserQuery query)
        {
            return userRepository.Search(query, new Guid?(RequestData.LanguageId));
        }

        public User Get(Guid id)
        {
            return userRepository.Get(id);
        }

        public Guid Upsert(User user)
        {
            return userRepository.Upsert(user);
        }

        public void UpdatePasswordToken(Guid id, string resetPasswordToken)
        {
            userRepository.UpdatePasswordToken(id, resetPasswordToken);
        }

        public void ChangeStatus(Guid statusId, Guid? userId, Guid automationUserId)
        {
            userRepository.ChangeStatus(statusId, userId, automationUserId);
        }

        public void ChangePassword(ChangePasswordModel model)
        {
            userRepository.ChangePassword(model);
        }

        public void SetUserRoles(SetRole model)
        {
            userRepository.SetUserRoles(model);
        }

        public List<string> SearchPositionByText(string text)
        {
            return userRepository.SearchPositionByText(text);
        }

        public List<string> GetPasswords(Guid? userId)
        {
            return userRepository.GetPasswords(userId);
        }

        public void UpsertControl(UsersControl model)
        {
            userRepository.UpsertControl(model);
        }

        public List<Nomenclature> GetUsersByRole(Guid? id)
        {
            return userRepository.GetUsersByRole(id);
        }

        public List<UserControlItem> GetControl(Guid objectId, Guid objectTypeId)
        {
            return userRepository.GetControl(objectId, objectTypeId);
        }

        public bool CheckIfUserHasRightsForObject(Guid userId, Guid? objectId, Guid objectTypeId, Guid activityId)
        {
            return userRepository.CheckIfUserHasRightsForObject(userId, objectId, objectTypeId, activityId);
        }

        public List<Nomenclature> GetOrganisationsForDdl()
        {
            return userRepository.GetOrganisationsForDdl(RequestData.LanguageId);
        }
    }
}