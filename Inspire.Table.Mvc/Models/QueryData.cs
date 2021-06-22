namespace Inspire.Table.Mvc.Models
{
    using System.Collections.Generic;

    using Kendo.Mvc.UI;

    public class QueryData<TQueryModel, TViewTableModel>
        where TQueryModel : class
        where TViewTableModel : class, new()
    {
        public TQueryModel QueryModel { get; set; }

        public IEnumerable<TViewTableModel> FindResult { get; set; }

        public HashSet<TViewTableModel> SelectedItems { get; set; }

        public DataSourceRequest DataSourceRequest { get; set; }
    }
}