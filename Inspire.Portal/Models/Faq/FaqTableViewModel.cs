namespace Inspire.Portal.Models.Faq
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Runtime.Serialization;

    using Inspire.Core.Infrastructure.Attribute;
    using Inspire.Model.Base;
    using Inspire.Portal.App_GlobalResources;

    [DataContract(Name = "Faq", Namespace = "")]
    public class FaqTableViewModel : BaseDbModel
    {
        [DataMember(Order = 1)]
        [Display(ResourceType = typeof(Resource), Name = "Question")]
        public string Question { get; set; }

        [DataMember(Order = 2)]
        [Display(ResourceType = typeof(Resource), Name = "RegisteredOn")]
        [TableOptions(Format = "g")]
        public DateTime? RegDate { get; set; }

        [DataMember(Order = 3)]
        [Display(ResourceType = typeof(Resource), Name = "LastChangeDate")]
        [TableOptions(Format = "g")]
        public DateTime? UpdateDate { get; set; }

        [DataMember(Order = 4)]
        [Display(ResourceType = typeof(Resource), Name = "Category")]
        public string Category { get; set; }

        [DataMember(Order = 5)]
        [Display(ResourceType = typeof(Resource), Name = "Status")]
        public string Status { get; set; }

        [Display]
        [TableOptions(DisableFilterable = true, ClientHtmlTemplateId = "faqActionTemplate", Width = 50)]
        public string Action => string.Empty;
    }
}