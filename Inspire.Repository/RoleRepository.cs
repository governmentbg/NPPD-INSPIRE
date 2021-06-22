namespace Inspire.Repository
{
    using System;
    using System.Collections.Generic;
    using System.Data.Common;
    using System.Linq;

    using Inspire.Core.Infrastructure.Context;
    using Inspire.Data.Repositories;
    using Inspire.Data.Utilities;
    using Inspire.Domain.Repositories;
    using Inspire.Model.Nomenclature;
    using Inspire.Model.Role;
    using Inspire.Repository.Utilities;
    using Inspire.Utilities.Extensions;

    public class RoleRepository : BaseRepository, IRoleRepository
    {
        public RoleRepository(IAisContext context)
            : base(context)
        {
        }

        public IEnumerable<Role> Search(RoleQuery query)
        {
            var result = new List<Role>();
            using (var command = Context.Connection.GenerateCommand(
                "admdata.search_role",
                new { id = query.Id, name = query.Name, curruserid = query.UserId }))
            {
                using (var reader = command.ExecuteReaderSafety())
                {
                    while (reader.Read())
                    {
                        result.Add(
                            new Role
                            {
                                Id = reader.GetFieldValue<Guid>("Id"),
                                Name = reader.GetFieldValue<string>("Name"),
                                Activities = GetActivitiesByReader(reader)
                            });
                    }
                }
            }

            return result;
        }

        public IEnumerable<Nomenclature> GetAllActivities()
        {
            var result = new List<Nomenclature>();
            using (var command = Context.Connection.GenerateCommand("admdata.get_activity"))
            {
                using (var reader = command.ExecuteReaderSafety())
                {
                    while (reader.Read())
                    {
                        result.Add(
                            new Nomenclature
                            {
                                Id = reader.GetFieldValue<Guid>("Id"),
                                Name = reader.GetFieldValue<string>("Name")
                            });
                    }
                }
            }

            return result;
        }

        public Role Get(Guid id, Guid userId)
        {
            using (var command = Context.Connection.GenerateCommand(
                "admdata.search_role",
                new { id, curruserid = userId }))
            {
                using (var reader = command.ExecuteReaderSafety())
                {
                    while (reader.Read())
                    {
                        return new Role
                               {
                                   Id = reader.GetFieldValue<Guid>("Id"),
                                   Name = reader.GetFieldValue<string>("Name"),
                                   Activities = GetActivitiesByReader(reader)
                               };
                    }
                }
            }

            return null;
        }

        public Guid Upsert(Role role)
        {
            using (var command = Context.Connection.GenerateCommand(
                "admdata.upsert_role",
                new { id = role.Id, name = role.Name, activityid = role.Activities.Select(item => item.Id).ToArray() }))
            {
                command.ExecuteNonQuerySafety();
                return (Guid)command.Parameters["pid"].Value;
            }
        }

        public List<Guid> GetUserRoles(Guid id)
        {
            using (var command = Context.Connection.GenerateCommand("ais.get_rolebyuser", new { userid = id }))
            {
                using (var reader = command.ExecuteReaderSafety())
                {
                    if (reader.Read())
                    {
                        return reader.GetFieldValue<Guid[]>("roleids")?.ToList();
                    }
                }
            }

            return null;
        }

        public void Delete(Guid id)
        {
            using (var command = Context.Connection.GenerateCommand("admdata.del_role", new { id }))
            {
                command.ExecuteNonQuerySafety();
            }
        }

        private List<Nomenclature> GetActivitiesByReader(DbDataReader reader)
        {
            var ids = reader.GetFieldValue<Guid[]>("activityIds");
            var names = reader.GetFieldValue<string[]>("activityNames");

            if (ids.IsNullOrEmpty() || names.IsNullOrEmpty())
            {
                return null;
            }

            return ids.Select((t, i) => new Nomenclature { Id = t, Name = names[i] }).ToList();
        }
    }
}