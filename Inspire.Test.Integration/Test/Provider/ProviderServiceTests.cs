namespace Inspire.Test.Integration.Test.Provider
{
    using Inspire.Domain.Services;

    using Ninject;

    public class ProviderServiceTests : BaseTestClass
    {
        private readonly IProviderService providerService;

        public ProviderServiceTests()
        {
            providerService = Kernel.Get<IProviderService>();
        }
    }
}