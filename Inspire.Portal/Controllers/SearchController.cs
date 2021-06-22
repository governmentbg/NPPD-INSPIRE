namespace Inspire.Portal.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Web.Mvc;

    using AutoMapper;

    using Inspire.Common.Mvc.Enums;
    using Inspire.Common.Mvc.Extensions;
    using Inspire.Common.Mvc.Infrastructure.BaseTypes;
    using Inspire.Core.Infrastructure.Logger;
    using Inspire.Core.Infrastructure.TransactionManager;
    using Inspire.Domain.Services;
    using Inspire.Portal.App_GlobalResources;
    using Inspire.Portal.Models.Search;
    using Inspire.Portal.Utilities;
    using Inspire.Utilities.Enums;
    using Inspire.Utilities.Extensions;

    [AllowAnonymous]
    public class SearchController : BaseDbController
    {
        private readonly ISearchService searchService;

        public SearchController(
            ILogger logger,
            IMapper mapper,
            IDbContextManager contextManager,
            ISearchService searchService)
            : base(logger, mapper, contextManager)
        {
            this.searchService = searchService;
        }

        [ChildActionOnly]
        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public ActionResult SearchForm()
        {
            return PartialView("_SearchForm");
        }

        [HttpPost]
        public ActionResult Search(string query)
        {
            var result = new List<SearchItemViewModel>();
            if (query.IsNotNullOrEmpty())
            {
                using (ContextManager.NewConnection())
                {
                    result = Mapper.Map<List<SearchItemViewModel>>(searchService.Search(query));
                }
            }

            var isAjax = Request.IsAjaxRequest();
            if (result.IsNullOrEmpty())
            {
                this.ShowMessage(MessageType.Info, Resource.NoDataFound);
                return isAjax
                    ? new EmptyResult()
                    : this.RedirectToRequestPage();
            }

            // Init info url by object type
            foreach (var item in result)
            {
                item.Url = GetObjectInfoUrl(item.Id, item.ObjectType);
            }

            var searchId = Guid.NewGuid();
            Session[$"{searchId}_SearchQuery"] = query;
            Session[$"{searchId}_SearchResult"] = result;

            var url = Url.Action("SearchResult", new { searchId });
            if (isAjax)
            {
                this.SetAjaxResponseRedirectUrl(url, true);
                return new EmptyResult();
            }

            return Redirect(url);
        }

        [HttpGet]
        public ActionResult SearchResult(string searchId)
        {
            var query = ViewBag.Query = Session[$"{searchId}_SearchQuery"] as string;

            this.InitViewTitleAndBreadcrumbs($"{Resource.SearchResult}: {query}");

            var searchItems = Session[$"{searchId}_SearchResult"] as IEnumerable<SearchItemViewModel>;

            return View("SearchResult", searchItems);
        }

        private string GetObjectInfoUrl(Guid objectId, ObjectType type)
        {
            switch (type)
            {
                case ObjectType.Publication:
                    return Url.DynamicAction("Info", typeof(PublicationController), new { id = objectId });

                default:
                    return null;
            }
        }
    }
}