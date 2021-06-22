namespace Inspire.Test.Integration.Test
{
    using System.Globalization;
    using System.Threading;
    using System.Web.Mvc;

    using Inspire.Core.Infrastructure.RequestData;
    using Inspire.Core.Infrastructure.TransactionManager;
    using Inspire.Domain.Repositories;
    using Inspire.Model.User;

    using Ninject;
    using Ninject.Web.Mvc;

    using Telerik.JustMock;

    public class BaseTestClass
    {
        protected BaseTestClass()
        {
            Thread.CurrentThread.CurrentCulture =
                Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo("bg-BG");

            InitKernel();
            ContextManager = Kernel.Get<IDbContextManager>();
            RequestData = Kernel.Get<IRequestData>();

            var dbManager = Kernel.Get<IDbManager>();
            dbManager.Init();
        }

        protected IDbContextManager ContextManager { get; }

        protected IRequestData RequestData { get; }

        protected CultureInfo CurrentCulture => Thread.CurrentThread.CurrentCulture;

        protected IKernel Kernel { get; private set; }

        protected void SetLoginUser(IUser user)
        {
            var mock = Kernel.Get<IRequestData>();

            var requestDataMock = Mock.Create<IRequestData>();
            Mock.Arrange(() => requestDataMock.Address).Returns(mock.Address);
            Mock.Arrange(() => requestDataMock.Host).Returns(mock.Host);
            Mock.Arrange(() => requestDataMock.Browser).Returns(mock.Browser);

            Mock.Arrange(() => requestDataMock.UserId).Returns(user.Id);
            Mock.Arrange(() => requestDataMock.UserName).Returns(user.Email);

            Kernel.Rebind<IRequestData>().ToConstant(requestDataMock).InSingletonScope();
        }

        private void InitKernel()
        {
            if (Kernel == null)
            {
                Kernel = new StandardKernel();
                Kernel.Load(new TestModule());

                DependencyResolver.SetResolver(new NinjectDependencyResolver(Kernel));
            }
        }
    }
}