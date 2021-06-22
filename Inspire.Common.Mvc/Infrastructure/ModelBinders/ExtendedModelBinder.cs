namespace Inspire.Common.Mvc.Infrastructure.ModelBinders
{
    using System.ComponentModel;
    using System.Linq;
    using System.Web.Mvc;

    public class ExtendedModelBinder : DefaultModelBinder
    {
        protected override void BindProperty(
            ControllerContext controllerContext,
            ModelBindingContext bindingContext,
            PropertyDescriptor propertyDescriptor)
        {
            var propBindAttr = propertyDescriptor.Attributes.OfType<PropertyBindAttribute>().FirstOrDefault();
            if (propBindAttr != null)
            {
                propBindAttr.BindProperty(controllerContext, bindingContext, propertyDescriptor);
                return;
            }

            base.BindProperty(controllerContext, bindingContext, propertyDescriptor);
        }
    }
}