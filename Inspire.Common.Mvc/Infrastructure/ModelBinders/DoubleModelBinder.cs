namespace Inspire.Common.Mvc.Infrastructure.ModelBinders
{
    using System.Globalization;
    using System.Web.Mvc;

    public class DoubleModelBinder : DefaultModelBinder
    {
        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var modelName = bindingContext.ModelName;
            var value = bindingContext.ValueProvider.GetValue(modelName);
            if (value != null && value.RawValue is double)
            {
                return value.RawValue as double?;
            }

            var attemptedValue = value != null ? value.AttemptedValue : null;
            try
            {
                if (bindingContext.ModelMetadata.IsNullableValueType && string.IsNullOrWhiteSpace(attemptedValue))
                {
                    return null;
                }

                double result;
                if (double.TryParse(attemptedValue, NumberStyles.Any, CultureInfo.CurrentCulture, out result))
                {
                    return result;
                }

                if (double.TryParse(attemptedValue, NumberStyles.Any, CultureInfo.InvariantCulture, out result))
                {
                    return result;
                }

                return null;
            }
            catch
            {
                bindingContext.ModelState.AddModelError(modelName, "Invalid double format!");
            }

            return attemptedValue;
        }
    }
}