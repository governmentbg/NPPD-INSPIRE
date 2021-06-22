namespace Inspire.Common.Mvc.Filters
{
    using System.Web.Mvc;

    using Inspire.Common.Mvc.Helpers;

    public class ControllerAreaAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var area = CustomHelpers.GetControllerArea(filterContext.Controller);
            if (!string.IsNullOrEmpty(area))
            {
                filterContext.Controller.ViewBag.Area = area;
            }

            base.OnActionExecuting(filterContext);
        }
    }
}