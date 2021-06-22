namespace Inspire.Core.Infrastructure.Attribute
{
    using System;

    /// <summary>
    ///     Attribute that defines which ClientDetailTemplateId should be used
    ///     a non-arbitary view is rendered in each controller for the specific
    ///     needs of that table model. Kendo only!
    ///     behavior.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class ClientTemplateAttribute : Attribute
    {
        public string Name { get; set; }
    }
}