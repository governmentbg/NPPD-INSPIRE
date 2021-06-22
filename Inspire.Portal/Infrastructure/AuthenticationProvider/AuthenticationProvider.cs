namespace Inspire.Portal.Infrastructure.AuthenticationProvider
{
    using System;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Security;

    using AutoMapper;

    using Inspire.Common.Mvc.Extensions;
    using Inspire.Common.Mvc.Infrastructure.BaseTypes;
    using Inspire.Core.Infrastructure.Logger;
    using Inspire.Core.Infrastructure.TransactionManager;
    using Inspire.Domain.Services;
    using Inspire.Infrastructure.Membership;
    using Inspire.Model.Helpers;
    using Inspire.Model.User;
    using Inspire.Portal.App_GlobalResources;
    using Inspire.Portal.Services.RestApiService;
    using Inspire.Portal.Utilities;
    using Inspire.Utilities.Encryption;
    using Inspire.Utilities.Exception;
    using Inspire.Utilities.Extensions;

    public class AuthenticationProvider : IAuthenticationProvider
    {
        private readonly IAccountService accountService;
        private readonly IDbContextManager contextManager;
        private readonly IMapper mapper;
        private readonly IUserService userService;
        private readonly IRestApiService restApiService;
        private readonly ILogger logger;

        public AuthenticationProvider(
            IMapper mapper,
            IDbContextManager contextManager,
            IAccountService accountService,
            IUserService userService,
            IRestApiService restApiService,
            ILogger logger)
        {
            this.mapper = mapper;
            this.contextManager = contextManager;
            this.accountService = accountService;
            this.userService = userService;
            this.restApiService = restApiService;
            this.logger = logger;
        }

        public ModelStateDictionary Login(string userName, string password, bool createPersistentCookie = false)
        {
            var user = GetUser(userName, password);
            var errors = ValidateUser(user);
            if (errors.IsNotNullOrEmpty())
            {
                return errors;
            }

            var key = $"incorrectLoginCount_{userName}";
            var incorrectLoginCount = HttpContext.Current.Session[key] != null
                ? int.Parse(HttpContext.Current.Session[key].ToString())
                : 0;
            incorrectLoginCount++;
            HttpContext.Current.Session[key] = incorrectLoginCount;
            if (incorrectLoginCount >= 5 && user != null)
            {
                using (var transaction = contextManager.NewTransaction())
                {
                    userService.ChangeStatus(
                        ConfigurationReader.UserStatusBlocked,
                        user.Id,
                        ConfigurationReader.AutomationUserId);
                    transaction.Commit();
                }

                HttpContext.Current.Session[key] = 0;
                throw new UserException(Resource.MaximumLogginAttempts);
            }

            // Set authentication user cookie
            SetAuthCookie(user.UserName, createPersistentCookie);

            // Map user to userPrincipal - init user roles - mapper create new role by user type
            var userPrincipal = mapper.Map<IUser, UserPrincipal>(user);
            userPrincipal.Password = password;

            // Log user login in DB - insert start date
            using (var transaction = contextManager.NewTransaction())
            {
                userPrincipal.DbSessionId = accountService.LogLoginAction(
                    user.Id.Value,
                    user.UserName);
                transaction.Commit();
            }

            HttpContext.Current.Session[BaseController.SessionUser] = HttpContext.Current.User = userPrincipal;
            return null;
        }

        public void SignOut()
        {
            FormsAuthentication.SignOut();
        }

        private User GetUser(string userName, string password)
        {
            if (userName.IsNullOrEmptyOrWhiteSpace() || password.IsNullOrEmptyOrWhiteSpace())
            {
                return null;
            }

            // Check user in geonetwork
            object geonetworkUser = null;
            try
            {
                using (var client = restApiService.GetClient(new UserPrincipal { UserName = userName, Password = password }))
                {
                    geonetworkUser = restApiService.GetRequest<object>(client, "me");
                }
            }
            catch (Exception e)
            {
                logger.Error(e);
            }

            User user = null;
            if (userName.IsNotNullOrEmpty())
            {
                using (contextManager.NewConnection())
                {
                    user = accountService.GetByUserName(userName);
                }
            }

            if (geonetworkUser != null)
            {
                return user;
            }

            if (user?.Password == null
                || password?.IsNotNullOrEmpty() != true
                || !PasswordTools.ValidatePassword(password, user.Password))
            {
                return null;
            }

            return user;
        }

        private ModelStateDictionary ValidateUser(User user)
        {
            var errors = new ModelStateDictionary();
            if (user == null)
            {
                errors.AddModelErrorSafety(string.Empty, Resource.IncorrectLogin);
            }
            else if (user.Status.Id != EnumHelper.GetStatusIdByEnum(UserStatus.Active))
            {
                errors.AddModelErrorSafety(string.Empty, Resource.RegistrationNotCompleteOrBlocked);
            }
            else if (user.RoleActivities.IsNullOrEmpty())
            {
                errors.AddModelErrorSafety(string.Empty, Resource.ForbiddenError);
            }

            return errors;
        }

        private void SetAuthCookie(string userName, bool createPersistentCookie = false, string userData = null)
        {
            var now = DateTime.Now;
            var timeout = now.Add(
                createPersistentCookie
                    ? new TimeSpan(365, 0, 0, 0)
                    : FormsAuthentication.Timeout);

            var ticket = new FormsAuthenticationTicket(
                1,
                userName,
                now,
                timeout,
                createPersistentCookie,
                userData ?? string.Empty);
            var encrypted = FormsAuthentication.Encrypt(ticket);
            var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encrypted)
            {
                Expires = timeout
            };

            HttpContext.Current.Response.Cookies.Add(cookie);
        }
    }
}