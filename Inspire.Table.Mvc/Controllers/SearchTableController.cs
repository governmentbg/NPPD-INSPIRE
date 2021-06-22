namespace Inspire.Table.Mvc.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;

    using AutoMapper;

    using Inspire.Common.Mvc.Filters;
    using Inspire.Common.Mvc.Infrastructure.BaseTypes;
    using Inspire.Common.Mvc.Infrastructure.CustomResult;
    using Inspire.Core.Infrastructure.Logger;
    using Inspire.Core.Infrastructure.ResourceManager;
    using Inspire.Core.Infrastructure.TransactionManager;
    using Inspire.Table.Mvc.Helpers;
    using Inspire.Table.Mvc.Models;
    using Inspire.Table.Mvc.Utilities;
    using Inspire.Table.Mvc.Utilities.Export;
    using Inspire.Utilities.Extensions;

    using Kendo.Mvc;
    using Kendo.Mvc.Extensions;
    using Kendo.Mvc.Infrastructure;
    using Kendo.Mvc.UI;

    [DisableCache]
    public abstract class SearchTableController<TQueryModel, TViewTableModel> : BaseDbController
        where TQueryModel : class, new()
        where TViewTableModel : class, new()
    {
        protected readonly bool AutoSearch;
        protected readonly bool Groupable;
        protected readonly bool SavePreviousGridState;
        protected readonly string SearchText;
        protected readonly bool Selectable;
        protected readonly bool ServerOperation;
        protected readonly bool IsSortable;
        protected readonly bool DisableSearchButton;
        private readonly IResourceManager resource;

        protected SearchTableController(
            ILogger logger,
            IMapper mapper,
            IDbContextManager contextManager,
            IResourceManager resource,
            string title = null,
            bool serverOperation = true,
            bool savePreviousGridState = false,
            bool autoSearch = true,
            bool selectable = true,
            bool groupable = false,
            string tableClass = "smallform",
            bool isRequestAjax = false,
            bool isSortable = false,
            bool disableSearchButton = false)
            : base(logger, mapper, contextManager)
        {
            this.resource = resource;
            SearchText = resource.Get("Search") ?? "Search";
            Title = title;
            SavePreviousGridState = savePreviousGridState;
            AutoSearch = autoSearch;
            ServerOperation = serverOperation;
            Selectable = selectable;
            Groupable = groupable;
            TableClass = tableClass;
            IsRequestAjax = isRequestAjax;
            IsSortable = isSortable;
            DisableSearchButton = disableSearchButton;
        }

        public string Title { get; private set; }

        protected bool IsRequestAjax { get; set; }

        protected string TableClass { get; set; }

        protected virtual string Layout { get; }

        protected virtual IEqualityComparer<TViewTableModel> ViewTableModelComparer { get; set; }

        protected Dictionary<string, QueryData<TQueryModel, TViewTableModel>> SearchQueries
        {
            get
            {
                var sessionName = $"{typeof(TQueryModel).Name}_{typeof(TViewTableModel).Name}";
                if (!(Session[sessionName] is Dictionary<string, QueryData<TQueryModel, TViewTableModel>>))
                {
                    Session[sessionName] = new Dictionary<string, QueryData<TQueryModel, TViewTableModel>>();
                }

                return (Dictionary<string, QueryData<TQueryModel, TViewTableModel>>)Session[sessionName];
            }
        }

        [HttpGet]
        public virtual ActionResult Index(TQueryModel model = null)
        {
            ViewBag.SearchButtonText = SearchText;
            ViewBag.Title = Title;
            ViewBag.Layout = Layout;
            var searchModel = model != null && ReflectionUtils.HasNonNullProperty(model)
                ? model
                : Activator.CreateInstance<TQueryModel>();

            InitialQuery(searchModel);

            ViewBag.BodyWrapperClass = $"nav-md {GetControllerName()}WrapperClass";
            ViewBag.AutoSearch = AutoSearch;
            ViewBag.SavePreviousGridState = SavePreviousGridState;
            ViewBag.TableClass = TableClass;
            ViewBag.IsAjaxRequest = IsRequestAjax;
            ViewBag.DisableSearchButton = DisableSearchButton;
            return Request.IsAjaxRequest()
                ? PartialView("SearchForm", searchModel)
                : View("SearchForm", searchModel) as ActionResult;
        }

        [HttpGet]
        public virtual ActionResult Scripts()
        {
            return PartialView("_Scripts");
        }

        [HttpGet]
        public virtual ActionResult Breadcrumbs()
        {
            return new EmptyResult();
        }

        [HttpGet]
        public virtual ActionResult AdditionalSearchButtons()
        {
            return new EmptyResult();
        }

        [HttpGet]
        public virtual ActionResult TableControls()
        {
            return new EmptyResult();
        }

        [HttpGet]
        public virtual ActionResult TableEndControls()
        {
            return new EmptyResult();
        }

        [HttpGet]
        public virtual ActionResult ClientTemplate()
        {
            return new EmptyResult();
        }

        [HttpGet]
        public virtual ActionResult SearchNoAjax(TQueryModel query, string searchQueryId = null)
        {
            var result = InitSearchQueriesAndViewBags(query, searchQueryId);

            return PartialView("_SearchResultsTable", result);
        }

        [HttpGet]
        public virtual ActionResult Search(TQueryModel query, string searchQueryId = null)
        {
            ViewBag.Search = true;
            var result = InitSearchQueriesAndViewBags(query, searchQueryId);
            if (IsRequestAjax)
            {
                return PartialView("_SearchResultsTable", result);
            }

            return Index(query);
        }

        [HttpGet]
        public virtual ActionResult RenderCustomGridScripts(string gridId)
        {
            return new EmptyResult();
        }

        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public virtual ActionResult Grid_ReadData(string searchQueryId, [DataSourceRequest] DataSourceRequest request)
        {
            request = request ?? new DataSourceRequest();

            ModifyFilters(request.Filters);

            if (SearchQueries.ContainsKey(searchQueryId))
            {
                SearchQueries[searchQueryId].DataSourceRequest = request;
            }
            else
            {
                SearchQueries.Add(
                    searchQueryId,
                    new QueryData<TQueryModel, TViewTableModel>
                    {
                        QueryModel = Activator.CreateInstance<TQueryModel>(),
                        DataSourceRequest = request
                    });
            }

            // Find result and create dataSource
            var result = GetFindResult(searchQueryId) ?? new List<TViewTableModel>();
            var tableData = result.ToDataSourceResult(request);

            // Check selected items in dataSource
            var data = InitGridData(request, result);

            var checkBoxPropertyData = SearchTableHelpers
                                       .GetPropertiesTableData(data).FirstOrDefault(item => item.Item1.IsCheckbox);
            var checkBoxPropertyName = checkBoxPropertyData?.Item1?.Name;
            if (!string.IsNullOrWhiteSpace(checkBoxPropertyName))
            {
                if (SearchQueries[searchQueryId].SelectedItems.IsNotNullOrEmpty())
                {
                    if (checkBoxPropertyName.IsNotNullOrEmpty())
                    {
                        foreach (var selectedItem in data)
                        {
                            selectedItem.GetType().GetProperty(checkBoxPropertyName).SetValue(
                                selectedItem,
                                SearchQueries[searchQueryId]
                                    .SelectedItems.Contains(selectedItem, ViewTableModelComparer),
                                null);
                        }
                    }
                }
                else
                {
                    if (data.IsNotNullOrEmpty())
                    {
                        foreach (var selectedItem in data)
                        {
                            selectedItem.GetType().GetProperty(checkBoxPropertyName)
                                        .SetValue(selectedItem, false, null);
                        }
                    }
                }
            }

            return new JsonResultMaxLength(tableData, JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public virtual FileResult Export(
            [DataSourceRequest] DataSourceRequest request,
            string searchQueryId,
            ExportFactory.ExportType type)
        {
            var data = ExportData(request, searchQueryId, type);

            return File(
                data.Value,
                MimeMapping.GetMimeMapping(data.Key),
                data.Key);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public void ChangeSelectedItems(
            string searchQueryId,
            SelectOperationType selectOperationType,
            IEnumerable<TViewTableModel> selectedObjects = null)
        {
            selectedObjects = selectedObjects?.Where(item => item != null);
            if (!SearchQueries.ContainsKey(searchQueryId))
            {
                SearchQueries[searchQueryId] = new QueryData<TQueryModel, TViewTableModel>
                {
                    SelectedItems = new HashSet<TViewTableModel>()
                };
            }

            if (SearchQueries[searchQueryId].SelectedItems.IsNullOrEmpty())
            {
                SearchQueries[searchQueryId].SelectedItems = new HashSet<TViewTableModel>();
            }

            var existSelectedObjects = SearchQueries[searchQueryId].SelectedItems;
            switch (selectOperationType)
            {
                case SelectOperationType.Add:
                    {
                        if (selectedObjects.IsNotNullOrEmpty())
                        {
                            SearchQueries[searchQueryId].SelectedItems = new HashSet<TViewTableModel>(
                                existSelectedObjects.Union(selectedObjects, ViewTableModelComparer));
                        }

                        break;
                    }

                case SelectOperationType.Remove:
                    {
                        if (selectedObjects.IsNotNullOrEmpty())
                        {
                            SearchQueries[searchQueryId].SelectedItems = new HashSet<TViewTableModel>(
                                existSelectedObjects.Except(selectedObjects, ViewTableModelComparer));
                        }

                        break;
                    }

                case SelectOperationType.AddAll:
                    {
                        var result = GetFindResult(searchQueryId);
                        SearchQueries[searchQueryId].SelectedItems = new HashSet<TViewTableModel>(result);
                        break;
                    }

                case SelectOperationType.RemoveAll:
                    {
                        SearchQueries[searchQueryId].SelectedItems.Clear();
                        break;
                    }

                default:
                    {
                        throw new ArgumentOutOfRangeException();
                    }
            }
        }

        protected IEnumerable<TViewTableModel> GetSearchResultByQueryId(
            string searchQueryId,
            [DataSourceRequest] DataSourceRequest request = null)
        {
            IEnumerable<TViewTableModel> result = null;
            if (SearchQueries.ContainsKey(searchQueryId))
            {
                if (request == null)
                {
                    request = SearchQueries[searchQueryId].DataSourceRequest ?? new DataSourceRequest();
                }

                request.PageSize = 0;
                result = GetFindResult(searchQueryId);
            }

            return result.IsNotNullOrEmpty()
                ? result.ToDataSourceResult(request).Data as IEnumerable<TViewTableModel>
                : null;
        }

        protected IEnumerable<TViewTableModel> GetSelectedItems(string searchQueryId)
        {
            return SearchQueries.ContainsKey(searchQueryId) ? SearchQueries[searchQueryId].SelectedItems : null;
        }

        protected IEnumerable<TViewTableModel> GetFindResult(string searchQueryId)
        {
            if (searchQueryId.IsNullOrEmpty() || !SearchQueries.ContainsKey(searchQueryId))
            {
                return null;
            }

            return SearchQueries[searchQueryId].FindResult;
        }

        protected TQueryModel GetQueryModel(string searchQueryId)
        {
            if (searchQueryId.IsNullOrEmpty() || !SearchQueries.ContainsKey(searchQueryId))
            {
                return null;
            }

            return SearchQueries[searchQueryId].QueryModel;
        }

        protected void RefreshGridItem(
            string searchQueryId,
            TViewTableModel newItem,
            Predicate<TViewTableModel> itemsMatch)
        {
            var searchQuery = searchQueryId.IsNotNullOrEmpty() && SearchQueries.ContainsKey(searchQueryId)
                ? SearchQueries[searchQueryId].FindResult
                : null;

            if (searchQuery != null)
            {
                var collection = searchQuery is List<TViewTableModel>
                    ? (List<TViewTableModel>)searchQuery
                    : searchQuery.ToList();

                var itemIndex = collection.FindIndex(itemsMatch);
                if (newItem != null)
                {
                    // Replace item with new value
                    if (itemIndex >= 0)
                    {
                        collection[itemIndex] = newItem;
                    }
                    else
                    {
                        collection.Add(newItem);
                    }
                }
                else
                {
                    // Remove index if not exist
                    if (itemIndex >= 0)
                    {
                        collection.RemoveAt(itemIndex);
                    }
                }

                SearchQueries[searchQueryId].FindResult = collection;
            }
        }

        protected virtual void InitialQuery(TQueryModel model)
        {
        }

        protected abstract IEnumerable<TViewTableModel> FindResults(TQueryModel query);

        protected virtual KeyValuePair<string, byte[]> ExportData(
            [DataSourceRequest] DataSourceRequest request,
            string searchQueryId,
            ExportFactory.ExportType type,
            string firstLevelName = null,
            string secondLevelName = null)
        {
            request = request ?? new DataSourceRequest();
            request.PageSize = 0;

            var result = GetSelectedItems(searchQueryId);
            if (result.IsNullOrEmpty())
            {
                result = GetFindResult(searchQueryId);
            }

            var data = InitGridData(request, result);
            if (data.IsNullOrEmpty())
            {
                throw new WarningException(resource.Get("NoDataFound") ?? "No data found");
            }

            var extension = ExportFactory.GetFileExtensionByType(type);
            var fileName = $"{(Title.IsNotNullOrEmpty() ? Title : "export")}-{DateTime.Now:d}.{extension}";

            var exporter = ExportFactory.CreateExport(type);
            var blob = exporter.Export(data, firstLevelName, secondLevelName);

            return new KeyValuePair<string, byte[]>(fileName, blob);
        }

        private IEnumerable<TViewTableModel> InitSearchQueriesAndViewBags(TQueryModel query, string searchQueryId = null)
        {
            ReflectionUtils.ExtendSearchObjectProps(ref query);

            var result = FindResults(query);

            searchQueryId = searchQueryId ?? Guid.NewGuid().ToString();
            if (SearchQueries.ContainsKey(searchQueryId))
            {
                SearchQueries[searchQueryId].QueryModel = query;
                SearchQueries[searchQueryId].FindResult = result;
            }
            else
            {
                SearchQueries[searchQueryId] = new QueryData<TQueryModel, TViewTableModel>
                {
                    QueryModel = query,
                    FindResult = result
                };
            }

            ViewBag.SearchQueryId = searchQueryId;
            ViewBag.ServerOperation = ServerOperation;
            ViewBag.Selectable = Selectable;
            ViewBag.Groupable = Groupable;
            ViewBag.IsSortable = IsSortable;

            return result;
        }

        private void ModifyFilters(IEnumerable<IFilterDescriptor> filters)
        {
            if (filters.IsNullOrEmpty())
            {
                return;
            }

            foreach (var filter in filters)
            {
                var descriptor = filter as FilterDescriptor;
                var stringData = descriptor?.Value as string;
                if (stringData.IsNotNullOrEmpty())
                {
                    descriptor.Value = stringData.Trim();
                }
                else
                {
                    var filterDescriptor = filter as CompositeFilterDescriptor;
                    if (filterDescriptor != null)
                    {
                        ModifyFilters(filterDescriptor.FilterDescriptors);
                    }
                }
            }
        }

        private List<TViewTableModel> InitGridData(DataSourceRequest request, IEnumerable<TViewTableModel> result)
        {
            var data = new List<TViewTableModel>();
            var isGrouped = request.Groups.IsNotNullOrEmpty();
            if (isGrouped)
            {
                var groups = result?.ToDataSourceResult(request)?.Data?.Cast<AggregateFunctionsGroup>().ToList();
                if (groups.IsNotNullOrEmpty())
                {
                    foreach (var group in groups)
                    {
                        if (group.HasSubgroups)
                        {
                            InitItemsFromSubGroups(group.Subgroups, data);
                        }
                        else
                        {
                            data.AddRange(group.Items.Cast<TViewTableModel>());
                        }
                    }
                }
            }
            else
            {
                data = result.IsNotNullOrEmpty()
                    ? result.ToDataSourceResult(request).Data as List<TViewTableModel>
                    : null;
            }

            return data;
        }

        private void InitItemsFromSubGroups(ReadOnlyCollection<IGroup> groups, List<TViewTableModel> data)
        {
            foreach (var groupSubgroup in groups)
            {
                if (groupSubgroup.HasSubgroups)
                {
                    InitItemsFromSubGroups(groupSubgroup.Subgroups, data);
                }
                else
                {
                    data.AddRange(groupSubgroup.Items.Cast<TViewTableModel>());
                }
            }
        }
    }
}