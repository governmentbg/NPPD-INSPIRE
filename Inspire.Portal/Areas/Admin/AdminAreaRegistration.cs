namespace Inspire.Portal.Areas.Admin
{
    using System.Web.Mvc;

    public class AdminAreaRegistration : AreaRegistration
    {
        public const string Location = "Admin";

        public override string AreaName => "Admin";

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                $"{Location}_default",
                "{" + RouteConfig.CultureKeyName + $"}}/{Location}/{{controller}}/{{action}}",
                constraints: new { culture = @"(^$)|(^[a-zA-Z]{2}$)|(^[a-zA-Z]{2}-[a-zA-Z]{2}$)" },
                defaults: new { action = "Index" },
                namespaces: RouteConfig.GetExecutingAssemblyControllerNamespaces(
                    namespaceName => namespaceName.Contains($".Areas.{Location}.")));
        }
    }
}