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
    using Inspire.Model;
    using Inspire.Model.TableModels;
    using Inspire.Portal.App_GlobalResources;
    using Inspire.Portal.Areas.Admin.Models.TransactionHistory;
    using Inspire.Portal.Models;
    using Inspire.Portal.Utilities;
    using Inspire.Table.Mvc.Controllers;
    using Inspire.Table.Mvc.Utilities;
    using Inspire.Utilities.Exception;

    [CustomAuthorize(ApplicationRights.TransactionsReport)]
    public class TransactionHistoryController : SearchTableController<TransactionHistoryQueryModel, TransactionHistoryTableViewModel>
    {
        private readonly ITransactionHistoryService transactionHistoryService;

        public TransactionHistoryController(ILogger logger, IMapper mapper, IDbContextManager contextManager, IResourceManager resource, ITransactionHistoryService transactionHistoryService)
            : base(logger, mapper, contextManager, resource, Resource.TransactionHistoryReport, autoSearch: false)
        {
            this.transactionHistoryService = transactionHistoryService;
        }

        [HttpGet]
        public override ActionResult Breadcrumbs()
        {
            return PartialView("_Breadcrums");
        }

        [HttpGet]
        public override ActionResult Index(TransactionHistoryQueryModel model = null)
        {
            if (model == null || !ReflectionUtils.HasNonNullProperty(model))
            {
                model = model ?? new TransactionHistoryQueryModel();
                model.FromDate = DateTime.Now.Date.AddMonths(-1);
                model.ToDate = DateTime.Now.Date;
            }

            this.InitAdminBreadcrumb(Title, string.Empty, true);
            return base.Index(model);
        }

        [HttpGet]
        public FileResult Metadata(Guid identifier)
        {
            byte[] xml;
            using (ContextManager.NewConnection())
            {
                xml = transactionHistoryService.MetadataXml(identifier);
            }

            if (xml?.Length < 1)
            {
                throw new UserException(Resource.NoDataFound);
            }

            var fileName = $"{identifier}.xml";
            return File(xml, MimeTypes.GetMimeType(fileName), fileName);
        }

        [HttpGet]
        public override ActionResult ClientTemplate()
        {
            return PartialView("_TableTemplate");
        }

        protected override IEnumerable<TransactionHistoryTableViewModel> FindResults(TransactionHistoryQueryModel query)
        {
            List<TransactionHistoryTableModel> result;
            var dbQuery = Mapper.Map<Model.QueryModels.TransactionHistoryQueryModel>(query);
            using (ContextManager.NewConnection())
            {
                result = transactionHistoryService.Search(dbQuery);
            }

            return Mapper.Map<List<TransactionHistoryTableViewModel>>(result ?? new List<TransactionHistoryTableModel>());
        }
    }
}