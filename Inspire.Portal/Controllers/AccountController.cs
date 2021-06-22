namespace Inspire.Portal.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Web.Mvc;

    using AutoMapper;

    using Inspire.Common.Mvc.Enums;
    using Inspire.Common.Mvc.Extensions;
    using Inspire.Common.Mvc.Filters.CustomAuthorize;
    using Inspire.Common.Mvc.Infrastructure.BaseTypes;
    using Inspire.Common.Mvc.Utilities;
    using Inspire.Core.Infrastructure.Logger;
    using Inspire.Core.Infrastructure.TransactionManager;
    using Inspire.Domain.Services;
    using Inspire.Infrastructure.Membership;
    using Inspire.Model.Helpers;
    using Inspire.Model.User;
    using Inspire.Portal.App_GlobalResources;
    using Inspire.Portal.Infrastructure.AuthenticationProvider;
    using Inspire.Portal.Infrastructure.RequestData;
    using Inspire.Portal.Models.Account;
    using Inspire.Portal.Services.CaptchaService;
    using Inspire.Portal.Services.RestApiService;
    using Inspire.Portal.Services.UserMailService;
    using Inspire.Portal.Utilities;
    using Inspire.Utilities.Encryption;
    using Inspire.Utilities.Exception;
    using Inspire.Utilities.Extensions;

    public class AccountController : BaseDbController
    {
        private readonly IAccountService accountService;
        private readonly IAuthenticationProvider authenticationProvider;
        private readonly ICaptchaService captchaService;
        private readonly IRestApiService restApiService;
        private readonly IUserMailService userMailService;
        private readonly IUserService userService;

        public AccountController(
            ILogger logger,
            IMapper mapper,
            IDbContextManager contextManager,
            IAccountService accountService,
            IAuthenticationProvider authenticationProvider,
            IUserService userService,
            IUserMailService userMailService,
            ICaptchaService captchaService,
            IRestApiService restApiService)
            : base(logger, mapper, contextManager)
        {
            this.accountService = accountService;
            this.authenticationProvider = authenticationProvider;
            this.userService = userService;
            this.userMailService = userMailService;
            this.captchaService = captchaService;
            this.restApiService = restApiService;
        }

        [HttpGet]
        public ActionResult Login(string returnUrl = null)
        {
            if (User?.Identity?.IsAuthenticated == true)
            {
                this.ShowMessage(MessageType.Warning, Resource.AlreadyLogIn);
                return Redirect(GetDefaultUrl());
            }

            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        public ActionResult Login(LoginViewModel model, string returnUrl = null)
        {
            if (User?.Identity?.IsAuthenticated == true)
            {
                this.ShowMessage(MessageType.Warning, Resource.AlreadyLogIn);

                returnUrl = GetDefaultUrl();
                if (Request.IsAjaxRequest())
                {
                    this.SetAjaxResponseRedirectUrl(returnUrl, true);
                    return new EmptyResult();
                }

                return Redirect(returnUrl);
            }

            // Validate captcha control
            if (!captchaService.Validate(Request["g-recaptcha-response"]))
            {
                ModelState.AddModelErrorSafety(string.Empty, Resource.WrongCaptchaMessage);
            }
            else
            {
                var errors = authenticationProvider.Login(model.Username, model.Password);
                if (errors != null)
                {
                    ModelState.Merge(errors);
                }
            }

            if (!ModelState.IsValid)
            {
                return Request.IsAjaxRequest()
                    ? PartialView("Login", model) as ActionResult
                    : View("Login", model);
            }

            returnUrl = returnUrl.IsNotNullOrEmpty() && Url.IsLocalUrl(returnUrl)
                ? returnUrl
                : GetDefaultUrl();

            if (Request.IsAjaxRequest())
            {
                this.SetAjaxResponseRedirectUrl(returnUrl, true);
                return new EmptyResult();
            }

            return Redirect(returnUrl);
        }

        [CustomAuthorize]
        [HttpPost]
        public ActionResult Logout()
        {
            authenticationProvider.SignOut();
            Session.Abandon();

            var defaultUrl = GetDefaultUrl();
            if (Request.IsAjaxRequest())
            {
                this.SetAjaxResponseRedirectUrl(defaultUrl, true);
                return new EmptyResult();
            }

            return Redirect(defaultUrl);
        }

        [AllowAnonymous]
        [HttpGet]
        public ActionResult ForgottenPassword()
        {
            return Request.IsAjaxRequest()
                ? PartialView("_ForgottenPassword") as ActionResult
                : View("_ForgottenPassword");
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult ForgottenPassword(ForgottenPasswordViewModel model)
        {
            // Validate captcha control
            if (!captchaService.Validate(Request["g-recaptcha-response"]))
            {
                ModelState.AddModelError(string.Empty, Resource.WrongCaptchaMessage);
            }

            if (!ModelState.IsValid)
            {
                return Json(
                    new { success = false, result = RenderRazorViewToString("_ForgottenPassword", model) },
                    JsonRequestBehavior.AllowGet);
            }

            User user;
            using (ContextManager.NewConnection())
            {
                user = accountService.GetByUserName(model.UserName);
            }

            if (user == null)
            {
                throw new UserException(Resource.UserNotFound);
            }

            if (user.Status.Id != EnumHelper.GetStatusIdByEnum(UserStatus.Active))
            {
                throw new UserException(Resource.ActivateYourAccount);
            }

            userMailService.SendChangePasswordMail(user);

            this.ShowMessage(MessageType.Success, Resource.ForgottenPasswordLinkSuccess);

            return Json(new { success = true, result = user }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult Activate(string token)
        {
            var user = GetUserByToken(token);

            if (user.Status.Id == EnumHelper.GetStatusIdByEnum(UserStatus.Blocked))
            {
                throw new UserException(Resource.RegistrationNotCompleteOrBlocked);
            }

            if (user.Id.HasValue)
            {
                var userPrincipal = Mapper.Map<IUser, UserPrincipal>(user);
                using (var transaction = ContextManager.NewTransaction(new RequestData(userPrincipal)))
                {
                    userService.ChangeStatus(
                        EnumHelper.GetStatusIdByEnum(UserStatus.Active),
                        user.Id.Value,
                        ConfigurationReader.AutomationUserId);
                    transaction.Commit();
                }

                return View("ChangePassword", new ChangePasswordViewModel { Token = token });
            }

            throw new UserException(Resource.InvalidToken);
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult ChangePassword(string token)
        {
            var user = token.IsNotNullOrEmpty()
                ? GetUserByToken(token)
                : User != null
                    ? new User { Id = User.Id, IsAdmin = User.IsAdmin }
                    : null;
            if (user?.Id.HasValue == true)
            {
                return View("ChangePassword", new ChangePasswordViewModel { Token = token, IsAdmin = user.IsAdmin });
            }

            throw new UserException(Resource.InvalidToken);
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult ChangePassword(ChangePasswordViewModel model)
        {
            ModelState.Clear();

            // Validate captcha control
            if (!captchaService.Validate(Request["g-recaptcha-response"]))
            {
                ModelState.AddModelError(string.Empty, Resource.WrongCaptchaMessage);
            }

            User user = null;
            if (model.Token.IsNotNullOrEmpty())
            {
                user = GetUserByToken(model.Token);
            }
            else if (User?.Id != null)
            {
                using (ContextManager.NewConnection())
                {
                    user = accountService.GetByUserName(User.UserName);
                }

                // Validate old password - if is same as login user password
                if (model.OldPassword.IsNullOrEmpty() ||
                    !PasswordTools.ValidatePassword(model.OldPassword, user.Password))
                {
                    ModelState.AddModelError("OldPassword", Resource.PasswordsDoesNotMatch);
                }
            }

            if (user == null)
            {
                throw new WarningException(Resource.InvalidToken);
            }

            model.IsAdmin = user.IsAdmin;
            model.UserId = user.Id.Value;

            if (!TryValidateModel(model) || !ModelState.IsValid)
            {
                return View("ChangePassword", model);
            }

            using (var client = restApiService.GetClient(new UserPrincipal { UserName = ConfigurationReader.GeoNetworkAdminUser, Password = ConfigurationReader.GeoNetworkAdminPass }))
            {
                restApiService.PostRequest(
                    client,
                    $"users/{user.GeoNetworkId}/actions/forget-password?password={model.Password}&password2={model.ConfirmPassword}");
            }

            model.Password = PasswordTools.CreateHash(model.Password);

            var userPrincipal = Mapper.Map<IUser, UserPrincipal>(user);
            using (var transaction = ContextManager.NewTransaction(new RequestData(userPrincipal)))
            {
                userService.ChangePassword(Mapper.Map<ChangePasswordModel>(model));

                // If user is not active - activate it
                if (model.Token.IsNotNullOrEmpty() && user.Status.Id == EnumHelper.GetStatusIdByEnum(UserStatus.InActive))
                {
                    userService.ChangeStatus(
                        EnumHelper.GetStatusIdByEnum(UserStatus.Active),
                        user.Id.Value,
                        ConfigurationReader.AutomationUserId);
                }

                transaction.Commit();
            }

            return RedirectToAction("Login");
        }

        [HttpGet]
        [CustomAuthorize]
        public ActionResult UserProfile()
        {
            User user;
            using (ContextManager.NewConnection())
            {
                user = userService.Get(User.Id);
            }

            return View("Profile", user);
        }

        private bool ValidateHash(KeyValuePair<Guid, long> decrypted)
        {
            var now = DateTime.Now.Ticks;

            var ts = TimeSpan.FromTicks(now - decrypted.Value);

            return !(ts.TotalMinutes > ConfigurationReader.LinkExpirationPeriod);
        }

        private User GetUserByToken(string token)
        {
            try
            {
                var decryptedHash = Cryptography.DeserializeIdToken(token, ConfigurationReader.EncryptKey);
                if (ValidateHash(decryptedHash))
                {
                    User user;
                    using (ContextManager.NewConnection())
                    {
                        user = userService.Get(decryptedHash.Key);
                    }

                    var userToken = Cryptography.Decode(user.Token);
                    token = Cryptography.Decode(token);
                    if (userToken == token)
                    {
                        return user;
                    }
                }
            }
            catch (Exception exception)
            {
                throw new UserLogException(Resource.InvalidToken, exception);
            }

            throw new UserException(Resource.InvalidToken);
        }
    }
}