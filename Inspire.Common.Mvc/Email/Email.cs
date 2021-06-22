namespace Inspire.Common.Mvc.Email
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Net.Mail;
    using System.Net.Mime;
    using System.Text;
    using System.Web.Configuration;

    using Inspire.Utilities.Extensions;

    public static class Email
    {
        private static readonly string MailAddress = WebConfigurationManager.AppSettings["MailAddress"];
        private static readonly string MailHost = WebConfigurationManager.AppSettings["MailHost"];
        private static readonly int MailPort = Convert.ToInt32(WebConfigurationManager.AppSettings["MailPort"]);

        private static readonly bool MailEnableSsl =
            Convert.ToBoolean(WebConfigurationManager.AppSettings["MailEnableSSL"]);

        private static readonly string MailUser = WebConfigurationManager.AppSettings["MailUser"];
        private static readonly string MailPassword = WebConfigurationManager.AppSettings["MailPassword"];

        /// <summary>
        ///     Send mail to client
        /// </summary>
        /// <param name="sendTo"></param>
        /// <param name="subject"></param>
        /// <param name="body"></param>
        /// <param name="replyTo"></param>
        /// <param name="attachments"></param>
        /// <param name="isBodyHtml"></param>
        /// <param name="encoding"></param>
        /// <param name="sendAsync"></param>
        /// <param name="additionalInlineFiles"></param>
        public static void SendMail(
            string sendTo,
            string subject,
            string body,
            string replyTo = null,
            IEnumerable<Attachment> attachments = null,
            bool isBodyHtml = true,
            Encoding encoding = null,
            TransferEncoding transferEncoding = TransferEncoding.Base64,
            bool sendAsync = false,
            bool isBcc = false,
            string[] additionalInlineFiles = null)
        {
            SendMails(
                !string.IsNullOrWhiteSpace(sendTo) && sendTo.Contains(";")
                    ? sendTo.Split(';').Select(x => x.Trim())
                    : new[]
                      {
                          sendTo
                      },
                subject,
                body,
                replyTo,
                attachments,
                isBodyHtml,
                encoding,
                transferEncoding,
                sendAsync,
                null,
                isBcc,
                additionalInlineFiles);
        }

        /// <summary>
        ///     Send mail to clients
        /// </summary>
        /// <param name="sendToAddresses"></param>
        /// <param name="subject"></param>
        /// <param name="body"></param>
        /// <param name="replyTo"></param>
        /// <param name="attachments"></param>
        /// <param name="isBodyHtml"></param>
        /// <param name="encoding"></param>
        /// <param name="transferEncoding"></param>
        /// <param name="sendAsync"></param>
        /// <param name="action"></param>
        /// <param name="additionalInlineFiles"></param>
        public static void SendMails(
            IEnumerable<string> sendToAddresses,
            string subject,
            string body,
            string replyTo = null,
            IEnumerable<Attachment> attachments = null,
            bool isBodyHtml = true,
            Encoding encoding = null,
            TransferEncoding transferEncoding = TransferEncoding.Base64,
            bool sendAsync = false,
            Action action = null,
            bool isBcc = false,
            string[] additionalInlineFiles = null)
        {
            if (sendToAddresses.IsNullOrEmpty())
            {
                return;
            }

            encoding = encoding ?? Encoding.UTF8;

            // Try to create smtp client to send mail
            using (var client = new SmtpClient(MailHost, MailPort))
            {
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                if (string.IsNullOrEmpty(MailUser) && string.IsNullOrEmpty(MailPassword))
                {
                    client.UseDefaultCredentials = true;
                }
                else
                {
                    client.Credentials = new NetworkCredential(MailUser, MailPassword);
                }

                client.EnableSsl = MailEnableSsl;
                if (MailEnableSsl)
                {
                    ServicePointManager.ServerCertificateValidationCallback =
                        (sender, certificate, chain, errors) => true;
                }

                // Try to create mail message
                using (var message = new MailMessage())
                {
                    ////http://stackoverflow.com/questions/18358534/send-inline-image-in-email
                    if (additionalInlineFiles.IsNotNullOrEmpty())
                    {
                        var alternateView =
                            AlternateView.CreateAlternateViewFromString(body, null, MediaTypeNames.Text.Html);

                        foreach (var file in additionalInlineFiles)
                        {
                            var inline = new LinkedResource(file)
                                         { ContentId = Path.GetFileNameWithoutExtension(file) };
                            alternateView.LinkedResources.Add(inline);
                        }

                        message.AlternateViews.Add(alternateView);
                    }

                    message.From = new MailAddress(MailAddress);
                    message.Sender = new MailAddress(MailAddress);
                    message.Subject = subject;
                    message.SubjectEncoding = encoding;
                    message.Body = body;

                    message.BodyEncoding = encoding;
                    message.IsBodyHtml = isBodyHtml;
                    message.BodyTransferEncoding = TransferEncoding.QuotedPrintable;
                    if (replyTo != null)
                    {
                        message.ReplyToList.Add(new MailAddress(replyTo));
                    }

                    // Try to add senders
                    foreach (var sendTo in sendToAddresses.Where(item => !string.IsNullOrEmpty(item)))
                    {
                        if (!isBcc)
                        {
                            message.To.Add(sendTo);
                        }
                        else
                        {
                            message.Bcc.Add(sendTo);
                        }
                    }

                    // Try to add attachments
                    if (attachments != null)
                    {
                        foreach (var attachment in attachments)
                        {
                            message.Attachments.Add(attachment);
                        }
                    }

                    // Try to send mail
                    if (sendAsync)
                    {
                        client.SendAsync(message, "SendAsyncMailToken");
                    }
                    else
                    {
                        client.Send(message);
                    }
                }
            }
        }
    }
}