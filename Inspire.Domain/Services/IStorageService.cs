namespace Inspire.Domain.Services
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    using Inspire.Model.Attachment;
    using Inspire.Utilities.Enums;

    public interface IStorageService
    {
        IEnumerable<Attachment> SaveToTemp(IEnumerable files, ChunkMetaData chunkMetaData = null);

        void Save(IEnumerable<Attachment> files, Guid id, ObjectType type, bool replaceExisting = true);

        void ClearTempDirectory();

        void StorageOperation(Action action);

        byte[] ReadAllBytes(Attachment file);

        T StorageOperation<T>(Func<T> func);

        void RemoveTempFilesByUrls(IEnumerable<string> urls);
    }
}