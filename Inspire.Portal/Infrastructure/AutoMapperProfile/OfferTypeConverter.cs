namespace Inspire.Portal.Infrastructure.AutoMapperProfile
{
    using System;
    using System.Linq;

    using AutoMapper;

    using Inspire.Model.Offer;
    using Inspire.Portal.Models.Address;
    using Inspire.Portal.Models.Offer;

    internal class OfferTypeConverter : ITypeConverter<Offer, OfferViewModel>
    {
        public OfferViewModel Convert(Offer source, OfferViewModel destination, ResolutionContext context) => new OfferViewModel
        {
            Id = source.Id,
            Address = new AddressViewModel
            {
                NormalizedAddress = source.OfferObject?.Address?.NormalizedAddress,
                Shape = source.OfferObject?.Address?.Shape
            },
            Title = source.Title,
            BathsCount = source.OfferObject?.BathsCount ?? 0,
            BuildingName = source.OfferObject?.Building?.Name,
            BuiltYear = source.OfferObject?.Building?.BuiltYear ?? 0,
            Client = new OfferClientViewModel
            {
                Id = source.Client?.Id ?? 0,
                Name = source.Client?.Name
            },
            CommonParts = source.OfferObject?.CommonParts ?? 0,
            CompletionStage = source.OfferObject.CompletionStage != null
                                                 ? context.Mapper.Map<CompletionStageViewModel>(source.OfferObject.CompletionStage)
                                                 : null,
            Construction = source.OfferObject?.Building?.Construction != null
                                              ? context.Mapper.Map<ConstructionViewModel>(source.OfferObject.Building.Construction)
                                              : null,
            Agent = source.Agents?.FirstOrDefault()?.FullName,
            Content = source.Content,
            ExposureIds = source.OfferObject?.Exposure?.Select(item => item.Id).ToList(),
            ExtrasIds = source.OfferObject?.Extras?.Select(item => item.Id).ToList(),
            Floor = source.OfferObject?.Floor ?? 0,
            HasElevator = (bool)source.OfferObject?.HasElevator,
            Furniture = source.OfferObject?.Furniture != null
                            ? context.Mapper.Map<FurnitureViewModel>(source.OfferObject.Furniture)
                            : null,
            HeatingIds = source.OfferObject?.Heating?.Select(item => item.Id).ToList(),
            Insulation = source.OfferObject?.Insulation != null
                             ? context.Mapper.Map<InsulationViewModel>(source.OfferObject.Insulation)
                             : null,
            IsExclusiveOffer = source.IsExclusiveOffer,
            IsSold = source.IsSold,
            MaintenanceFee = source.OfferObject?.MaintenanceFee ?? 0,
            Neighbourhood = source.OfferObject?.Address?.Neighbourhood != null
                                ? context.Mapper.Map<NeighbourhoodViewModel>(source.OfferObject.Address.Neighbourhood)
                                : null,
            Number = source.Number,
            OfferType = source.OfferType != null
                            ? context.Mapper.Map<OfferTypeViewModel>(source.OfferType)
                            : null,
            OthersIds = source.OfferObject?.Others?.Select(item => item.Id).ToList(),
            ParkingIds = source.OfferObject?.Parking?.Select(item => item.Id).ToList(),
            PriceWithVAT = source.Price?.PriceWithVAT ?? 0,
            PriceWithoutVAT = source.Price?.PriceWithoutVAT ?? 0,
            PropertyType = source.OfferObject?.PropertyType != null
                               ? context.Mapper.Map<PropertyTypeViewModel>(source.OfferObject.PropertyType)
                               : null,
            PureArea = source.OfferObject?.PureArea ?? 0,
            Region = source.OfferObject?.Address?.Region != null
                         ? context.Mapper.Map<RegionViewModel>(source.OfferObject.Address.Region)
                         : null,
            RoomsCount = source.OfferObject?.RoomsCount ?? 0,
            Settlement = source.OfferObject?.Address?.Settlement != null
                             ? context.Mapper.Map<SettlementViewModel>(source.OfferObject.Address.Settlement)
                             : null,
            TerraceArea = source.OfferObject?.TerraceArea ?? 0,
            WindowFrame = source.OfferObject?.WindowFrame != null
                              ? context.Mapper.Map<WindowFrameViewModel>(source.OfferObject.WindowFrame)
                              : null,
            RegDate = source.RegTransactionData?.Date ?? DateTime.MinValue,
        };
    }
}