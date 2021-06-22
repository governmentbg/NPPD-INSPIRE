namespace Inspire.Portal
{
    using System;
    using System.Linq;
    using System.Reflection;
    using System.Web.Mvc;
    using System.Web.Routing;
    using Inspire.Portal.Infrastructure.RouteConstraint;

    public class RouteConfig
    {
        internal const string CultureKeyName = "culture";
        private const string DefaultCulture = "bg";

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Sitemap",
                url: "{" + CultureKeyName + "}/sitemap.xml",
                constraints: new { culture = @"(^$)|(^[a-zA-Z]{2}$)|(^[a-zA-Z]{2}-[a-zA-Z]{2}$)" },
                defaults: new { controller = "Home", action = "SitemapXml", culture = DefaultCulture });

            routes.MapRoute(
                name: "CmsRoute",
                url: "{" + CultureKeyName + "}/{*permalink}",
                constraints: new { culture = @"(^$)|(^[a-zA-Z]{2}$)|(^[a-zA-Z]{2}-[a-zA-Z]{2}$)", permalink = new CmsUrlConstraint() },
                defaults: new { controller = "Cms", action = "Render", culture = DefaultCulture });

            routes.MapRoute(
                "Login",
                "{" + CultureKeyName + "}/Login",
                constraints: new { culture = @"(^$)|(^[a-zA-Z]{2}$)|(^[a-zA-Z]{2}-[a-zA-Z]{2}$)" },
                defaults: new { controller = "Account", action = "LogIn", culture = DefaultCulture });

            routes.MapRoute(
                "LoginWithoutCulture",
                "Login",
                defaults: new { controller = "Account", action = "LogIn" });

            routes.MapRoute(
                "Error",
                "Error/{action}",
                new { controller = "Error", action = "Index" });

            routes.MapRoute(
                "Default",
                "{" + CultureKeyName + "}/{controller}/{action}",
                constraints: new { culture = @"(^$)|(^[a-zA-Z]{2}$)|(^[a-zA-Z]{2}-[a-zA-Z]{2}$)" },
                defaults: new { controller = "Home", action = "Index", culture = DefaultCulture },
                namespaces: GetExecutingAssemblyControllerNamespaces(namespaceName => !namespaceName.Contains(".Areas.")));
        }

        internal static string[] GetExecutingAssemblyControllerNamespaces(Func<string, bool> namespaceFunc)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var controlType = typeof(Controller);
            var namespaces =
                assembly.GetTypes()
                        .Where(t => controlType.IsAssignableFrom(t) && namespaceFunc(t.Namespace))
                        .Select(item => item.Namespace);
            return namespaces.ToArray();
        }
    }
}