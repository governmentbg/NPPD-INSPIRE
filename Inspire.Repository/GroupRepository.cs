namespace Inspire.Repository
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Inspire.Core.Infrastructure.Context;
    using Inspire.Data.Repositories;
    using Inspire.Data.Utilities;
    using Inspire.Domain.Repositories;
    using Inspire.Model.Group;
    using Inspire.Portal.Areas.Admin.Models.Queries;
    using Inspire.Repository.Utilities;
    using Inspire.Utilities.Extensions;

    public class GroupRepository : BaseRepository, IGroupRepository
    {
        public GroupRepository(IAisContext context)
            : base(context)
        {
        }

        public void Upsert(Group model)
        {
            using (var command = Context.Connection.GenerateCommand(
                "ais.upsert_organization",
                new
                {
                    id = model.Id,
                    bulstat = model.Bulstat,
                    email = model.Email,
                    geonetworkid = model.GeoNetworkId,
                    languageid = model.Names.Keys.Select(Guid.Parse).ToArray(),
                    name = model.Names.Values.Select(StringExt.PrepareToDb).ToArray(),
                    contactperson = model.ContactPersons.Values.Select(StringExt.PrepareToDb).ToArray(),
                    description = model.Description.Values.Select(StringExt.PrepareToDb).ToArray(),
                    link = model.Website.Values.Select(StringExt.PrepareToDb).ToArray()
                }))
            {
                command.ExecuteNonQuerySafety();
                model.Id = (Guid)command.Parameters["pid"].Value;
            }
        }

        public Group Get(Guid id, Guid? languageId = null)
        {
            using (var command = Context.Connection.GenerateCommand(
                "ais.get_organization",
                new
                {
                    id,
                    languageid = languageId
                }))
            {
                using (var reader = command.ExecuteReaderSafety())
                {
                    if (reader.Read())
                    {
                        var group = new Group
                        {
                            Id = reader.GetFieldValue<Guid?>("id"),
                            GeoNetworkId = reader.GetFieldValue<long?>("geonetworkid"),
                            Bulstat = reader.GetFieldValue<string>("bulstat"),
                            Email = reader.GetFieldValue<string>("email"),
                            Names = new SortedDictionary<string, string>(),
                            ContactPersons = new SortedDictionary<string, string>(),
                            Description = new SortedDictionary<string, string>(),
                            Website = new SortedDictionary<string, string>()
                        };

                        var languages = reader.GetFieldValue<Guid[]>("languageid");
                        var names = reader.GetFieldValue<string[]>("name");
                        var contactPersons = reader.GetFieldValue<string[]>("contactperson");
                        var descriptions = reader.GetFieldValue<string[]>("description");
                        var websites = reader.GetFieldValue<string[]>("link");

                        for (var i = 0; i < languages.Length; i++)
                        {
                            var language = languages[i].ToString();

                            AddValueToDictionary(group.Names, names, language, i);
                            AddValueToDictionary(group.ContactPersons, contactPersons, language, i);
                            AddValueToDictionary(group.Description, descriptions, language, i);
                            AddValueToDictionary(group.Website, websites, language, i);
                        }

                        return group;
                    }

                    return null;
                }
            }
        }

        public List<GroupTableModel> Search(GroupQueryModel query, Guid languageId)
        {
            using (var command = Context.Connection.GenerateCommand(
                "ais.search_organization",
                query,
                new Dictionary<string, object>
                {
                    { "planguageid", languageId }
                }))
            {
                var result = new List<GroupTableModel>();
                using (var reader = command.ExecuteReaderSafety())
                {
                    while (reader.Read())
                    {
                        result.Add(
                            new GroupTableModel
                            {
                                Id = reader.GetFieldValue<Guid>("id"),
                                GeoNetworkId = reader.GetFieldValue<long?>("geonetworkid"),
                                Email = reader.GetFieldValue<string>("email"),
                                Bulstat = reader.GetFieldValue<string>("bulstat"),
                                Name = reader.GetFieldValue<string>("name"),
                                ContactPerson = reader.GetFieldValue<string>("contactperson")
                            });
                    }
                }

                return result;
            }
        }

        private void AddValueToDictionary(IDictionary<string, string> dest, IReadOnlyList<string> source, string language, int index)
        {
            if (source.IsNotNullOrEmpty() && source.Count() > index)
            {
                dest.Add(language, source[index]);
            }
        }
    }
}
