namespace Inspire.Test.Integration.Test
{
    using System;
    using System.Configuration;
    using System.Threading;

    using AutoMapper;

    using Inspire.Core.Infrastructure.Cache;
    using Inspire.Core.Infrastructure.Context;
    using Inspire.Core.Infrastructure.Logger;
    using Inspire.Core.Infrastructure.RequestData;
    using Inspire.Core.Infrastructure.TransactionManager;
    using Inspire.Data.Context;
    using Inspire.Domain.Repositories;
    using Inspire.Domain.Services;
    using Inspire.Infrastructure.Logger;
    using Inspire.Portal;
    using Inspire.Portal.Infrastructure.AutoMapperProfile;
    using Inspire.Portal.Utilities;
    using Inspire.Repository;
    using Inspire.Repository.Utilities;
    using Inspire.Services;

    using Ninject.Modules;

    using Telerik.JustMock;

    internal class TestModule : NinjectModule
    {
        public override void Load()
        {
            if (Kernel != null)
            {
                Kernel.Settings.AllowNullInjection = true;
            }

            // DbContextManager in request scope
            Bind<IAisContext>().To<AisContext>()
                               .InThreadScope()
                               .WithConstructorArgument(
                                   "connectionString",
                                   ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);

            Bind<IDbContextManager>().To<DbContextManager>().InThreadScope();
            Bind<IConnectionContext>().To<ConnectionContext>().InThreadScope();

            Bind<ILogger>().To<NLogger>();

            // Bind AutoMapper profile - services used IMapper
            Bind<IMapper>()
                .ToConstant(
                    new MapperConfiguration(
                            cfg => { cfg.AddProfile(new AutoMapperProfile()); })
                        .CreateMapper());

            var requestDataMock = Mock.Create<IRequestData>();
            Mock.Arrange(() => requestDataMock.Address).Returns("Address");
            Mock.Arrange(() => requestDataMock.Host).Returns("Host");
            Mock.Arrange(() => requestDataMock.Browser).Returns("Browser");
            Mock.Arrange(() => requestDataMock.UserName).Returns("AutomationUser");
            Mock.Arrange(() => requestDataMock.UserId).Returns(ConfigurationReader.AutomationUserId);
            Mock.Arrange(() => requestDataMock.LanguageId)
                .Returns(Guid.Parse(Global.Cultures[Thread.CurrentThread.CurrentUICulture]));

            Bind<IRequestData>().ToConstant(requestDataMock).InThreadScope();
            Bind<IDbManager>().To<DbManager>().InSingletonScope();

            BindRepositories();
            BindServices();

            Bind<ICacheService>().To<NoCacheService>();
            Bind<IStorageService>().ToConstant(Mock.Create<IStorageService>());
        }

        private void BindRepositories()
        {
            Bind<IUserRepository>().To<UserRepository>();
            Bind<INomenclatureRepository>().To<NomenclatureRepository>();
            Bind<IPublicationRepository>().To<PublicationRepository>();
            Bind<IAccountRepository>().To<AccountRepository>();
            Bind<IAttachmentRepository>().To<AttachmentRepository>();
            Bind<IRoleRepository>().To<RoleRepository>();
            Bind<IEmailRepository>().To<EmailRepository>();
            Bind<IAdminRepository>().To<AdminRepository>();
            Bind<IHistoryRepository>().To<HistoryRepository>();
            Bind<ISearchRepository>().To<SearchRepository>();
            Bind<IProviderRepository>().To<ProviderRepository>();
        }

        private void BindServices()
        {
            Bind<IUserService>().To<UserService>();
            Bind<INomenclatureService>().To<NomenclatureService>();
            Bind<IPublicationService>().To<PublicationService>();
            Bind<IAccountService>().To<AccountService>();
            Bind<IRoleService>().To<RoleService>();
            Bind<IEmailService>().To<EmailService>();
            Bind<IAdminService>().To<AdminService>();
            Bind<IHistoryService>().To<HistoryService>();
            Bind<ISearchService>().To<SearchService>();
            Bind<IProviderService>().To<ProviderService>();
        }
    }
}