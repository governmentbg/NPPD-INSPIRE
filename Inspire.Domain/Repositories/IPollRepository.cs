namespace Inspire.Domain.Repositories
{
    using System;
    using System.Collections.Generic;

    using Inspire.Model.Poll;
    using Inspire.Model.QueryModels;
    using Inspire.Model.TableModels;

    public interface IPollRepository
    {
        Guid Upsert(Poll poll);

        Guid UpsertQuestion(Question question);

        void InsertQuestionsOrder(Guid[] ids);

        List<PollTableModel> Search(PollQueryModel query);

        Poll GetPoll(Guid pollId, Guid? languageId);

        List<Question> GetPollQuestions(Guid pollId, Guid? langugageId, Guid? questionId = null);

        void DeleteQuestions(Guid[] questionIds);

        void DeletePoll(Guid pollId);

        void UpdateDates(Poll poll);

        Guid InsertPollResponse(Guid pollId, Guid[] questionsId, string[] answersId, Guid languageId);

        List<PollResult> GetPollResults(Guid pollId, Guid languageId);
    }
}
