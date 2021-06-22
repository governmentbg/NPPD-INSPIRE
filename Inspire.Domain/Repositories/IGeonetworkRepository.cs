namespace Inspire.Domain.Repositories
{
    using System.Collections.Generic;

    using Inspire.Model.TableModels;

    public interface IGeonetworkRepository
    {
        List<PriorityMetadataTableModel> GetPriorityData();

        List<NonPriorityMetadataTableModel> GetNonPriorityData();
    }
}