namespace Inspire.Core.Infrastructure.Attribute
{
    using System;

    /// <summary>
    ///     Attribute that defines what should happen if td of the web grid is
    ///     clicked, to set the name of the column, to show if the column should be visible or summed at the end of the table.
    ///     This will be used in the Table models to define different
    ///     behavior.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class TableOptionsAttribute : Attribute
    {
        /// <summary>
        ///     Gets or sets use for dictionary properties
        /// </summary>
        public string Key { get; set; }

        public string Name { get; set; }

        public string Title { get; set; }

        public bool Ignore { get; set; }

        public bool IsHidden { get; set; }

        public bool IsSummable { get; set; }

        public bool IsCheckbox { get; set; }

        public bool DisableFilterable { get; set; }

        public string CssClass { get; set; }

        public string ClientHtmlTemplate { get; set; }

        public string ClientHtmlTemplateId { get; set; }

        public string Format { get; set; }

        public string DataAttributes { get; set; }

        public int Order { get; set; }

        /// <summary>
        ///     Gets or sets column width in pixels
        /// </summary>
        public int Width { get; set; }

        public string HeaderClass { get; set; }

        public bool IsGroupable { get; set; }

        public bool ShouldExport { get; set; }

        public bool IsOnlyForSignedUsers { get; set; }
    }
}