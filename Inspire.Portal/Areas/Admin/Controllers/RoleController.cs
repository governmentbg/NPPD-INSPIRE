namespace Inspire.Portal.Areas.Admin.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;

    using AutoMapper;

    using Inspire.Common.Mvc.Enums;
    using Inspire.Common.Mvc.Extensions;
    using Inspire.Common.Mvc.Filters.CustomAuthorize;
    using Inspire.Common.Mvc.Infrastructure.CustomResult;
    using Inspire.Core.Infrastructure.Logger;
    using Inspire.Core.Infrastructure.ResourceManager;
    using Inspire.Core.Infrastructure.TransactionManager;
    using Inspire.Domain.Services;
    using Inspire.Model.Nomenclature;
    using Inspire.Model.Role;
    using Inspire.Portal.App_GlobalResources;
    using Inspire.Portal.Areas.Admin.Models.Queries;
    using Inspire.Portal.Areas.Admin.Models.Role;
    using Inspire.Portal.Models;
    using Inspire.Portal.Utilities;
    using Inspire.Table.Mvc.Controllers;
    using Inspire.Utilities.Extensions;

    using Kendo.Mvc.Extensions;
    using Kendo.Mvc.UI;

    [CustomAuthorize]
    public class RoleController : SearchTableController<RoleQueryModel, RoleTableViewModel>
    {
        private readonly IRoleService roleService;

        public RoleController(
            ILogger logger,
            IMapper mapper,
            IDbContextManager contextManager,
            IResourceManager resource,
            IRoleService roleService)
            : base(logger, mapper, contextManager, resource, Resource.Roles)
        {
            this.roleService = roleService;
        }

        [HttpGet]
        [CustomAuthorize(ApplicationRights.SearchRole)]
        public override ActionResult Index(RoleQueryModel model = null)
        {
            this.InitAdminBreadcrumb(
                Title,
                string.Empty,
                true);

            return base.Index(model);
        }

        [HttpGet]
        public override ActionResult TableControls()
        {
            return PartialView("_TableControls");
        }

        [HttpGet]
        [CustomAuthorize(ApplicationRights.UpsertRole)]
        public ActionResult Upsert(Guid? id)
        {
            var model = new RoleUpsertViewModel();
            if (id.HasValue)
            {
                using (ContextManager.NewConnection())
                {
                    model = Mapper.Map<RoleUpsertViewModel>(roleService.Get(id.Value));
                }
            }

            this.InitAdminBreadcrumb(
                Title,
                string.Format(
                    model.Id.HasValue
                        ? string.Format(Resource.Editing, model.Name)
                        : string.Format(Resource.Creating, Resource.Role.ToLower())),
                true);

            return View(model);
        }

        [HttpPost]
        [CustomAuthorize(ApplicationRights.UpsertRole)]
        public ActionResult Upsert(RoleUpsertViewModel model)
        {
            if (!ModelState.IsValid)
            {
                this.InitAdminBreadcrumb(
                    Title,
                    string.Format(
                        model.Id.HasValue
                            ? string.Format(Resource.Editing, model.Name)
                            : string.Format(Resource.Creating, Resource.Role.ToLower())),
                    true);

                return View(model);
            }

            Guid id;
            using (var transaction = ContextManager.NewTransaction())
            {
                id = roleService.Upsert(Mapper.Map<Role>(model));
                transaction.Commit();
            }

            this.ShowMessage(MessageType.Success, Resource.ChangesSuccessfullySaved);
            return RedirectToAction("Index", new { Id = id });
        }

        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        [CustomAuthorize(ApplicationRights.UpsertRole)]
        public ActionResult ReadActivities(Guid? roleId, [DataSourceRequest] DataSourceRequest request)
        {
            Role role = null;
            List<Nomenclature> allActivities;
            using (ContextManager.NewConnection())
            {
                if (roleId.HasValue)
                {
                    role = roleService.Search(new RoleQuery { Id = roleId, UserId = User.Id }).First();
                }

                allActivities = roleService.GetAllActivities().ToList();
            }

            var activities = allActivities.Select(
                item => new Activity
                        {
                            Id = item.Id,
                            Name = item.Name,
                            IsChecked =
                                role != null
                                && role.Activities.IsNotNullOrEmpty()
                                && role.Activities.Any(a => a.Id == item.Id)
                        }).ToList();

            return Json(
                activities.ToDataSourceResult(request ?? new DataSourceRequest()),
                JsonRequestBehavior.AllowGet);
        }

        [HttpDelete]
        [CustomAuthorize(ApplicationRights.DeleteRole)]
        public void Delete(Guid id)
        {
            using (var transaction = ContextManager.NewTransaction())
            {
                roleService.Delete(id);
                transaction.Commit();
            }
        }

        [HttpGet]
        public override ActionResult ClientTemplate()
        {
            return PartialView("_TableTemplate");
        }

        [HttpGet]
        public override ActionResult Scripts()
        {
            return PartialView("_Scripts");
        }

        [HttpGet]
        public override ActionResult Breadcrumbs()
        {
            return PartialView("_Breadcrums");
        }

        [HttpGet]
        public ActionResult GetAllRoles(string text = null)
        {
            List<Nomenclature> result;
            using (ContextManager.NewConnection())
            {
                result = Mapper.Map<List<Nomenclature>>(roleService.Search(new RoleQuery()));
            }

            if (text.IsNotNullOrEmpty())
            {
                return new JsonResultMaxLength(
                    result.Where(item => item.Name.IndexOf(text, StringComparison.InvariantCultureIgnoreCase) >= 0));
            }

            return new JsonResultMaxLength(result);
        }

        [CustomAuthorize(ApplicationRights.SearchRole)]
        protected override IEnumerable<RoleTableViewModel> FindResults(RoleQueryModel query)
        {
            var mappedQuery = Mapper.Map<RoleQuery>(query);
            mappedQuery.UserId = User.Id;

            using (ContextManager.NewConnection())
            {
                return Mapper.Map<List<RoleTableViewModel>>(roleService.Search(mappedQuery));
            }
        }
    }
}