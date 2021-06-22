namespace Inspire.Model.TableModels
{
    using System;

    public class NonPriorityMetadataTableModel
    {
        public Guid? MetadataGuid { get; set; }

        public string MetadataTitle { get; set; }

        public string MetadataTheme { get; set; }

        public bool? IsMetadataValid { get; set; }

        public bool? IsMetadataEUValid { get; set; }
    }
}