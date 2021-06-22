namespace Inspire.Test.Unit
{
    using System;
    using System.Web.Mvc;
    using System.Web.Routing;

    using Inspire.Portal;

    using Telerik.JustMock;
    using Telerik.JustMock.Helpers;

    public sealed class ModuleInitializer : IDisposable
    {
        static ModuleInitializer()
        {
            Current = new ModuleInitializer();
        }

        private ModuleInitializer()
        {
            InitMvcApplication();
        }

        ~ModuleInitializer()
        {
            Dispose();
        }

        public static ModuleInitializer Current { get; }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            //// Assembly teardown code goes here
        }

        internal static void Run()
        {
        }

        private void InitMvcApplication()
        {
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            var view = Mock.Create<IView>();
            var engine = Mock.Create<IViewEngine>();

            var viewEngineResult = new ViewEngineResult(view, engine);
            engine.Arrange(
                      e => e.FindPartialView(Arg.IsAny<ControllerContext>(), Arg.IsAny<string>(), Arg.IsAny<bool>()))
                  .Returns(viewEngineResult);

            ViewEngines.Engines.Clear();
            ViewEngines.Engines.Add(engine);
        }
    }
}