namespace Inspire.Repository
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Inspire.Core.Infrastructure.Context;
    using Inspire.Data.Repositories;
    using Inspire.Data.Utilities;
    using Inspire.Domain.Repositories;
    using Inspire.Model.Nomenclature;
    using Inspire.Model.User;
    using Inspire.Repository.Utilities;
    using Inspire.Utilities.Extensions;

    public class AccountRepository : BaseRepository, IAccountRepository
    {
        public AccountRepository(IAisContext context)
            : base(context)
        {
        }

        public User GetByUserName(string userName)
        {
            using (var command = Context.Connection.GenerateCommand(
                "ais.get_userbyusername",
                new
                {
                    userName
                }))
            {
                using (var reader = command.ExecuteReaderSafety())
                {
                    if (reader.Read())
                    {
                        return new User
                               {
                                   Id = reader.GetFieldValue<Guid?>("id"),
                                   UserName = reader.GetFieldValue<string>("username"),
                                   Email = reader.GetFieldValue<string>("email"),
                                   Password = reader.GetFieldValue<string>("password"),
                                   RoleActivities = reader.GetFieldValue<Guid[]>("activityids")?.ToList(),
                                   ////LastChangePasswordDate = reader.GetFieldValue<DateTime?>("lastchangepassworddate"),
                                   Status = new Nomenclature
                                            {
                                                Id = reader.GetFieldValue<Guid?>("statusid"),
                                                Name = reader.GetFieldValue<string>("statusname")
                                            },
                                   IsAdmin = reader.GetFieldValue<bool>("isadmin")
                               };
                    }

                    return null;
                }
            }
        }

        public Guid LogLoginAction(Guid id, string userEmail, string ipAddress)
        {
            using (var command = Context.Connection.GenerateCommand(
                "ais.ins_userlogin",
                null,
                new Dictionary<string, object>
                {
                    { "pusername", userEmail },
                    { "puserid", id },
                    { "puserip", ipAddress }
                }))
            {
                command.ExecuteNonQuerySafety();
                return (Guid)command.Parameters["id"].Value;
            }
        }
    }
}