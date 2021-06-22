namespace Inspire.Services
{
    using System.Collections.Generic;

    using AutoMapper;

    using Inspire.Core.Infrastructure.RequestData;
    using Inspire.Domain.Repositories;
    using Inspire.Domain.Services;
    using Inspire.Model.Search;

    public class SearchService : BaseService, ISearchService
    {
        private readonly ISearchRepository searchRepository;

        public SearchService(IMapper mapper, IRequestData requestData, ISearchRepository searchRepository)
            : base(mapper, requestData)
        {
            this.searchRepository = searchRepository;
        }

        public List<SearchItem> Search(string query)
        {
            return searchRepository.Search(query, RequestData.LanguageId);
        }
    }
}