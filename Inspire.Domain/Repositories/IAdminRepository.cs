namespace Inspire.Domain.Repositories
{
    using System;
    using System.Collections.Generic;

    using Inspire.Model.Admin;
    using Inspire.Model.QueryModels;
    using Inspire.Model.TableModels;

    public interface IAdminRepository
    {
        void UpsertUISetting(UISettingsModel model);

        UISettingsModel GetUISettings(Guid? languageId);

        List<UserLoginTableModel> SearchUserLogin(UserLoginQueryModel query);
    }
}