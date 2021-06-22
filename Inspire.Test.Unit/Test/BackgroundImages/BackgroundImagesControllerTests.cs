namespace Inspire.Test.Unit.Test.BackgroundImage
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Routing;

    using FluentAssertions;

    using Inspire.Domain.Services;
    using Inspire.Model.Attachment;
    using Inspire.Portal.Areas.Admin.Controllers;
    using Inspire.Test.Unit.Helpers;

    using Telerik.JustMock;
    using Telerik.JustMock.Helpers;

    using Xunit;

    public class BackgroundImagesControllerTests : BaseTestClass
    {
        private readonly IAdminService adminService;
        private readonly BackgroundImagesController backgroundImagesController;
        private readonly IStorageService storageService;

        public BackgroundImagesControllerTests()
        {
            adminService = Mock.Create<IAdminService>();
            storageService = Mock.Create<IStorageService>();

            backgroundImagesController = new BackgroundImagesController(
                Logger,
                Mapper,
                ContextManager,
                adminService,
                storageService);

            InitContext();
        }

        [Fact(DisplayName = "BackgroundImagesController: index")]
        public void GetIndex()
        {
            ////Act
            var result = backgroundImagesController.Index();
            ////Assert
            result.Should().BeOfType(typeof(ViewResult));
        }

        [Fact(DisplayName = "BackgroundImagesController: index should return view List of attachment")]
        public void GetIndexReturnModel()
        {
            ////Arrange
            var shouldReturn = new List<Attachment> { DataHelper.GetTestImage() };
            adminService.Arrange(s => s.GetHomeImages()).Returns(shouldReturn);
            ////Act
            var result = backgroundImagesController.Index() as ViewResult;
            ////Assert
            result.Model.Should().BeOfType<List<Attachment>>();
        }

        [Fact(
            DisplayName = "BackgroundImagesController: index should return view List of attachment from getHomeImages")]
        public void GetIndexReturnedModel()
        {
            ////Arrange
            var model = DataHelper.GetTestImage();
            model.Name = "TestName";
            var shouldReturn = new List<Attachment> { model };
            adminService.Arrange(s => s.GetHomeImages()).Returns(shouldReturn);
            ////Act
            var result = backgroundImagesController.Index() as ViewResult;
            ////Assert
            var returnedModel = result.Model as List<Attachment>;
            returnedModel.FirstOrDefault().Name.Should().Be("TestName");
        }

        [Fact(DisplayName = "BackgroundImagesController: Get Upsert, should return partial view")]
        public void GetUpsert()
        {
            ////Act
            var result = backgroundImagesController.Upsert(string.Empty);
            ////Assert
            result.Should().BeOfType(typeof(PartialViewResult));
        }

        [Fact(DisplayName = "BackgroundImagesController: Get Upsert, should return partial view")]
        public void GetUpsertViewName()
        {
            ////Act
            var result = backgroundImagesController.Upsert(string.Empty) as PartialViewResult;
            ////Assert
            result.ViewName.Should().Be("_Upsert");
        }

        [Fact(DisplayName = "BackgroundImagesController: Post upsert, should return json with success")]
        public void PostUpsert()
        {
            ////Arrange
            var model = DataHelper.GetTestImage();
            ////Act
            var result = backgroundImagesController.Upsert(model, string.Empty);
            ////Assert
            result.Should().BeOfType(typeof(JsonResult));
        }

        private void InitContext()
        {
            HttpContext.Current = CommonHelpers.GetHttpContext();
            var wrapper = new HttpContextWrapper(HttpContext.Current);
            backgroundImagesController.ControllerContext = new ControllerContext(
                wrapper,
                new RouteData(),
                backgroundImagesController);
            backgroundImagesController.Init();
        }
    }
}