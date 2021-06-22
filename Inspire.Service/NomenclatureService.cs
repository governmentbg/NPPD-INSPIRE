namespace Inspire.Services
{
    using System.Collections.Generic;

    using AutoMapper;

    using Inspire.Core.Infrastructure.RequestData;
    using Inspire.Domain.Repositories;
    using Inspire.Domain.Services;
    using Inspire.Model.Nomenclature;

    public class NomenclatureService : BaseService, INomenclatureService
    {
        private readonly INomenclatureRepository nomenclatureRepository;

        public NomenclatureService(
            IMapper mapper,
            IRequestData requestData,
            INomenclatureRepository nomenclatureRepository)
            : base(mapper, requestData)
        {
            this.nomenclatureRepository = nomenclatureRepository;
        }

        public List<Nomenclature> Get(string name, bool? isVisible = true, int? flag = null)
        {
            return nomenclatureRepository.Get(name, RequestData.LanguageId, isVisible, flag);
        }
    }
}