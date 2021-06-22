namespace Inspire.Portal.Infrastructure.Attribute
{
    using System.Threading;
    using System.Web.Mvc;

    public class LocalizationFilterAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            if (!filterContext.RouteData.Values.ContainsKey(RouteConfig.CultureKeyName))
            {
                filterContext.RouteData.Values[RouteConfig.CultureKeyName] =
                    Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName;
            }
        }
    }
}