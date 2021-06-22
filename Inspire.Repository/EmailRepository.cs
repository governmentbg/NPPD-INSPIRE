namespace Inspire.Repository
{
    using System;

    using Inspire.Core.Infrastructure.Context;
    using Inspire.Data.Repositories;
    using Inspire.Data.Utilities;
    using Inspire.Domain.Repositories;
    using Inspire.Model.Email;
    using Inspire.Repository.Utilities;

    public class EmailRepository : BaseRepository, IEmailRepository
    {
        public EmailRepository(IAisContext context)
            : base(context)
        {
        }

        public Guid Insert(Email model)
        {
            using (var command = Context.Connection.GenerateCommand("ais.ins_useremail", model))
            {
                command.ExecuteNonQuerySafety();
                var emailId = Guid.Parse(command.Parameters["pid"].Value?.ToString());
                return emailId;
            }
        }

        public byte[] GetMailContentById(Guid mailContentId)
        {
            using (var command = Context.Connection.GenerateCommand("ais.get_useremail", new { id = mailContentId }))
            {
                command.ExecuteNonQuerySafety();
                return (byte[])command.Parameters["mailcontent"].Value;
            }
        }
    }
}