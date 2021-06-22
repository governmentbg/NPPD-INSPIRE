namespace Inspire.Common.Mvc.Filters.CustomAuthorize
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Net;
    using System.Security.Principal;
    using System.Text;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.WebPages;

    using Inspire.Core.Infrastructure.Membership;

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class CustomAuthorizeAttribute : AuthorizeAttribute
    {
        /*
     *  Exp -> SubExp '&' SubExp // AND
     *  Exp -> SubExp '|' SubExp // OR
     *  Exp -> SubExp '^' SubExp // XOR
     *  SubExp -> '(' Exp ')'
     *  SubExp -> '!' Exp        // NOT
     *  SubExp -> RoleName
     *  RoleName -> [a-z]
     */

        private const char TokenSeparator = ' ';
        private const char TokenAnd = '&';
        private const char TokenOr = '|';
        private const char TokenXor = '^';
        private const char TokenNot = '!';
        private const char TokenParentheseOpen = '(';
        private const char TokenParentheseClose = ')';

        private ActionDescriptor actionDescriptor;

        public CustomAuthorizeAttribute(string rightsExpression = null)
        {
            Roles = rightsExpression;
        }

        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            if (filterContext == null)
            {
                throw new ArgumentNullException("filterContext");
            }

            if (OutputCacheAttribute.IsChildActionCacheActive(filterContext))
            {
                throw new InvalidOperationException("Invalid operation type!");
            }

            actionDescriptor = filterContext.ActionDescriptor;
            if (AuthorizeCore(filterContext.HttpContext))
            {
                // ** IMPORTANT **
                // Since we're performing authorization at the action level, the authorization code runs
                // after the output caching module. In the worst case this could allow an authorized user
                // to cause the page to be cached, then an unauthorized user would later be served the
                // cached page. We work around this by telling proxies not to cache the sensitive page,
                // then we hook our custom authorization code into the caching mechanism so that we have
                // the final say on whether a page should be served from the cache.
                var cachePolicy = filterContext.HttpContext.Response.Cache;
                cachePolicy.SetProxyMaxAge(new TimeSpan(0));
                cachePolicy.AddValidationCallback(CacheValidateHandler, null /* data */);
            }
            else
            {
                HandleUnauthorizedRequest(filterContext);
            }
        }

        public bool HasRightsToAction(ActionDescriptor actionDescriptor)
        {
            var skipAuthorization = true;
            if (actionDescriptor.IsDefined(typeof(CustomAuthorizeAttribute), true))
            {
                skipAuthorization = false;
            }
            else if (!actionDescriptor.IsDefined(typeof(AllowAnonymousAttribute), true)
                     && actionDescriptor.ControllerDescriptor.IsDefined(typeof(CustomAuthorizeAttribute), true))
            {
                skipAuthorization = false;
            }

            return skipAuthorization || HasRights();
        }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            return HasRightsToAction(actionDescriptor);
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            var httpContext = filterContext.HttpContext;

            var request = httpContext.Request;
            var user = httpContext.User;
            if (request.IsAjaxRequest())
            {
                var response = httpContext.Response;
                response.SuppressFormsAuthenticationRedirect = true;

                if (user == null || !user.Identity.IsAuthenticated)
                {
                    filterContext.Result = new HttpStatusCodeResult(HttpStatusCode.Unauthorized);
                }
                else
                {
                    filterContext.Result = new HttpStatusCodeResult(HttpStatusCode.Forbidden);
                }
            }
            else
            {
                if (user == null || !user.Identity.IsAuthenticated)
                {
                    filterContext.Result = new HttpStatusCodeResult(HttpStatusCode.Unauthorized);
                }
                else
                {
                    throw new HttpException(HttpStatusCode.Forbidden.GetHashCode(), "Forbidden");
                }
            }
        }

        protected override HttpValidationStatus OnCacheAuthorization(HttpContextBase httpContext)
        {
            if (httpContext == null)
            {
                throw new ArgumentNullException("httpContext");
            }

            return AuthorizeCore(httpContext)
                ? HttpValidationStatus.Valid
                : HttpValidationStatus.IgnoreThisRequest;
        }

        private bool HasRights()
        {
            if (!(HttpContext.Current.User is IUserPrincipal user) || !user.Identity.IsAuthenticated)
            {
                return false;
            }

            return Roles.IsEmpty() || IsUserInRole(user);
        }

        private bool IsUserInRole(IPrincipal user)
        {
            // Calculate expression value
            var expression = Parse(Roles);
            return expression == null || expression.Eval(user);
        }

        private Node Parse(string text)
        {
            if (text == null)
            {
                throw new ArgumentNullException("text");
            }

            var delimiters = new[]
                             {
                                 TokenSeparator, TokenAnd, TokenOr, TokenXor, TokenNot, TokenParentheseOpen,
                                 TokenParentheseClose
                             };

            var tokens = new List<string>();
            var sb = new StringBuilder();

            foreach (var c in text.Where(c => c != TokenSeparator))
            {
                if (delimiters.ToList().Contains(c))
                {
                    if (sb.Length > 0)
                    {
                        tokens.Add(sb.ToString());
                        sb.Clear();
                    }

                    tokens.Add(c.ToString(CultureInfo.InvariantCulture));
                }
                else
                {
                    sb.Append(c);
                }
            }

            if (sb.Length > 0)
            {
                tokens.Add(sb.ToString());
            }

            return Parse(tokens.ToArray());
        }

        private Node Parse(string[] tokens)
        {
            var index = 0;
            return ParseExp(tokens, ref index);
        }

        private Node ParseExp(string[] tokens, ref int index)
        {
            var leftExp = ParseSubExp(tokens, ref index);
            if (index >= tokens.Length)
            {
                return leftExp;
            }

            var token = tokens[index];

            if (token == "&")
            {
                index++;
                var rightExp = ParseSubExp(tokens, ref index);
                return new AndNode(leftExp, rightExp);
            }

            if (token == "|")
            {
                index++;
                var rightExp = ParseSubExp(tokens, ref index);
                return new OrNode(leftExp, rightExp);
            }

            if (token == "^")
            {
                index++;
                var rightExp = ParseSubExp(tokens, ref index);
                return new XorNode(leftExp, rightExp);
            }

            throw new Exception("Expected '&' or '|' or EOF");
        }

        private Node ParseSubExp(string[] tokens, ref int index)
        {
            var token = tokens[index];

            if (token == "(")
            {
                index++;
                var node = ParseExp(tokens, ref index);

                if (tokens[index] != ")")
                {
                    throw new Exception("Expected ')'");
                }

                index++; // Skip ')'

                return node;
            }

            if (token == "!")
            {
                index++;
                var node = ParseExp(tokens, ref index);
                return new NotNode(node);
            }

            index++;
            return new RoleNode(token);
        }

        private void CacheValidateHandler(HttpContext context, object data, ref HttpValidationStatus validationStatus)
        {
            validationStatus = OnCacheAuthorization(new HttpContextWrapper(context));
        }
    }
}