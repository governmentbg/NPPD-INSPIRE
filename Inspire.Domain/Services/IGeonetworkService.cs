namespace Inspire.Domain.Services
{
    using System.Collections.Generic;

    using Inspire.Model.TableModels;

    public interface IGeonetworkService
    {
        List<PriorityMetadataTableModel> GetPriorityData();

        List<NonPriorityMetadataTableModel> GetNonPriorityData();
    }
}