namespace Inspire.Common.Mvc.Infrastructure.ModelBinders
{
    using System;
    using System.ComponentModel;
    using System.Configuration;
    using System.Web.Mvc;

    using Inspire.Common.Mvc.Extensions;
    using Inspire.Utilities.Encryption;
    using Inspire.Utilities.Extensions;

    public class DecryptPropertyBindAttribute : PropertyBindAttribute
    {
        public override bool BindProperty(
            ControllerContext controllerContext,
            ModelBindingContext bindingContext,
            PropertyDescriptor propertyDescriptor)
        {
            var value =
                bindingContext.ValueProvider.GetValue($"{bindingContext.ModelName}.{propertyDescriptor.Name}") ??
                bindingContext.ValueProvider.GetValue(propertyDescriptor.Name);

            var attemptedValue = value?.AttemptedValue;
            if (attemptedValue.IsNotNullOrEmpty())
            {
                try
                {
                    var decryptedData = EncryptionTools.Decrypt(
                        attemptedValue,
                        ConfigurationManager.AppSettings["EncryptKey"]);
                    var convertor = TypeDescriptor.GetConverter(propertyDescriptor.PropertyType);
                    propertyDescriptor.SetValue(bindingContext.Model, convertor.ConvertFrom(decryptedData));
                    return true;
                }
                catch (Exception)
                {
                    bindingContext.ModelState.AddModelError(
                        propertyDescriptor.Name,
                        $"The field '{propertyDescriptor.Name}' has an invalid value in it!");
                    return false;
                }
            }

            propertyDescriptor.SetValue(bindingContext.Model, bindingContext.ModelType.GetDefaultValue());
            return true;
        }
    }
}