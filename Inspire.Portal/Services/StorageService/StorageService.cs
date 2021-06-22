namespace Inspire.Portal.Services.StorageService
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Configuration;
    using System.IO;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;

    using AutoMapper;

    using Inspire.Core.Infrastructure.RequestData;
    using Inspire.Domain.Repositories;
    using Inspire.Domain.Services;
    using Inspire.Model.Attachment;
    using Inspire.Portal.Utilities;
    using Inspire.Services;
    using Inspire.Utilities.Enums;
    using Inspire.Utilities.Extensions;

    using SimpleImpersonation;

    public class StorageService : BaseService, IStorageService
    {
        private readonly IAttachmentRepository attachmentRepository;

        private readonly string domain = ConfigurationManager.AppSettings["ImpersonationDomain"];
        private readonly string password = ConfigurationManager.AppSettings["ImpersonationPassword"];
        private readonly UrlHelper urlHelper = new UrlHelper(HttpContext.Current.Request.RequestContext);
        private readonly string user = ConfigurationManager.AppSettings["ImpersonationUser"];

        public StorageService(IMapper mapper, IRequestData requestData, IAttachmentRepository attachmentRepository)
            : base(mapper, requestData)
        {
            this.attachmentRepository = attachmentRepository;
        }

        public IEnumerable<Attachment> SaveToTemp(IEnumerable files, ChunkMetaData chunkMetaData = null)
        {
            if (!(files is HttpFileCollectionBase httpFiles))
            {
                return null;
            }

            var attachments = new List<Attachment>();
            StorageOperation(
                () =>
                {
                    var directoryPath = Path.Combine(
                        HttpContext.Current.Server.MapPath(ConfigurationReader.AttachmentsVirtualPath),
                        ConfigurationReader.AttachmentsTempDir);

                    for (var i = 0; i < httpFiles.Count; i++)
                    {
                        var file = httpFiles[i];
                        var fileName = Path.GetFileName(chunkMetaData?.FileName ?? file.FileName);
                        var filePath =
                            chunkMetaData?.UploadUid.IsNotNullOrEmpty() == true
                                ? HttpContext.Current.Session[chunkMetaData.UploadUid] as string
                                : string.Empty;

                        if (filePath.IsNullOrEmpty())
                        {
                            filePath = Path.Combine(directoryPath, fileName);
                            filePath = GetUniqueFullFilePath(filePath);

                            if (chunkMetaData?.UploadUid.IsNotNullOrEmpty() == true)
                            {
                                HttpContext.Current.Session[chunkMetaData.UploadUid] = filePath;
                            }
                        }

                        if (chunkMetaData == null)
                        {
                            file.SaveAs(filePath);
                        }
                        else
                        {
                            AppendToFile(filePath, file.InputStream);
                        }

                        var newFileName = Path.GetFileName(filePath);
                        var attachment = new Attachment
                                         {
                                             Url = urlHelper.Content(
                                                 $"{ConfigurationReader.AttachmentsVirtualPath}/{ConfigurationReader.AttachmentsTempDir}/{newFileName}"
                                                     .Trim()),
                                             Name = newFileName,
                                             Size = file.ContentLength
                                         };

                        attachments.Add(attachment);
                    }
                });

            return attachments;
        }

        public void Save(IEnumerable<Attachment> files, Guid id, ObjectType type, bool replaceExisting = true)
        {
            if (files == null || files.IsNullOrEmpty())
            {
                Remove(attachmentRepository.UpsertAttachments(files, id, type));
                return;
            }

            StorageOperation(
                () =>
                {
                    var virtualDirectoryPath = Path.Combine(
                        ConfigurationReader.AttachmentsVirtualPath,
                        Enum.GetName(typeof(ObjectType), type),
                        id.ToString());

                    var physicalDirectoryPath = HttpContext.Current.Server.MapPath(virtualDirectoryPath);
                    if (!Directory.Exists(physicalDirectoryPath))
                    {
                        Directory.CreateDirectory(physicalDirectoryPath);
                    }

                    foreach (var file in files)
                    {
                        if (!file.IsNewlyAdded)
                        {
                            continue;
                        }

                        // Existing files may be not moved
                        var filePath = HttpContext.Current.Server.MapPath(file.Url);
                        var oldDirectoryName = Path.GetDirectoryName(filePath);
                        if (physicalDirectoryPath.Equals(oldDirectoryName, StringComparison.InvariantCultureIgnoreCase))
                        {
                            continue;
                        }

                        var physicalFilePath = Path.Combine(physicalDirectoryPath, Path.GetFileName(filePath));
                        var fullFileName = GetUniqueFullFilePath(physicalFilePath);
                        File.Move(filePath, fullFileName);
                        file.Url = urlHelper.Content(
                            Path.Combine(virtualDirectoryPath, Path.GetFileName(fullFileName)));
                    }
                });

            var filesToDelete = attachmentRepository.UpsertAttachments(files, id, type, replaceExisting);
            Remove(filesToDelete);
        }

        public byte[] ReadAllBytes(Attachment file)
        {
            return StorageOperation(
                () =>
                {
                    var filePath = HttpContext.Current.Server.MapPath(file.Url);
                    return File.ReadAllBytes(filePath);
                });
        }

        public void ClearTempDirectory()
        {
            StorageOperation(
                () =>
                {
                    var directoryPath = Path.Combine(
                        HttpContext.Current.Server.MapPath(ConfigurationReader.AttachmentsVirtualPath),
                        ConfigurationReader.AttachmentsTempDir);
                    if (Directory.Exists(directoryPath))
                    {
                        Directory.Delete(directoryPath, true);
                    }

                    Directory.CreateDirectory(directoryPath);
                });
        }

        public void StorageOperation(Action action)
        {
            if (user.IsNotNullOrEmpty())
            {
                Impersonation.RunAsUser(
                    new UserCredentials(domain, user, password),
                    LogonType.NetworkCleartext,
                    action);
            }
            else
            {
                action.Invoke();
            }
        }

        public T StorageOperation<T>(Func<T> func)
        {
            if (user.IsNotNullOrEmpty())
            {
                return Impersonation.RunAsUser(
                    new UserCredentials(domain, user, password),
                    LogonType.NetworkCleartext,
                    func);
            }

            return func.Invoke();
        }

        public void RemoveTempFilesByUrls(IEnumerable<string> urls)
        {
            Remove(urls, true);
        }

        private void Remove(IEnumerable<string> urls, bool onlyTempDir = false)
        {
            if (urls.IsNullOrEmpty())
            {
                return;
            }

            StorageOperation(
                () =>
                {
                    var paths = urls.Select(item => HttpContext.Current.Server.MapPath(item));
                    if (onlyTempDir)
                    {
                        var tempDir = Path.Combine(
                            HttpContext.Current.Server.MapPath(ConfigurationReader.AttachmentsVirtualPath),
                            ConfigurationReader.AttachmentsTempDir);
                        paths = paths.Where(
                            path => Path.GetDirectoryName(path) == tempDir);
                    }

                    foreach (var physicalPath in paths)
                    {
                        if (File.Exists(physicalPath))
                        {
                            File.Delete(physicalPath);
                        }
                    }
                });
        }

        private string GetUniqueFullFilePath(string fullFilePath)
        {
            var directoryPath = Path.GetDirectoryName(fullFilePath);

            var fileName = Path.GetFileName(fullFilePath);
            var fileExtension = Path.GetExtension(fileName);

            fileName = $"{Guid.NewGuid().ToString().Substring(0, 8)}{fileExtension}";
            return Path.Combine(directoryPath, fileName);
        }

        private void AppendToFile(string fullPath, Stream content)
        {
            using (var stream = new FileStream(fullPath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite))
            {
                using (content)
                {
                    content.CopyTo(stream);
                }
            }
        }
    }
}