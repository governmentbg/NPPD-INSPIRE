namespace Inspire.Repository
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Inspire.Core.Infrastructure.Context;
    using Inspire.Data.Repositories;
    using Inspire.Data.Utilities;
    using Inspire.Domain.Repositories;
    using Inspire.Model.Faq;
    using Inspire.Model.Nomenclature;
    using Inspire.Model.QueryModels;
    using Inspire.Model.TableModels;
    using Inspire.Repository.Utilities;
    using Inspire.Utilities.Extensions;

    public class FaqRepository : BaseRepository, IFaqRepository
    {
        public FaqRepository(IAisContext context)
            : base(context)
        {
        }

        public void Delete(Guid id)
        {
            using (var command = Context.Connection.GenerateCommand(
                "ais.del_faq",
                new { id }))
            {
                command.ExecuteNonQuerySafety();
            }
        }

        public void DeleteFaqCategory(Guid id, Guid? newCategoryId)
        {
            using (var command = Context.Connection.GenerateCommand(
                "ais.del_faqcategory",
                new
                {
                    id,
                    newid = newCategoryId
                }))
            {
                command.ExecuteNonQuerySafety();
            }
        }

        public void Rearrange(Guid[] ids)
        {
            using (var command = Context.Connection.GenerateCommand(
                "ais.upd_faqorder",
                new { ids }))
            {
                command.ExecuteNonQuerySafety();
            }
        }

        public Faq GetFaq(Guid id, Guid? languageId)
        {
            using (var command = Context.Connection.GenerateCommand(
                "ais.get_faq",
                new
                {
                    id,
                    languageid = languageId
                }))
            {
                using (var reader = command.ExecuteReaderSafety())
                {
                    while (reader.Read())
                    {
                        return new Faq
                               {
                                   Id = reader.GetFieldValue<Guid>("id"),
                                   Answers = CultureHelper.GetDictionaryData(
                                       reader.GetFieldValue<Guid[]>("languageid"),
                                       reader.GetFieldValue<string[]>("answer")),
                                   Questions = CultureHelper.GetDictionaryData(
                                       reader.GetFieldValue<Guid[]>("languageid"),
                                       reader.GetFieldValue<string[]>("question")),
                                   Category = new Nomenclature
                                              {
                                                  Id = reader.GetFieldValue<Guid>("faqcategoryid"),
                                                  Name = reader.GetFieldValue<string>("faqcategoryname")
                                              },
                                   RegDate = reader.GetFieldValue<DateTime?>("regdate"),
                                   UpdateDate = reader.GetFieldValue<DateTime?>("updatedate"),
                                   Status = new Nomenclature
                                            {
                                                Id = reader.GetFieldValue<Guid>("faqstatusid"),
                                                Name = reader.GetFieldValue<string>("faqstatusname")
                                            },
                                   ScheduledArchiveDate = reader.GetFieldValue<DateTime?>("schedulearchivedate"),
                                   ActualArchiveDate = reader.GetFieldValue<DateTime?>("actualarchivedate")
                               };
                    }
                }
            }

            return null;
        }

        public FaqCategory GetFaqCategory(Guid id, Guid? languageId)
        {
            using (var command = Context.Connection.GenerateCommand(
                "ais.get_faqcategory",
                new
                {
                    id,
                    languageid = languageId
                }))
            {
                using (var reader = command.ExecuteReaderSafety())
                {
                    while (reader.Read())
                    {
                        return new FaqCategory
                               {
                                   Id = reader.GetFieldValue<Guid>("id"),
                                   Names = CultureHelper.GetDictionaryData(
                                       reader.GetFieldValue<Guid[]>("languageid"),
                                       reader.GetFieldValue<string[]>("name")),
                                   IsValid = reader.GetFieldValue<bool>("isvalid")
                               };
                    }
                }
            }

            return null;
        }

        public List<FaqTableModel> Search(FaqQueryModel query)
        {
            var result = new List<FaqTableModel>();

            using (var command = Context.Connection.GenerateCommand(
                "ais.search_faq",
                new
                {
                    languageid = query.LanguageId,
                    fromdate = query.From,
                    todate = query.To,
                    faqcategoryid = query.Category?.Id,
                    faqstatusid = query.Status,
                    word = query.SearchWord
                }))
            {
                using (var reader = command.ExecuteReaderSafety())
                {
                    while (reader.Read())
                    {
                        result.Add(
                            new FaqTableModel
                            {
                                Id = reader.GetFieldValue<Guid?>("id"),
                                Question = reader.GetFieldValue<string>("question"),
                                Answer = reader.GetFieldValue<string>("answer"),
                                RegDate = reader.GetFieldValue<DateTime?>("regdate"),
                                UpdateDate = reader.GetFieldValue<DateTime?>("updatedate"),
                                Category = new Nomenclature
                                           {
                                               Id = reader.GetFieldValue<Guid>("faqcategoryid"),
                                               Name = reader.GetFieldValue<string>("faqcategoryname")
                                           },
                                Status = new Nomenclature
                                         {
                                             Id = reader.GetFieldValue<Guid>("faqstatusid"),
                                             Name = reader.GetFieldValue<string>("faqstatusname")
                                         }
                            });
                    }
                }
            }

            return result;
        }

        public List<FaqCategory> SearchFaqCategories(Guid languageId, bool isValid = true)
        {
             var result = new List<FaqCategory>();

             using (var command = Context.Connection.GenerateCommand(
                "ais.search_faqcategory",
                new
                {
                    languageid = languageId,
                    isvalid = isValid
                }))
            {
                using (var reader = command.ExecuteReaderSafety())
                {
                    while (reader.Read())
                    {
                        result.Add(
                            new FaqCategory
                            {
                                Id = reader.GetFieldValue<Guid?>("id"),
                                Names = new SortedDictionary<string, string>
                                         {
                                             {
                                                 languageId.ToString(), reader.GetFieldValue<string>("name")
                                             }
                                         },
                                IsValid = reader.GetFieldValue<bool>("isvalid"),
                                HasQuestions = reader.GetFieldValue<bool>("hasquestions")
                            });
                    }
                }
            }

             return result;
        }

        public void Upsert(Faq model)
        {
            using (var command = Context.Connection.GenerateCommand(
                "ais.upsert_faq",
                model,
                new Dictionary<string, object>
                {
                    { "pfaqcategoryid", model.Category.Id },
                    { "pfaqstatusid", model.Status.Id },
                    { "pschedulearchivedate", model.ScheduledArchiveDate },
                    { "planguageid", model.Questions.Keys.Select(Guid.Parse).ToArray() },
                    { "pquestion", model.Questions.Values },
                    { "panswer", model.Answers.Values },
                }))
            {
                command.ExecuteNonQuerySafety();
                model.Id = (Guid)command.Parameters["pid"].Value;
            }
        }

        public Guid UpsertFaqCategory(FaqCategory category)
        {
            using (var command = Context.Connection.GenerateCommand(
                "ais.upsert_faqcategory",
                category,
                new Dictionary<string, object>
                {
                    { "planguageid", category.Names.Keys.Select(Guid.Parse).ToArray() },
                    { "pname", category.Names.Values }
                }))
            {
                command.ExecuteNonQuerySafety();
                return (Guid)command.Parameters["pid"].Value;
            }
        }
    }
}
