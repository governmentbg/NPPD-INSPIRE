namespace Inspire.Domain.Services
{
    using System.Collections.Generic;

    using Inspire.Model.Search;

    public interface ISearchService
    {
        List<SearchItem> Search(string query);
    }
}