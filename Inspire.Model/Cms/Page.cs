namespace Inspire.Model.Cms
{
    using System;
    using System.Collections.Generic;

    using Inspire.Core.Infrastructure.Attribute;
    using Inspire.Model.Base;
    using Inspire.Model.Helpers;
    using Inspire.Model.Nomenclature;

    public class Page : BaseDbModel
    {
        [Ignore]
        public long DbId { get; set; }

        public Nomenclature Type { get; set; }

        public Nomenclature VisibilityType { get; set; }

        public Nomenclature LocationType { get; set; }

        public string PermanentLink { get; set; }

        public Guid? ParentId { get; set; }

        [Ignore]
        public long? ParentDbId { get; set; }

        public DateTime? CreateDate { get; set; }

        public SortedDictionary<string, string> Titles { get; set; }

        public SortedDictionary<string, string> TitlesMenu { get; set; }

        public SortedDictionary<string, string> Contents { get; set; }

        public SortedDictionary<string, string> Keywords { get; set; }

        public bool IsInNewWindow { get; set; }

        public long Order { get; set; }

        public PageType PageType => Type?.Id.HasValue == true
            ? EnumHelper.GetPageTypeById(Type.Id.Value)
            : Cms.PageType.None;

        public VisibilityType Visibility => VisibilityType?.Id.HasValue == true
            ? EnumHelper.GetPageVisibilityTypeById(VisibilityType.Id.Value)
            : Cms.VisibilityType.Hide;

        public LocationType Location => LocationType?.Id.HasValue == true
            ? EnumHelper.GetPageLocationTypeById(LocationType.Id.Value)
            : Cms.LocationType.MainMenu;
    }
}