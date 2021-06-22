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

    public class UserRepository : BaseRepository, IUserRepository
    {
        public UserRepository(IAisContext context)
            : base(context)
        {
        }

        public IEnumerable<User> Search(UserQuery query, Guid? languageId)
        {
            var result = new List<User>();

            using (var command = Context.Connection.GenerateCommand(
                "ais.search_user",
                new
                {
                    id = query.Id,
                    name = query.Name,
                    username = query.UserName,
                    email = query.Email,
                    statusid = query.Status,
                    organisation = query.Organisation,
                    languageId = languageId
                }))
            {
                using (var reader = command.ExecuteReaderSafety())
                {
                    while (reader.Read())
                    {
                        var user = new User
                                   {
                                       Id = reader.GetFieldValue<Guid?>("id"),
                                       UserName = reader.GetFieldValue<string>("username"),
                                       FirstName = reader.GetFieldValue<string>("fullname"),
                                       Phone = reader.GetFieldValue<string>("phone"),
                                       Email = reader.GetFieldValue<string>("email"),
                                       Status = new Nomenclature
                                                {
                                                    Id = reader.GetFieldValue<Guid?>("statusid"),
                                                    Name = reader.GetFieldValue<string>("statusname")
                                                },
                                       Roles = reader.GetFieldValue<string[]>("rolenames")?.ToList(),
                                       Organisation = reader.GetFieldValue<string>("organizationname")
                                   };

                        result.Add(user);
                    }
                }
            }

            return result;
        }

        public User Get(Guid id)
        {
            User user = null;
            using (var command = Context.Connection.GenerateCommand("ais.get_user", new { id }))
            {
                using (var reader = command.ExecuteReaderSafety())
                {
                    while (reader.Read())
                    {
                        user = new User
                               {
                                   Id = reader.GetFieldValue<Guid?>("id"),
                                   UserName = reader.GetFieldValue<string>("username"),
                                   FirstName = reader.GetFieldValue<string>("firstname"),
                                   LastName = reader.GetFieldValue<string>("lastname"),
                                   Phone = reader.GetFieldValue<string>("phone"),
                                   Fax = reader.GetFieldValue<string>("fax"),
                                   Email = reader.GetFieldValue<string>("email"),
                                   Department = reader.GetFieldValue<string>("department"),
                                   Status = new Nomenclature
                                            {
                                                Id = reader.GetFieldValue<Guid?>("statusid"),
                                                Name = reader.GetFieldValue<string>("statusname")
                                            },
                                   Token = reader.GetFieldValue<string>("token"),
                                   IsAdmin = reader.GetFieldValue<bool>("isadmin"),
                                   GeoNetworkId = reader.GetFieldValue<long?>("geonetworkid")
                               };
                    }
                }
            }

            return user;
        }

        public Guid Upsert(User user)
        {
            using (var command = Context.Connection.GenerateCommand("ais.upsert_user", user))
            {
                command.ExecuteNonQuerySafety();
                return (Guid)command.Parameters["pid"].Value;
            }
        }

        public void UpdatePasswordToken(Guid id, string resetPasswordToken)
        {
            using (var command = Context.Connection.GenerateCommand(
                "ais.upd_usertoken",
                new { id, token = resetPasswordToken }))
            {
                command.ExecuteNonQuerySafety();
            }
        }

        public void ChangeStatus(Guid statusId, Guid? userId, Guid automationUserId)
        {
            using (var command = Context.Connection.GenerateCommand("ais.upd_userstatus", new { userId, statusId }))
            {
                command.ExecuteNonQuerySafety();
            }
        }

        public void ChangePassword(ChangePasswordModel model)
        {
            using (var command = Context.Connection.GenerateCommand(
                "ais.upd_userpassword",
                new { id = model.UserId, password = model.Password }))
            {
                command.ExecuteNonQuerySafety();
            }
        }

        public void SetUserRoles(SetRole model)
        {
            using (var command = Context.Connection.GenerateCommand(
                "ais.upsert_xuserrole",
                new { userid = model.UserId, roleid = model.Roles }))
            {
                command.ExecuteNonQuerySafety();
            }
        }

        public List<string> SearchPositionByText(string text)
        {
            var result = new List<string>();
            using (var command = Context.Connection.GenerateCommand("ais.search_userposition", new { position = text }))
            {
                using (var reader = command.ExecuteReaderSafety())
                {
                    while (reader.Read())
                    {
                        result.Add(reader.GetFieldValue<string>("position"));
                    }
                }
            }

            return result;
        }

        public List<string> GetPasswords(Guid? userId)
        {
            using (var command = Context.Connection.GenerateCommand("ais.get_userpasswords", new { userid = userId }))
            {
                command.ExecuteNonQuerySafety();
                return (command.Parameters["passwords"].Value as string[])?.ToList();
            }
        }

        public void UpsertControl(UsersControl model)
        {
            var roleIds = new List<Guid?>();
            var userIds = new List<Guid?>();
            if (model.Items.IsNotNullOrEmpty())
            {
                foreach (var item in model.Items)
                {
                    roleIds.Add(item.Role?.Id);
                    userIds.Add(item.User?.Id);
                }
            }

            using (var command = Context.Connection.GenerateCommand(
                "ais.upsert_xobjactivity",
                new
                {
                    objectid = model.ItemId,
                    objecttypeid = model.ItemType,
                    roleId = roleIds,
                    userId = userIds
                }))
            {
                command.ExecuteNonQuerySafety();
            }
        }

        public List<Nomenclature> GetUsersByRole(Guid? id)
        {
            using (var command = Context.Connection.GenerateCommand(
                "ais.get_userbyrole",
                new
                {
                    roleid = id
                }))
            {
                var result = new List<Nomenclature>();
                using (var reader = command.ExecuteReaderSafety())
                {
                    while (reader.Read())
                    {
                        result.Add(
                            new Nomenclature
                            {
                                Id = reader.GetFieldValue<Guid?>("id"),
                                Name = reader.GetFieldValue<string>("username")
                            });
                    }
                }

                return result;
            }
        }

        public List<UserControlItem> GetControl(Guid objectId, Guid objectTypeId)
        {
            var result = new List<UserControlItem>();
            using (var command = Context.Connection.GenerateCommand(
                "ais.get_xobjactivity",
                new
                {
                    objectid = objectId,
                    objecttypeid = objectTypeId
                }))
            {
                using (var reader = command.ExecuteReaderSafety())
                {
                    while (reader.Read())
                    {
                        result.Add(
                            new UserControlItem
                            {
                                Role = new Nomenclature
                                       {
                                           Id = reader.GetFieldValue<Guid?>("roleid"),
                                           Name = reader.GetFieldValue<string>("rolename")
                                       },
                                User = new Nomenclature
                                       {
                                           Id = reader.GetFieldValue<Guid?>("userid"),
                                           Name = reader.GetFieldValue<string>("username")
                                       }
                            });
                    }
                }

                return result;
            }
        }

        public bool CheckIfUserHasRightsForObject(Guid userId, Guid? objectId, Guid objectTypeId, Guid activityId)
        {
            using (var command = Context.Connection.GenerateCommand(
                "ais.check_userhasrightsforobject",
                new
                {
                    userid = userId,
                    objectid = objectId,
                    objecttypeid = objectTypeId,
                    activityid = activityId
                }))
            {
                command.ExecuteNonQuerySafety();
                return command.Parameters["hasrights"].Value is bool && (bool)command.Parameters["hasrights"].Value;
            }
        }

        public List<Nomenclature> GetOrganisationsForDdl(Guid languageId)
        {
            var result = new List<Nomenclature>();
            using (var command = Context.Connection.GenerateCommand(
                "ais.search_organization_dropdown",
                new
                {
                    languageId = languageId,
                }))
            {
                using (var reader = command.ExecuteReaderSafety())
                {
                    while (reader.Read())
                    {
                        result.Add(
                            new Nomenclature()
                            {
                                 Id = reader.GetFieldValue<Guid?>("id"),
                                 Name = reader.GetFieldValue<string>("name")
                            });
                    }
                }

                return result;
            }
        }
    }
}