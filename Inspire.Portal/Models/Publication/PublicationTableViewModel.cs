namespace Inspire.Portal.Models.Publication
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Runtime.Serialization;

    using Inspire.Core.Infrastructure.Attribute;
    using Inspire.Portal.App_GlobalResources;
    using Inspire.Portal.Utilities;

    using Newtonsoft.Json;

    [DataContract(Name = "Publication", Namespace = "")]
    public class PublicationTableViewModel
    {
        public Guid Id { get; set; }

        [DataMember(Order = 1)]
        [Display(ResourceType = typeof(Resource), Name = "Title")]
        [TableOptions(ClientHtmlTemplateId = "publicationInfoTemplate")]
        public string Title
        {
            get => Titles.GetValueForCurrentCulture();
            private set { }
        }

        [JsonIgnore]
        public SortedDictionary<string, string> Titles { get; set; }

        [DataMember(Order = 2)]
        [Display(ResourceType = typeof(Resource), Name = "RegisteredOn")]
        [DataType(DataType.DateTime)]
        [TableOptions(Format = "d")]
        public DateTime? EnterDate { get; set; }

        [DataMember(Order = 3)]
        [Display(ResourceType = typeof(Resource), Name = "StartDate")]
        [DataType(DataType.DateTime)]
        [TableOptions(Format = "d")]
        public DateTime? StartDate { get; set; }

        [DataMember(Order = 4)]
        [Display(ResourceType = typeof(Resource), Name = "EndDate")]
        [DataType(DataType.DateTime)]
        [TableOptions(Format = "d")]
        public DateTime? EndDate { get; set; }

        [DataMember(Order = 5)]
        [Display(ResourceType = typeof(Resource), Name = "Type")]
        public string Type { get; set; }

        [JsonIgnore]
        public bool IsVisibleInWeb { get; set; }

        [DataMember(Order = 6)]
        [Display(ResourceType = typeof(Resource), Name = "Visibility")]
        public string IsVisible
        {
            get => IsVisibleInWeb ? Resource.Yes : Resource.No;
            private set { }
        }

        [Display]
        [TableOptions(DisableFilterable = true, ClientHtmlTemplateId = "publicationActionTemplate", Width = 50)]
        public string Action => string.Empty;
    }
}