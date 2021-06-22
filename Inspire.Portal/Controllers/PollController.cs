namespace Inspire.Portal.Controllers
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
    using Inspire.Core.Infrastructure.Cache;
    using Inspire.Core.Infrastructure.Logger;
    using Inspire.Core.Infrastructure.ResourceManager;
    using Inspire.Core.Infrastructure.TransactionManager;
    using Inspire.Domain.Services;
    using Inspire.Model.Helpers;
    using Inspire.Model.Nomenclature;
    using Inspire.Model.Poll;
    using Inspire.Model.QueryModels;
    using Inspire.Model.TableModels;
    using Inspire.Portal.App_GlobalResources;
    using Inspire.Portal.Models;
    using Inspire.Portal.Models.Poll;
    using Inspire.Portal.Services.SessionStorageService;
    using Inspire.Portal.Utilities;
    using Inspire.Table.Mvc.Controllers;
    using Inspire.Utilities.Exception;
    using Inspire.Utilities.Extensions;

    [CustomAuthorize]
    public class PollController : SearchTableController<PollQueryViewModel, PollTableViewModel>
    {
        private readonly IPollService pollService;
        private readonly ISessionStorageService sessionStorageService;
        private readonly INomenclatureService nomenclatureService;
        private readonly ICacheService cacheService;

        public PollController(
            ILogger logger,
            IMapper mapper,
            IDbContextManager contextManager,
            IResourceManager resource,
            IPollService pollService,
            ISessionStorageService sessionStorageService,
            INomenclatureService nomenclatureService,
            ICacheService cacheService)
            : base(logger, mapper, contextManager, resource, Resource.Polls)
        {
            this.pollService = pollService;
            this.sessionStorageService = sessionStorageService;
            this.nomenclatureService = nomenclatureService;
            this.cacheService = cacheService;
        }

        [HttpGet]
        [CustomAuthorize(ApplicationRights.SearchQuestionnaires)]
        public override ActionResult Index(PollQueryViewModel model = null)
        {
            this.InitAdminBreadcrumb(
                Title,
                string.Empty,
                true);

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

        [HttpGet]
        [CustomAuthorize(ApplicationRights.UpsertQuestionnaires)]
        public ActionResult Upsert(Guid? id = null)
        {
            var model = new PollViewModel();
            if (id.HasValue)
            {
                Poll dbModel;
                using (ContextManager.NewConnection())
                {
                    dbModel = pollService.GetPoll(id.Value, false);
                }

                model = Mapper.Map<PollViewModel>(dbModel);

                if (model.Questions.IsNotNullOrEmpty())
                {
                    foreach (var question in model.Questions)
                    {
                        sessionStorageService.Upsert(question);
                        if (question.Options.IsNotNullOrEmpty())
                        {
                            foreach (var option in question.Options)
                            {
                                sessionStorageService.Upsert(option);
                            }
                        }
                    }
                }
            }
            else
            {
                // The task requires to set dates as follows
                model.ValidFrom = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day + 1, 8, 0, 0);
                model.ValidTo = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day + 2, 8, 0, 0);
            }

            this.InitAdminBreadcrumb(
                Title,
                id.HasValue ? Resource.EditPoll : Resource.AddPoll,
                true);

            return View(model);
        }

        [HttpPost]
        [CustomAuthorize(ApplicationRights.UpsertQuestionnaires)]
        public ActionResult Upsert(PollViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var dbModel = Mapper.Map<Poll>(model);

            using (var transaction = ContextManager.NewTransaction())
            {
                pollService.Upsert(dbModel);

                transaction.Commit();
            }

            this.ShowMessage(MessageType.Success, Resource.Success);
            return RedirectToAction("Index");
        }

        [HttpGet]
        [CustomAuthorize(ApplicationRights.UpsertQuestionnaires)]
        public ActionResult UpsertQuestion(string uniqueId)
        {
            var model = sessionStorageService.Get<QuestionViewModel>(uniqueId);

            return PartialView(model);
        }

        [HttpPost]
        [CustomAuthorize(ApplicationRights.UpsertQuestionnaires)]
        public ActionResult UpsertQuestion(QuestionViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return Json(RenderRazorViewToString("UpsertQuestion", model), JsonRequestBehavior.AllowGet);
            }

            sessionStorageService.Upsert(model);

            return Json(
                new
                {
                    success = true,
                    model
                },
                JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [CustomAuthorize(ApplicationRights.UpsertQuestionnaires)]
        public ActionResult EditOption(string uniqueId)
        {
            var model = sessionStorageService.Get<OptionViewModel>(uniqueId);

            return PartialView("_EditOption", model);
        }

        [HttpGet]
        [CustomAuthorize(ApplicationRights.UpsertQuestionnaires)]
        public ActionResult AddDefaultOption()
        {
            var model = new OptionViewModel();
            model.Values = new SortedDictionary<string, string>();
            foreach (var languageElement in Global.Languages)
            {
                model.Values.Add(languageElement.Id.ToString(), string.Empty);
            }

            sessionStorageService.Upsert(model);

            return Json(
                new
                {
                    success = true,
                    model
                },
                JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [CustomAuthorize(ApplicationRights.UpsertQuestionnaires)]
        public ActionResult EditOption(OptionViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return Json(RenderRazorViewToString("_EditOption", model), JsonRequestBehavior.AllowGet);
            }

            sessionStorageService.Upsert(model);
            this.ShowMessage(MessageType.Success, Resource.Success);
            return Json(
                new
                {
                    success = true,
                    result = model
                },
                JsonRequestBehavior.AllowGet);
        }

        [HttpDelete]
        public void DeleteOption(string uniqueId)
        {
            var option = sessionStorageService.Get<OptionViewModel>(uniqueId);
            if (option == null)
            {
                throw new UserException(Resource.NoDataFound);
            }

            sessionStorageService.Remove<OptionViewModel>(uniqueId);
        }

        [HttpDelete]
        public void DeleteQuestion(Guid questionId)
        {
            using (var transaction = ContextManager.NewTransaction())
            {
                pollService.DeleteQuestions(new[] { questionId });
                transaction.Commit();
            }
        }

        [HttpDelete]
        public void DeleteQuestionFromSession(string uniqueId)
        {
            var question = sessionStorageService.Get<QuestionViewModel>(uniqueId);
            if (question == null)
            {
                throw new UserException(Resource.NoDataFound);
            }

            sessionStorageService.Remove<QuestionViewModel>(uniqueId);
        }

        [HttpDelete]
        [CustomAuthorize(ApplicationRights.DelQuestionnaires)]
        public void DeletePoll(Guid pollId)
        {
            using (var transaction = ContextManager.NewTransaction())
            {
                pollService.DeletePoll(pollId);
                transaction.Commit();
            }
        }

        [HttpGet]
        [CustomAuthorize(ApplicationRights.UpdDatesQuestionnaires)]
        public ActionResult ChangeDates(Guid id, string searchQueryId)
        {
            Poll dbModel;
            using (ContextManager.NewConnection())
            {
                dbModel = pollService.GetPoll(id, false);
            }

            ViewBag.SearchQueryId = searchQueryId;
            return PartialView(Mapper.Map<PollViewModel>(dbModel));
        }

        [HttpPost]
        [CustomAuthorize(ApplicationRights.UpdDatesQuestionnaires)]
        public ActionResult ChangeDates(PollViewModel model, string searchQueryId)
        {
            bool hasFromValue = model.ValidFrom.HasValue;
            bool hasToValue = model.ValidTo.HasValue;
            if (!hasFromValue ||
                !hasToValue)
            {
                if (!hasFromValue)
                {
                    ModelState.AddModelError("ValidFrom", string.Format(Resource.Required, Resource.From));
                }

                if (!hasToValue)
                {
                    ModelState.AddModelError("ValidTo", string.Format(Resource.Required, Resource.To));
                }

                return Json(RenderRazorViewToString("ChangeDates", model), JsonRequestBehavior.AllowGet);
            }

            var dbModel = Mapper.Map<Poll>(model);
            PollTableViewModel tableModel;
            using (var transaction = ContextManager.NewTransaction())
            {
                pollService.UpdateDates(dbModel);
                tableModel = Mapper.Map<PollTableViewModel>(
                    pollService.Search(new PollQueryModel()).SingleOrDefault(x => x.Id == dbModel.Id));
                transaction.Commit();
            }

            RefreshGridItem(searchQueryId, tableModel, m => m.Id == tableModel.Id);
            this.ShowMessage(MessageType.Success, Resource.Success);
            return Json(
                new { success = true, refreshgrid = true, searchqueryid = searchQueryId },
                JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult Polls()
        {
            List<PollTableModel> polls;
            using (ContextManager.NewConnection())
            {
                polls = pollService.Search(new PollQueryModel())
                                 .Where(x => x.Status.Id == EnumHelper.PollStatuses[PollStatus.Valid]).ToList();
            }

            var model = Mapper.Map<List<PollTableViewModel>>(polls);
            this.InitViewTitleAndBreadcrumbs(
                Resource.Polls);
            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult Poll(Guid pollId)
        {
            Poll poll;
            using (ContextManager.NewConnection())
            {
                poll = pollService.GetPoll(pollId);
            }

            var model = Mapper.Map<PollViewModel>(poll);

            this.InitViewTitleAndBreadcrumbs(
                model.Title,
                new List<Breadcrumb>
                {
                    new Breadcrumb
                    {
                        Title = Resource.Polls,
                        Url = Url.Action("Polls", "Poll")
                    }
                },
                false);
            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult SubmitPoll(PollViewModel model)
        {
            var questionsId = new List<Guid>();
            var answersId = new List<string>();

            if (HasInvalidQuestions(model.Questions))
            {
                this.ShowMessage(MessageType.Warning, Resource.FillAllRequiredQuestions);
                this.InitViewTitleAndBreadcrumbs(
                    model.Title,
                    new List<Breadcrumb>
                    {
                        new Breadcrumb
                        {
                            Title = Resource.Polls,
                            Url = Url.Action("Polls", "Poll")
                        }
                    },
                    false);
                return View("Poll", model);
            }

            foreach (var question in model.Questions)
            {
                questionsId.Add(question.Id.Value);

                if (question.Type.Id == EnumHelper.QuestionStatuses[QuestionType.TextBox] ||
                        question.Type.Id == EnumHelper.QuestionStatuses[QuestionType.TextArea])
                {
                    answersId.Add(question.Options[0].Value);
                }
                else if (question.Type.Id == EnumHelper.QuestionStatuses[QuestionType.RadioButton])
                {
                    if (question.RadioBtn.IsNotNullOrEmpty())
                    {
                        var filteredOption = question.Options.FirstOrDefault(x => x.Id.ToString().Equals(question.RadioBtn));
                        if (filteredOption != null)
                        {
                            answersId.Add(filteredOption.Id.ToString());
                        }
                    }
                    else
                    {
                        answersId.Add(null);
                    }
                }
                else
                {
                    var ids = new List<string>();

                    foreach (var option in question.Options)
                    {
                        if (option.IsSelected)
                        {
                            ids.Add(option.Id.Value.ToString());
                        }
                    }

                    if (ids.IsNullOrEmpty())
                    {
                        ids.Add(null);
                    }

                    answersId.Add(string.Join(",", ids));
                }
            }

            using (var transaction = ContextManager.NewTransaction())
            {
                pollService.InsertPollResponse(model.Id.Value, questionsId.ToArray(), answersId.ToArray());
                transaction.Commit();
            }

            this.ShowMessage(MessageType.Success, Resource.SuccessfullySend);
            return RedirectToAction("Polls");
        }

        [HttpGet]
        public ActionResult PollResult(Guid pollId)
        {
            List<PollResult> pollResults;
            Poll poll;
            using (ContextManager.NewConnection())
            {
                pollResults = pollService.GetPollResults(pollId);
                poll = pollService.GetPoll(pollId);
            }

            var model = Mapper.Map<List<PollResultViewModel>>(pollResults);
            ViewBag.PollName = poll.Titles.GetValueForCurrentCulture();

            this.InitViewTitleAndBreadcrumbs(
                poll.Titles.GetValueForCurrentCulture(),
                new List<Breadcrumb>
                {
                    new Breadcrumb
                    {
                        Title = Resource.Polls,
                        Url = Url.Action("Index", "Poll")
                    },
                    new Breadcrumb
                    {
                        Title = Resource.Results
                    }
                });
            return View(model);
        }

        [HttpPost]
        public ActionResult ExportPollResult(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);
            return File(fileContents, contentType, fileName);
        }

        [HttpGet]
        [OutputCache(CacheProfile = "Default")]
        [AllowAnonymous]
        public JsonResult GetQuestionTypes()
        {
            var resource = cacheService.GetOrSetCache(
                "questionTypes",
                () => new[]
                      {
                          EnumHelper.QuestionStatuses[QuestionType.TextBox].ToString(),
                          EnumHelper.QuestionStatuses[QuestionType.TextArea].ToString()
                      });
            return new JsonResultMaxLength(resource);
        }

        protected override void InitialQuery(PollQueryViewModel model)
        {
            List<Nomenclature> statuses;
            using (ContextManager.NewConnection())
            {
                statuses = nomenclatureService.Get("nquestionnairestatus");
            }

            model.StatusDataSource = statuses
                                     .Select(
                                         status => new KeyValuePair<string, string>(status.Id?.ToString(), status.Name))
                                     .ToList().AddDefaultValue(false);
        }

        protected override IEnumerable<PollTableViewModel> FindResults(PollQueryViewModel query)
        {
            List<PollTableViewModel> result;
            using (ContextManager.NewConnection())
            {
                var polls = pollService.Search(Mapper.Map<PollQueryModel>(query));
                result = Mapper.Map<List<PollTableViewModel>>(polls);
            }

            return result;
        }

        private bool HasInvalidQuestions(List<QuestionViewModel> questions)
        {
            foreach (var question in questions)
            {
                if (question.Mandatory &&
                    (question.Type.Id == EnumHelper.QuestionStatuses[QuestionType.TextBox] ||
                     question.Type.Id == EnumHelper.QuestionStatuses[QuestionType.TextArea]) &&
                    question.Options.All(x => x.Value.IsNullOrEmpty()))
                {
                    return true;
                }

                if (question.Mandatory &&
                    question.Type.Id == EnumHelper.QuestionStatuses[QuestionType.Checkbox] &&
                    question.Options.All(x => !x.IsSelected))
                {
                    return true;
                }

                if (question.Mandatory &&
                    question.Type.Id == EnumHelper.QuestionStatuses[QuestionType.RadioButton] &&
                    question.RadioBtn.IsNullOrEmpty())
                {
                    return true;
                }
            }

            return false;
        }
    }
}