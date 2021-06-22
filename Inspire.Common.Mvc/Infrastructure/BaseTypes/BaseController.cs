namespace Inspire.Common.Mvc.Infrastructure.BaseTypes
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Text.RegularExpressions;
    using System.Threading;
    using System.Web.Mvc;

    using AutoMapper;

    using Inspire.Core.Infrastructure.Logger;
    using Inspire.Core.Infrastructure.Membership;
    using Inspire.Utilities.Extensions;

    public abstract class BaseController : Controller
    {
        public const string SessionUser = "USER";
        protected readonly ILogger Logger;
        protected readonly IMapper Mapper;

        protected BaseController(ILogger logger, IMapper mapper)
        {
            Logger = logger;
            Mapper = mapper;
        }

        protected new virtual IUserPrincipal User => HttpContext?.User as IUserPrincipal;

        protected CultureInfo CurrentCulture => Thread.CurrentThread.CurrentCulture;

        public string RenderRazorViewToString(string viewName, object model)
        {
            ViewData.Model = model;
            using (var sw = new StringWriter())
            {
                var viewResult = ViewEngines.Engines.FindPartialView(ControllerContext, viewName);
                var viewContext = new ViewContext(ControllerContext, viewResult.View, ViewData, TempData, sw);
                viewResult.View.Render(viewContext, sw);
                viewResult.ViewEngine.ReleaseView(ControllerContext, viewResult.View);
                return sw.GetStringBuilder().ToString();
            }
        }

        public string GetDefaultUrl()
        {
            return Url.Content($"~/{CurrentCulture.TwoLetterISOLanguageName}");
        }

        public string GetUrl(string permanentLink, bool toAbsolute = false)
        {
            if (permanentLink.IsNullOrEmpty())
            {
                return null;
            }

            permanentLink = Regex.Replace(permanentLink, "{{lngISO2}}", CurrentCulture.TwoLetterISOLanguageName, RegexOptions.IgnoreCase);
            permanentLink = Regex.Replace(permanentLink, "{{lngISO3}}", CurrentCulture.ThreeLetterISOLanguageName, RegexOptions.IgnoreCase);

            string url = null;
            if (Uri.IsWellFormedUriString(permanentLink, UriKind.Absolute))
            {
                url = permanentLink;
            }
            else if (Uri.IsWellFormedUriString(permanentLink, UriKind.Relative))
            {
                url = Extensions.UrlHelperExt.Content(Url, $"~/{CurrentCulture.TwoLetterISOLanguageName}/{permanentLink}", toAbsolute);
            }

            return url;
        }

        protected RedirectResult RedirectToDefault()
        {
            return Redirect(GetDefaultUrl());
        }

        protected string GetControllerName()
        {
            var routeValues = RouteData.Values;
            return routeValues.ContainsKey("controller")
                ? (string)routeValues["controller"]
                : null;
        }
    }
}