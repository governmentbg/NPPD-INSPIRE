namespace Inspire.Domain.Services
{
    using System.Collections.Generic;

    using Inspire.Model.Nomenclature;

    public interface INomenclatureService
    {
        List<Nomenclature> Get(string name, bool? isVisible = true, int? flag = null);
    }
}