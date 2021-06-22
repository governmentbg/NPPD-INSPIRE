namespace Inspire.Portal.Infrastructure.AutoMapperProfile
{
    using System.Collections.Generic;
    using System.Linq;

    using AutoMapper;

    using Inspire.Model.Address;
    using Inspire.Model.Agent;
    using Inspire.Model.Client;
    using Inspire.Model.Nomenclature;
    using Inspire.Model.Offer;
    using Inspire.Portal.Models.Offer;
    using Inspire.Utilities.Extensions;

    internal class OfferViewModelTypeConverter : ITypeConverter<OfferViewModel, Offer>
    {
        public Offer Convert(OfferViewModel source, Offer destination, ResolutionContext context)
        {
            return new Offer
                       {
                           Id = source.Id,
                           Number = source.Number,
                           Title = source.Title,
                           Content = source.Content,
                           IsSold = source.IsSold,
                           IsExclusiveOffer = source.IsExclusiveOffer,
                           OfferType = new Nomenclature
                                           {
                                               Id = source.OfferType.Id,
                                               Name = source.OfferType.Name
                                           },
                           Price = new Price
                                       {
                                           PriceWithoutVAT = source.PriceWithoutVAT,
                                           PriceWithVAT = source.PriceWithVAT
                                       },
                           Client = new Client
                                        {
                                            Id = source.Client?.Id,
                                            Name = source.Client?.Name
                                        },
                           Agents = new List<Agent>(new[] { new Agent { FullName = source.Agent } }),
                           OfferObject = new OfferObject
                                             {
                                                 Address = new Address
                                                               {
                                                                   NormalizedAddress =
                                                                       source.Address?.NormalizedAddress,
                                                                   Neighbourhood = source.Neighbourhood != null
                                                                                       ? context
                                                                                           .Mapper.Map<Nomenclature>(
                                                                                               source.Neighbourhood)
                                                                                       : null,
                                                                   Region = source.Neighbourhood != null
                                                                                ? context.Mapper.Map<Nomenclature>(
                                                                                    source.Region)
                                                                                : null,
                                                                   Settlement = source.Settlement != null
                                                                                    ? context.Mapper.Map<Nomenclature>(
                                                                                        source.Settlement)
                                                                                    : null,
                                                                   Shape = source.Address?.Shape
                                                               },
                                                 BathsCount = source.BathsCount,
                                                 Building = new Building
                                                                {
                                                                    Name = source.BuildingName,
                                                                    BuiltYear = source.BuiltYear,
                                                                    Construction = source.Construction != null
                                                                                       ? context
                                                                                           .Mapper.Map<Nomenclature>(
                                                                                               source.Construction)
                                                                                       : null
                                                                },
                                                 CommonParts = source.CommonParts,
                                                 PureArea = source.PureArea,
                                                 RoomsCount = source.RoomsCount,
                                                 TerraceArea = source.TerraceArea,
                                                 Floor = source.Floor,
                                                 HasElevator = source.HasElevator,
                                                 MaintenanceFee = source.MaintenanceFee,
                                                 CompletionStage = source.CompletionStage != null
                                                                       ? context.Mapper.Map<Nomenclature>(
                                                                           source.CompletionStage)
                                                                       : null,
                                                 Exposure = source.ExposureIds.IsNotNullOrEmpty()
                                                                ? source.ExposureIds.Select(
                                                                            item => new Nomenclature { Id = item })
                                                                        .ToList()
                                                                : null,
                                                 Extras = source.ExposureIds.IsNotNullOrEmpty()
                                                              ? source.ExposureIds.Select(
                                                                          item => new Nomenclature { Id = item })
                                                                      .ToList()
                                                              : null,
                                                 Heating = source.HeatingIds.IsNotNullOrEmpty()
                                                               ? source.HeatingIds.Select(
                                                                           item => new Nomenclature { Id = item })
                                                                       .ToList()
                                                               : null,
                                                 Parking = source.ParkingIds.IsNotNullOrEmpty()
                                                               ? source.ParkingIds.Select(
                                                                           item => new Nomenclature { Id = item })
                                                                       .ToList()
                                                               : null,
                                                 Others = source.OthersIds.IsNotNullOrEmpty()
                                                              ? source.OthersIds.Select(
                                                                          item => new Nomenclature { Id = item })
                                                                      .ToList()
                                                              : null,
                                                 Furniture = source.Furniture != null
                                                                 ? context.Mapper.Map<Nomenclature>(source.Furniture)
                                                                 : null,
                                                 Insulation = source.Insulation != null
                                                                  ? context.Mapper.Map<Nomenclature>(source.Insulation)
                                                                  : null,
                                                 PropertyType = source.PropertyType != null
                                                                    ? context.Mapper.Map<Nomenclature>(
                                                                        source.PropertyType)
                                                                    : null,
                                                 WindowFrame = source.WindowFrame != null
                                                                   ? context.Mapper.Map<Nomenclature>(
                                                                       source.WindowFrame)
                                                                   : null
                                             }
                       };
        }
    }
}