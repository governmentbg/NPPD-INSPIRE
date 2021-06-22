namespace Inspire.Model.Attachment
{
    using System.IO;

    using Inspire.Model.Base;
    using Inspire.Model.Helpers;
    using Inspire.Model.Nomenclature;
    using Inspire.Utilities.Extensions;

    public class Attachment : BaseDbModel
    {
        public Nomenclature Type { get; set; }

        public AttachmentType AttachmentType =>
            Type?.Id != null ? EnumHelper.GetAttachmentTypeByTypeId(Type.Id.Value) : AttachmentType.Image;

        public string Url { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public int Size { get; set; }

        public string MimeType => Name.IsNotNullOrEmpty() ? MimeTypes.GetMimeType(Name) : string.Empty;

        public bool IsNewlyAdded => Id == null;

        public bool IsMain { get; set; }

        public string Extension => Name.IsNotNullOrEmpty() ? Path.GetExtension(Name) : string.Empty;
    }
}