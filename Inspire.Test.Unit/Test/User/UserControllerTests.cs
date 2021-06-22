namespace Inspire.Test.Unit.Test.User
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Routing;

    using FluentAssertions;

    using Inspire.Domain.Services;
    using Inspire.Infrastructure.Membership;
    using Inspire.Portal.Areas.Admin.Controllers;
    using Inspire.Portal.Areas.Admin.Models.User;
    using Inspire.Portal.Models.GeoNetwork.Group;
    using Inspire.Portal.Models.GeoNetwork.User;
    using Inspire.Portal.Services.RestApiService;
    using Inspire.Portal.Services.UserMailService;
    using Inspire.Test.Unit.Helpers;

    using Telerik.JustMock;
    using Telerik.JustMock.Helpers;

    using Xunit;

    public class UserControllerTests : BaseTestClass
    {
        private readonly IAccountService accountService;
        private readonly INomenclatureService nomenclatureService;
        private readonly IRestApiService restApiService;
        private readonly IRoleService roleService;
        private readonly UserController userController;
        private readonly IUserMailService userMailService;
        private readonly IUserService userService;
        private readonly IGroupService groupService;

        public UserControllerTests()
        {
            accountService = Mock.Create<IAccountService>();
            nomenclatureService = Mock.Create<INomenclatureService>();
            restApiService = Mock.Create<IRestApiService>();
            roleService = Mock.Create<IRoleService>();
            userMailService = Mock.Create<IUserMailService>();
            userService = Mock.Create<IUserService>();
            groupService = Mock.Create<IGroupService>();

            userController = new UserController(
                Logger,
                Mapper,
                ContextManager,
                ResourceManager,
                userService,
                userMailService,
                accountService,
                nomenclatureService,
                roleService,
                restApiService,
                groupService);

            InitContext();
        }

        [Fact(DisplayName = "UserController: Load Index view")]
        public void Index()
        {
            ////Act
            var result = userController.Index();
            ////Assert
            result.Should().NotBeNull();
        }

        [Fact(DisplayName = "UserController: Load Index should return SearchForm")]
        public void IndexShouldReturnSearchForm()
        {
            ////Act
            var result = userController.Index() as ViewResult;
            ////Assert
            result.ViewName.Should().Be("SearchForm");
        }

        [Fact(DisplayName = "UserController: Load Index ViewData should not be null")]
        public void IndexViewDataShouldNotBeNull()
        {
            ////Act
            var result = userController.Index() as ViewResult;
            ////Assert
            result.ViewData.Should().NotBeNull();
        }

        [Fact(DisplayName = "UserController: Get Upsert, should return upsert view")]
        public void GetUpsertShouldReturnView()
        {
            ////Act
            var result = userController.Upsert((Guid?)null);

            ////Assert
            result.Should().BeOfType(typeof(ViewResult));
        }

        [Fact(DisplayName = "UserController: Get Upsert with id, should return upsert view")]
        public void GetUpsertWithIdShouldReturnView()
        {
            ////Arrange
            var testUser = DataHelper.GetTestUser();
            var testRoles = DataHelper.GetTestRoles();
            userService.Arrange(s => s.Get(Arg.IsAny<Guid>())).Returns(testUser);
            roleService.Arrange(s => s.Get(Arg.IsAny<Guid>())).Returns(testRoles);

            ////Act
            var result = userController.Upsert(Guid.NewGuid());

            ////Assert
            result.Should().BeOfType(typeof(ViewResult));
            var mappedResult = result as ViewResult;
            mappedResult.Model.Should().Be(typeof(UserUpsertViewModel));
            var mappedModel = mappedResult.Model as UserUpsertViewModel;
            mappedModel.UserName.Should().NotBeNullOrWhiteSpace();
            mappedModel.UserName.Should().Be(testUser.UserName);
            mappedModel.Roles.Should().NotBeNullOrEmpty();
            mappedModel.Roles.Count.Should().Be(testRoles.Count);
        }

        [Fact(DisplayName = "UserController: Get Upsert with id and GeoNetworkId, should return upsert view")]
        public void GetUpsertWithIdAndGeoNetWorkIdShouldReturnView()
        {
            ////Arrange
            var testUser = DataHelper.GetTestUser();
            testUser.GeoNetworkId = 5;
            var testRoles = DataHelper.GetTestRoles();
            var testUserDTO = DataHelper.GetTestUserDTO();
            userService.Arrange(s => s.Get(Arg.IsAny<Guid>())).Returns(testUser);
            roleService.Arrange(s => s.Get(Arg.IsAny<Guid>())).Returns(testRoles);
            restApiService.Arrange(s => s.GetClient(Arg.IsAny<UserPrincipal>())).Returns(new HttpClient());
            restApiService.Arrange(s => s.GetRequest<UserDTO>(Arg.IsAny<HttpClient>(), Arg.IsAny<string>()))
                          .Returns(testUserDTO);
            restApiService.Arrange(s => s.GetRequest<List<UserGroup>>(Arg.IsAny<HttpClient>(), Arg.IsAny<string>()))
                          .Returns(testUserDTO);

            ////Act
            var result = userController.Upsert(Guid.NewGuid());

            ////Assert
            result.Should().BeOfType(typeof(ViewResult));
            var mappedResult = result as ViewResult;
            mappedResult.Model.Should().Be(typeof(UserUpsertViewModel));
            var mappedModel = mappedResult.Model as UserUpsertViewModel;
            mappedModel.UserName.Should().NotBeNullOrWhiteSpace();
            mappedModel.UserName.Should().Be(testUser.UserName);
            mappedModel.Roles.Should().NotBeNullOrEmpty();
            mappedModel.Roles.Count.Should().Be(testRoles.Count);
        }

        private void InitContext()
        {
            HttpContext.Current = CommonHelpers.GetHttpContext();
            var wrapper = new HttpContextWrapper(HttpContext.Current);
            userController.ControllerContext = new ControllerContext(
                wrapper,
                new RouteData(),
                userController);
            userController.Init();
        }
    }
}