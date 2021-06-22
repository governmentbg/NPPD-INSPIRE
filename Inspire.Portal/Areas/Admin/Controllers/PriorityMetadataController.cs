namespace Inspire.Portal.Areas.Admin.Controllers
{
    using System.Collections.Generic;
    using System.Web.Mvc;

    using AutoMapper;

    using Inspire.Common.Mvc.Filters.CustomAuthorize;
    using Inspire.Core.Infrastructure.Logger;
    using Inspire.Core.Infrastructure.ResourceManager;
    using Inspire.Core.Infrastructure.TransactionManager;
    using Inspire.Domain.Services;
    using Inspire.Model.TableModels;
    using Inspire.Portal.App_GlobalResources;
    using Inspire.Portal.Areas.Admin.Models.PriorityMetadata;
    using Inspire.Portal.Models;
    using Inspire.Portal.Utilities;
    using Inspire.Table.Mvc.Controllers;

    [CustomAuthorize(ApplicationRights.PriorityMetadataReport)]
    public class PriorityMetadataController
        : SearchTableController<PriorityMetadataQueryModel, PriorityMetadataTableViewModel>
    {
        private readonly IGeonetworkService geonetworkService;

        public PriorityMetadataController(
            ILogger logger,
            IMapper mapper,
            IDbContextManager contextManager,
            IResourceManager resource,
            IGeonetworkService geonetworkService)
            : base(logger, mapper, contextManager, resource, Resource.PriorityMetadata, autoSearch: true)
        {
            this.geonetworkService = geonetworkService;
        }

        [HttpGet]
        public override ActionResult Index(PriorityMetadataQueryModel model = null)
        {
            this.InitAdminBreadcrumb(Title, string.Empty, true);
            return base.Index(model);
        }

        [HttpGet]
        public override ActionResult Breadcrumbs()
        {
            return PartialView("_Breadcrums");
        }

        [HttpGet]
        public override ActionResult ClientTemplate()
        {
            return PartialView("_MetadataTableTemplate");
        }

        protected override IEnumerable<PriorityMetadataTableViewModel> FindResults(PriorityMetadataQueryModel query)
        {
            List<PriorityMetadataTableModel> result;
            using (ContextManager.NewConnection())
            {
                result = geonetworkService.GetPriorityData();
            }

            return Mapper.Map<List<PriorityMetadataTableViewModel>>(
                result ?? new List<PriorityMetadataTableModel>());
        }
    }
}