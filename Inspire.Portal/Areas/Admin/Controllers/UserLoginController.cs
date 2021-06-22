namespace Inspire.Portal.Areas.Admin.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Web.Mvc;

    using AutoMapper;

    using Inspire.Common.Mvc.Filters.CustomAuthorize;
    using Inspire.Core.Infrastructure.Logger;
    using Inspire.Core.Infrastructure.ResourceManager;
    using Inspire.Core.Infrastructure.TransactionManager;
    using Inspire.Domain.Services;
    using Inspire.Model.QueryModels;
    using Inspire.Model.TableModels;
    using Inspire.Portal.App_GlobalResources;
    using Inspire.Portal.Areas.Admin.Models.UserLogin;
    using Inspire.Portal.Models;
    using Inspire.Portal.Utilities;
    using Inspire.Table.Mvc.Controllers;
    using Inspire.Table.Mvc.Utilities;

    using TransactionHistoryQueryModel = Inspire.Portal.Areas.Admin.Models.TransactionHistory.TransactionHistoryQueryModel;

    [CustomAuthorize(ApplicationRights.UserLoginReport)]
    public class UserLoginController : SearchTableController<UserLoginQueryViewModel, UserLoginTableViewModel>
    {
        private readonly IAdminService adminService;

        public UserLoginController(
            ILogger logger,
            IMapper mapper,
            IDbContextManager contextManager,
            IResourceManager resource,
            IAdminService adminService)
            : base(logger, mapper, contextManager, resource, Resource.UserLoginReport)
        {
            this.adminService = adminService;
        }

        [HttpGet]
        [CustomAuthorize(ApplicationRights.SearchRole)]
        public override ActionResult Index(UserLoginQueryViewModel model = null)
        {
            if (model == null || (!ReflectionUtils.HasNonNullProperty(model) || model.IsFromGN == default(bool)))
            {
                model = model ?? new UserLoginQueryViewModel();
                model.LoginDateFrom = DateTime.Now.Date.AddMonths(-1);
                model.LoginDateTo = DateTime.Now.Date;
            }

            this.InitAdminBreadcrumb(
                Title,
                string.Empty,
                true);

            return base.Index(model);
        }

        [HttpGet]
        public override ActionResult Breadcrumbs()
        {
            return PartialView("_Breadcrums");
        }

        protected override IEnumerable<UserLoginTableViewModel> FindResults(UserLoginQueryViewModel query)
        {
            var dbQuery = Mapper.Map<UserLoginQueryModel>(query);
            List<UserLoginTableModel> result;
            using (ContextManager.NewConnection())
            {
                result = adminService.SearchUserLogin(dbQuery);
            }

            return Mapper.Map<List<UserLoginTableViewModel>>(result ?? new List<UserLoginTableModel>());
        }
    }
}