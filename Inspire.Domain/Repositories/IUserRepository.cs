namespace Inspire.Domain.Repositories
{
    using System;
    using System.Collections.Generic;

    using Inspire.Model.Nomenclature;
    using Inspire.Model.User;

    public interface IUserRepository
    {
        IEnumerable<User> Search(UserQuery query, Guid? languageId);

        User Get(Guid id);

        Guid Upsert(User user);

        void UpdatePasswordToken(Guid id, string resetPasswordToken);

        void ChangeStatus(Guid statusId, Guid? userId, Guid automationUserId);

        void ChangePassword(ChangePasswordModel model);

        void SetUserRoles(SetRole model);

        List<string> SearchPositionByText(string text);

        List<string> GetPasswords(Guid? userId);

        void UpsertControl(UsersControl model);

        List<Nomenclature> GetUsersByRole(Guid? id);

        List<UserControlItem> GetControl(Guid objectId, Guid objectTypeId);

        bool CheckIfUserHasRightsForObject(Guid userId, Guid? objectId, Guid objectTypeId, Guid activityId);

        List<Nomenclature> GetOrganisationsForDdl(Guid languageId);
    }
}