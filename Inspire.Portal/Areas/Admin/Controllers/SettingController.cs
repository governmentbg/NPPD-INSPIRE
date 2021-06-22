namespace Inspire.Portal.Areas.Admin.Controllers
{
    using System.Web.Mvc;

    using AutoMapper;

    using Inspire.Common.Mvc.Enums;
    using Inspire.Common.Mvc.Extensions;
    using Inspire.Common.Mvc.Filters.CustomAuthorize;
    using Inspire.Common.Mvc.Infrastructure.BaseTypes;
    using Inspire.Core.Infrastructure.Logger;
    using Inspire.Core.Infrastructure.TransactionManager;
    using Inspire.Domain.Services;
    using Inspire.Model.Admin;
    using Inspire.Portal.App_GlobalResources;
    using Inspire.Portal.Models;
    using Inspire.Portal.Utilities;

    public class SettingController : BaseDbController
    {
        private readonly IAdminService adminService;

        public SettingController(
            ILogger logger,
            IMapper mapper,
            IDbContextManager contextManager,
            IAdminService adminService)
            : base(logger, mapper, contextManager)
        {
            this.adminService = adminService;
        }

        [HttpGet]
        [CustomAuthorize(ApplicationRights.EditUISettings)]
        public ActionResult UpsertUISettings()
        {
            UISettingsViewModel model;
            using (ContextManager.NewConnection())
            {
                model = Mapper.Map<UISettingsViewModel>(adminService.GetUISettings(true));
            }

            this.InitAdminBreadcrumb(Resource.Admin, Resource.UISettings);

            return View(model);
        }

        [HttpPost]
        [CustomAuthorize(ApplicationRights.EditUISettings)]
        public ActionResult UpsertUISettings(UISettingsViewModel model)
        {
            if (!ModelState.IsValid)
            {
                this.InitViewTitleAndBreadcrumbs(
                    Resource.UISettings,
                    new[] { new Breadcrumb { Title = Resource.Settings } });
                return View(model);
            }

            using (var transaction = ContextManager.NewTransaction())
            {
                adminService.UpsertUISettings(Mapper.Map<UISettingsModel>(model));

                transaction.Commit();
            }

            this.ShowMessage(MessageType.Success, Resource.ChangesSuccessfullySaved);
            return RedirectToDefault();
        }
    }
}