namespace Inspire.Model.Email
{
    using System;

    public class Email
    {
        public Guid? Id { get; set; }

        public byte[] Content { get; set; }

        public Guid? SendUserId { get; set; }

        public Guid? RecepientId { get; set; }

        public Guid SentMailMessageType { get; set; }
    }
}