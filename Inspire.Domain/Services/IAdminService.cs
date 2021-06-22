namespace Inspire.Domain.Services
{
    using System.Collections.Generic;

    using Inspire.Model.Admin;
    using Inspire.Model.Attachment;
    using Inspire.Model.QueryModels;
    using Inspire.Model.TableModels;

    public interface IAdminService
    {
        void UpsertUISettings(UISettingsModel model);

        UISettingsModel GetUISettings(bool isUpsert);

        List<Attachment> GetHomeImages();

        List<UserLoginTableModel> SearchUserLogin(UserLoginQueryModel query);
    }
}