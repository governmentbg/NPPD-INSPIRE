namespace Inspire.Common.Mvc.Infrastructure.ModelBinders
{
    using System;
    using System.ComponentModel;
    using System.Configuration;
    using System.Web.Mvc;

    using Inspire.Common.Mvc.Extensions;
    using Inspire.Utilities.Encryption;
    using Inspire.Utilities.Extensions;

    [AttributeUsage(
        AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Interface |
        AttributeTargets.Parameter,
        Inherited = false)]
    public class DecriptModelBinderAttribute : CustomModelBinderAttribute
    {
        private readonly IModelBinder binder = new DecriptBinder();

        public override IModelBinder GetBinder()
        {
            return binder;
        }

        private class DecriptBinder : DefaultModelBinder
        {
            public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
            {
                var modelName = bindingContext.ModelName;
                var value = bindingContext.ValueProvider.GetValue(modelName);
                var attemptedValue = value != null ? value.AttemptedValue : null;
                if (attemptedValue.IsNotNullOrEmpty())
                {
                    try
                    {
                        var decriptedData = EncryptionTools.Decrypt(
                            attemptedValue,
                            ConfigurationManager.AppSettings["EncryptKey"]);
                        var convertor = TypeDescriptor.GetConverter(bindingContext.ModelType);
                        return convertor.ConvertFrom(decriptedData);
                    }
                    catch
                    {
                        return bindingContext.ModelType.GetDefaultValue();
                    }
                }

                return ModelBinders.Binders.DefaultBinder.BindModel(controllerContext, bindingContext);
            }
        }
    }
}