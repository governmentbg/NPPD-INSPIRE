namespace Inspire.Test.Unit.Test.Home
{
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Routing;

    using Inspire.Core.Infrastructure.Cache;
    using Inspire.Domain.Services;
    using Inspire.Portal.Controllers;
    using Inspire.Portal.Services.RestApiService;
    using Inspire.Test.Unit.Helpers;

    using Telerik.JustMock;

    public class HomeControllerTests : BaseTestClass
    {
        private readonly IAdminService adminService;
        private readonly ICacheService cacheService;
        private readonly ICmsService cmsService;
        private readonly IFaqService faqService;
        private readonly HomeController homeController;
        private readonly INomenclatureService nomenclatureService;
        private readonly IProviderService providerService;
        private readonly IPublicationService publicationService;
        private readonly IRestApiService restApiService;

        public HomeControllerTests()
        {
            nomenclatureService = Mock.Create<INomenclatureService>();
            publicationService = Mock.Create<IPublicationService>();
            adminService = Mock.Create<IAdminService>();
            cacheService = Mock.Create<ICacheService>();
            faqService = Mock.Create<IFaqService>();
            cmsService = Mock.Create<ICmsService>();
            restApiService = Mock.Create<IRestApiService>();
            providerService = Mock.Create<IProviderService>();

            homeController = new HomeController(
                Logger,
                Mapper,
                ContextManager,
                nomenclatureService,
                publicationService,
                adminService,
                cacheService,
                faqService,
                cmsService,
                restApiService,
                providerService);
            InitContext();
        }

        private void InitContext()
        {
            HttpContext.Current = CommonHelpers.GetHttpContext();
            var wrapper = new HttpContextWrapper(HttpContext.Current);
            homeController.ControllerContext = new ControllerContext(
                wrapper,
                new RouteData(),
                homeController);
            homeController.Init();
        }
    }
}