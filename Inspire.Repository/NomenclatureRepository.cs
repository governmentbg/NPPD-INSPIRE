namespace Inspire.Repository
{
    using System;
    using System.Collections.Generic;

    using Inspire.Core.Infrastructure.Cache;
    using Inspire.Core.Infrastructure.Context;
    using Inspire.Core.Infrastructure.Logger;
    using Inspire.Data.Repositories;
    using Inspire.Data.Utilities;
    using Inspire.Domain.Repositories;
    using Inspire.Model.Nomenclature;
    using Inspire.Repository.Utilities;
    using Inspire.Utilities.Extensions;

    public class NomenclatureRepository : CacheRepository, INomenclatureRepository
    {
        public NomenclatureRepository(IAisContext context, ICacheService cacheService, ILogger logger)
            : base(context, cacheService, logger)
        {
        }

        public List<Nomenclature> Get(string tableName, Guid? languageId, bool? isVisible, int? flag = null)
        {
            using (var command = Context.Connection.GenerateCommand(
                "admdata.get_ntables",
                new
                {
                    tableName,
                    languageId,
                    isVisible,
                    flag
                }))
            {
                return GetOrSetCache(
                    command,
                    dbCommand =>
                    {
                        var result = new List<Nomenclature>();
                        using (var reader = dbCommand.ExecuteReaderSafety())
                        {
                            while (reader.Read())
                            {
                                result.Add(
                                    new Nomenclature
                                    {
                                        Id = reader.GetFieldValue<Guid?>("id"),
                                        Name = reader.GetFieldValue<string>("name")
                                    });
                            }
                        }

                        return result;
                    });
            }
        }
    }
}