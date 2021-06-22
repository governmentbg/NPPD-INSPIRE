namespace Inspire.Utilities.Extensions
{
    using System;
    using System.Linq;

    public static class AttributeExtensions
    {
        public static TValue GetAttributeValue<TAttribute, TValue>(
            this Type type,
            Func<TAttribute, TValue> valueSelector)
            where TAttribute : Attribute
        {
            if (type.GetCustomAttributes(typeof(TAttribute), true).FirstOrDefault() is TAttribute att)
            {
                return valueSelector(att);
            }

            return default;
        }

        public static TExpected GetAttributeValue<T, TExpected>(this Enum enumeration, Func<T, TExpected> expression)
            where T : Attribute
        {
            var member = enumeration.GetType().GetMember(enumeration.ToString());
            var attribute = member.IsNotNullOrEmpty()
                ? member[0].GetCustomAttributes(typeof(T), false).Cast<T>().FirstOrDefault()
                : null;
            return attribute == null ? default : expression(attribute);
        }
    }
}