namespace Inspire.Portal
{
    using System;
    using System.Web.Mvc;

    using Inspire.Common.Mvc.Infrastructure.ModelBinders;

    public static class BindersConfig
    {
        public static void RegisterModelBinders(ModelBinderDictionary binders)
        {
            binders.Add(typeof(DateTime), new DateTimeBinder());
            binders.Add(typeof(DateTime?), new DateTimeBinder());
            binders.Add(typeof(TimeSpan), new TimeSpanBinder());
            binders.Add(typeof(TimeSpan?), new TimeSpanBinder());
            binders.Add(typeof(double), new DoubleModelBinder());
            binders.Add(typeof(double?), new DoubleModelBinder());
            binders.Add(typeof(decimal), new DecimalModelBinder());
            binders.Add(typeof(decimal?), new DecimalModelBinder());
        }
    }
}