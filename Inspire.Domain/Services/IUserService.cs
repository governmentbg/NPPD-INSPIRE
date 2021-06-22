namespace Inspire.Domain.Services
{
    using System;
    using System.Collections.Generic;

    using Inspire.Model.Nomenclature;
    using Inspire.Model.User;

    public interface IUserService
    {
        IEnumerable<User> Search(UserQuery query);

        User Get(Guid id);

        Guid Upsert(User user);

        void UpdatePasswordToken(Guid id, string resetPasswordToken);

        void ChangeStatus(Guid statusId, Guid? userId, Guid automationUserId);

        void ChangePassword(ChangePasswordModel model);

        void SetUserRoles(SetRole model);

        List<string> SearchPositionByText(string text);

        List<string> GetPasswords(Guid? userId);

        List<Nomenclature> GetUsersByRole(Guid? id);

        void UpsertControl(UsersControl model);

        List<UserControlItem> GetControl(Guid objectId, Guid objectTypeId);

        bool CheckIfUserHasRightsForObject(Guid userId, Guid? objectId, Guid objectTypeId, Guid activityId);

        List<Nomenclature> GetOrganisationsForDdl();
    }
}