namespace Inspire.Portal.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Web.Mvc;

    using AutoMapper;

    using Inspire.Common.Mvc.Enums;
    using Inspire.Common.Mvc.Extensions;
    using Inspire.Common.Mvc.Filters.CustomAuthorize;
    using Inspire.Core.Infrastructure.Logger;
    using Inspire.Core.Infrastructure.ResourceManager;
    using Inspire.Core.Infrastructure.TransactionManager;
    using Inspire.Domain.Services;
    using Inspire.Model.Provider;
    using Inspire.Model.QueryModels;
    using Inspire.Portal.App_GlobalResources;
    using Inspire.Portal.Models;
    using Inspire.Portal.Models.Provider;
    using Inspire.Portal.Utilities;
    using Inspire.Table.Mvc.Controllers;
    using Inspire.Utilities.Extensions;

    [CustomAuthorize(ApplicationRights.SearchProvider)]
    public class ProviderController : SearchTableController<ProviderQueryModel, ProviderTableViewModel>
    {
        private readonly INomenclatureService nomenclatureService;
        private readonly IProviderService providerService;

        public ProviderController(
            ILogger logger,
            IMapper mapper,
            IDbContextManager contextManager,
            IResourceManager resource,
            IProviderService providerService,
            INomenclatureService nomenclatureService)
            : base(
                logger,
                mapper,
                contextManager,
                resource,
                Resource.Providers,
                isSortable: true,
                disableSearchButton: true)
        {
            this.providerService = providerService;
            this.nomenclatureService = nomenclatureService;
        }

        [HttpGet]
        [CustomAuthorize(ApplicationRights.SearchProvider)]
        public override ActionResult Index(ProviderQueryModel model = null)
        {
            this.InitViewTitleAndBreadcrumbs(
                Title,
                new[] { new Breadcrumb { Title = Resource.Admin } });
            return base.Index(model);
        }

        [HttpGet]
        [CustomAuthorize(ApplicationRights.UpsertProvider)]
        public ActionResult Upsert(Guid? id)
        {
            ProviderViewModel model = null;
            if (id != null)
            {
                using (ContextManager.NewConnection())
                {
                    model = Mapper.Map<ProviderViewModel>(providerService.Get(id.Value, true));
                }
            }

            InitUpsertBreadcrumb(model);
            return View(model);
        }

        [HttpPost]
        [CustomAuthorize(ApplicationRights.UpsertProvider)]
        public ActionResult Upsert(ProviderViewModel model)
        {
            if (!ModelState.IsValid)
            {
                InitUpsertBreadcrumb(model);
                return View(model);
            }

            using (var transaction = ContextManager.NewTransaction())
            {
                model.Id = providerService.Upsert(Mapper.Map<Provider>(model));
                transaction.Commit();
            }

            this.ShowMessage(MessageType.Success, Resource.ChangesSuccessfullySaved);
            return RedirectToAction("Index", new ProviderQueryModel { Id = model.Id });
        }

        [HttpGet]
        public ActionResult GetStatuses()
        {
            using (ContextManager.NewConnection())
            {
                var data = nomenclatureService.Get("nsupplierstatus");
                return Json(data, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpDelete]
        [CustomAuthorize(ApplicationRights.DeleteProvider)]
        public void Delete(Guid id, string searchQueryId = null)
        {
            using (var transaction = ContextManager.NewTransaction())
            {
                providerService.Delete(id);
                transaction.Commit();
            }

            RefreshGridItem(searchQueryId, null, model => model.Id == id);
        }

        [HttpPost]
        [CustomAuthorize(ApplicationRights.UpsertProvider)]
        public ActionResult Rearrange(int oldIndex, int newIndex)
        {
            using (var transaction = ContextManager.NewTransaction())
            {
                var providers = new ObservableCollection<Provider>(providerService.Search(new ProviderQueryModel()));
                providers.Move(oldIndex, newIndex);
                providerService.Rearrange(providers.Select(x => x.Id.Value).ToArray());
                transaction.Commit();
            }

            return Json(new { success = true });
        }

        [HttpGet]
        public override ActionResult ClientTemplate()
        {
            return PartialView("_TableTemplate");
        }

        [HttpGet]
        public override ActionResult TableControls()
        {
            return PartialView("_TableControls");
        }

        [HttpGet]
        public override ActionResult Breadcrumbs()
        {
            return PartialView("_Breadcrums");
        }

        protected override IEnumerable<ProviderTableViewModel> FindResults(ProviderQueryModel query)
        {
            using (ContextManager.NewConnection())
            {
                return Mapper.Map<List<ProviderTableViewModel>>(providerService.Search(query));
            }
        }

        private void InitUpsertBreadcrumb(ProviderViewModel model)
        {
            this.InitAdminBreadcrumb(
                Title,
                model?.Id != null
                    ? string.Format(
                        Resource.Editing,
                        model.Names != null && model.Names.Values.IsNotNullOrEmpty()
                            ? model.Names.Values.FirstOrDefault()
                            : string.Empty)
                    : Resource.CreateProvider,
                true);
        }
    }
}