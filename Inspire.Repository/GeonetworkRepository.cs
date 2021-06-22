namespace Inspire.Repository
{
    using System;
    using System.Collections.Generic;

    using Inspire.Core.Infrastructure.Context;
    using Inspire.Data.Repositories;
    using Inspire.Data.Utilities;
    using Inspire.Domain.Repositories;
    using Inspire.Model.TableModels;
    using Inspire.Repository.Utilities;
    using Inspire.Utilities.Extensions;

    public class GeonetworkRepository : BaseRepository, IGeonetworkRepository
    {
        public GeonetworkRepository(IAisContext context)
            : base(context)
        {
        }

        public List<PriorityMetadataTableModel> GetPriorityData()
        {
            var result = new List<PriorityMetadataTableModel>();
            using (var command = Context.Connection.GenerateCommand("ais.report_priority"))
            {
                using (var reader = command.ExecuteReaderSafety())
                {
                    while (reader.Read())
                    {
                        Guid? metadataGuid = null;
                        var id = reader.GetFieldValue<string>("id");
                        if (id.IsNotNullOrEmpty() && Guid.TryParse(id,  out var uuid))
                        {
                            metadataGuid = uuid;
                        }

                        result.Add(
                            new PriorityMetadataTableModel
                            {
                                DirectiveCode = reader.GetFieldValue<string>("code"),
                                DirectiveTitle = reader.GetFieldValue<string>("dataset_bg"),
                                DirectiveOrganization = reader.GetFieldValue<string>("organization_daeu"),
                                Organization = reader.GetFieldValue<string>("organization_nppd"),
                                MetadataGuid = metadataGuid,
                                InspirePriorityDataSet = reader.GetFieldValue<string>("inspirepriority"),
                                MetadataTitle = reader.GetFieldValue<string>("title"),
                                IsMetadataValid = reader.GetFieldValue<bool?>("isvalid_nppd"),
                                IsMetadataEUValid = reader.GetFieldValue<bool?>("isvalid_eu")
                            });
                    }
                }
            }

            return result;
        }

        public List<NonPriorityMetadataTableModel> GetNonPriorityData()
        {
            var result = new List<NonPriorityMetadataTableModel>();
            using (var command = Context.Connection.GenerateCommand("ais.report_nonpriority"))
            {
                using (var reader = command.ExecuteReaderSafety())
                {
                    while (reader.Read())
                    {
                        Guid? metadataGuid = null;
                        var id = reader.GetFieldValue<string>("id");
                        if (id.IsNotNullOrEmpty() && Guid.TryParse(id,  out var uuid))
                        {
                            metadataGuid = uuid;
                        }

                        result.Add(
                            new NonPriorityMetadataTableModel
                            {
                                MetadataGuid = metadataGuid,
                                MetadataTitle = reader.GetFieldValue<string>("title"),
                                MetadataTheme = reader.GetFieldValue<string>("theme"),
                                IsMetadataValid = reader.GetFieldValue<bool?>("isvalid_nppd"),
                                IsMetadataEUValid = reader.GetFieldValue<bool?>("isvalid_eu")
                            });
                    }
                }
            }

            return result;
        }
    }
}