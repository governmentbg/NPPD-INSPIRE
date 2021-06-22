namespace Inspire.Common.Mvc.Extensions
{
    using System.Web.Mvc;

    public static class ModelStateDictionaryExt
    {
        public static void AddModelErrorSafety(this ModelStateDictionary errors, string key, string message)
        {
            if (errors.ContainsKey(key))
            {
                errors[key].Errors.Add(message);
            }
            else
            {
                errors.AddModelError(key, message);
            }
        }
    }
}