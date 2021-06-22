namespace Inspire.Portal.Controllers
{
    using System.IO;
    using System.Web;
    using System.Web.Mvc;

    using Inspire.Domain.Services;
    using Inspire.Portal.Utilities;

    using Kendo.Mvc.UI;

    public class ImageBrowserController : EditorImageBrowserController
    {
        public static string VirtualPath = Path.Combine(ConfigurationReader.AttachmentsVirtualPath, PrettyName);
        private const string PrettyName = "Images/";

        private readonly IStorageService storageService;

        public ImageBrowserController(IStorageService storageService)
        {
            this.storageService = storageService;
        }

        public override string ContentPath => CreateUserFolder();

        public override ActionResult Create(string path, FileBrowserEntry entry)
        {
            return storageService.StorageOperation(() => base.Create(path, entry));
        }

        public override ActionResult Upload(string path, HttpPostedFileBase file)
        {
            return storageService.StorageOperation(() => base.Upload(path, file));
        }

        public override ActionResult Destroy(string path, FileBrowserEntry entry)
        {
            return storageService.StorageOperation(() => base.Destroy(path, entry));
        }

        protected override void DeleteDirectory(string path)
        {
            storageService.StorageOperation(
                () => { base.DeleteDirectory(path); });
        }

        protected override void DeleteFile(string path)
        {
            storageService.StorageOperation(
                () => { base.DeleteFile(path); });
        }

        private string CreateUserFolder()
        {
            storageService.StorageOperation(
                () =>
                {
                    var path = Server.MapPath(VirtualPath);
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }
                });

            return VirtualPath;
        }
    }
}