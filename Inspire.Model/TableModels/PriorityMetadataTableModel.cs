namespace Inspire.Model.TableModels
{
    using System;

    public class PriorityMetadataTableModel
    {
        public string DirectiveCode { get; set; }

        public string DirectiveTitle { get; set; }

        public string DirectiveOrganization { get; set; }

        public string Organization { get; set; }

        public string InspirePriorityDataSet { get; set; }

        public Guid? MetadataGuid { get; set; }

        public string MetadataTitle { get; set; }

        public bool? IsMetadataValid { get; set; }

        public bool? IsMetadataEUValid { get; set; }
    }
}