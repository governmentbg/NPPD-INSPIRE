namespace Inspire.Portal.App_Start
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Net;
    using System.Web;
    using System.Web.Helpers;
    using System.Web.Mvc;
    using System.Web.Optimization;
    using System.Web.Routing;

    using AutoMapper;

    using Inspire.Common.Mvc.Infrastructure.ModelBinders;
    using Inspire.Common.Mvc.ProviderFactories;
    using Inspire.Core.Infrastructure.Cache;
    using Inspire.Core.Infrastructure.Logger;
    using Inspire.Core.Infrastructure.RequestData;
    using Inspire.Core.Infrastructure.ResourceManager;
    using Inspire.Domain.Repositories;
    using Inspire.Domain.Services;
    using Inspire.Portal.Filters;
    using Inspire.Portal.Infrastructure.Attribute;
    using Inspire.Portal.Infrastructure.AuthenticationProvider;
    using Inspire.Portal.Infrastructure.AutoMapperProfile;
    using Inspire.Portal.Infrastructure.Cache;
    using Inspire.Portal.Infrastructure.RequestData;
    using Inspire.Portal.Infrastructure.ResourceManager;
    using Inspire.Portal.IoC.Module;
    using Inspire.Portal.Services.CaptchaService;
    using Inspire.Portal.Services.RestApiService;
    using Inspire.Portal.Services.SessionStorageService;
    using Inspire.Portal.Services.StorageService;
    using Inspire.Portal.Services.UserMailService;
    using Inspire.Portal.Utilities;

    using Ninject;
    using Ninject.Web.Common;

    using Owin;

    public class Startup
    {
        public static IKernel CreateKernel()
        {
            var kernel = new StandardKernel();
            kernel.Bind<Func<IKernel>>().ToMethod(ctx => () => new Bootstrapper().Kernel);
            kernel.Bind<IHttpModule>().To<HttpApplicationInitializationHttpModule>();
            kernel.Load(new AppModule());

            kernel.Bind<IMapper>().ToConstant(
                new MapperConfiguration(
                        cfg =>
                        {
                            cfg.AllowNullCollections = true;
                            cfg.AllowNullDestinationValues = true;
                            cfg.AddProfile(new AutoMapperProfile());
                        })
                    .CreateMapper());

            kernel.Bind<IRequestData>().To<RequestData>();
            kernel.Bind<IResourceManager>().To<ResourceManager>();
            kernel.Bind<IAuthenticationProvider>().To<AuthenticationProvider>()
                  .When(x => HttpContext.Current != null)
                  .InRequestScope();
            kernel.Bind<ICacheService>().To<MemoryCacheService>();
            kernel.Bind<ICaptchaService>().To<ReCaptchaService>();
            kernel.Bind<ISessionStorageService>().To<SessionStorageService>();
            kernel.Bind<IStorageService>().To<StorageService>();
            kernel.Bind<IMailSender>().To<MailSender>();
            kernel.Bind<IUserMailService>().To<UserMailService>();
            kernel.Bind<IRestApiService>().To<RestApiService>();

            return kernel;
        }

        public void Configuration(IAppBuilder app)
        {
            AppStart();

            var kernel = CreateKernel();

            InitNinject(app, kernel);
            InitDb(kernel);

            try
            {
                // Clear temp directory
                var storageService = kernel.Get<IStorageService>();
                storageService.ClearTempDirectory();
            }
            catch (Exception e)
            {
                kernel.Get<ILogger>().Error(e);
            }
        }

        private static void AppStart()
        {
            ////Scripts.DefaultTagFormat = @"<script src=""{0}"" async></script>";
            Styles.DefaultTagFormat = @"<link href=""{0}"" rel=""stylesheet"" async/>";

            MvcHandler.DisableMvcResponseHeader = true;

            AreaRegistration.RegisterAllAreas();

            ViewEngines.Engines.Clear();
            ViewEngines.Engines.Add(new RazorViewEngine { FileExtensions = new[] { "cshtml" } });

            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            BundleTable.EnableOptimizations = false;
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            BindersConfig.RegisterModelBinders(ModelBinders.Binders);

            // Create controller with localization - before model bind
            ControllerBuilder.Current.SetControllerFactory(
                new DefaultControllerFactory(new LocalizationControllerActivator()));

            // Remove and JsonValueProviderFactory and add JsonDotNetValueProviderFactory
            if (ConfigurationReader.ChangeDefaultJsonValueProviderFactory)
            {
                ValueProviderFactories.Factories.Remove(
                    ValueProviderFactories
                        .Factories.OfType<JsonValueProviderFactory>().FirstOrDefault());
                ValueProviderFactories.Factories.Add(new JsonDotNetValueProviderFactory());
            }

            // Add client validation localization - for this moment is only for DateType and RequiredAttribute
            ClientDataTypeModelValidatorProvider.ResourceClassKey = "Resource";
            DefaultModelBinder.ResourceClassKey = "Resource";

            DataAnnotationsModelValidatorProvider.AddImplicitRequiredAttributeForValueTypes = false;
            DataAnnotationsModelValidatorProvider.RegisterAdapter(
                typeof(RequiredAttribute),
                typeof(LocalizationRequiredAttributeAdapter));

            AntiForgeryConfig.SuppressIdentityHeuristicChecks = true;

            ServicePointManager.ServerCertificateValidationCallback = (s, certificate, chain, sslPolicyErrors) => true;

            // Changed default model binder - may to use custom property binder as attribute in view model. Example is decrypting of ids
            ModelBinders.Binders.DefaultBinder = new ExtendedModelBinder();
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            // If you have enabled SSL. Uncomment this line to ensure that the Anti-Forgery
            // cookie requires SSL to be sent across the wire.
            AntiForgeryConfig.RequireSsl = ConfigurationReader.AntiForgeryConfigRequireSsl;

            // Rename the Anti-Forgery cookie from "__RequestVerificationToken" to "f".
            // This adds a little security through obscurity and also saves sending a
            // few characters over the wire.
            AntiForgeryConfig.CookieName = "f";
        }

        private static void InitNinject(IAppBuilder app, IKernel kernel)
        {
            app.UseNinject(() => kernel);
        }

        private static void InitDb(IKernel kernel)
        {
            var dbManager = kernel.Get<IDbManager>();
            dbManager.Init();
        }
    }
}