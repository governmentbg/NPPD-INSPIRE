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
    using Inspire.Model.Publication;
    using Inspire.Repository.Utilities;
    using Inspire.Utilities.Extensions;

    public class PublicationRepository : BaseRepository, IPublicationRepository
    {
        public PublicationRepository(IAisContext context)
            : base(context)
        {
        }

        public Guid Upsert(Publication model)
        {
            using (var command = Context.Connection.GenerateCommand(
                "ais.upsert_news",
                model,
                new Dictionary<string, object>
                {
                    { "pnewtypeid", model.Type?.Id },
                    { "pisvisible", model.IsVisibleInWeb },
                    { "ptitle", model.Titles.Values },
                    { "pcontent", model.Contents.Values },
                    { "pislead", model.IsLead },
                    { "planguageid", model.Titles.Keys.Select(Guid.Parse).ToArray() }
                }))
            {
                command.ExecuteNonQuerySafety();
                return (Guid)command.Parameters["pid"].Value;
            }
        }

        public Publication Get(Guid id, Guid? language = null)
        {
            using (var command = Context.Connection.GenerateCommand(
                "ais.get_news",
                new
                {
                    id,
                    languageid = language
                }))
            {
                using (var reader = command.ExecuteReaderSafety())
                {
                    while (reader.Read())
                    {
                        return new Publication
                               {
                                   Id = reader.GetFieldValue<Guid?>("id"),
                                   EnterDate = reader.GetFieldValue<DateTime?>("enterdate"),
                                   StartDate = reader.GetFieldValue<DateTime?>("startdate"),
                                   EndDate = reader.GetFieldValue<DateTime?>("enddate"),
                                   Type = new Nomenclature
                                          {
                                              Id = reader.GetFieldValue<Guid?>("newtypeid")
                                          },
                                   IsVisibleInWeb = reader.GetFieldValue<bool>("isvisible"),
                                   IsLead = reader.GetFieldValue<bool>("islead"),
                                   Titles = CultureHelper.GetDictionaryData(
                                       reader.GetFieldValue<Guid[]>("languageid"),
                                       reader.GetFieldValue<string[]>("title")),
                                   Contents = CultureHelper.GetDictionaryData(
                                       reader.GetFieldValue<Guid[]>("languageid"),
                                       reader.GetFieldValue<string[]>("content"))
                               };
                    }
                }
            }

            return null;
        }

        public List<Publication> Search(PublicationQuery query)
        {
            var result = new List<Publication>();
            var lang = query.LanguageId.ToString();

            using (var command = Context.Connection.GenerateCommand(
                "ais.search_news",
                new
                {
                    id = query.Id,
                    title = query.Name,
                    newtypeid = query.TypeId,
                    regdatefrom = query.EnterDateFrom,
                    regdateto = query.EnterDateTo,
                    languageid = query.LanguageId,
                    startdatefrom = query.StartDateFrom,
                    startdateto = query.StartDateTo,
                    isvisible = query.IsVisible
                }))
            {
                using (var reader = command.ExecuteReaderSafety())
                {
                    while (reader.Read())
                    {
                        result.Add(
                            new Publication
                            {
                                Id = reader.GetFieldValue<Guid?>("id"),
                                EnterDate = reader.GetFieldValue<DateTime?>("regdate"),
                                StartDate = reader.GetFieldValue<DateTime?>("startdate"),
                                EndDate = reader.GetFieldValue<DateTime?>("enddate"),
                                IsVisibleInWeb = reader.GetFieldValue<bool>("isvisible"),
                                Type = new Nomenclature
                                       {
                                           Id = reader.GetFieldValue<Guid?>("newtypeid"),
                                           Name = reader.GetFieldValue<string>("newtypename")
                                       },
                                IsLead = reader.GetFieldValue<bool>("islead"),
                                Titles = new SortedDictionary<string, string>
                                         {
                                             {
                                                 lang, reader.GetFieldValue<string>("title")
                                             }
                                         },
                                Contents = new SortedDictionary<string, string>
                                           {
                                               {
                                                   lang, reader.GetFieldValue<string>("content")
                                               }
                                           }
                            });
                    }
                }
            }

            return result;
        }

        public void Delete(Guid id)
        {
            using (var command = Context.Connection.GenerateCommand(
                "ais.del_news",
                new { id }))
            {
                command.ExecuteNonQuerySafety();
            }
        }
    }
}