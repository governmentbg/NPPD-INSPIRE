namespace Inspire.Common.Mvc.Infrastructure.ModelBinders
{
    using System;
    using System.Globalization;
    using System.Web.Mvc;

    public class TimeSpanBinder : IModelBinder
    {
        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var modelName = bindingContext.ModelName;
            var value = bindingContext.ValueProvider.GetValue(modelName);
            if (value != null && value.RawValue is TimeSpan)
            {
                return value.RawValue as TimeSpan?;
            }

            var attemptedValue = value != null ? value.AttemptedValue.Replace("ч.", string.Empty) : null;
            try
            {
                if (bindingContext.ModelMetadata.IsNullableValueType && string.IsNullOrWhiteSpace(attemptedValue))
                {
                    return null;
                }

                TimeSpan result;
                if (TimeSpan.TryParse(attemptedValue, value.Culture, out result))
                {
                    return result;
                }

                if (TimeSpan.TryParse(attemptedValue, CultureInfo.CurrentCulture, out result))
                {
                    return result;
                }

                if (TimeSpan.TryParse(attemptedValue, CultureInfo.CurrentUICulture, out result))
                {
                    return result;
                }

                if (TimeSpan.TryParse(attemptedValue, CultureInfo.InvariantCulture, out result))
                {
                    return result;
                }

                return null;
            }
            catch
            {
                bindingContext.ModelState.AddModelError(modelName, "Invalid time span format!");
            }

            return attemptedValue;
        }
    }
}