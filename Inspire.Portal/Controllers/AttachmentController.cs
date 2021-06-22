namespace Inspire.Portal.Controllers
{
    using System.IO;
    using System.Runtime.Serialization.Json;
    using System.Text;
    using System.Web.Mvc;

    using AutoMapper;

    using Inspire.Common.Mvc.Infrastructure.BaseTypes;
    using Inspire.Core.Infrastructure.Logger;
    using Inspire.Core.Infrastructure.TransactionManager;
    using Inspire.Domain.Services;
    using Inspire.Model.Attachment;

    public class AttachmentController : BaseDbController
    {
        private readonly IStorageService storageService;

        public AttachmentController(
            ILogger logger,
            IMapper mapper,
            IDbContextManager contextManager,
            IStorageService storageService)
            : base(logger, mapper, contextManager)
        {
            this.storageService = storageService;
        }

        [AcceptVerbs(HttpVerbs.Post | HttpVerbs.Get)]
        [AllowAnonymous]
        public ActionResult Upload(string metaData)
        {
            if (Request.Files == null || Request.Files.Count < 1)
            {
                return new EmptyResult();
            }

            ChunkMetaData chunkData = null;
            if (metaData != null)
            {
                var serializer = new DataContractJsonSerializer(typeof(ChunkMetaData));
                var ms = new MemoryStream(Encoding.UTF8.GetBytes(metaData));
                chunkData = serializer.ReadObject(ms) as ChunkMetaData;
            }

            var uploadedFiles = storageService.SaveToTemp(Request.Files, chunkData);

            var uploaded = chunkData == null || chunkData.TotalChunks - 1 <= chunkData.ChunkIndex;
            return Json(
                new
                {
                    uploaded,
                    fileUid = chunkData?.UploadUid,
                    files = uploaded ? uploadedFiles : null
                },
                JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post | HttpVerbs.Get)]
        [AllowAnonymous]
        public ActionResult Remove(string[] urls)
        {
            storageService.RemoveTempFilesByUrls(urls);
            return Json(string.Empty);
        }
    }
}