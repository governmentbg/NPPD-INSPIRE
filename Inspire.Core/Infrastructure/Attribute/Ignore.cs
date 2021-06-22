namespace Inspire.Core.Infrastructure.Attribute
{
    using System;

    [AttributeUsage(AttributeTargets.Property)]
    public class Ignore : Attribute
    {
    }
}