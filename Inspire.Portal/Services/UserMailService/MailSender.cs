namespace Inspire.Portal.Services.UserMailService
{
    using System;
    using System.Text;
    using System.Threading;
    using System.Web.Hosting;
    using System.Web.Mvc;

    using Inspire.Common.Mvc.Email;
    using Inspire.Core.Infrastructure.TransactionManager;
    using Inspire.Domain.Services;
    using Inspire.Utilities.Extensions;

    public class MailSender : IMailSender
    {
        public void SendMail(Guid mailId, string sendTo, string subject, string replyTo = null)
        {
            ScheduledMailTask(
                () =>
                {
                    if (sendTo.IsNullOrEmpty())
                    {
                        return;
                    }

                    var contextManager = DependencyResolver.Current.GetService<IDbContextManager>();
                    var mailService = DependencyResolver.Current.GetService<IEmailService>();

                    string messageBody;
                    using (contextManager.NewConnection())
                    {
                        messageBody = new string(Encoding.UTF8.GetChars(mailService.GetMailContentById(mailId)));
                    }

                    if (messageBody.IsNotNullOrEmpty())
                    {
                        Email.SendMail(string.Join(";", sendTo), subject, messageBody, replyTo);
                    }
                });
        }

        private void ScheduledMailTask(Action action)
        {
            var culture = Thread.CurrentThread.CurrentCulture;
            HostingEnvironment.QueueBackgroundWorkItem(
                cancellationToken =>
                {
                    Thread.CurrentThread.CurrentCulture = Thread.CurrentThread.CurrentUICulture = culture;
                    Thread.Sleep(5000);
                    action.Invoke();
                });
        }
    }
}