namespace Inspire.Portal.Infrastructure.Attribute
{
    using System.ComponentModel.DataAnnotations;
    using System.Web.Mvc;

    using Inspire.Portal.App_GlobalResources;
    using Inspire.Utilities.Extensions;

    public class LocalizationRequiredAttributeAdapter : RequiredAttributeAdapter
    {
        public LocalizationRequiredAttributeAdapter(
            ModelMetadata metadata,
            ControllerContext context,
            RequiredAttribute attribute)
            : base(metadata, context, attribute)
        {
            if (attribute.ErrorMessage.IsNullOrEmpty() && attribute.ErrorMessageResourceType == null &&
                attribute.ErrorMessageResourceName.IsNullOrEmpty())
            {
                attribute.ErrorMessageResourceType = typeof(Resource);
                attribute.ErrorMessageResourceName = "Required";
            }
        }
    }
}