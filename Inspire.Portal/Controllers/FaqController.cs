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
    using Inspire.Common.Mvc.Infrastructure.CustomResult;
    using Inspire.Core.Infrastructure.Logger;
    using Inspire.Core.Infrastructure.ResourceManager;
    using Inspire.Core.Infrastructure.TransactionManager;
    using Inspire.Domain.Services;
    using Inspire.Model.Faq;
    using Inspire.Model.QueryModels;
    using Inspire.Model.TableModels;
    using Inspire.Portal.App_GlobalResources;
    using Inspire.Portal.Models;
    using Inspire.Portal.Models.Faq;
    using Inspire.Portal.Utilities;
    using Inspire.Table.Mvc.Controllers;
    using Inspire.Utilities.Exception;

    using Kendo.Mvc.Extensions;
    using Kendo.Mvc.UI;

    using Breadcrumb = Inspire.Portal.Models.Breadcrumb;

    [CustomAuthorize]
    public class FaqController : SearchTableController<FaqQueryViewModel, FaqTableViewModel>
    {
        private readonly IFaqService faqService;
        private readonly INomenclatureService nomenclatureService;

        public FaqController(
            ILogger logger,
            IMapper mapper,
            IDbContextManager contextManager,
            IResourceManager resource,
            IFaqService faqService,
            INomenclatureService nomenclatureService)
            : base(logger, mapper, contextManager, resource, Resource.FAQ, isSortable: true)
        {
            this.faqService = faqService;
            this.nomenclatureService = nomenclatureService;
        }

        [HttpGet]
        [CustomAuthorize(ApplicationRights.SearchFAQ)]
        public override ActionResult Index(FaqQueryViewModel model = null)
        {
            this.InitViewTitleAndBreadcrumbs(
                Title,
                new[] { new Breadcrumb { Title = Resource.Admin } });
            return base.Index(model);
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
        public override ActionResult Scripts()
        {
            return PartialView("_Scripts");
        }

        [HttpGet]
        public override ActionResult Breadcrumbs()
        {
            return PartialView("_Breadcrums");
        }

        [HttpPost]
        [CustomAuthorize(ApplicationRights.UpsertFAQ)]
        public ActionResult Rearrange(int oldIndex, int newIndex)
        {
            using (var transaction = ContextManager.NewTransaction())
            {
                var faqs = new ObservableCollection<FaqTableModel>(faqService.Search(new FaqQueryModel()));
                faqs.Move(oldIndex, newIndex);
                faqService.Rearrange(faqs.Select(x => x.Id.Value).ToArray());
                transaction.Commit();
            }

            return Json(new { success = true });
        }

        [HttpGet]
        [CustomAuthorize(ApplicationRights.UpsertFAQ)]
        public ActionResult Upsert(Guid? id = null)
        {
            var model = new FaqUpsertViewModel();
            if (id.HasValue)
            {
                using (ContextManager.NewConnection())
                {
                    model = Mapper.Map<FaqUpsertViewModel>(faqService.GetFaq(id.Value, true));
                }
            }

            InitUpsertBreadcrumb(model);
            return View(model);
        }

        [HttpPost]
        [CustomAuthorize(ApplicationRights.UpsertFAQ)]
        public ActionResult Upsert(FaqUpsertViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var dbModel = Mapper.Map<Faq>(model);
                    using (var transaction = ContextManager.NewTransaction())
                    {
                        faqService.Upsert(dbModel);
                        transaction.Commit();
                    }

                    this.ShowMessage(MessageType.Success, Resource.ChangesSuccessfullySaved);
                    return RedirectToAction("Index", new FaqQueryViewModel { Id = model.Id });
                }
                catch (UserDbException e)
                {
                    Logger.Error(e);
                    ModelState.AddModelError(string.Empty, Resource.DbErrorMessage);
                }
                catch (UserException e)
                {
                    ModelState.AddModelError(string.Empty, e.Message);
                }
            }

            InitUpsertBreadcrumb(model);
            return View(model);
        }

        [HttpGet]
        [CustomAuthorize(ApplicationRights.UpsertFAQCategory)]
        public ActionResult ManageCategory()
        {
            return PartialView();
        }

        [HttpGet]
        [CustomAuthorize(ApplicationRights.UpsertFAQCategory)]
        public ActionResult UpsertCategory(Guid? id)
        {
            var model = new FaqCategoryViewModel();
            if (id.HasValue)
            {
                using (ContextManager.NewConnection())
                {
                    model = Mapper.Map<FaqCategoryViewModel>(faqService.GetFaqCategory(id.Value, true));
                }
            }

            return PartialView(model);
        }

        [HttpPost]
        [CustomAuthorize(ApplicationRights.UpsertFAQCategory)]
        public ActionResult UpsertCategory([DataSourceRequest] DataSourceRequest request, FaqCategoryViewModel model)
        {
            if (model != null && ModelState.IsValid)
            {
                using (var transaction = ContextManager.NewTransaction())
                {
                    var entity = Mapper.Map<FaqCategory>(model);
                    model.Id = faqService.UpsertFaqCategory(entity);
                    transaction.Commit();
                }
            }

            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public JsonResult GetFaqCategories()
        {
            return new JsonResultMaxLength(
                SearchAllFaqCagetories()
                    .Select(item => new KeyValuePair<string, string>(item.Id?.ToString(), item.Name))
                    .ToList());
        }

        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public JsonResult GetFaqFilteredCategories(Guid categoryId)
        {
            var result = SearchAllFaqCagetories()
                         .Where(x => x.Id != categoryId)
                         .Select(item => new KeyValuePair<string, string>(item.Id?.ToString(), $"{Resource.MoveTo} {item.Name}"))
                         .ToList();

            return new JsonResultMaxLength(result);
        }

        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public ActionResult SearchFaqCategories([DataSourceRequest] DataSourceRequest request)
        {
            return new JsonResultMaxLength(SearchAllFaqCagetories().ToDataSourceResult(request));
        }

        [HttpPost]
        [CustomAuthorize(ApplicationRights.DeleteFAQ)]
        public void Delete(Guid id, string searchQueryId = null)
        {
            using (var transaction = ContextManager.NewTransaction())
            {
                faqService.Delete(id);
                transaction.Commit();
            }

            RefreshGridItem(searchQueryId, null, model => model.Id == id);
        }

        [HttpDelete]
        [CustomAuthorize(ApplicationRights.DeleteFAQCategory)]
        public ActionResult DeleteCategory(Guid id, Guid? newCategoryId = null)
        {
            using (var transaction = ContextManager.NewTransaction())
            {
                faqService.DeleteFaqCategory(id, newCategoryId);
                transaction.Commit();
            }

            var url = Url.Action("Index");
            if (Request.IsAjaxRequest())
            {
                this.SetAjaxResponseRedirectUrl(url);
                return new EmptyResult();
            }

            return Redirect(url);
        }

        protected override void InitialQuery(FaqQueryViewModel model)
        {
            using (ContextManager.NewConnection())
            {
                var categories = Mapper.Map<List<FaqCategoryViewModel>>(faqService.SearchFaqCategories());
                model.CategoryDataSource = categories
                                           .Select(
                                               item => new KeyValuePair<string, string>(item.Id?.ToString(), item.Name))
                                           .ToList().AddDefaultValue();
                model.StatusDataSource = nomenclatureService.Get("nfaqstatus")
                                                  .Select(
                                                      item => new KeyValuePair<string, string>(item.Id?.ToString(), item.Name))
                                                  .ToList().AddDefaultValue();
            }
        }

        [CustomAuthorize(ApplicationRights.SearchFAQ)]
        protected override IEnumerable<FaqTableViewModel> FindResults(FaqQueryViewModel query)
        {
            List<FaqTableViewModel> result;

            using (ContextManager.NewConnection())
            {
                var faqs = faqService.Search(Mapper.Map<FaqQueryModel>(query));
                result = Mapper.Map<List<FaqTableViewModel>>(faqs);
            }

            return result;
        }

        private List<FaqCategoryViewModel> SearchAllFaqCagetories()
        {
            using (ContextManager.NewConnection())
            {
                return Mapper.Map<List<FaqCategoryViewModel>>(faqService.SearchFaqCategories());
            }
        }

        private void InitUpsertBreadcrumb(FaqUpsertViewModel model)
        {
            this.InitAdminBreadcrumb(
                    Title,
                    model.Id.HasValue
                        ? string.Format(Resource.Editing, model.Question)
                        : Resource.CreateFaq,
                    true);
        }
    }
}