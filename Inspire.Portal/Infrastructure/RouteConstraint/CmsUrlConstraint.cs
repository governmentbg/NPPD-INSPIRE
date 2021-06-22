namespace Inspire.Portal.Infrastructure.RouteConstraint
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Routing;

    using Inspire.Core.Infrastructure.TransactionManager;
    using Inspire.Domain.Services;
    using Inspire.Model.Cms;
    using Inspire.Model.QueryModels;
    using Inspire.Utilities.Extensions;

    public class CmsUrlConstraint : IRouteConstraint
    {
        public bool Match(
            HttpContextBase httpContext,
            Route route,
            string parameterName,
            RouteValueDictionary values,
            RouteDirection routeDirection)
        {
            if (values[parameterName] != null)
            {
                var permalink = values[parameterName].ToString();
                if (permalink.IsNotNullOrEmpty())
                {
                    var contextManager = (IDbContextManager)DependencyResolver.Current.GetService(typeof(IDbContextManager));
                    var cmsService = (ICmsService)DependencyResolver.Current.GetService(typeof(ICmsService));

                    List<Page> pages;
                    using (contextManager.NewConnection())
                    {
                        pages = cmsService.SearchPages(new PageQueryModel { PermanentLink = permalink });
                    }

                    // Validata page visibility
                    var page = pages?.FirstOrDefault(item => item.PermanentLink?.Equals(permalink, StringComparison.InvariantCultureIgnoreCase) == true);
                    if (page != null)
                    {
                        var isVisible = page?.Visibility == VisibilityType.Public
                            || (httpContext.User?.Identity?.IsAuthenticated == true && page?.Visibility == VisibilityType.AuthenticatedUsed);
                        values.Add("id", page.Id);
                        return !isVisible || page.PageType == PageType.Content;
                    }
                }

                return false;
            }

            return false;
        }
    }
}