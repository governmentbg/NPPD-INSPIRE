namespace Inspire.Portal.Filters
{
    using Inspire.Portal.App_GlobalResources;

    public class CustomErrorHandler : Common.Mvc.Filters.CustomErrorHandler
    {
        protected override string DbErrorMessage => Resource.DbErrorMessage;
    }
}