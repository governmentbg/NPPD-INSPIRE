namespace Inspire.Test.Unit.Test.Poll
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Routing;

    using FluentAssertions;

    using Inspire.Common.Mvc.Infrastructure.CustomResult;
    using Inspire.Core.Infrastructure.Cache;
    using Inspire.Domain.Services;
    using Inspire.Model.Helpers;
    using Inspire.Model.Language;
    using Inspire.Portal.Controllers;
    using Inspire.Portal.Models.Poll;
    using Inspire.Portal.Services.SessionStorageService;
    using Inspire.Test.Unit.Helpers;

    using Telerik.JustMock;

    using Xunit;

    public class PollControllerTests : BaseTestClass
    {
        private readonly ICacheService cacheService;
        private readonly INomenclatureService nomenclatureService;
        private readonly PollController pollController;
        private readonly IPollService pollService;
        private readonly ISessionStorageService sessionStorageService;

        public PollControllerTests()
        {
            pollService = Mock.Create<IPollService>();
            nomenclatureService = Mock.Create<INomenclatureService>();
            cacheService = Mock.Create<ICacheService>();
            sessionStorageService = Mock.Create<ISessionStorageService>();

            pollController = new PollController(
                Logger,
                Mapper,
                ContextManager,
                ResourceManager,
                pollService,
                sessionStorageService,
                nomenclatureService,
                cacheService);
            InitContext();
        }

        [Fact(DisplayName = "PollController: Load Index view")]
        public void IndexShouldNotBeNull()
        {
            var result = pollController.Index();
            result.Should().NotBeNull();
        }

        [Fact(DisplayName = "PollController: Load Index should return SearchForm")]
        public void IndexShouldReturnSearchForm()
        {
            var result = pollController.Index() as ViewResult;
            result.ViewName.Should().Be("SearchForm");
        }

        [Fact(DisplayName = "PollController: Load Index ViewData should not be null")]
        public void IndexViewDataShouldNotBeNull()
        {
            var result = pollController.Index() as ViewResult;
            result.ViewData.Should().NotBeNull();
        }

        [Fact(DisplayName = "PollController: Get Upsert, should return ViewResult")]
        public void UpsertReturnViewResult()
        {
            var result = pollController.Upsert();
            result.Should().BeOfType(typeof(ViewResult));
        }

        [Fact(DisplayName = "PollController: Get Upsert, model should not be null")]
        public void UpsertModelNotNull()
        {
            var result = pollController.Upsert() as ViewResult;
            result.Model.Should().NotBeNull();
        }

        [Fact(DisplayName = "PollController: Upsert invalid model should redirect to same page")]
        public void UpsertInvalidModelShouldRedirectToSamePage()
        {
            var model = new PollViewModel();
            SimulateValidation(model, pollController);
            var result = pollController.Upsert(model);
            result.Should().BeOfType(typeof(ViewResult));
        }

        [Fact(DisplayName = "PollController: Post Upsert, invalid model should return same model")]
        public void UpsertPostInvalidModelShouldReturnSameModel()
        {
            var model = new PollViewModel();
            model.Titles = new SortedDictionary<string, string>
                           {
                               { EnumHelper.GetLanguageId(Language.BG).ToString(), "Име тест" },
                               { EnumHelper.GetLanguageId(Language.EN).ToString(), "Name Test" }
                           };

            SimulateValidation(model, pollController);

            var result = pollController.Upsert(model) as ViewResult;
            var returnModel = result.Model as PollViewModel;

            result.Should().BeOfType(typeof(ViewResult));
            returnModel.Should().BeEquivalentTo(model);
        }

        [Fact(DisplayName = "PollController: Upsert valid model should redirect")]
        public void UpsertValidModelShouldRedirect()
        {
            var model = DataHelper.GetPoll();

            SimulateValidation(model, pollController);

            var result = pollController.Upsert(model);

            result.Should().BeOfType(typeof(RedirectToRouteResult));
        }

        [Fact(DisplayName = "PollController: Get Upsert, return empty model")]
        public void UpsertQuestionReturnsEmptyModel()
        {
            var result = pollController.UpsertQuestion(string.Empty) as PartialViewResult;
            result.Should().BeOfType(typeof(PartialViewResult));
        }

        [Fact(DisplayName = "PollController: Upsert invalid question model should return empty data")]
        public void UpsertQuestionWithInvalidModel()
        {
            var question = new QuestionViewModel();
            SimulateValidation(question, pollController);
            var result = pollController.UpsertQuestion(question) as JsonResult;

            result.Data.Should().Be(string.Empty);
            result.Should().BeOfType(typeof(JsonResult));
        }

        [Fact(DisplayName = "PollController: Upsert valid question model should return model with success")]
        public void UpsertQuestionWithValidModel()
        {
            var question = DataHelper.GetPoll().Questions.First();

            SimulateValidation(question, pollController);
            var result = pollController.UpsertQuestion(question) as JsonResult;

            result.Data.Should().NotBeNull();
            pollController.ModelState.IsValid.Should().BeTrue();
            Assert.Equal(JsonRequestBehavior.AllowGet, result.JsonRequestBehavior);
            result.Should().BeOfType(typeof(JsonResult));
        }

        [Fact(DisplayName = "PollController: Post change dates with invalid dates")]
        public void PollChangeDatesWithInvalidData()
        {
            var model = DataHelper.GetPoll();
            model.ValidFrom = null;

            var result = pollController.ChangeDates(model, null) as JsonResult;

            result.Data.Should().Be(string.Empty);
            result.Should().BeOfType(typeof(JsonResult));
        }

        [Fact(DisplayName = "PollController: GetQuestionTypes should not return empty result")]
        public void GetQuestionTypesShouldNotBeEmpty()
        {
            var result = pollController.GetQuestionTypes();

            result.Should().BeOfType(typeof(JsonResultMaxLength));
        }

        private void InitContext()
        {
            HttpContext.Current = CommonHelpers.GetHttpContext();
            var wrapper = new HttpContextWrapper(HttpContext.Current);
            pollController.ControllerContext = new ControllerContext(
                wrapper,
                new RouteData(),
                pollController);
            pollController.Init();
        }
    }
}