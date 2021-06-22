namespace Inspire.Repository
{
    using System;
    using System.Collections.Generic;

    using Inspire.Core.Infrastructure.Context;
    using Inspire.Data.Repositories;
    using Inspire.Data.Utilities;
    using Inspire.Domain.Repositories;
    using Inspire.Repository.Utilities;
    using Inspire.Utilities.Extensions;

    using Action = Inspire.Model.History.Action;

    public class HistoryRepository : BaseRepository, IHistoryRepository
    {
        public HistoryRepository(IAisContext context)
            : base(context)
        {
        }

        public List<Action> GetObjectHistory(Guid? objectId, Guid objectType, Guid language)
        {
            var result = new List<Action>();
            using (var command = Context.Connection.GenerateCommand(
                "ais.get_changelog",
                new
                {
                    objecttypelogid = objectType,
                    objectid = objectId,
                    languageid = language
                }))
            {
                using (var reader = command.ExecuteReaderSafety())
                {
                    while (reader.Read())
                    {
                        result.Add(
                            new Action
                            {
                                Name = reader.GetFieldValue<string>("actiontype"),
                                Date = reader.GetFieldValue<DateTime>("actiondate"),
                                Reason = reader.GetFieldValue<string>("reason"),
                                UserId = reader.GetFieldValue<Guid?>("userid"),
                                UserName = reader.GetFieldValue<string>("username")
                            });
                    }
                }
            }

            return result;
        }
    }
}