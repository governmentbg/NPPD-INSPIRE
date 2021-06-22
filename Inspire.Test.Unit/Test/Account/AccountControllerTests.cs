namespace Inspire.Test.Unit.Test.Account
{
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Routing;

    using Inspire.Domain.Services;
    using Inspire.Portal.Controllers;
    using Inspire.Portal.Infrastructure.AuthenticationProvider;
    using Inspire.Portal.Services.CaptchaService;
    using Inspire.Portal.Services.RestApiService;
    using Inspire.Portal.Services.UserMailService;
    using Inspire.Test.Unit.Helpers;

    using Telerik.JustMock;

    public class AccountControllerTests : BaseTestClass
    {
        private readonly AccountController accountController;
        private readonly IAccountService accountService;
        private readonly IAuthenticationProvider authenticationProvider;
        private readonly ICaptchaService captchaService;
        private readonly IRestApiService restApiService;
        private readonly IUserMailService userMailService;
        private readonly IUserService userService;

        public AccountControllerTests()
        {
            accountService = Mock.Create<IAccountService>();
            authenticationProvider = Mock.Create<IAuthenticationProvider>();
            captchaService = Mock.Create<ICaptchaService>();
            restApiService = Mock.Create<IRestApiService>();
            userMailService = Mock.Create<IUserMailService>();
            userService = Mock.Create<IUserService>();
            accountController = new AccountController(
                Logger,
                Mapper,
                ContextManager,
                accountService,
                authenticationProvider,
                userService,
                userMailService,
                captchaService,
                restApiService);
            InitContext();
        }

        private void InitContext()
        {
            HttpContext.Current = CommonHelpers.GetHttpContext();
            var wrapper = new HttpContextWrapper(HttpContext.Current);
            accountController.ControllerContext = new ControllerContext(
                wrapper,
                new RouteData(),
                accountController);
            accountController.Init();
        }
    }
}