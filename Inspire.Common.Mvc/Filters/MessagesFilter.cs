namespace Inspire.Common.Mvc.Filters
{
    using System.Web.Mvc;

    using Inspire.Common.Mvc.Extensions;

    public class MessagesFilter : ActionFilterAttribute
    {
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            if (filterContext.Exception != null)
            {
                return;
            }

            var isResultRedirect = filterContext.Result is RedirectToRouteResult ||
                                   filterContext.Result is RedirectResult;
            filterContext.Controller.SendMessageData(filterContext.HttpContext, isResultRedirect);
        }
    }
}