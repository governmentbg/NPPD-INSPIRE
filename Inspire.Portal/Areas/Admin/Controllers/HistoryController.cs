namespace Inspire.Portal.Areas.Admin.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Web.Mvc;

    using AutoMapper;

    using Inspire.Common.Mvc.Enums;
    using Inspire.Common.Mvc.Extensions;
    using Inspire.Common.Mvc.Filters.CustomAuthorize;
    using Inspire.Common.Mvc.Infrastructure.BaseTypes;
    using Inspire.Core.Infrastructure.Logger;
    using Inspire.Core.Infrastructure.TransactionManager;
    using Inspire.Domain.Services;
    using Inspire.Model.Helpers;
    using Inspire.Portal.App_GlobalResources;
    using Inspire.Portal.Models;
    using Inspire.Portal.Utilities;
    using Inspire.Utilities.Enums;
    using Inspire.Utilities.Extensions;

    using Action = Inspire.Model.History.Action;

    [CustomAuthorize]
    public class HistoryController : BaseDbController
    {
        private readonly IHistoryService historyService;

        public HistoryController(
            ILogger logger,
            IMapper mapper,
            IDbContextManager contextManager,
            IHistoryService historyService)
            : base(logger, mapper, contextManager)
        {
            this.historyService = historyService;
        }

        [HttpGet]
        public ActionResult Index(Guid? objectId, ObjectType objectType)
        {
            if (!HasRightToViewHistoryByObjectType(objectType))
            {
                if (Request.IsAjaxRequest())
                {
                    return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
                }

                return RedirectToAction("Forbidden", "Error", new { Area = string.Empty });
            }

            List<Action> history;
            var objectTypeId = EnumHelper.GetObjectIdByObjectTypeId(objectType);
            using (ContextManager.NewConnection())
            {
                history = historyService.GetObjectHistory(objectId, objectTypeId);
            }

            if (Request.IsAjaxRequest())
            {
                if (history.IsNullOrEmpty())
                {
                    this.ShowMessage(MessageType.Info, Resource.NoDataFound);
                    return new EmptyResult();
                }

                return PartialView("Index", history);
            }

            this.InitViewTitleAndBreadcrumbs(Resource.History);
            return View("Index", history);
        }

        private bool HasRightToViewHistoryByObjectType(ObjectType objectType)
        {
            if (User == null)
            {
                return false;
            }

            switch (objectType)
            {
                case ObjectType.User:
                    return User.IsInRole(ApplicationRights.HistoryUser);

                case ObjectType.Publication:
                    return User.IsInRole(ApplicationRights.HistoryNews);

                default:
                    throw new ArgumentOutOfRangeException(nameof(objectType), objectType, null);
            }
        }
    }
}