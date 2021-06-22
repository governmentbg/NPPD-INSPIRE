namespace Inspire.Domain.Repositories
{
    using System;
    using System.Collections.Generic;

    using Inspire.Core.Infrastructure.Repository;
    using Inspire.Model.Attachment;
    using Inspire.Utilities.Enums;

    public interface IAttachmentRepository : IRepository
    {
        IEnumerable<string> UpsertAttachments(
            IEnumerable<Attachment> attachments,
            Guid objectId,
            ObjectType objectTypeId,
            bool replaceExisting = true);

        List<Attachment> GetFiles(Guid id, ObjectType objectType);
    }
}