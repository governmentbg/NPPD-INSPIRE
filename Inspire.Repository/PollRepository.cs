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
    using Inspire.Model.Poll;
    using Inspire.Model.QueryModels;
    using Inspire.Model.TableModels;
    using Inspire.Repository.Utilities;
    using Inspire.Utilities.Extensions;

    public class PollRepository : BaseRepository, IPollRepository
    {
        public PollRepository(IAisContext context)
            : base(context)
        {
        }

        public void DeletePoll(Guid pollId)
        {
            using (var command = Context.Connection.GenerateCommand(
                "ais.del_questionnaire",
                new { id = pollId }))
            {
                command.ExecuteNonQuerySafety();
            }
        }

        public void DeleteQuestions(Guid[] questionIds)
        {
            using (var command = Context.Connection.GenerateCommand(
                "ais.del_questions",
                new { id = questionIds }))
            {
                command.ExecuteNonQuerySafety();
            }
        }

        public Poll GetPoll(Guid pollId, Guid? languageId)
        {
            var result = new Poll();

            using (var command = Context.Connection.GenerateCommand(
                "ais.get_questionnaire",
                new
                {
                    id = pollId,
                    languageid = languageId,
                }))
            {
                using (var reader = command.ExecuteReaderSafety())
                {
                    while (reader.Read())
                    {
                        result = new Poll
                        {
                            Id = reader.GetFieldValue<Guid>("id"),
                            Status = new Nomenclature
                            {
                                Id = reader.GetFieldValue<Guid>("statusid")
                            },
                            ValidFrom = reader.GetFieldValue<DateTime>("validfrom"),
                            ValidTo = reader.GetFieldValue<DateTime>("validto"),
                            RegDate = reader.GetFieldValue<DateTime>("regdate"),
                            LastUpdate = reader.GetFieldValue<DateTime>("upddate"),
                            Author = reader.GetFieldValue<string>("regusername"),
                            Titles = CultureHelper.GetDictionaryData(
                                        reader.GetFieldValue<Guid[]>("languageid"),
                                        reader.GetFieldValue<string[]>("title")),
                            Descriptions = CultureHelper.GetDictionaryData(
                                        reader.GetFieldValue<Guid[]>("languageid"),
                                        reader.GetFieldValue<string[]>("description"))
                        };
                    }
                }
            }

            return result;
        }

        public List<Question> GetPollQuestions(Guid pollId, Guid? langugageId, Guid? questionId = null)
        {
            var result = new List<Question>();

            using (var command = Context.Connection.GenerateCommand(
                "ais.get_questions",
                new
                {
                    questionnaireid = pollId,
                    languageid = langugageId,
                    id = questionId
                }))
            {
                using (var reader = command.ExecuteReaderSafety())
                {
                    while (reader.Read())
                    {
                        var question = new Question
                        {
                            Id = reader.GetFieldValue<Guid?>("id"),
                            Type = new Nomenclature
                            {
                                Id = reader.GetFieldValue<Guid>("typeid")
                            },
                            Mandatory = reader.GetFieldValue<bool>("isrequired"),
                            Titles = CultureHelper.GetDictionaryData(
                                               reader.GetFieldValue<Guid[]>("languageid"),
                                               reader.GetFieldValue<string[]>("title")),
                            Descriptions = CultureHelper.GetDictionaryData(
                                               reader.GetFieldValue<Guid[]>("languageid"),
                                               reader.GetFieldValue<string[]>("description")),
                        };

                        var optionIds = reader.GetFieldValue<Guid[]>("vvalueids");
                        var optionValues = reader.GetFieldValue<string[]>("vvalues")?.ToList();
                        var langs = reader.GetFieldValue<Guid[]>("languageid");

                        if (optionIds.IsNotNullOrEmpty() &&
                            optionValues.IsNotNullOrEmpty())
                        {
                            question.Options = new List<Option>();

                            foreach (var id in optionIds)
                            {
                                var opt = new Option
                                          {
                                              Id = id,
                                              Values = new SortedDictionary<string, string>()
                                          };

                                for (var i = 0; i < langs.Length; i++)
                                {
                                    opt.Values.Add(langs[i].ToString(), optionValues[0]);
                                    optionValues.RemoveAt(0);
                                }

                                question.Options.Add(opt);
                            }
                        }

                        result.Add(question);
                    }
                }
            }

            return result;
        }

        public void InsertQuestionsOrder(Guid[] ids)
        {
            using (var command = Context.Connection.GenerateCommand(
                "ais.upd_questionorder",
                new { ids }))
            {
                command.ExecuteNonQuerySafety();
            }
        }

        public List<PollTableModel> Search(PollQueryModel query)
        {
            var result = new List<PollTableModel>();

            using (var command = Context.Connection.GenerateCommand(
                "ais.search_questionnaire",
                new
                {
                    languageid = query.LanguageId,
                    statusid = query.Status,
                }))
            {
                using (var reader = command.ExecuteReaderSafety())
                {
                    while (reader.Read())
                    {
                        result.Add(
                            new PollTableModel
                            {
                                Id = reader.GetFieldValue<Guid?>("id"),
                                Title = reader.GetFieldValue<string>("title"),
                                Description = reader.GetFieldValue<string>("description"),
                                Status = new Nomenclature
                                         {
                                             Id = reader.GetFieldValue<Guid>("statusid"),
                                             Name = reader.GetFieldValue<string>("statusname")
                                         },
                                ValidFrom = reader.GetFieldValue<DateTime?>("validfrom"),
                                ValidTo = reader.GetFieldValue<DateTime?>("validto"),
                                RegDate = reader.GetFieldValue<DateTime?>("regdate"),
                                Author = reader.GetFieldValue<string>("regusername")
                            });
                    }
                }
            }

            return result;
        }

        public void UpdateDates(Poll poll)
        {
            using (var command = Context.Connection.GenerateCommand(
                "ais.upd_questionnairedates",
                new
                {
                    id = poll.Id,
                    validfrom = poll.ValidFrom,
                    validto = poll.ValidTo
                }))
            {
                command.ExecuteNonQuerySafety();
            }
        }

        public Guid InsertPollResponse(Guid pollId, Guid[] questionsId, string[] answersId, Guid languageId)
        {
            using (var command = Context.Connection.GenerateCommand(
                "ais.ins_qresponse",
                new
                {
                    questionnaireid = pollId,
                    questionid = questionsId,
                    answer = answersId,
                    languageid = languageId
                }))
            {
                command.ExecuteNonQuerySafety();
                return (Guid)command.Parameters["pid"].Value;
            }
        }

        public List<PollResult> GetPollResults(Guid pollId, Guid languageId)
        {
            var result = new List<PollResult>();
            using (var command = Context.Connection.GenerateCommand(
                "ais.get_qresults",
                new
                {
                    questionnaireid = pollId,
                    languageid = languageId
                }))
            {
                using (var reader = command.ExecuteReaderSafety())
                {
                    while (reader.Read())
                    {
                        result.Add(
                            new PollResult
                            {
                                Id = reader.GetFieldValue<Guid?>("id"),
                                RegDate = reader.GetFieldValue<DateTime>("regdate"),
                                Ip = reader.GetFieldValue<string>("ip"),
                                QuestionAnswers = reader.GetFieldValue<string[]>("questionanswer").ToList(),
                                QuestionTitles = reader.GetFieldValue<string[]>("questiontitle").ToList()
                            });
                    }
                }
            }

            return result;
        }

        public Guid Upsert(Poll poll)
        {
            using (var command = Context.Connection.GenerateCommand(
                "ais.upsert_questionnaire",
                poll,
                new Dictionary<string, object>
                {
                    { "pvalidfrom", poll.ValidFrom },
                    { "pvalidto", poll.ValidTo },
                    { "planguageid", poll.Titles.Keys.Select(Guid.Parse).ToArray() },
                    { "ptitle", poll.Titles.Values },
                    { "pdescription", poll.Descriptions.Values },
                }))
            {
                command.ExecuteNonQuerySafety();
                return (Guid)command.Parameters["pid"].Value;
            }
        }

        public Guid UpsertQuestion(Question question)
        {
            var values = new List<string>();
            var languages = question.Titles.Keys.Select(Guid.Parse).ToArray();

            if (question.Options.IsNotNullOrEmpty())
            {
                foreach (var language in languages)
                {
                    foreach (var option in question.Options)
                    {
                        values.Add(option.Values[language.ToString()]);
                    }
                }
            }

            using (var command = Context.Connection.GenerateCommand(
                "ais.upsert_question",
                question,
                new Dictionary<string, object>
                {
                    { "pquestionnaireid", question.PollId },
                    { "ptypeid", question.Type.Id },
                    { "pisrequired", question.Mandatory },
                    { "planguageid", question.Titles.Keys.Select(Guid.Parse).ToArray() },
                    { "ptitle", question.Titles.Values },
                    { "pdescription", question.Descriptions.Values },
                    { "pvalues", values },
                }))
            {
                command.ExecuteNonQuerySafety();
                return (Guid)command.Parameters["pid"].Value;
            }
        }
    }
}
