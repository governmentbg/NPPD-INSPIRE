namespace Inspire.Portal.Filters
{
    using System;
    using System.Configuration;
    using System.Globalization;
    using System.Linq;
    using System.Threading;
    using System.Web.Configuration;
    using System.Web.Mvc;
    using System.Web.Routing;

    using Inspire.Utilities.Extensions;
    using Inspire.Utilities.Utilities;

    public class LocalizationControllerActivator : IControllerActivator
    {
        private GlobalizationSection globalizationSection;

        private GlobalizationSection GlobalizationSection => globalizationSection ?? (globalizationSection =
            ConfigurationManager.GetSection("system.web/globalization") as GlobalizationSection);

        public IController Create(RequestContext requestContext, Type controllerType)
        {
            InitClientCulture(requestContext);
            return DependencyResolver.Current.GetService(controllerType) as IController;
        }

        private void InitClientCulture(RequestContext httpContext)
        {
            var routeData = httpContext.RouteData;

            CultureInfo clientCulture = null;
            try
            {
                var languageRouteData = routeData.Values.ContainsKey(RouteConfig.CultureKeyName)
                    ? routeData.Values[RouteConfig.CultureKeyName]
                    : null;
                if (languageRouteData != null)
                {
                    clientCulture = new CultureInfo(languageRouteData.ToString());
                }
                else if (httpContext.HttpContext.Request.UserLanguages.IsNotNullOrEmpty() &&
                         GlobalizationSection != null && GlobalizationSection.EnableClientBasedCulture)
                {
                    clientCulture = new CultureInfo(
                        httpContext.HttpContext.Request.UserLanguages.First().Substring(0, 2));
                }
            }
            catch
            {
            }

            SetClientCulture(clientCulture);
        }

        private void SetClientCulture(CultureInfo clientCulture)
        {
            if (clientCulture == null || !Global.AllowCultures.Contains(
                clientCulture,
                new LambdaComparer<CultureInfo>((x, y) => x.TwoLetterISOLanguageName == y.TwoLetterISOLanguageName)))
            {
                if (GlobalizationSection != null && GlobalizationSection.Culture.IsNotNullOrEmpty())
                {
                    clientCulture = new CultureInfo(GlobalizationSection.Culture.Substring(0, 2));
                }
            }

            clientCulture = clientCulture != null
                ? Global.AllowCultures.FirstOrDefault(
                      item => item.TwoLetterISOLanguageName == clientCulture.TwoLetterISOLanguageName) ??
                  Global.AllowCultures.First()
                : Global.AllowCultures.First();

            if (clientCulture.Name != Thread.CurrentThread.CurrentUICulture.Name)
            {
                clientCulture = (CultureInfo)clientCulture.Clone();

                if (clientCulture.Name != Global.AllowCultures.First(
                                                    item => item.TwoLetterISOLanguageName.Equals(
                                                        "bg",
                                                        StringComparison.InvariantCultureIgnoreCase))
                                                .Name)
                {
                    clientCulture.NumberFormat.CurrencySymbol = "lev";
                    clientCulture.NumberFormat.CurrencyPositivePattern = 3;
                    clientCulture.NumberFormat.CurrencyNegativePattern = 8;
                    clientCulture.NumberFormat.NumberDecimalSeparator = ".";
                }
            }
            else
            {
                // Separator is always dot
                clientCulture.NumberFormat.NumberDecimalSeparator = ".";
            }

            Thread.CurrentThread.CurrentCulture = Thread.CurrentThread.CurrentUICulture = clientCulture;
        }
    }
}