namespace Inspire.Portal.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
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
    using Inspire.Model.Helpers;
    using Inspire.Model.Nomenclature;
    using Inspire.Model.Publication;
    using Inspire.Portal.App_GlobalResources;
    using Inspire.Portal.Models;
    using Inspire.Portal.Models.Publication;
    using Inspire.Portal.Services.SessionStorageService;
    using Inspire.Portal.Utilities;
    using Inspire.Table.Mvc.Controllers;
    using Inspire.Utilities.Extensions;

    [CustomAuthorize]
    public class PublicationController : SearchTableController<PublicationQueryModel, PublicationTableViewModel>
    {
        private readonly INomenclatureService nomenclatureService;
        private readonly IPublicationService publicationService;
        private readonly ISessionStorageService sessionStorageService;

        public PublicationController(
            ILogger logger,
            IMapper mapper,
            IDbContextManager contextManager,
            IResourceManager resource,
            INomenclatureService nomenclatureService,
            IPublicationService publicationService,
            ISessionStorageService sessionStorageService)
            : base(logger, mapper, contextManager, resource, Resource.Publications)
        {
            this.nomenclatureService = nomenclatureService;
            this.publicationService = publicationService;
            this.sessionStorageService = sessionStorageService;
        }

        [HttpGet]
        [CustomAuthorize(ApplicationRights.SearchPublication)]
        public override ActionResult Index(PublicationQueryModel model = null)
        {
            this.InitViewTitleAndBreadcrumbs(
                Title,
                new[] { new Breadcrumb { Title = Resource.Admin } });
            return base.Index(model);
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

        [HttpGet]
        [CustomAuthorize(ApplicationRights.UpsertPublication)]
        public ActionResult Upsert(PublicationType? type = null, Guid? id = null, string sessionId = null)
        {
            PublicationUpsertViewModel model;
            if (id.HasValue)
            {
                using (ContextManager.NewConnection())
                {
                    model = Mapper.Map<PublicationUpsertViewModel>(publicationService.Get(id.Value, true));
                }

                sessionStorageService.Upsert(model);
            }
            else if (sessionId.IsNotNullOrEmpty())
            {
                model = sessionStorageService.Get<PublicationUpsertViewModel>(sessionId);
            }
            else if (type.HasValue)
            {
                if (!EnumHelper.PublicationTypes.ContainsKey(type.Value) || type.Value == PublicationType.None)
                {
                    throw new ArgumentOutOfRangeException("type");
                }

                model = new PublicationUpsertViewModel
                        {
                            Type = new Nomenclature
                                   {
                                       Id = EnumHelper.GetPublicationType(type.Value)
                                   }
                        };
            }
            else
            {
                throw new WarningException(Resource.NoDataFound);
            }

            InitUpsertBreadcrumb(model);
            return View(model);
        }

        [HttpPost]
        [CustomAuthorize(ApplicationRights.UpsertPublication)]
        public ActionResult Upsert(PublicationUpsertViewModel model, bool preview = false)
        {
            if (!ModelState.IsValid)
            {
                InitUpsertBreadcrumb(model);
                return View(model);
            }

            if (preview)
            {
                model.EnterDate = sessionStorageService.Get<PublicationUpsertViewModel>(model.UniqueId)?.EnterDate;
                sessionStorageService.Upsert(model);

                return RedirectToAction("Preview", new { sessionId = model.UniqueId });
            }

            using (var transaction = ContextManager.NewTransaction())
            {
                model.Id = publicationService.Upsert(Mapper.Map<Publication>(model));
                transaction.Commit();
            }

            this.ShowMessage(MessageType.Success, Resource.ChangesSuccessfullySaved);
            return RedirectToAction("Index", new PublicationQueryModel { Id = model.Id });
        }

        [HttpGet]
        [CustomAuthorize(ApplicationRights.UpsertPublication)]
        public ActionResult Preview(string sessionId)
        {
            var model = sessionStorageService.Get<PublicationUpsertViewModel>(sessionId);
            InitBreadcrumb(model.Title, model.Type.Id);
            ViewBag.IsPreview = true;

            return View("Info", Mapper.Map<PublicationPublicViewModel>(model));
        }

        [HttpPost]
        [CustomAuthorize(ApplicationRights.UpsertPublication)]
        public ActionResult Save(string sessionId = null)
        {
            if (!sessionId.IsNotNullOrEmpty())
            {
                throw new ArgumentOutOfRangeException();
            }

            var model = sessionStorageService.Get<PublicationUpsertViewModel>(sessionId);
            using (var transaction = ContextManager.NewTransaction())
            {
                model.Id = publicationService.Upsert(Mapper.Map<Publication>(model));
                transaction.Commit();
            }

            var url = Url.Action("Index", "Publication", new PublicationQueryModel { Id = model.Id });

            this.ShowMessage(MessageType.Success, Resource.ChangesSuccessfullySaved);
            if (Request.IsAjaxRequest())
            {
                this.SetAjaxResponseRedirectUrl(url, true);
                return new EmptyResult();
            }

            return Redirect(url);
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("News")]
        public ActionResult News()
        {
            return GetActivePublicationsByType(PublicationType.News);
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("Events")]
        public ActionResult Events(DateTime? date = null)
        {
            Func<PublicationPublicViewModel, bool> filter = null;
            if (date.HasValue)
            {
                filter = model => model.StartDate?.Date == date.Value.Date;
            }

            return GetActivePublicationsByType(PublicationType.Event, filter);
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult Info(Guid id)
        {
            PublicationPublicViewModel model;
            using (ContextManager.NewConnection())
            {
                model = Mapper.Map<PublicationPublicViewModel>(publicationService.Get(id, false));
            }

            InitBreadcrumb(model.Title, model.Type.Id, false, false);

            return View(model);
        }

        [HttpDelete]
        [CustomAuthorize(ApplicationRights.DeletePublication)]
        public void Delete(Guid id, string searchQueryId = null)
        {
            using (var transaction = ContextManager.NewTransaction())
            {
                publicationService.Delete(id);
                transaction.Commit();
            }

            RefreshGridItem(searchQueryId, null, model => model.Id == id);
        }

        [HttpGet]
        public override ActionResult ClientTemplate()
        {
            return PartialView("_TableTemplate");
        }

        [CustomAuthorize(ApplicationRights.SearchPublication)]
        protected override IEnumerable<PublicationTableViewModel> FindResults(PublicationQueryModel query)
        {
            using (ContextManager.NewConnection())
            {
                return Mapper.Map<List<PublicationTableViewModel>>(
                    publicationService.Search(Mapper.Map<PublicationQuery>(query)));
            }
        }

        protected override void InitialQuery(PublicationQueryModel model)
        {
            List<Nomenclature> statuses;
            using (ContextManager.NewConnection())
            {
                statuses = nomenclatureService.Get("nnewtype").ToList();
            }

            model.TypeIdDataSource = statuses
                                     .Select(item => new KeyValuePair<string, string>(item.Id?.ToString(), item.Name))
                                     .ToList();
            model.TypeIdDataSource.Insert(0, new KeyValuePair<string, string>(null, Resource.All));

            model.IsVisibleDataSource = new List<KeyValuePair<string, string>>
                                        {
                                            new KeyValuePair<string, string>(null, Resource.All),
                                            new KeyValuePair<string, string>("true", Resource.Yes),
                                            new KeyValuePair<string, string>("false", Resource.No)
                                        };
        }

        private void InitBreadcrumb(string title, Guid? type = null, bool isUpsert = false, bool showTitle = true)
        {
            List<Breadcrumb> breadcrumbs = null;
            if (isUpsert)
            {
                breadcrumbs = new List<Breadcrumb>
                              {
                                  new Breadcrumb
                                  {
                                      Url = this.GetUrl(typeof(PublicationController), "Index"),
                                      Title = Resource.Publications
                                  }
                              };
            }

            if (type != null)
            {
                var pubType = EnumHelper.GetPublicationTypeById(type.Value);

                breadcrumbs = new List<Breadcrumb>
                              {
                                  new Breadcrumb
                                  {
                                      Url = this.GetUrl(
                                          typeof(PublicationController),
                                          pubType == PublicationType.News ? "News" : "Events"),
                                      Title = pubType == PublicationType.News ? Resource.News : Resource.Events
                                  }
                              };
            }

            this.InitViewTitleAndBreadcrumbs(title, breadcrumbs, showTitle);
        }

        private void InitUpsertBreadcrumb(PublicationUpsertViewModel model)
        {
            this.InitAdminBreadcrumb(
                Title,
                model.Id.HasValue
                    ? string.Format(Resource.Editing, model.Title)
                    : model.Type != null && model.Type.Id == EnumHelper.GetPublicationType(PublicationType.Event)
                        ? Resource.CreateEvent
                        : Resource.CreateNews,
                true);
        }

        private ActionResult GetActivePublicationsByType(
            PublicationType type,
            Func<PublicationPublicViewModel, bool> filter = null)
        {
            List<Publication> result;
            using (ContextManager.NewConnection())
            {
                result = publicationService.GetVisiblePublicationsByType(type);
            }

            var publications = Mapper.Map<List<PublicationPublicViewModel>>(result ?? new List<Publication>());
            if (filter != null)
            {
                publications = publications.Where(filter).ToList();
            }

            PublicContentHelper.DecodeAndTrimContent(publications, ConfigurationReader.TrimLength);
            InitBreadcrumb(type == PublicationType.News ? Resource.News : Resource.Events);
            ViewBag.PublicationType = type;
            return View("Publications", publications);
        }
    }
}