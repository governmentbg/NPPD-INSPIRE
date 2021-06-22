namespace Inspire.Test.Unit.Test.Faq
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Routing;

    using FluentAssertions;

    using Inspire.Common.Mvc.Infrastructure.CustomResult;
    using Inspire.Domain.Services;
    using Inspire.Model.Helpers;
    using Inspire.Model.Language;
    using Inspire.Portal.Controllers;
    using Inspire.Portal.Models.Faq;
    using Inspire.Test.Unit.Helpers;

    using Kendo.Mvc.UI;

    using Telerik.JustMock;
    using Telerik.JustMock.Helpers;

    using Xunit;

    public class FaqControllerTests : BaseTestClass
    {
        private readonly FaqController faqController;
        private readonly IFaqService faqService;
        private readonly INomenclatureService nomenclatureService;

        public FaqControllerTests()
        {
            faqService = Mock.Create<IFaqService>();
            nomenclatureService = Mock.Create<INomenclatureService>();
            faqController = new FaqController(Logger, Mapper, ContextManager, ResourceManager, faqService, nomenclatureService);
            InitContext();
        }

        [Fact(DisplayName = "FaqController: Load Index view")]
        public void Index()
        {
            ////Act
            var result = faqController.Index();
            ////Assert
            result.Should().NotBeNull();
        }

        [Fact(DisplayName = "FaqController: Load Index should return SearchForm")]
        public void IndexShouldReturnSearchForm()
        {
            ////Act
            var result = faqController.Index() as ViewResult;
            ////Assert
            result.ViewName.Should().Be("SearchForm");
        }

        [Fact(DisplayName = "FaqController: Load Index ViewData should not be null")]
        public void IndexViewDataShouldNotBeNull()
        {
            ////Act
            var result = faqController.Index() as ViewResult;
            ////Assert
            result.ViewData.Should().NotBeNull();
        }

        [Fact(DisplayName = "FaqController: Get Upsert, should return viewresult")]
        public void UpsertReturnViewResult()
        {
            ////Act
            var result = faqController.Upsert();
            ////Assert
            result.Should().BeOfType(typeof(ViewResult));
        }

        [Fact(DisplayName = "FaqController: Get Upsert, model should not be null")]
        public void UpsertModelNotNull()
        {
            ////Act
            var result = faqController.Upsert() as ViewResult;
            ////Assert
            result.Model.Should().NotBeNull();
        }

        [Fact(DisplayName = "FaqController: Post Upsert with invalid model, should be invalid modelstate")]
        public void UpsertInvalidModel()
        {
            ////Arrange
            var model = new FaqUpsertViewModel();
            ////Act
            SimulateValidation(model, faqController);
            var result = faqController.Upsert(model);
            ////Assert
            faqController.ModelState.IsValid.Should().Be(false);
        }

        [Fact(DisplayName = "FaqController: Post Upsert with valid model, should be valid modelstate")]
        public void UpsertValidModel()
        {
            ////Arrange
            var model = DataHelper.GetFaq();
            ////Act
            SimulateValidation(model, faqController);
            var result = faqController.Upsert(model);
            ////Assert
            faqController.ModelState.IsValid.Should().Be(true);
        }

        [Fact(DisplayName = "FaqController: Post Upsert on success should redirect to index")]
        public void UpsertValidModelRedirectToIndex()
        {
            ////Arrange
            var model = DataHelper.GetFaq();
            ////Act
            SimulateValidation(model, faqController);
            var result = faqController.Upsert(model);
            ////Assert
            result.Should().BeOfType(typeof(RedirectToRouteResult));
        }

        [Fact(DisplayName = "FaqController: Post Upsert on fail should return View")]
        public void UpsertInvalidModelRedirectToUpsert()
        {
            ////Arrange
            var model = new FaqUpsertViewModel();
            ////Act
            SimulateValidation(model, faqController);
            var result = faqController.Upsert(model);
            ////Assert
            result.Should().BeOfType(typeof(ViewResult));
        }

        [Fact(DisplayName = "FaqController: Post Upsert on fail should return same model")]
        public void UpsertInvalidModelReturnSameModel()
        {
            ////Arrange
            var model = new FaqUpsertViewModel();
            var question = new SortedDictionary<string, string>
                           {
                               { EnumHelper.GetLanguageId(Language.BG).ToString(), "Въпрос тест" },
                               { EnumHelper.GetLanguageId(Language.EN).ToString(), "Question Test" }
                           };

            model.Questions = question;
            ////Act
            SimulateValidation(model, faqController);
            var result = faqController.Upsert(model) as ViewResult;
            ////Assert
            var resultModel = result.Model as FaqUpsertViewModel;
            resultModel.Questions.Should().BeEquivalentTo(question);
        }

        [Fact(DisplayName = "FaqController: Get upsert category should return partial view")]
        public void UpsertCategoryReturnPartialView()
        {
            ////Act
            var result = faqController.UpsertCategory(null);
            ////Assert
            result.Should().BeOfType(typeof(PartialViewResult));
        }

        [Fact(DisplayName = "FaqController: Get upsert category model should not be null")]
        public void UpsertCategoryReturnModel()
        {
            ////Act
            var result = faqController.UpsertCategory(null) as PartialViewResult;
            ////Assert
            result.Model.Should().NotBeNull();
        }

        [Fact(DisplayName = "FaqController: Post upsert category with empty model, modelstate should be invalid")]
        public void UpsertPostCategoryEmptyModel()
        {
            ////Arrange
            var model = new FaqCategoryViewModel();
            var request = new DataSourceRequest();
            ////Act
            var result = faqController.UpsertCategory(request, model);
            SimulateValidation(model, faqController);
            ////Assert
            faqController.ModelState.IsValid.Should().Be(false);
        }

        [Fact(DisplayName = "FaqController: Post upsert category without name, modelstate should be invalid")]
        public void UpsertPostCategoryWithoutName()
        {
            ////Arange
            var model = DataHelper.GetFaqCategory();
            model.Names = null;
            var request = new DataSourceRequest();
            ////Act
            var result = faqController.UpsertCategory(request, model);
            SimulateValidation(model, faqController);
            ////Assert
            faqController.ModelState.IsValid.Should().Be(false);
        }

        [Fact(DisplayName = "FaqController:Post upsert category valid model, modelstate should be valid")]
        public void UpsertPortCategoryValid()
        {
            ////Arrange
            var model = DataHelper.GetFaqCategory();
            var request = new DataSourceRequest();
            ////Act
            var result = faqController.UpsertCategory(request, model);
            SimulateValidation(model, faqController);
            ////Assert
            faqController.ModelState.IsValid.Should().Be(true);
        }

        [Fact(DisplayName = "FaqController: Post upsert category, should redirect")]
        public void UpsertPostCategoryShouldRedirect()
        {
            ////Arrange
            var model = DataHelper.GetFaqCategory();
            var request = new DataSourceRequest();
            ////Act
            var result = faqController.UpsertCategory(request, model);
            SimulateValidation(model, faqController);
            ////Assert
            result.Should().BeOfType(typeof(JsonResult));
        }

        [Fact(DisplayName = "FaqController: Post upsert category, should return model in json ")]
        public void UpsertPostCategoryOnSuccessreditrectToUpsertFaq()
        {
            ////Arrange
            var model = DataHelper.GetFaqCategory();
            var request = new DataSourceRequest();
            ////Act
            var result = faqController.UpsertCategory(request, model) as JsonResult;
            ////Assert
            var returnedData = result.Data as DataSourceResult;
            var returnedDatModel = returnedData.Data as List<FaqCategoryViewModel>;
            returnedDatModel.First().Name.Should().Be(model.Name);
        }

        [Fact(DisplayName = "FaqController: Post delete category, should redirect")]
        public void DeleteCategory()
        {
            faqController.Arrange(s => s.Url.Action(Arg.IsAny<string>())).Returns("Index");
            ////Act
            var result = faqController.DeleteCategory(Guid.NewGuid());
            ////Assert
            result.Should().BeOfType(typeof(RedirectResult));
        }

        [Fact(DisplayName = "FaqController:Get GetFaqCategories, should return json with data")]
        public void GetFaqCategoriesReturnJson()
        {
            ////Act
            var result = faqController.GetFaqCategories();
            ////Assert
            result.Should().BeOfType(typeof(JsonResultMaxLength));
        }

        private void InitContext()
        {
            HttpContext.Current = CommonHelpers.GetHttpContext();
            var wrapper = new HttpContextWrapper(HttpContext.Current);
            faqController.ControllerContext = new ControllerContext(
                wrapper,
                new RouteData(),
                faqController);
            faqController.Init();
        }
    }
}