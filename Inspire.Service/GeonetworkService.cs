namespace Inspire.Services
{
    using System.Collections.Generic;

    using AutoMapper;

    using Inspire.Core.Infrastructure.RequestData;
    using Inspire.Domain.Repositories;
    using Inspire.Domain.Services;
    using Inspire.Model.TableModels;

    public class GeonetworkService : BaseService, IGeonetworkService
    {
        private readonly IGeonetworkRepository geonetworkRepository;

        public GeonetworkService(IMapper mapper, IRequestData requestData, IGeonetworkRepository geonetworkRepository)
            : base(mapper, requestData)
        {
            this.geonetworkRepository = geonetworkRepository;
        }

        public List<PriorityMetadataTableModel> GetPriorityData()
        {
            return geonetworkRepository.GetPriorityData();
        }

        public List<NonPriorityMetadataTableModel> GetNonPriorityData()
        {
            return geonetworkRepository.GetNonPriorityData();
        }
    }
}