namespace Inspire.Services
{
    using System;
    using System.Collections.Generic;
    using AutoMapper;
    using Inspire.Core.Infrastructure.RequestData;
    using Inspire.Domain.Repositories;
    using Inspire.Domain.Services;
    using Inspire.Model.Poll;
    using Inspire.Model.QueryModels;
    using Inspire.Model.TableModels;
    using Inspire.Utilities.Extensions;

    public class PollService : BaseService, IPollService
    {
        private readonly IPollRepository pollRepository;

        public PollService(
            IMapper mapper,
            IRequestData requestData,
            IPollRepository pollRepository)
            : base(mapper, requestData)
        {
            this.pollRepository = pollRepository;
        }

        public void DeletePoll(Guid pollId)
        {
           pollRepository.DeletePoll(pollId);
        }

        public void DeleteQuestions(Guid[] questionIds)
        {
            pollRepository.DeleteQuestions(questionIds);
        }

        public Poll GetPoll(Guid pollId, bool passLanguage = true)
        {
            var poll = pollRepository.GetPoll(pollId, passLanguage ? new Guid?(RequestData.LanguageId) : null);
            poll.Questions = pollRepository.GetPollQuestions(poll.Id.Value, passLanguage ? new Guid?(RequestData.LanguageId) : null);
            return poll;
        }

        public List<Question> GetPollQuestions(Guid pollId, bool passLanguage = true, Guid? questionId = null)
        {
            return pollRepository.GetPollQuestions(pollId, passLanguage ? new Guid?(RequestData.LanguageId) : null, questionId);
        }

        public void InsertQuestionsOrder(Guid[] ids)
        {
            pollRepository.InsertQuestionsOrder(ids);
        }

        public List<PollTableModel> Search(PollQueryModel query)
        {
            query.LanguageId = RequestData.LanguageId;
            return pollRepository.Search(query);
        }

        public void UpdateDates(Poll poll)
        {
            pollRepository.UpdateDates(poll);
        }

        public Guid InsertPollResponse(Guid pollId, Guid[] questionsId, string[] answersId)
        {
            return pollRepository.InsertPollResponse(pollId, questionsId, answersId, RequestData.LanguageId);
        }

        public List<PollResult> GetPollResults(Guid pollId)
        {
            return pollRepository.GetPollResults(pollId, RequestData.LanguageId);
        }

        public Guid Upsert(Poll poll)
        {
            var id = pollRepository.Upsert(poll);
            var questionIds = new List<Guid>();
            if (poll.Questions.IsNotNullOrEmpty())
            {
                foreach (var dbModelQuestion in poll.Questions)
                {
                    dbModelQuestion.PollId = id;
                    questionIds.Add(pollRepository.UpsertQuestion(dbModelQuestion));
                }

                pollRepository.InsertQuestionsOrder(questionIds.ToArray());
            }

            return id;
        }

        public Guid UpsertQuestion(Question question)
        {
            return pollRepository.UpsertQuestion(question);
        }
    }
}
