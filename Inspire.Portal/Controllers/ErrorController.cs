namespace Inspire.Portal.Controllers
{
    using System.Web.Mvc;

    using AutoMapper;

    using Inspire.Common.Mvc.Infrastructure.BaseTypes;
    using Inspire.Core.Infrastructure.Logger;
    using Inspire.Portal.App_GlobalResources;
    using Inspire.Portal.Models.Error;

    [AllowAnonymous]
    public class ErrorController : BaseController
    {
        public ErrorController(ILogger logger, IMapper mapper)
            : base(logger, mapper)
        {
        }

        [OutputCache(CacheProfile = "Default")]
        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public ActionResult Index()
        {
            return View(
                "Index",
                new ErrorViewModel
                {
                    Title = Resource.InternalServerError,
                    Message = Resource.InternalServerErrorMessage,
                    Code = "500"
                });
        }

        [OutputCache(CacheProfile = "Default")]
        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public ActionResult Forbidden()
        {
            return View(
                "Index",
                new ErrorViewModel
                {
                    Title = Resource.ForbiddenError,
                    Message = Resource.ForbiddenErrorMessage,
                    Code = "403"
                });
        }

        [OutputCache(CacheProfile = "Default")]
        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public ActionResult NotFound()
        {
            return View(
                "Index",
                new ErrorViewModel
                {
                    Title = Resource.NotFoundError,
                    Message = Resource.NotFoundErrorMessage,
                    Code = "404"
                });
        }

        [HttpPost]
        public void LogClientErrors(string error, string url, string line)
        {
            Logger.Error($"Client error: {error} url: {url} line: {line}");
        }
    }
}