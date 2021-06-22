namespace Inspire.Domain.Services
{
    using System;
    using System.Collections.Generic;

    using Inspire.Model.Poll;
    using Inspire.Model.QueryModels;
    using Inspire.Model.TableModels;

    public interface IPollService
    {
        Guid Upsert(Poll poll);

        Guid UpsertQuestion(Question question);

        void InsertQuestionsOrder(Guid[] ids);

        List<PollTableModel> Search(PollQueryModel query);

        Poll GetPoll(Guid pollId, bool passLanguage = true);

        List<Question> GetPollQuestions(Guid pollId, bool passLanguage = true, Guid? questionId = null);

        void DeleteQuestions(Guid[] questionIds);

        void DeletePoll(Guid pollId);

        void UpdateDates(Poll poll);

        Guid InsertPollResponse(Guid pollId, Guid[] questionsId, string[] answersId);

        List<PollResult> GetPollResults(Guid pollId);
    }
}
