namespace Inspire.Domain.Repositories
{
    using System;
    using System.Collections.Generic;

    using Inspire.Model.Nomenclature;

    public interface INomenclatureRepository
    {
        List<Nomenclature> Get(string tableName, Guid? languageId, bool? isVisible = true, int? flag = null);
    }
}