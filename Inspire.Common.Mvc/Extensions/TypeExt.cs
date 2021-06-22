namespace Inspire.Common.Mvc.Extensions
{
    using System;
    using System.Linq;
    using System.Web.Mvc;

    using Inspire.Common.Mvc.Filters.CustomAuthorize;

    public static class TypeExt
    {
        public static object GetDefaultValue(this Type type)
        {
            return type.IsValueType ? Activator.CreateInstance(type) : null;
        }

        public static bool HasRightsToAction(this Type controllerType, string actionName)
        {
            var controllerDescriptor = new ReflectedControllerDescriptor(controllerType);
            if (controllerDescriptor == null)
            {
                throw new ArgumentException("Invalid controller type!");
            }

            var actionDescriptor = controllerDescriptor.GetCanonicalActions()
                                                       .FirstOrDefault(item => item.ActionName == actionName);

            if (actionDescriptor?.GetCustomAttributes(typeof(CustomAuthorizeAttribute), false).FirstOrDefault() is
                CustomAuthorizeAttribute attribute && !attribute.HasRightsToAction(actionDescriptor))
            {
                return false;
            }

            attribute = controllerDescriptor.GetCustomAttributes(typeof(CustomAuthorizeAttribute), false)
                                            .FirstOrDefault() as CustomAuthorizeAttribute ??
                        new CustomAuthorizeAttribute();
            return attribute.HasRightsToAction(actionDescriptor);
        }
    }
}