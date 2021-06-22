namespace Inspire.Portal.Services.UserMailService
{
    using System.IO;
    using System.Text;
    using System.Web;
    using System.Web.Mvc;

    using AutoMapper;

    using Inspire.Common.Mvc.Extensions;
    using Inspire.Common.Mvc.Utilities;
    using Inspire.Core.Infrastructure.RequestData;
    using Inspire.Core.Infrastructure.TransactionManager;
    using Inspire.Domain.Services;
    using Inspire.Model.Email;
    using Inspire.Model.Email.SendMessages;
    using Inspire.Model.Helpers;
    using Inspire.Model.User;
    using Inspire.Portal.App_GlobalResources;
    using Inspire.Portal.Utilities;
    using Inspire.Services;

    public class UserMailService : BaseService, IUserMailService
    {
        private readonly IDbContextManager contextManager;

        private readonly IEmailService emailService;

        private readonly IMailSender mailSender;

        private readonly IUserService userService;

        public UserMailService(
            IMapper mapper,
            IRequestData requestData,
            IDbContextManager contextManager,
            IUserService userService,
            IEmailService emailService,
            IMailSender mailSender)
            : base(mapper, requestData)
        {
            this.contextManager = contextManager;
            this.userService = userService;
            this.emailService = emailService;
            this.mailSender = mailSender;
        }

        public void SendChangePasswordMail(User user)
        {
            var token = Cryptography.GenerateTokenById(user.Id.Value, ConfigurationReader.EncryptKey);
            var url = new UrlHelper(HttpContext.Current.Request.RequestContext);
            var changePasswordUrl = url.AbsoluteAction(
                "ChangePassword",
                "Account",
                new { token, area = string.Empty });
            var changePasswordLink = $@"<a href=""{changePasswordUrl}""><strong>{changePasswordUrl}</strong></a><br/>";

            var path = Path.Combine(
                HttpContext.Current.Server.MapPath("~/App_Data"),
                @"Templates\RequestNewPasswordTemplate.html");
            var messageBody = FileOperator.ReadFile(path);
            messageBody = messageBody.Replace("$$userFullName", user.UserName);
            messageBody = messageBody.Replace("$$requestNewPasswordLink", changePasswordLink);

            var email = new Email
                        {
                            Content = Encoding.UTF8.GetBytes(messageBody),
                            RecepientId = user.Id.Value,
                            SendUserId = ConfigurationReader.AutomationUserId,
                            SentMailMessageType =
                                EnumHelper.GetMessageTypeIdByEnum(SentMailMessageType.ForgottenPasswordMail)
                        };

            using (var transaction = contextManager.NewTransaction())
            {
                userService.UpdatePasswordToken(user.Id.Value, token);
                email.Id = emailService.Insert(email);
                transaction.Commit();
            }

            mailSender.SendMail(email.Id.Value, user.Email, Resource.ResetPassword);
        }

        public void SendCompleteRegistrationEmail(User user)
        {
            var path = Path.Combine(
                HttpContext.Current.Server.MapPath("~/App_Data"),
                @"Templates\RegistrationMessageTemplate.html");
            var messageBody = FileOperator.ReadFile(path);

            var token = Cryptography.GenerateTokenById(user.Id.Value, ConfigurationReader.EncryptKey);
            var url = new UrlHelper(HttpContext.Current.Request.RequestContext);
            var activationUrl = url.AbsoluteAction(
                "Activate",
                "Account",
                new { token, area = string.Empty });

            var activationLink = $@"<a href=""{activationUrl}""><strong>{activationUrl}</strong></a><br/> ";

            messageBody = messageBody
                          .Replace("$$FullName", $"{user.FirstName} {user.LastName}")
                          .Replace("$$activationLink", activationLink)
                          .Replace("$$username", user.UserName);

            var email = new Email
                        {
                            Content = Encoding.UTF8.GetBytes(messageBody),
                            RecepientId = user.Id.Value,
                            SendUserId = ConfigurationReader.AutomationUserId,
                            SentMailMessageType =
                                EnumHelper.GetMessageTypeIdByEnum(SentMailMessageType.UserRegistrationMail)
                        };

            using (var transaction = contextManager.NewTransaction())
            {
                userService.UpdatePasswordToken(user.Id.Value, token);
                email.Id = emailService.Insert(email);
                transaction.Commit();
            }

            mailSender.SendMail(email.Id.Value, user.Email, Resource.ConfirmRegistrationMailSubject);
        }
    }
}