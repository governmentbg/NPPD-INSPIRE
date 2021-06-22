namespace Inspire.Test.Unit.Test.Provider
{
    using System;
    using System.Collections.Generic;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Routing;

    using FluentAssertions;

    using Inspire.Domain.Services;
    using Inspire.Model.Provider;
    using Inspire.Model.QueryModels;
    using Inspire.Portal.Controllers;
    using Inspire.Portal.Models.Provider;
    using Inspire.Test.Unit.Helpers;

    using Telerik.JustMock;
    using Telerik.JustMock.Helpers;

    using Xunit;

    public class ProviderControllerTests : BaseTestClass
    {
        private readonly INomenclatureService nomenclatureService;
        private readonly ProviderController providerController;
        private readonly IProviderService providerService;

        public ProviderControllerTests()
        {
            providerService = Mock.Create<IProviderService>();
            nomenclatureService = Mock.Create<INomenclatureService>();

            providerController = new ProviderController(
                Logger,
                Mapper,
                ContextManager,
                ResourceManager,
                providerService,
                nomenclatureService);
            InitContext();
        }

        [Fact(DisplayName = "ProviderController: Load Index view")]
        public void Index()
        {
            var result = providerController.Index() as ViewResult;
            result.Should().NotBeNull();
        }

        [Fact(DisplayName = "ProviderController: Load Index should return SearchForm")]
        public void IndexShouldReturnSearchForm()
        {
            var result = providerController.Index() as ViewResult;
            result.ViewName.Should().Be("SearchForm");
        }

        [Fact(DisplayName = "ProviderController: Load Index ViewData should not be null")]
        public void IndexViewDataShouldNotBeNull()
        {
            var result = providerController.Index() as ViewResult;
            result.ViewData.Should().NotBeNull();
        }

        [Fact(DisplayName = "ProviderController: Get Upsert provider should return result.")]
        public void UpsertProviderWithRightsShouldReturnView()
        {
            ////Act
            var result = providerController.Upsert(id: null) as ViewResult;
            ////Assert
            result.Should().NotBeNull();
            result.Should().BeOfType(typeof(ViewResult));
        }

        [Fact(DisplayName = "ProviderController: Post Upsert invalid model, modelstate should be invalid.")]
        public void UpsertProviderInvalidModel()
        {
            ////Arrange
            var viewModel = new ProviderViewModel();
            ////Act
            SimulateValidation(viewModel, providerController);
            var result = providerController.Upsert(viewModel);
            ////Assert
            providerController.ModelState.IsValid.Should().Be(false);
        }

        [Fact(DisplayName = "ProviderController: Post Upsert valid model, modelstate should be valid.")]
        public void UpsertProviderValidModel()
        {
            ////Arrange
            var viewModel = DataHelper.GetProvider();
            ////Act
            SimulateValidation(viewModel, providerController);
            var result = providerController.Upsert(viewModel);
            ////Assert
            providerController.ModelState.IsValid.Should().Be(true);
        }

        [Fact(DisplayName = "ProviderController: Post Upsert valid model without image, modelstate should be invalid.")]
        public void UpsertProviderValidModelWithoutImage()
        {
            ////Arrange
            var viewModel = DataHelper.GetProvider();
            viewModel.MainPicture = null;
            ////Act
            SimulateValidation(viewModel, providerController);
            var result = providerController.Upsert(viewModel);
            ////Assert
            providerController.ModelState.IsValid.Should().Be(false);
        }

        [Fact(DisplayName = "ProviderController: Post Upsert valid model, should redirect to index.")]
        public void UpsertProviderValidModelShouldRedirect()
        {
            ////Arrange
            var viewModel = DataHelper.GetProvider();
            ////Act
            SimulateValidation(viewModel, providerController);
            var result = providerController.Upsert(viewModel);
            ////Assert
            result.Should().BeOfType(typeof(RedirectToRouteResult));
        }

        [Fact(DisplayName = "ProviderController: Get GetStatuses, should return JSON with result.")]
        public void GetStatusesShouldReturnJson()
        {
            ////Act
            var result = providerController.GetStatuses();
            ////Assert
            result.Should().BeOfType(typeof(JsonResult));
        }

        [Fact(DisplayName = "ProviderController: Get GetStatuses, should return not null result.")]
        public void GetStatusesShouldReturnResult()
        {
            ////Act
            var result = providerController.GetStatuses() as JsonResult;
            ////Assert
            result.Data.Should().NotBeNull();
        }

        [Fact(DisplayName = "ProviderController: Get GetStatuses, should have JSONReqestBehavior.AllowGet")]
        public void GetStatusesShouldHaveRequestBehaviorGet()
        {
            ////Act
            var result = providerController.GetStatuses() as JsonResult;
            ////Assert
            Assert.Equal(JsonRequestBehavior.AllowGet, result.JsonRequestBehavior);
        }

        [Fact(DisplayName = "ProviderController: Post Delete, should return Json with success property")]
        public void DeleteShouldReturnJson()
        {
            ////Act
            providerController.Delete(Guid.NewGuid(), "testSearchQueryId");
        }

        [Fact(DisplayName = "ProviderController: Post Rearrange, should return json")]
        public void RearrangeShouldReturnJson()
        {
            ////Arrange
            providerService.Arrange(s => s.Search(Arg.IsAny<ProviderQueryModel>())).Returns(
                new List<Provider>
                { Mapper.Map<Provider>(DataHelper.GetProvider()), Mapper.Map<Provider>(DataHelper.GetProvider()) });
            ////Act
            var result = providerController.Rearrange(0, 1);
            ////Assert
            result.Should().BeOfType(typeof(JsonResult));
        }

        private void InitContext()
        {
            HttpContext.Current = CommonHelpers.GetHttpContext();
            var wrapper = new HttpContextWrapper(HttpContext.Current);
            providerController.ControllerContext = new ControllerContext(
                wrapper,
                new RouteData(),
                providerController);
            providerController.Init();
        }
    }
}