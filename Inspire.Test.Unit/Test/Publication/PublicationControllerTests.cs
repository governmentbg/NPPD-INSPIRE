namespace Inspire.Test.Unit.Test.Publication
{
    using System;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Routing;

    using FluentAssertions;

    using Inspire.Domain.Services;
    using Inspire.Model.Helpers;
    using Inspire.Model.Publication;
    using Inspire.Portal.Controllers;
    using Inspire.Portal.Models.Publication;
    using Inspire.Portal.Services.SessionStorageService;
    using Inspire.Test.Unit.Helpers;

    using Telerik.JustMock;
    using Telerik.JustMock.Helpers;

    using Xunit;

    public class PublicationControllerTests : BaseTestClass
    {
        private readonly INomenclatureService nomenclatureService;
        private readonly PublicationController publicationController;
        private readonly IPublicationService publicationService;
        private readonly ISessionStorageService sessionStorageService;

        public PublicationControllerTests()
        {
            publicationService = Mock.Create<IPublicationService>();
            nomenclatureService = Mock.Create<INomenclatureService>();
            sessionStorageService = Mock.Create<ISessionStorageService>();

            publicationController = new PublicationController(
                Logger,
                Mapper,
                ContextManager,
                ResourceManager,
                nomenclatureService,
                publicationService,
                sessionStorageService);

            InitContext();
        }

        [Fact(DisplayName = "PublicationController: Load Index view")]
        public void Index()
        {
            ////Act
            var result = publicationController.Index();
            ////Assert
            result.Should().NotBeNull();
        }

        [Fact(DisplayName = "PublicationController: Load Index should return SearchForm")]
        public void IndexShouldReturnSearchForm()
        {
            ////Act
            var result = publicationController.Index() as ViewResult;
            ////Assert
            result.ViewName.Should().Be("SearchForm");
        }

        [Fact(DisplayName = "PublicationController: Load Index ViewData should not be null")]
        public void IndexViewDataShouldNotBeNull()
        {
            ////Act
            var result = publicationController.Index() as ViewResult;
            ////Assert
            result.ViewData.Should().NotBeNull();
        }

        [Fact(DisplayName = "PublicationController: Get Upsert, should return upsert view")]
        public void GetUpsertShouldReturnView()
        {
            ////Act
            var result = publicationController.Upsert(PublicationType.Event);
            ////Assert
            result.Should().BeOfType(typeof(ViewResult));
        }

        [Fact(
            DisplayName = "PublicationController: Get Upsert with event type should return publicationUpsertviewmodel")]
        public void GetUpsertShouldReturnModel()
        {
            ////Act
            var result = publicationController.Upsert(PublicationType.Event) as ViewResult;
            ////Assert
            result.Model.Should().BeOfType(typeof(PublicationUpsertViewModel));
        }

        [Fact(DisplayName = "PublicationController: Get Upsert with event type should return model with type event")]
        public void GetUpsertEvent()
        {
            ////Act
            var result = publicationController.Upsert(PublicationType.Event) as ViewResult;
            ////Assert
            result.Model.Should().BeOfType(typeof(PublicationUpsertViewModel));
            var model = result.Model as PublicationUpsertViewModel;
            Assert.Equal(model.Type.Id, EnumHelper.GetPublicationType(PublicationType.Event));
        }

        [Fact(DisplayName = "PublicationController: Get Upsert with news type should return model with type news")]
        public void GetUpsertNews()
        {
            ////Act
            var result = publicationController.Upsert(PublicationType.News) as ViewResult;
            ////Assert
            result.Model.Should().BeOfType(typeof(PublicationUpsertViewModel));
            var model = result.Model as PublicationUpsertViewModel;
            Assert.Equal(model.Type.Id, EnumHelper.GetPublicationType(PublicationType.News));
        }

        [Fact(DisplayName = "PublicationController: Post Upsert should redirect")]
        public void PostUpsertNews()
        {
            ////Arrange
            var model = DataHelper.GetPublicationNews();
            ////Act
            var result = publicationController.Upsert(model);
            ////Assert
            result.Should().BeOfType(typeof(RedirectToRouteResult));
        }

        [Fact(DisplayName = "PublicationController: Post Upsert invalid model, should be invalid modelstate")]
        public void PostUpsertInvalidModel()
        {
            ////Arrange
            var model = new PublicationUpsertViewModel();
            ////Act
            SimulateValidation(model, publicationController);
            var result = publicationController.Upsert(model);
            ////Assert
            publicationController.ModelState.IsValid.Should().Be(false);
        }

        [Fact(DisplayName = "PublicationController: Post Upsert valid model, should be valid modelstate")]
        public void PostUpsertValidModel()
        {
            ////Arrange
            var model = DataHelper.GetPublicationNews();
            ////Act
            SimulateValidation(model, publicationController);
            var result = publicationController.Upsert(model);
            ////Assert
            publicationController.ModelState.IsValid.Should().Be(true);
        }

        [Fact(DisplayName = "PublicationController: Post Upsert valid model with preview, should redirect")]
        public void PostUpsertValidModelWithPreview()
        {
            ////Arrange
            var model = DataHelper.GetPublicationNews();
            ////Act
            SimulateValidation(model, publicationController);
            var result = publicationController.Upsert(model, true);
            ////Assert
            result.Should().BeOfType(typeof(RedirectToRouteResult));
            var redirectResult = result as RedirectToRouteResult;
            Assert.Equal("Preview", redirectResult.RouteValues.Values.FirstOrDefault().ToString());
        }

        [Fact(DisplayName = "PublicationController: Post Upsert invalid model, return to upsert view")]
        public void PostUpsertInvalidModelReturnToUpsertView()
        {
            ////Arrange
            var model = DataHelper.GetPublicationNews();
            model.Titles = null;
            ////Act
            SimulateValidation(model, publicationController);
            var result = publicationController.Upsert(model, true);
            ////Assert
            result.Should().BeOfType(typeof(ViewResult));
            var viewResult = result as ViewResult;
            viewResult.ViewName.Should().Be(string.Empty);
        }

        [Fact(DisplayName = "PublicationController: Get Preview, should return view")]
        public void GetPreview()
        {
            ////Arrange
            sessionStorageService.Arrange(s => s.Get<PublicationUpsertViewModel>(Arg.IsAny<string>()))
                                 .Returns(DataHelper.GetPublicationNews);
            ////Act
            var result = publicationController.Preview("testSession");
            ////Assert
            result.Should().BeOfType(typeof(ViewResult));
        }

        [Fact(DisplayName = "PublicationController: Get Preview, should return info view")]
        public void GetPreviewInfo()
        {
            ////Arrange
            sessionStorageService.Arrange(s => s.Get<PublicationUpsertViewModel>(Arg.IsAny<string>()))
                                 .Returns(DataHelper.GetPublicationNews);
            ////Act
            var result = publicationController.Preview("testSession") as ViewResult;
            ////Assert
            result.ViewName.Should().Be("Info");
        }

        [Fact(DisplayName = "PublicationController: Get Preview, should have viewbag ispreview")]
        public void GetPreviewViewBag()
        {
            ////Arrange
            sessionStorageService.Arrange(s => s.Get<PublicationUpsertViewModel>(Arg.IsAny<string>()))
                                 .Returns(DataHelper.GetPublicationNews);
            ////Act
            var result = publicationController.Preview("testSession") as ViewResult;
            ////Assert
            var isPreview = result.ViewBag.IsPreview as bool?;
            isPreview.Should().Be(true);
        }

        [Fact(DisplayName = "PublicationController: Post Save, should throw exception wtihout sessionid")]
        public void PostSaveIncorrect()
        {
            ////Act
            try
            {
                var result = publicationController.Save();
            }
            ////Assert
            catch (Exception e)
            {
                e.Should().BeOfType(typeof(ArgumentOutOfRangeException));
            }
        }

        private void InitContext()
        {
            HttpContext.Current = CommonHelpers.GetHttpContext();
            var wrapper = new HttpContextWrapper(HttpContext.Current);
            publicationController.ControllerContext = new ControllerContext(
                wrapper,
                new RouteData(),
                publicationController);
            publicationController.Init();
        }
    }
}