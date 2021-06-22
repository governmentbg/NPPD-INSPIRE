namespace Inspire.Portal.Utilities
{
    using System;
    using System.Collections.Generic;

    using Inspire.Common.Mvc.Extensions;
    using Inspire.Common.Mvc.Infrastructure.BaseTypes;
    using Inspire.Portal.App_GlobalResources;
    using Inspire.Portal.Controllers;
    using Inspire.Portal.Models;
    using Inspire.Table.Mvc.Controllers;
    using Inspire.Utilities.Extensions;

    public static class BaseControllerExtension
    {
        public static void AddToMenu(
            this BaseController controller,
            ICollection<MenuItem> menu,
            Type controllerType,
            string action,
            string title,
            object routeValues = null,
            string @class = null,
            bool isAjax = false,
            string httpMethod = "GET",
            IEnumerable<MenuItem> items = null)
        {
            if (controllerType.HasRightsToAction(action))
            {
                menu.Add(
                    new MenuItem
                    {
                        Title = title,
                        Url = controller.Url.DynamicAction(action, controllerType, routeValues),
                        Class = @class,
                        IsAjax = isAjax,
                        HttpMethod = httpMethod,
                        Items = items
                    });
            }
        }

        public static void InitViewTitleAndBreadcrumbs(
            this BaseController controller,
            string pageTitle,
            IEnumerable<Breadcrumb> breadcrumbs = null,
            bool showTitle = true)
        {
            controller.ViewBag.Title = pageTitle;
            controller.ViewBag.ShowTitle = showTitle;

            var items = new List<Breadcrumb>
                        {
                            new Breadcrumb
                            {
                                Title = Resource.Home,
                                Url = controller.GetDefaultUrl()
                            }
                        };

            if (breadcrumbs.IsNotNullOrEmpty())
            {
                items.AddRange(breadcrumbs);
            }

            items.Add(
                new Breadcrumb
                {
                    Title = pageTitle
                });

            controller.TempData["Breadcrumbs"] = items;
        }

        public static string GetUrl(
            this BaseController controller,
            Type controllerType,
            string action,
            object routeValues = null)
        {
            return controllerType.HasRightsToAction(action)
                ? controller.Url.DynamicAction(action, controllerType, routeValues)
                : null;
        }

        public static void InitAdminBreadcrumb(this BaseController controller, string controllerTitle, string title, bool isUpsert = false, List<Breadcrumb> additionalBreadcrumbs = null)
        {
            var breadcrumbs = new List<Breadcrumb>
                              {
                                  new Breadcrumb
                                  {
                                      Title = Resource.Admin
                                  }
                              };

            if (isUpsert)
            {
                breadcrumbs.Add(
                    new Breadcrumb
                    {
                        Url = controller.GetUrl(controller.GetType(), "Index"),
                        Title = controllerTitle
                    });
            }

            if (additionalBreadcrumbs.IsNotNullOrEmpty())
            {
                breadcrumbs.AddRange(additionalBreadcrumbs);
            }

            InitViewTitleAndBreadcrumbs(controller, title, breadcrumbs);
        }
    }
}