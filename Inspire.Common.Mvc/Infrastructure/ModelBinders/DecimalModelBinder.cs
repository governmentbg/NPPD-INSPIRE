namespace Inspire.Common.Mvc.Infrastructure.ModelBinders
{
    using System.Globalization;
    using System.Web.Mvc;

    public class DecimalModelBinder : DefaultModelBinder
    {
        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var modelName = bindingContext.ModelName;
            var value = bindingContext.ValueProvider.GetValue(modelName);
            if (value != null && value.RawValue is decimal)
            {
                return value.RawValue as decimal?;
            }

            var attemptedValue = value != null ? value.AttemptedValue : null;
            try
            {
                if (bindingContext.ModelMetadata.IsNullableValueType && string.IsNullOrWhiteSpace(attemptedValue))
                {
                    return null;
                }

                decimal result;
                if (decimal.TryParse(attemptedValue, NumberStyles.Any, CultureInfo.CurrentCulture, out result))
                {
                    return result;
                }

                if (decimal.TryParse(attemptedValue, NumberStyles.Any, CultureInfo.InvariantCulture, out result))
                {
                    return result;
                }

                return null;
            }
            catch
            {
                bindingContext.ModelState.AddModelError(modelName, "Invalid decimal format!");
            }

            return attemptedValue;
        }
    }
}