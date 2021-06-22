namespace Inspire.Domain.Services
{
    using System;

    public interface IMailSender
    {
        void SendMail(Guid mailId, string sendTo, string subject, string replyTo = null);
    }
}