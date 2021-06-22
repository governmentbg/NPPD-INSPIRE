namespace Inspire.Domain.Repositories
{
    using System;
    using System.Collections.Generic;

    using Inspire.Model.Search;

    public interface ISearchRepository
    {
        List<SearchItem> Search(string query, Guid language);
    }
}