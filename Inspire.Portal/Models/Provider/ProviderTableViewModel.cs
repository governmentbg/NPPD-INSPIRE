namespace Inspire.Portal.Models.Provider
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Runtime.Serialization;

    using Inspire.Core.Infrastructure.Attribute;
    using Inspire.Portal.App_GlobalResources;
    using Inspire.Portal.Utilities;

    using Newtonsoft.Json;

    [DataContract(Name = "Provider", Namespace = "")]
    public class ProviderTableViewModel
    {
        public Guid? Id { get; set; }

        [DataMember(Order = 1)]
        [Display(ResourceType = typeof(Resource), Name = "Name")]
        public string Name
        {
            get => Names.GetValueForCurrentCulture();
            private set { }
        }

        [JsonIgnore]
        public SortedDictionary<string, string> Names { get; set; }

        [DataMember(Order = 2)]
        [Display(ResourceType = typeof(Resource), Name = "Description")]
        public string Description
        {
            get => Descriptions.GetValueForCurrentCulture();
            private set { }
        }

        [JsonIgnore]
        public SortedDictionary<string, string> Descriptions { get; set; }

        [DataMember(Order = 3)]
        [Display(ResourceType = typeof(Resource), Name = "Link")]
        [TableOptions(ClientHtmlTemplate = "<a href='#: Link #' target='_blank'> #: Link # </a>")]
        public string Link
        {
            get => Links.GetValueForCurrentCulture();
            private set { }
        }

        [JsonIgnore]
        public SortedDictionary<string, string> Links { get; set; }

        [Display]
        [TableOptions(DisableFilterable = true, ClientHtmlTemplateId = "providerActionButtonClientTemplate", Width = 50)]
        public string Action => string.Empty;
    }
}