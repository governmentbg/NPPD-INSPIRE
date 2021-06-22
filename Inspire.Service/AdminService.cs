namespace Inspire.Services
{
    using System;
    using System.Collections.Generic;

    using AutoMapper;

    using Inspire.Core.Infrastructure.RequestData;
    using Inspire.Domain.Repositories;
    using Inspire.Domain.Services;
    using Inspire.Model.Admin;
    using Inspire.Model.Attachment;
    using Inspire.Model.QueryModels;
    using Inspire.Model.TableModels;
    using Inspire.Utilities.Enums;

    public class AdminService : BaseService, IAdminService
    {
        private readonly IAdminRepository adminRepository;
        private readonly IAttachmentRepository attachmentRepository;

        public AdminService(IMapper mapper, IRequestData requestData, IAdminRepository adminRepository, IAttachmentRepository attachmentRepository)
            : base(mapper, requestData)
        {
            this.adminRepository = adminRepository;
            this.attachmentRepository = attachmentRepository;
        }

        public void UpsertUISettings(UISettingsModel model)
        {
            adminRepository.UpsertUISetting(model);
        }

        public UISettingsModel GetUISettings(bool isUpsert)
        {
            return adminRepository.GetUISettings(isUpsert ? default(Guid?) : RequestData.LanguageId);
        }

        public List<Attachment> GetHomeImages()
        {
            return attachmentRepository.GetFiles(default(Guid), ObjectType.HomeBackgroundImage);
        }

        public List<UserLoginTableModel> SearchUserLogin(UserLoginQueryModel query)
        {
            return adminRepository.SearchUserLogin(query);
        }
    }
}