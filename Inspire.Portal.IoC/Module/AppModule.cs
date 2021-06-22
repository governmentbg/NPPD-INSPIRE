namespace Inspire.Portal.IoC.Module
{
    using System.Configuration;
    using System.Web;

    using Inspire.Core.Infrastructure.Context;
    using Inspire.Core.Infrastructure.Logger;
    using Inspire.Core.Infrastructure.Membership;
    using Inspire.Core.Infrastructure.TransactionManager;
    using Inspire.Data.Context;
    using Inspire.Domain.Repositories;
    using Inspire.Domain.Services;
    using Inspire.Infrastructure.Logger;
    using Inspire.Infrastructure.Membership;
    using Inspire.Repository;
    using Inspire.Repository.Utilities;
    using Inspire.Services;

    using Ninject;
    using Ninject.Activation;
    using Ninject.Modules;
    using Ninject.Web.Common;

    public class AppModule : NinjectModule
    {
        public override void Load()
        {
            if (Kernel != null)
            {
                Kernel.Settings.AllowNullInjection = true;
            }

            Bind<HttpContext>().ToMethod(ctx => HttpContext.Current).InRequestScope();

            Bind<IAisContext>().To<AisContext>()
                               .When(x => HttpContext.Current != null)
                               .InRequestScope()
                               .WithConstructorArgument(
                                   "connectionString",
                                   GetConnectionByUser);

            Bind<IAisContext>().To<AisContext>()
                               .When(x => HttpContext.Current == null)
                               .InThreadScope()
                               .WithConstructorArgument(
                                   "connectionString",
                                   ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);

            Bind<IDbContextManager>().To<DbContextManager>().When(x => HttpContext.Current != null)
                                     .InRequestScope();
            Bind<IDbContextManager>().To<DbContextManager>().When(x => HttpContext.Current == null)
                                     .InThreadScope();

            Bind<IConnectionContext>().To<ConnectionContext>().When(x => HttpContext.Current != null)
                                      .InRequestScope();
            Bind<IConnectionContext>().To<ConnectionContext>().When(x => HttpContext.Current == null)
                                      .InThreadScope();

            Bind<IUserPrincipal>().To<UserPrincipal>();
            Bind<ILogger>().To<NLogger>();

            Bind<IDbManager>().To<DbManager>().InSingletonScope();

            BindRepositories();
            BindServices();
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
            Bind<ILogRepository>().To<LogRepository>();
            Bind<ICmsRepository>().To<CmsRepository>();
            Bind<IFaqRepository>().To<FaqRepository>();
            Bind<IProviderRepository>().To<ProviderRepository>();
            Bind<IGroupRepository>().To<GroupRepository>();
            Bind<IPollRepository>().To<PollRepository>();
            Bind<ITransactionHistoryRepository>().To<TransactionHistoryRepository>();
            Bind<IGeonetworkRepository>().To<GeonetworkRepository>();
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
            Bind<ILogService>().To<LogService>();
            Bind<ICmsService>().To<CmsService>();
            Bind<IFaqService>().To<FaqService>();
            Bind<IProviderService>().To<ProviderService>();
            Bind<IGroupService>().To<GroupService>();
            Bind<IPollService>().To<PollService>();
            Bind<ITransactionHistoryService>().To<TransactionHistoryService>();
            Bind<IGeonetworkService>().To<GeonetworkService>();
        }

        private string GetConnectionByUser(IContext context)
        {
            var httpContext = context.Kernel.Get<HttpContextBase>();
            var connectionName = httpContext?.User?.Identity?.IsAuthenticated == true
                ? "LoginUserConnection"
                : "DefaultConnection";

            return ConfigurationManager.ConnectionStrings[connectionName].ConnectionString;
        }
    }
}