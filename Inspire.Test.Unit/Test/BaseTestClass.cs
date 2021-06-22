namespace Inspire.Test.Unit.Test
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Web.Mvc;

    using AutoMapper;

    using Inspire.Core.Infrastructure.Logger;
    using Inspire.Core.Infrastructure.ResourceManager;
    using Inspire.Core.Infrastructure.TransactionManager;
    using Inspire.Portal.Infrastructure.AutoMapperProfile;

    using Telerik.JustMock;

    using ValidationContext = System.ComponentModel.DataAnnotations.ValidationContext;

    public class BaseTestClass
    {
        protected readonly IDbContextManager ContextManager;
        protected readonly ILogger Logger;
        protected readonly IMapper Mapper;
        protected readonly IResourceManager ResourceManager;

        protected BaseTestClass()
        {
            Logger = Mock.Create<ILogger>();
            Mapper = new MapperConfiguration(cfg => { cfg.AddProfile(new AutoMapperProfile()); }).CreateMapper();
            ContextManager = Mock.Create<IDbContextManager>();
            ResourceManager = Mock.Create<IResourceManager>();

            ModuleInitializer.Run();
        }

        protected void SimulateValidation(object model, Controller controller)
        {
            var validationContext = new ValidationContext(model, null, null);
            var validationResults = new List<ValidationResult>();
            Validator.TryValidateObject(model, validationContext, validationResults, true);
            foreach (var validationResult in validationResults)
            {
                controller.ModelState.AddModelError(
                    validationResult.MemberNames.FirstOrDefault(),
                    validationResult.ErrorMessage);
            }
        }
    }
}