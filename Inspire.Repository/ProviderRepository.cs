namespace Inspire.Repository
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Inspire.Core.Infrastructure.Context;
    using Inspire.Data.Repositories;
    using Inspire.Data.Utilities;
    using Inspire.Domain.Repositories;
    using Inspire.Model.Attachment;
    using Inspire.Model.Nomenclature;
    using Inspire.Model.Provider;
    using Inspire.Model.QueryModels;
    using Inspire.Repository.Utilities;
    using Inspire.Utilities.Extensions;

    public class ProviderRepository : BaseRepository, IProviderRepository
    {
        public ProviderRepository(IAisContext context)
            : base(context)
        {
        }

        public void Delete(Guid id)
        {
            using (var command = Context.Connection.GenerateCommand(
                "ais.del_supplier",
                new { id }))
            {
                command.ExecuteNonQuerySafety();
            }
        }

        public Provider Get(Guid id, Guid? language = null)
        {
            Provider provider = null;

            using (var command = Context.Connection.GenerateCommand(
                "ais.get_supplier",
                new
                {
                    id = id,
                    languageid = language
                }))
            {
                using (var reader = command.ExecuteReaderSafety())
                {
                    if (reader.Read())
                    {
                        provider = new Provider
                                   {
                                       Id = reader.GetFieldValue<Guid?>("id"),
                                       Status = new Nomenclature
                                                {
                                                    Id = reader.GetFieldValue<Guid?>("supplierstatusid"),
                                                    Name = reader.GetFieldValue<string>("supplierstatusname")
                                                },
                                       Links = CultureHelper.GetDictionaryData(
                                           reader.GetFieldValue<Guid[]>("languageid"),
                                           reader.GetFieldValue<string[]>("link")),
                                       Names = CultureHelper.GetDictionaryData(
                                           reader.GetFieldValue<Guid[]>("languageid"),
                                           reader.GetFieldValue<string[]>("name")),
                                       Descriptions = CultureHelper.GetDictionaryData(
                                           reader.GetFieldValue<Guid[]>("languageid"),
                                           reader.GetFieldValue<string[]>("description")),
                                       MainPicture = new Attachment
                                                     {
                                                         Id = reader.GetFieldValue<Guid?>("picid"),
                                                         Name = reader.GetFieldValue<string>("picname"),
                                                         Url = reader.GetFieldValue<string>("picurl")
                                                     }
                                   };
                    }
                }
            }

            return provider;
        }

        public void Rearrange(Guid[] ids)
        {
            using (var command = Context.Connection.GenerateCommand(
                "ais.upd_supplierorder",
                new { ids }))
            {
                command.ExecuteNonQuerySafety();
            }
        }

        public IEnumerable<Provider> Search(ProviderQueryModel query)
        {
            var result = new List<Provider>();
            var lang = query.LanguageId.ToString();

            using (var command = Context.Connection.GenerateCommand(
                "ais.search_supplier",
                new
                {
                    supplierstatusid = query.StatusId,
                    languageid = query.LanguageId,
                    id = query.Id
                }))
            {
                using (var reader = command.ExecuteReaderSafety())
                {
                    while (reader.Read())
                    {
                        result.Add(
                            new Provider
                            {
                                Id = reader.GetFieldValue<Guid?>("id"),
                                Status = new Nomenclature
                                         {
                                             Id = reader.GetFieldValue<Guid>("supplierstatusid"),
                                             Name = reader.GetFieldValue<string>("supplierstatusname")
                                         },
                                Names = new SortedDictionary<string, string>
                                         {
                                             {
                                                 lang, reader.GetFieldValue<string>("name")
                                             }
                                         },
                                Descriptions = new SortedDictionary<string, string>
                                         {
                                             {
                                                 lang, reader.GetFieldValue<string>("description")
                                             }
                                         },
                                Links = new SortedDictionary<string, string>
                                         {
                                             {
                                                 lang, reader.GetFieldValue<string>("link")
                                             }
                                         },
                                MainPicture = new Attachment
                                         {
                                             Url = reader.GetFieldValue<string>("picurl")
                                         },
                                LanguageId = query.LanguageId
                            });
                    }
                }
            }

            return result;
        }

        public Guid Upsert(Provider model)
        {
            using (var command = Context.Connection.GenerateCommand(
                "ais.upsert_supplier",
                model,
                new Dictionary<string, object>
                {
                    { "pid", model.Id },
                    { "pstatusid", model.Status?.Id },
                    { "planguageid", model.Names.Keys.Select(Guid.Parse).ToArray() },
                    { "pname", model.Names.Values },
                    { "pdescription", model.Descriptions.Values },
                    { "plink", model.Links.Values },
                }))
            {
                command.ExecuteNonQuerySafety();
                return (Guid)command.Parameters["pid"].Value;
            }
        }
    }
}
