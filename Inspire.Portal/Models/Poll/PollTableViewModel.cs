namespace Inspire.Portal.Models.Poll
{
    using System.ComponentModel.DataAnnotations;
    using System.Runtime.Serialization;

    using Inspire.Core.Infrastructure.Attribute;
    using Inspire.Model.Base;
    using Inspire.Model.Helpers;
    using Inspire.Model.Nomenclature;
    using Inspire.Model.Poll;
    using Inspire.Portal.App_GlobalResources;

    using Newtonsoft.Json;

    [DataContract(Name = "Poll", Namespace = "")]
    public class PollTableViewModel : BaseDbModel
    {
        [DataMember(Order = 1)]
        [Display(Name = "Title", ResourceType = typeof(Resource))]
        public string Title { get; set; }

        [DataMember(Order = 2)]
        [Display(Name = "Status", ResourceType = typeof(Resource))]
        public string StatusName { get; set; }

        [DataMember(Order = 3)]
        [Display(Name = "From", ResourceType = typeof(Resource))]
        public string ValidFrom { get; set; }

        [DataMember(Order = 4)]
        [Display(Name = "To", ResourceType = typeof(Resource))]
        public string ValidTo { get; set; }

        [DataMember(Order = 5)]
        [Display(Name = "Registered", ResourceType = typeof(Resource))]
        public string RegDate { get; set; }

        [DataMember(Order = 6)]
        [Display(Name = "Автор")]
        public string Author { get; set; }

        [JsonIgnore]
        public Nomenclature Status { get; set; }

        public string Description { get; set; }

        public bool EnableEdit => Status.Id == EnumHelper.PollStatuses[PollStatus.Valid] ||
                                  Status.Id == EnumHelper.PollStatuses[PollStatus.Finished];

        [Display]
        [TableOptions(DisableFilterable = true, ClientHtmlTemplateId = "pollActionTemplate", Width = 50)]
        public string Action => string.Empty;
    }
}