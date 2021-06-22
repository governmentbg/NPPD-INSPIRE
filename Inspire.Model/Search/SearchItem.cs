namespace Inspire.Model.Search
{
    using System;
    using System.Linq;

    using Inspire.Model.Helpers;
    using Inspire.Utilities.Enums;

    public class SearchItem
    {
        public Guid Id { get; set; }

        public Guid Type { get; set; }

        public ObjectType ObjectType => EnumHelper.ObjectTypes.First(item => item.Value == Type).Key;

        public string Name { get; set; }

        public string Description { get; set; }
    }
}