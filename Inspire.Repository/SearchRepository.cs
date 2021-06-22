namespace Inspire.Repository
{
    using System;
    using System.Collections.Generic;

    using Inspire.Core.Infrastructure.Context;
    using Inspire.Data.Repositories;
    using Inspire.Data.Utilities;
    using Inspire.Domain.Repositories;
    using Inspire.Model.Search;
    using Inspire.Repository.Utilities;
    using Inspire.Utilities.Extensions;

    public class SearchRepository : BaseRepository, ISearchRepository
    {
        public SearchRepository(IAisContext context)
            : base(context)
        {
        }

        public List<SearchItem> Search(string query, Guid language)
        {
            var result = new List<SearchItem>();
            using (var command = Context.Connection.GenerateCommand(
                "ais.search_globalsearch",
                new
                {
                    searchtext = query,
                    languageid = language
                }))
            {
                using (var reader = command.ExecuteReaderSafety())
                {
                    while (reader.Read())
                    {
                        result.Add(
                            new SearchItem
                            {
                                Id = reader.GetFieldValue<Guid>("objectid"),
                                Type = reader.GetFieldValue<Guid>("objecttypeid"),
                                Name = reader.GetFieldValue<string>("name"),
                                Description = reader.GetFieldValue<string>("description")
                            });
                    }
                }
            }

            return result;
        }
    }
}