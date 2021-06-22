namespace Inspire.Portal.Models.Search
{
    using System;

    using Inspire.Portal.App_GlobalResources;
    using Inspire.Utilities.Enums;

    public class SearchItemViewModel
    {
        public Guid Id { get; set; }

        public ObjectType ObjectType { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string Group => GetGroupNameByType();

        public string Url { get; set; }

        private string GetGroupNameByType()
        {
            switch (ObjectType)
            {
                case ObjectType.Publication:
                    return Resource.Publications;

                default:
                    return null;
            }
        }
    }
}