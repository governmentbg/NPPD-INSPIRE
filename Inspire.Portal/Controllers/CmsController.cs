namespace Inspire.Portal.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Routing;

    using AutoMapper;

    using Inspire.Common.Mvc.Enums;
    using Inspire.Common.Mvc.Extensions;
    using Inspire.Common.Mvc.Filters.CustomAuthorize;
    using Inspire.Common.Mvc.Infrastructure.BaseTypes;
    using Inspire.Common.Mvc.Infrastructure.CustomResult;
    using Inspire.Core.Infrastructure.Logger;
    using Inspire.Core.Infrastructure.TransactionManager;
    using Inspire.Domain.Services;
    using Inspire.Model.Cms;
    using Inspire.Model.Helpers;
    using Inspire.Model.Nomenclature;
    using Inspire.Model.QueryModels;
    using Inspire.Portal.App_GlobalResources;
    using Inspire.Portal.Models;
    using Inspire.Portal.Models.Cms;
    using Inspire.Portal.Services.SessionStorageService;
    using Inspire.Portal.Utilities;
    using Inspire.Utilities.Exception;
    using Inspire.Utilities.Extensions;

    using Kendo.Mvc.Extensions;
    using Kendo.Mvc.UI;

    using Breadcrumb = Inspire.Portal.Models.Breadcrumb;

    [CustomAuthorize(ApplicationRights.UpsertPage)]
    public class CmsController : BaseDbController
    {
        private readonly ICmsService cmsService;
        private readonly INomenclatureService nomenclatureService;
        private readonly ISessionStorageService sessionStorageService;

        public CmsController(ILogger logger, IMapper mapper, IDbContextManager contextManager, ICmsService cmsService, INomenclatureService nomenclatureService, ISessionStorageService sessionStorageService)
            : base(logger, mapper, contextManager)
        {
            this.cmsService = cmsService;
            this.nomenclatureService = nomenclatureService;
            this.sessionStorageService = sessionStorageService;
        }

        [HttpGet]
        public ActionResult Index()
        {
            this.InitAdminBreadcrumb(Resource.Pages, Resource.Pages);

            return View();
        }

        [HttpGet]
        public ActionResult Upsert(Guid? id = null, Guid? parentId = null, string sessionId = null)
        {
            Page page = null;
            if (id.HasValue)
            {
                using (ContextManager.NewConnection())
                {
                    page = cmsService.GetPage(id.Value, true);
                }
            }

            var model = sessionId.IsNotNullOrEmpty()
                ? sessionStorageService.Get<PageUpsertViewModel>(sessionId)
                : Mapper.Map<PageUpsertViewModel>(page ?? new Page { ParentId = parentId });

            this.InitAdminBreadcrumb(
                Resource.Pages,
                string.Format(
                    model?.Id.HasValue == true
                        ? string.Format(Resource.Editing, model.Title)
                        : string.Format(Resource.Creating, Resource.Page.ToLower())),
                true);

            return View(model);
        }

        [HttpPost]
        public ActionResult Upsert(PageUpsertViewModel model, bool preview = false, string sessionId = null)
        {
            if (sessionId.IsNotNullOrEmpty())
            {
                model = sessionStorageService.Get<PageUpsertViewModel>(sessionId);

                ModelState.Clear();
                ValidateModel(model);
            }

            // Validate parent id
            if (model.Id.HasValue && model.ParentId.HasValue)
            {
                List<Page> parentPages;
                using (ContextManager.NewConnection())
                {
                    parentPages = cmsService.GetParentPages(model.Id.Value);
                }

                if (parentPages?.Any(item => item.ParentId == model.ParentId) == true)
                {
                    ModelState.AddModelError("ParentId", string.Format(Resource.InvalidFieldValue, Resource.ParentPage));
                }
            }

            // Custom page validation - add errors to model state
            Validate(model);

            var isAjax = Request.IsAjaxRequest();
            if (ModelState.IsValid)
            {
                if (preview)
                {
                    sessionStorageService.Upsert(model);
                    var url = Url.Action("Preview", new { sessionId = model.UniqueId });
                    if (isAjax)
                    {
                        this.SetAjaxResponseRedirectUrl(url);
                        return new EmptyResult();
                    }

                    return Redirect(url);
                }

                var dbModel = Mapper.Map<Page>(model);
                try
                {
                    using (var transaction = ContextManager.NewTransaction())
                    {
                        cmsService.Upsert(dbModel);
                        transaction.Commit();
                    }

                    this.ShowMessage(MessageType.Success, Resource.ChangesSuccessfullySaved);

                    var redirectUrl = Url.Action("Index");
                    if (isAjax)
                    {
                        this.SetAjaxResponseRedirectUrl(redirectUrl);
                        return new EmptyResult();
                    }

                    return Redirect(redirectUrl);
                }
                catch (UserException exc)
                {
                    ModelState.AddModelError(string.Empty, exc.Message);
                }
            }

            InitBreadcrumb(
                string.Format(
                    model?.Id.HasValue == true
                        ? string.Format(Resource.Editing, model.Title)
                        : string.Format(Resource.Creating, Resource.Page.ToLower())));

            return isAjax
                ? PartialView(model)
                : View(model) as ActionResult;
        }

        [HttpGet]
        public ActionResult Preview(string sessionId)
        {
            var model = sessionStorageService.Get<PageUpsertViewModel>(sessionId);
            InitBreadcrumb($"{Resource.Preview} {model.Title}");
            ViewBag.SessionId = sessionId;

            return View("Render", Mapper.Map<PageViewModel>(model));
        }

        [HttpDelete]
        public void Delete(Guid id)
        {
            using (var transaction = ContextManager.NewTransaction())
            {
                cmsService.Delete(id);
                transaction.Commit();
            }
        }

        [OutputCache(CacheProfile = "Default")]
        [AllowAnonymous]
        [HttpGet]
        public ActionResult Render(Guid id)
        {
            Page page;
            List<Page> parentPages;
            using (ContextManager.NewConnection())
            {
                page = cmsService.GetPage(id, false);
                if (page == null)
                {
                    throw new HttpException(HttpStatusCode.NotFound.GetHashCode(), "Not found");
                }

                if (page.Visibility != VisibilityType.Public && (User?.Identity?.IsAuthenticated != true || page.Visibility != VisibilityType.AuthenticatedUsed))
                {
                    throw new HttpException(HttpStatusCode.Unauthorized.GetHashCode(), "Unauthorized");
                }

                parentPages = cmsService.GetParentPages(page.Id.Value);
            }

            if (page.PageType != PageType.Content)
            {
                var url = this.GetUrl(page.PermanentLink);
                if (url.IsNullOrEmpty())
                {
                    throw new HttpException(HttpStatusCode.NotFound.GetHashCode(), "Not found");
                }

                return Redirect(url);
            }

            var viewModel = Mapper.Map<PageViewModel>(page);

            ViewBag.MetaDescription = viewModel.Title;
            ViewBag.MetaKeywords = viewModel.Keywords;

            var breadcrumbs = parentPages?
                .Select(
                    item => new Breadcrumb
                    {
                        Title = item.Titles.GetValueForCurrentCulture(),
                        Url = this.GetUrl(item.PermanentLink)
                    })
                .ToList();
            breadcrumbs.Reverse();

            this.InitViewTitleAndBreadcrumbs(viewModel.Title, breadcrumbs);

            return View("Render", viewModel);
        }

        public JsonResult ReadAllPages([DataSourceRequest] DataSourceRequest request)
        {
            List<Page> pages;
            using (ContextManager.NewConnection())
            {
                pages = cmsService.SearchPages(new PageQueryModel());
            }

            var data = Mapper.Map<List<PageViewModel>>(pages ?? new List<Page>());
            var result = data.ToTreeDataSourceResult(
                request,
                e => e.DbId,
                e => e.ParentDbId,
                e => e);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult ReadParentPages(Guid? id = null)
        {
            List<Page> pages;
            using (ContextManager.NewConnection())
            {
                pages = cmsService.SearchPages(new PageQueryModel());
            }

            var data = pages
                        .Where(
                            item => id.HasValue
                                ? item.ParentId == id
                                : item.ParentId == null)
                        .Select(
                            item => new
                            {
                                Id = item.Id,
                                Title = item.Titles.GetValueForCurrentCulture(),
                                hasChildren = pages.Any(p => p.ParentId == item.Id)
                            });

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetPageTypes()
        {
            List<Nomenclature> types;
            using (ContextManager.NewConnection())
            {
                types = nomenclatureService.Get("npagetype");
            }

            return new JsonResultMaxLength(types);
        }

        [HttpGet]
        public ActionResult GetPageVisibilityTypes()
        {
            List<Nomenclature> types;
            using (ContextManager.NewConnection())
            {
                types = nomenclatureService.Get("npagevisibility");
            }

            return new JsonResultMaxLength(types);
        }

        [HttpGet]
        public ActionResult GetPageLocationTypes()
        {
            List<Nomenclature> types;
            using (ContextManager.NewConnection())
            {
                types = nomenclatureService.Get("npagelocation");
            }

            return new JsonResultMaxLength(types);
        }

        [HttpPost]
        public void ChangePosition(Guid sourceId, Guid destinationId, DropPositionType position)
        {
            using (var transaction = ContextManager.NewTransaction())
            {
                var source = cmsService.SearchPages(new PageQueryModel { Id = sourceId }).First();
                var destination = cmsService.SearchPages(new PageQueryModel { Id = destinationId }).First();

                long newPosition = 1;
                Guid? newParentId = null;
                switch (position)
                {
                    case DropPositionType.Before:
                        {
                            newPosition = destination.Order;
                            newParentId = destination.ParentId;

                            break;
                        }

                    case DropPositionType.After:
                        {
                            newPosition = destination.Order + (source.ParentId == destination.ParentId ? 2 : 1);
                            newParentId = destination.ParentId;

                            break;
                        }

                    case DropPositionType.Over:
                        {
                            newParentId = destination.Id;

                            break;
                        }
                }

                cmsService.ChangePagePosition(sourceId, newPosition, newParentId);

                transaction.Commit();
            }
        }

        private void InitBreadcrumb(string title)
        {
            var breadcrumbs = new List<Breadcrumb>
                              {
                                  new Breadcrumb
                                  {
                                      Url = this.GetUrl(typeof(CmsController), "Index"),
                                      Title = Resource.Pages
                                  }
                              };

            this.InitViewTitleAndBreadcrumbs(title, breadcrumbs);
        }

        private void Validate(PageUpsertViewModel model)
        {
            if (model?.Type?.Id.HasValue == true && EnumHelper.GetPageTypeById(model.Type.Id.Value) == PageType.Content)
            {
                var url = this.GetUrl(model.PermanentLink);
                if (url.IsNotNullOrEmpty() && Url.IsLocalUrl(url))
                {
                    var absoluteUrl = this.GetUrl(model.PermanentLink, true);
                    var uri = new Uri(absoluteUrl);
                    var httpContext = new HttpContext(
                        new HttpRequest(null, uri.GetLeftPart(UriPartial.Path), uri.Query),
                        new HttpResponse(new System.IO.StringWriter()));
                    var baseHttpContext = new HttpContextWrapper(httpContext);

                    var routeData = Url.RouteCollection.GetRouteData(baseHttpContext);
                    if (routeData?.RouteHandler != null)
                    {
                        try
                        {
                            var httpHandler = routeData.RouteHandler.GetHttpHandler(new RequestContext(baseHttpContext, routeData));
                            httpHandler.ProcessRequest(httpContext);

                            if (baseHttpContext.Response.StatusCode == HttpStatusCode.OK.GetHashCode())
                            {
                                var addError = true;
                                if (model.Id.HasValue)
                                {
                                    List<Page> pages;
                                    using (ContextManager.NewConnection())
                                    {
                                        pages = cmsService.SearchPages(new PageQueryModel { PermanentLink = model.PermanentLink });
                                    }

                                    addError = pages?.Count(
                                        item => item.PermanentLink?.Equals(
                                                    model.PermanentLink,
                                                    StringComparison.InvariantCultureIgnoreCase) == true &&
                                                item.Id == model.Id) != 1;
                                }

                                if (addError)
                                {
                                    ModelState.AddModelError(string.Empty, string.Format(Resource.PageWithLinkAlreadyExistMessage, model.PermanentLink));
                                }
                            }
                        }
                        catch
                        {
                        }
                    }
                }
            }
        }
    }
}