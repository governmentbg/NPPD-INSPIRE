namespace Inspire.Test.Unit.Test.Cms
{
    using System;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Routing;

    using FluentAssertions;

    using Inspire.Domain.Services;
    using Inspire.Model.Cms;
    using Inspire.Portal.Controllers;
    using Inspire.Portal.Models.Cms;
    using Inspire.Portal.Services.SessionStorageService;
    using Inspire.Test.Unit.Helpers;

    using Telerik.JustMock;
    using Telerik.JustMock.Helpers;

    using Xunit;

    public class CmsControllerTests : BaseTestClass
    {
        private readonly CmsController cmsController;
        private readonly ICmsService cmsService;
        private readonly INomenclatureService nomenclatureService;
        private readonly ISessionStorageService sessionStorageService;

        public CmsControllerTests()
        {
            cmsService = Mock.Create<ICmsService>();
            nomenclatureService = Mock.Create<INomenclatureService>();
            sessionStorageService = Mock.Create<ISessionStorageService>();
            cmsController = new CmsController(
                Logger,
                Mapper,
                ContextManager,
                cmsService,
                nomenclatureService,
                sessionStorageService);

            InitContext();
        }

        [Fact(DisplayName = "CmsController: Get Index returns viewresult")]
        public void GetIndex()
        {
            ////Act
            var result = cmsController.Index();

            ////Assert
            result.Should().BeOfType<ViewResult>();
        }

        [Fact(DisplayName = "CmsController: Get Upsert returns viewresult")]
        public void GetUpsertView()
        {
            ////Act
            var result = cmsController.Upsert();

            ////Assert
            result.Should().BeOfType<ViewResult>();
        }

        [Fact(DisplayName = "CmsController: Get upsert, when pageid is not null should get model from cmsService")]
        public void UpsertGetModelFromCmsService()
        {
            ////Arrange
            var modelFromService = DataHelper.GetPage();
            cmsService.Arrange(s => s.GetPage(Arg.IsAny<Guid>(), Arg.IsAny<bool>())).Returns(modelFromService);

            ////Act
            var result = cmsController.Upsert(Guid.NewGuid()) as ViewResult;

            ////Assert
            var returnedModelToView = result.Model as PageUpsertViewModel;
            returnedModelToView.Id.Should().Be(modelFromService.Id);
        }

        [Fact(
            DisplayName = "CmsController: Get upsert, when sessionId is not null should get model from sessionService")]
        public void UpsertGetModelFromSessionService()
        {
            ////Arrange
            var modelFromService = DataHelper.GetPage();
            sessionStorageService.Arrange(s => s.Get<PageUpsertViewModel>(Arg.IsAny<string>()))
                                 .Returns(Mapper.Map<PageUpsertViewModel>(modelFromService));
            ////Act
            var result = cmsController.Upsert(null, null, "sessionIdTest") as ViewResult;

            ////Assert
            var returnedModelToView = result.Model as PageUpsertViewModel;
            returnedModelToView.Id.Should().Be(modelFromService.Id);
        }

        [Fact(
            DisplayName =
                "CmsController: Get upsert, when pageid and sessionid are not null should get model from sessionService")]
        public void UpsertGetModel()
        {
            ////Arrange
            var modelFromService = DataHelper.GetPage();
            sessionStorageService.Arrange(s => s.Get<PageUpsertViewModel>(Arg.IsAny<string>()))
                                 .Returns(Mapper.Map<PageUpsertViewModel>(modelFromService));
            ////Act
            var result = cmsController.Upsert(Guid.NewGuid(), Guid.NewGuid(), "sessionIdTest") as ViewResult;

            ////Assert
            var returnedModelToView = result.Model as PageUpsertViewModel;
            returnedModelToView.Id.Should().Be(modelFromService.Id);
        }

        [Fact(
            DisplayName =
                "CmsController: Get upsert, when pageid and parentid are sent must get from cmsService and set sent parentid ")]
        public void UpsertGetModelParentId()
        {
            ////Arrange
            var modelFromService = DataHelper.GetPage();
            cmsService.Arrange(s => s.GetPage(Arg.IsAny<Guid>(), Arg.IsAny<bool>())).Returns(modelFromService);
            var parentGuidSent = Guid.NewGuid();

            ////Act
            var result = cmsController.Upsert(Guid.NewGuid(), parentGuidSent) as ViewResult;

            ////Assert
            var returnedModelToView = result.Model as PageUpsertViewModel;
            returnedModelToView.ParentId.Should().Equals(parentGuidSent);
        }

        [Fact(
            DisplayName =
                "CmsController: Get upsert, when 3 sent parameters are null, returned model should not be null")]
        public void UpsertGetModelNotNull()
        {
            ////Act
            var result = cmsController.Upsert() as ViewResult;

            ////Assert
            var returnedModelToView = result.Model as PageUpsertViewModel;
            returnedModelToView.Should().NotBeNull();
        }

        [Fact(DisplayName = "CmsController: Post upsert, with session id should get model from sessionService")]
        public void UpsertPostModelFromSession()
        {
            ////Arrange
            var modelFromService = DataHelper.GetPage();
            sessionStorageService.Arrange(s => s.Get<PageUpsertViewModel>(Arg.IsAny<string>()))
                                 .Returns(Mapper.Map<PageUpsertViewModel>(modelFromService));
            cmsController.Arrange(s => s.Url.Action(Arg.IsAny<string>())).Returns("Preview");
            ////Act
            var result = cmsController.Upsert(null, false, "testSessionId");

            ////Assert
            result.Should().BeOfType(typeof(RedirectResult));
        }

        [Fact(DisplayName = "CmsController: Post upsert, with session id should get model from formdata")]
        public void UpsertPostModelFromFormData()
        {
            ////Arrange
            var formDataModel = Mapper.Map<PageUpsertViewModel>(DataHelper.GetPage());
            cmsController.Arrange(s => s.Url.Action(Arg.IsAny<string>())).Returns("Preview");
            ////Act
            var result = cmsController.Upsert(formDataModel);

            ////Assert
            result.Should().BeOfType(typeof(RedirectResult));
        }

        [Fact(
            DisplayName = "CmsController: Post upsert, with session id and model should get model from sessionService")]
        public void UpsertPostModelFromSession2()
        {
            ////Arrange
            var modelFromService = DataHelper.GetPage();
            sessionStorageService.Arrange(s => s.Get<PageUpsertViewModel>(Arg.IsAny<string>()))
                                 .Returns(Mapper.Map<PageUpsertViewModel>(modelFromService));
            cmsController.Arrange(s => s.Url.Action(Arg.IsAny<string>())).Returns("Preview");
            ////Act
            var result = cmsController.Upsert(Mapper.Map<PageUpsertViewModel>(modelFromService));

            ////Assert
            result.Should().BeOfType(typeof(RedirectResult));
        }

        [Fact(
            DisplayName =
                "CmsController: Post upsert, with session id and preview seto to true should return RedirectResult")]
        public void UpsertPostModelPreview()
        {
            ////Arrange
            var modelFromService = DataHelper.GetPage();
            sessionStorageService.Arrange(s => s.Get<PageUpsertViewModel>(Arg.IsAny<string>()))
                                 .Returns(Mapper.Map<PageUpsertViewModel>(modelFromService));
            cmsController.Arrange(s => s.Url.Action(Arg.IsAny<string>(), Arg.IsAny<object>())).Returns("Preview");
            ////Act
            var result = cmsController.Upsert(null, true, "testSessionId");

            ////Assert
            result.Should().BeOfType(typeof(RedirectResult));
        }

        [Fact(
            DisplayName =
                "CmsController: Post upsert, with session id and preview seto to true should return Preview view")]
        public void UpsertPostModelPreviewReturnedView()
        {
            ////Arrange
            var modelFromService = DataHelper.GetPage();
            sessionStorageService.Arrange(s => s.Get<PageUpsertViewModel>(Arg.IsAny<string>()))
                                 .Returns(Mapper.Map<PageUpsertViewModel>(modelFromService));
            cmsController.Arrange(s => s.Url.Action(Arg.IsAny<string>(), Arg.IsAny<object>())).Returns("Preview");

            ////Act
            var result = cmsController.Upsert(null, true, "testSessionId") as RedirectResult;

            ////Assert
            result.Url.Should().Contain("Preview");
        }

        [Fact(DisplayName = "CmsController: Post upsert, with session id should get invalid model from sessionService")]
        public void UpsertPostInvalidModelFromSession()
        {
            ////Arrange
            var modelFromService = new Page();
            sessionStorageService.Arrange(s => s.Get<PageUpsertViewModel>(Arg.IsAny<string>()))
                                 .Returns(Mapper.Map<PageUpsertViewModel>(modelFromService));

            ////Act
            try
            {
                var result = cmsController.Upsert(null, false, "testSessionId");
            }
            catch (Exception exception)
            {
                ////Assert
                exception.Message.Should().Be(
                    "The model of type 'Inspire.Portal.Models.Cms.PageUpsertViewModel' is not valid.");
            }
        }

        [Fact(DisplayName = "CmsController: Post upsert, with session id should get valid model from sessionService")]
        public void UpsertPostValidModelFromSession()
        {
            ////Arrange
            var modelFromService = DataHelper.GetPage();
            sessionStorageService.Arrange(s => s.Get<PageUpsertViewModel>(Arg.IsAny<string>()))
                                 .Returns(Mapper.Map<PageUpsertViewModel>(modelFromService));
            cmsController.Arrange(s => s.Url.Action(Arg.IsAny<string>())).Returns("Preview");
            ////Act
            var result = cmsController.Upsert(null, false, "testSessionId");
            SimulateValidation(modelFromService, cmsController);
            ////Assert
            cmsController.ModelState.IsValid.Should().Be(true);
        }

        [Fact(DisplayName = "CmsController: Post upsert, with model from formdata should be valid model")]
        public void UpsertPostValidModelFromFormData()
        {
            ////Arrange
            var model = Mapper.Map<PageUpsertViewModel>(DataHelper.GetPage());
            cmsController.Arrange(s => s.Url.Action(Arg.IsAny<string>())).Returns("Preview");
            ////Act
            var result = cmsController.Upsert(model);
            SimulateValidation(model, cmsController);
            ////Assert
            cmsController.ModelState.IsValid.Should().Be(true);
        }

        [Fact(DisplayName = "CmsController: Post upsert, with model from formdata should be invalid model")]
        public void UpsertPostInvalidModelFromFormData()
        {
            ////Arrange
            var model = new PageUpsertViewModel();
            cmsController.Arrange(s => s.Url.Action(Arg.IsAny<string>())).Returns("Preview");
            ////Act
            var result = cmsController.Upsert(model);
            SimulateValidation(model, cmsController);
            ////Assert
            cmsController.ModelState.IsValid.Should().Be(false);
        }

        [Fact(
            DisplayName =
                "CmsController: Post upsert, with invalid model from formdata and valid from session should be valid model")]
        public void UpsertPostInvalidModelFromFormDataAndValidFromSession()
        {
            ////Arrange
            var model = Mapper.Map<PageUpsertViewModel>(DataHelper.GetPage());
            model.Type = null;

            var modelFromService = DataHelper.GetPage();
            sessionStorageService.Arrange(s => s.Get<PageUpsertViewModel>(Arg.IsAny<string>()))
                                 .Returns(Mapper.Map<PageUpsertViewModel>(modelFromService));

            cmsController.Arrange(s => s.Url.Action(Arg.IsAny<string>())).Returns("Preview");

            ////Act
            var result = cmsController.Upsert(model, false, "testsessionId");
            SimulateValidation(modelFromService, cmsController);

            ////Assert
            cmsController.ModelState.IsValid.Should().Be(true);
        }

        [Fact(
            DisplayName =
                "CmsController: Post upsert, with valid model from formdata and invalid from session should be invalid model")]
        public void UpsertPostValidModelFromFormDataAndInvalidFromSession()
        {
            ////Arrange
            var model = Mapper.Map<PageUpsertViewModel>(DataHelper.GetPage());

            var modelFromService = new Page();
            sessionStorageService.Arrange(s => s.Get<PageUpsertViewModel>(Arg.IsAny<string>()))
                                 .Returns(Mapper.Map<PageUpsertViewModel>(modelFromService));

            ////Act
            try
            {
                var result = cmsController.Upsert(model, false, "testSessionId");
            }
            catch (Exception exception)
            {
                ////Assert
                exception.Message.Should().Be(
                    "The model of type 'Inspire.Portal.Models.Cms.PageUpsertViewModel' is not valid.");
            }
        }

        private void InitContext()
        {
            HttpContext.Current = CommonHelpers.GetHttpContext();
            var wrapper = new HttpContextWrapper(HttpContext.Current);
            cmsController.ControllerContext = new ControllerContext(
                wrapper,
                new RouteData(),
                cmsController);
            cmsController.Init();
        }
    }
}