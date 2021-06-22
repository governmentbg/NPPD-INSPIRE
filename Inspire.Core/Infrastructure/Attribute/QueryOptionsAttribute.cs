namespace Inspire.Core.Infrastructure.Attribute
{
    using System;

    [AttributeUsage(AttributeTargets.Property)]
    public class QueryOptionsAttribute : Attribute
    {
        public enum VisualType
        {
            None,
            DropDownList,
            MultiSelect,
            Custom,
            Hidden,
            CascadeDropDownList
        }

        public VisualType Type { get; set; }

        public string EditorName { get; set; }

        public string CssClass { get; set; }

        public string CascadeFromField { get; set; }

        public string ActionName { get; set; }

        public string ControllerName { get; set; }

        public bool IsOnlyForSignedUsers { get; set; }
    }
}