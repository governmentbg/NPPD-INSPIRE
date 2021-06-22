namespace Inspire.Portal.Areas.Admin.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;

    using AutoMapper;

    using Inspire.Common.Mvc.Filters.CustomAuthorize;
    using Inspire.Common.Mvc.Infrastructure.BaseTypes;
    using Inspire.Core.Infrastructure.Logger;
    using Inspire.Core.Infrastructure.TransactionManager;
    using Inspire.Domain.Services;
    using Inspire.Model.Attachment;
    using Inspire.Model.Helpers;
    using Inspire.Model.Nomenclature;
    using Inspire.Portal.App_GlobalResources;
    using Inspire.Portal.Models;
    using Inspire.Portal.Utilities;
    using Inspire.Utilities.Enums;
    using Inspire.Utilities.Exception;
    using Inspire.Utilities.Extensions;

    using Kendo.Mvc.Extensions;

    [CustomAuthorize(ApplicationRights.EditHomePagePicture)]
    public class BackgroundImagesController : BaseDbController
    {
        private readonly IAdminService adminService;
        private readonly IStorageService storageService;

        public BackgroundImagesController(
            ILogger logger,
            IMapper mapper,
            IDbContextManager contextManager,
            IAdminService adminService,
            IStorageService storageService)
            : base(logger, mapper, contextManager)
        {
            this.adminService = adminService;
            this.storageService = storageService;
        }

        [HttpGet]
        [CustomAuthorize(ApplicationRights.EditHomePagePicture)]
        public ActionResult Index()
        {
            List<Attachment> result;
            using (ContextManager.NewConnection())
            {
                result = adminService.GetHomeImages();
            }

            this.InitViewTitleAndBreadcrumbs(
                Resource.EditHomeImages,
                new[] { new Breadcrumb { Title = Resource.Admin } });

            return View(result);
        }

        [HttpGet]
        [CustomAuthorize(ApplicationRights.EditHomePagePicture)]
        public ActionResult Upsert(string path)
        {
            if (path.HasValue())
            {
                using (ContextManager.NewConnection())
                {
                    var images = adminService.GetHomeImages();
                    var model = images.FirstOrDefault(x => x.Url == path);
                    return PartialView("_Upsert", model);
                }
            }

            return PartialView("_Upsert");
        }

        [HttpPost]
        [CustomAuthorize(ApplicationRights.EditHomePagePicture)]
        public ActionResult Upsert(Attachment backgroundImage, string editedImgePath)
        {
            List<Attachment> images = null;
            if (backgroundImage.Url.HasValue())
            {
                using (var transaction = ContextManager.NewTransaction())
                {
                    backgroundImage.Type = new Nomenclature
                    { Id = EnumHelper.GetAttachmentTypeGuidByType(AttachmentType.Image) };
                    images = adminService.GetHomeImages();
                    if (editedImgePath.IsNotNullOrEmpty())
                    {
                        images.Remove(images.FirstOrDefault(x => x.Url == editedImgePath));
                    }

                    images.Add(backgroundImage);
                    storageService.Save(
                        images,
                        EnumHelper.GetObjectIdByObjectTypeId(ObjectType.HomeBackgroundImage),
                        ObjectType.HomeBackgroundImage);
                    transaction.Commit();
                }
            }

            return Json(new { success = true, images }, JsonRequestBehavior.AllowGet);
        }

        [HttpDelete]
        [CustomAuthorize(ApplicationRights.EditHomePagePicture)]
        public void Delete(string url)
        {
            using (var transaction = ContextManager.NewTransaction())
            {
                var images = adminService.GetHomeImages();
                var image = images.FirstOrDefault(x => x.Url.Equals(url));
                if (image == null)
                {
                    throw new UserException(Resource.NoDataFound);
                }

                images.Remove(image);
                storageService.Save(
                    images,
                    EnumHelper.GetObjectIdByObjectTypeId(ObjectType.HomeBackgroundImage),
                    ObjectType.HomeBackgroundImage);
                transaction.Commit();
            }
        }

        [HttpGet]
        public ActionResult Breadcrumbs()
        {
            return PartialView("_Breadcrums");
        }
    }
}