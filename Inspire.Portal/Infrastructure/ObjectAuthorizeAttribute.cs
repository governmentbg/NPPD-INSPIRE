namespace Inspire.Portal.Infrastructure
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Web;
    using System.Web.Mvc;

    using Inspire.Common.Mvc.Extensions;
    using Inspire.Common.Mvc.Filters.CustomAuthorize;
    using Inspire.Core.Infrastructure.Membership;
    using Inspire.Core.Infrastructure.TransactionManager;
    using Inspire.Domain.Services;
    using Inspire.Model.Helpers;
    using Inspire.Utilities.Enums;

    public class ObjectAuthorizeAttribute : ActionFilterAttribute
    {
        public string Parameter { get; set; }

        public ObjectType ObjectType { get; set; }

        public string Right { get; set; }

        public static bool HasUserRightsForObject(
            Type controllerType,
            string action,
            Guid? objectId,
            ObjectType objectType,
            string right)
        {
            if (!(HttpContext.Current.User is IUserPrincipal user))
            {
                return controllerType.HasRightsToAction(action);
            }

            var objectTypeId = EnumHelper.GetObjectIdByObjectTypeId(objectType);
            return CheckIfUserHasRightsForObject(user.Id, objectId, objectTypeId, Guid.Parse(right));
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            bool isAuthorized;
            if (!(filterContext.HttpContext.User is IUserPrincipal user))
            {
                isAuthorized = new CustomAuthorizeAttribute(Right).HasRightsToAction(filterContext.ActionDescriptor);
            }
            else
            {
                var objectId = GetObjectId(filterContext);
                var objectTypeId = EnumHelper.GetObjectIdByObjectTypeId(ObjectType);
                isAuthorized = CheckIfUserHasRightsForObject(user.Id, objectId, objectTypeId, Guid.Parse(Right));
            }

            if (!isAuthorized)
            {
                if (filterContext.HttpContext.Request.IsAjaxRequest())
                {
                    filterContext.Result = new HttpStatusCodeResult(HttpStatusCode.Forbidden);
                }
                else
                {
                    throw new HttpException(HttpStatusCode.Forbidden.GetHashCode(), "Forbidden");
                }
            }
        }

        private static bool CheckIfUserHasRightsForObject(Guid userId, Guid? objectId, Guid objectTypeId, Guid right)
        {
            var contextManager = DependencyResolver.Current.GetService<IDbContextManager>();
            var userService = DependencyResolver.Current.GetService<IUserService>();

            using (contextManager.NewConnection())
            {
                return userService.CheckIfUserHasRightsForObject(userId, objectId, objectTypeId, right);
            }
        }

        private Guid? GetObjectId(ActionExecutingContext filterContext)
        {
            var data = Parameter.Split('.');
            if (!filterContext.ActionParameters.ContainsKey(data[0]))
            {
                return null;
            }

            var model = filterContext.ActionParameters[data[0]];
            var val = data.Length < 2
                ? model
                : GetPropertyValue(model, data[1], data.Skip(2).ToList());

            return val is Guid guid
                ? guid
                : val != null && Guid.TryParse(val.ToString(), out var objectId)
                    ? objectId
                    : default(Guid?);
        }

        private object GetPropertyValue(object obj, string head, IEnumerable<string> tail)
        {
            var property = obj.GetType().GetProperties()
                              .FirstOrDefault(p => p.Name.Equals(head, StringComparison.OrdinalIgnoreCase));
            if (property == null)
            {
                return null;
            }

            var innerObj = property.GetValue(obj, null);
            if (innerObj == null || !tail.Any())
            {
                return innerObj;
            }

            return GetPropertyValue(innerObj, tail.First(), tail.Skip(1));
        }
    }
}