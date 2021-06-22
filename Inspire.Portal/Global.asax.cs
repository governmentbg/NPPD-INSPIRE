namespace Inspire.Portal
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Threading;
    using System.Web;
    using System.Web.Mvc;

    using Inspire.Common.Mvc.Infrastructure.BaseTypes;
    using Inspire.Core.Infrastructure.Configuration;
    using Inspire.Core.Infrastructure.Logger;
    using Inspire.Core.Infrastructure.Membership;

    public class Global : HttpApplication
    {
        public static readonly List<LanguageElement> Languages =
            LanguageSection.Instance.Languages.Cast<LanguageElement>().ToList();

        public static readonly Dictionary<CultureInfo, string> Cultures = Languages.ToDictionary(
            key => key.Culture,
            val => val.Id.ToString());

        public static readonly CultureInfo[] AllowCultures = Cultures.Keys.ToArray();

        private static ILogger Logger => DependencyResolver.Current.GetService<ILogger>();

        public override string GetVaryByCustomString(HttpContext context, string custom)
        {
            return custom == "culture"
                ? Thread.CurrentThread.CurrentUICulture.ToString()
                : base.GetVaryByCustomString(context, custom);
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            var exception = Server.GetLastError().GetBaseException();
            if (Logger != null)
            {
                Logger.Error(exception);

                var innerException = exception.InnerException;
                while (innerException != null)
                {
                    Logger.Error(innerException);
                    innerException = innerException.InnerException;
                }
            }
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            if (sender is HttpApplication app)
            {
                app.Context.Response.Headers.Remove("Server");
            }
        }

        protected void Application_AcquireRequestState(object sender, EventArgs e)
        {
            if (Context.Session == null)
            {
                return;
            }

            var userPrincipal = Context.Session[BaseController.SessionUser] as IUserPrincipal;
            Context.Session[BaseController.SessionUser] = Context.User =
            userPrincipal?.UserName?.Equals(
                Context.User?.Identity?.Name,
                StringComparison.InvariantCultureIgnoreCase) == true
                ? userPrincipal
                : null;
        }
    }
}