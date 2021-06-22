namespace Inspire.Test.Unit.Helpers
{
    using System;
    using System.Collections.Specialized;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Routing;

    using Inspire.Common.Mvc.Infrastructure.BaseTypes;
    using Inspire.Infrastructure.Membership;
    using Inspire.Portal;

    using Telerik.JustMock;
    using Telerik.JustMock.Helpers;

    internal static class ControllerContextHelper
    {
        /// <param name="controller">controller</param>
        /// <summary>
        ///     Returns a standard controller context with no User, non-ajax request type and no server data
        /// </summary>
        public static void Init(this Controller controller)
        {
            var httpContext = GetHttpContextBase();

            var controllerContext = Mock.Create<ControllerContext>();
            controllerContext.Arrange(c => c.HttpContext).Returns(httpContext);
            controllerContext.Arrange(c => c.RouteData).Returns(RouteTable.Routes.GetRouteData(httpContext));

            controller.ControllerContext = controllerContext;

            var requestContext = Mock.Create<RequestContext>();
            requestContext.Arrange(r => r.HttpContext).Returns(httpContext);
            requestContext.Arrange(r => r.RouteData).Returns(RouteTable.Routes.GetRouteData(httpContext));
            requestContext.Arrange(x => x.HttpContext.Request.UrlReferrer)
                          .Returns(new Uri("http://localhost/"));

            var routes = new RouteCollection();
            RouteConfig.RegisterRoutes(routes);

            var url = Mock.Create<UrlHelper>(requestContext, routes);

            url.Arrange(u => u.Content(Arg.IsAny<string>())).Returns("~/");
            url.Arrange(u => u.IsLocalUrl(Arg.IsAny<string>())).Returns(true);
            controller.Url = url;

            controller.AddServerData();
        }

        public static void SetUser(this Controller controller, UserPrincipal user)
        {
            controller.Session[BaseController.SessionUser] = controller.HttpContext.User = user;
        }

        public static void SetAjax(this Controller controller)
        {
            controller.Request.Arrange(r => r["X-Requested-With"]).Returns("XMLHttpRequest");
        }

        private static void AddServerData(this Controller controller)
        {
            var server = Mock.Create<HttpServerUtilityBase>();

            controller.ControllerContext.HttpContext.Request
                      .Arrange(x => x.ServerVariables)
                      .Returns(
                          new NameValueCollection
                          {
                              { "SERVER_NAME", "localhost" },
                              { "SCRIPT_NAME", "localhost" },
                              { "SERVER_PORT", "80" },
                              { "REMOTE_ADDR", "127.0.0.1" },
                              { "REMOTE_HOST", "127.0.0.1" }
                          });

            controller.ControllerContext.HttpContext
                      .Arrange(x => x.Server)
                      .Returns(server);
        }

        private static HttpRequestBase GetRequestBase()
        {
            var request = Mock.Create<HttpRequestBase>();

            request.Arrange(req => req.ApplicationPath).Returns("/");
            request.Arrange(req => req.AppRelativeCurrentExecutionFilePath).Returns("~/");
            request.Arrange(req => req.PathInfo).Returns(string.Empty);

            var url = Mock.Create<Uri>("http://localhost/");
            request.Arrange(req => req.Url).Returns(url);

            return request;
        }

        private static HttpResponseBase GetResponseBase()
        {
            var response = Mock.Create<HttpResponseBase>();

            response.Arrange(res => res.ApplyAppPathModifier(Arg.IsAny<string>())).Returns<string>(s => s);

            return response;
        }

        private static HttpContextBase GetHttpContextBase()
        {
            var httpContext = Mock.Create<HttpContextBase>();
            var request = GetRequestBase();
            var response = GetResponseBase();
            var server = Mock.Create<HttpServerUtilityBase>();
            var requestContext = Mock.Create<RequestContext>();
            var session = Mock.Create<HttpSessionStateBase>();

            Mock.Arrange(() => httpContext.Request.ApplicationPath)
                .Returns("http://localhost");

            Mock.Arrange(() => httpContext.Response.ApplyAppPathModifier(Arg.IsAny<string>()))
                .Returns(arg => arg);

            Mock.Arrange(() => httpContext.GetService(typeof(HttpWorkerRequest)))
                .Returns(Mock.Create<HttpWorkerRequest>(Behavior.CallOriginal));

            httpContext.Arrange(x => x.Request).Returns(request);
            httpContext.Arrange(x => x.Response).Returns(response);
            httpContext.Arrange(ctx => ctx.Server).Returns(server);
            httpContext.Arrange(ctx => ctx.Session).Returns(session);
            requestContext.Arrange(rc => rc.HttpContext).Returns(httpContext);
            requestContext.Arrange(rc => rc.RouteData).Returns(RouteTable.Routes.GetRouteData(httpContext));
            httpContext.Request.Arrange(rq => rq.RequestContext).Returns(requestContext);

            return httpContext;
        }
    }
}