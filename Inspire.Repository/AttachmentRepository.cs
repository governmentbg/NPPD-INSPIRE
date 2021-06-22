namespace Inspire.Repository
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Inspire.Core.Infrastructure.Context;
    using Inspire.Data.Repositories;
    using Inspire.Data.Utilities;
    using Inspire.Domain.Repositories;
    using Inspire.Model.Attachment;
    using Inspire.Model.Helpers;
    using Inspire.Model.Nomenclature;
    using Inspire.Repository.Utilities;
    using Inspire.Utilities.Enums;
    using Inspire.Utilities.Extensions;

    public class AttachmentRepository : BaseRepository, IAttachmentRepository
    {
        public AttachmentRepository(IAisContext context)
            : base(context)
        {
        }

        public IEnumerable<string> UpsertAttachments(
            IEnumerable<Attachment> attachments,
            Guid objectId,
            ObjectType objectTypeId,
            bool replaceExisting = true)
        {
            var types = attachments?.Select(item => item.Type?.Id).ToArray();
            var urls = attachments?.Select(item => item.Url).ToArray();
            var names = attachments?.Select(item => item.Name).ToArray();
            var descriptions = attachments?.Select(item => item.Description).ToArray();
            var mimeTypes = attachments?.Select(item => item.MimeType).ToArray();
            var sizes = attachments?.Select(item => item.Size).ToArray();
            var isMain = attachments?.Select(item => item.IsMain).ToArray();

            using (var command = Context.Connection.GenerateCommand(
                "ais.upsert_attachmentbyobject",
                attachments,
                new Dictionary<string, object>
                {
                    {
                        "pobjectid",
                        objectId
                    },
                    {
                        "pobjecttypeid",
                        EnumHelper.GetObjectIdByObjectTypeId(objectTypeId)
                    },
                    {
                        "ptype",
                        types
                    },
                    {
                        "purl",
                        urls
                    },
                    {
                        "pname",
                        names
                    },
                    {
                        "pdescription",
                        descriptions
                    },
                    {
                        "pmimetype",
                        mimeTypes
                    },
                    {
                        "psize",
                        sizes
                    },
                    {
                        "pismain",
                        isMain
                    },
                    {
                        "pflagreplace",
                        replaceExisting
                    }
                }))
            {
                command.ExecuteNonQuerySafety();

                var urlsToDelete = Convert.ToString(command.Parameters["delurls"].Value);
                if (!urlsToDelete.IsNullOrEmptyOrWhiteSpace())
                {
                    var returnUrs = urlsToDelete.Split(';');
                    return returnUrs;
                }

                return null;
            }
        }

        public List<Attachment> GetFiles(Guid id, ObjectType objectType)
        {
            var result = new List<Attachment>();
            using (var command = Context.Connection.GenerateCommand(
                "ais.get_attachmentbyobject",
                new
                {
                    objectid = id,
                    objecttypeid = EnumHelper.GetObjectIdByObjectTypeId(objectType)
                }))
            {
                using (var reader = command.ExecuteReaderSafety())
                {
                    while (reader.Read())
                    {
                        if (reader["id"] != DBNull.Value)
                        {
                            result.Add(
                                new Attachment
                                {
                                    Id = reader.GetFieldValue<Guid>("id"),
                                    Type = new Nomenclature
                                           {
                                               Id = reader.GetFieldValue<Guid>("type_id"),
                                               Name = reader.GetFieldValue<string>("type_name")
                                           },
                                    Size = reader.GetFieldValue<int>("size"),
                                    Url = reader.GetFieldValue<string>("url"),
                                    Name = reader.GetFieldValue<string>("name"),
                                    Description = reader.GetFieldValue<string>("description"),
                                    IsMain = reader.GetFieldValue<bool>("ismain")
                                });
                        }
                    }
                }
            }

            return result;
        }
    }
}