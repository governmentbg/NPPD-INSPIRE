namespace Inspire.Common.Mvc.Infrastructure.ModelBinders
{
    using System;
    using System.Globalization;
    using System.Web.Mvc;

    public class DateTimeBinder : IModelBinder
    {
        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var modelName = bindingContext.ModelName;
            var value = bindingContext.ValueProvider.GetValue(modelName);
            if (value != null && value.RawValue is DateTime)
            {
                return value.RawValue as DateTime?;
            }

            var attemptedValue = value != null ? value.AttemptedValue : null;
            try
            {
                if (bindingContext.ModelMetadata.IsNullableValueType && string.IsNullOrWhiteSpace(attemptedValue))
                {
                    return null;
                }

                DateTime result;
                if (DateTime.TryParse(attemptedValue, CultureInfo.CurrentCulture, DateTimeStyles.None, out result))
                {
                    return result;
                }

                if (DateTime.TryParse(attemptedValue, CultureInfo.InvariantCulture, DateTimeStyles.None, out result))
                {
                    return result;
                }

                result = new DateTime(Convert.ToInt32(attemptedValue), 1, 1);
                if (result != DateTime.MinValue)
                {
                    return result;
                }

                return null;
            }
            catch
            {
                bindingContext.ModelState.AddModelError(modelName, "Invalid date format!");
            }

            return attemptedValue;
        }
    }
}