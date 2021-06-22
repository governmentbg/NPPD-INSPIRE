namespace Inspire.Table.Mvc.Utilities.Export.Xml
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.Serialization;

    using Inspire.Core.Infrastructure.Attribute;

    using Inspire.Utilities.Extensions;

    public class XmlTableExport : TableExport
    {
        protected override byte[] Export<T>(IEnumerable<T> table, IEnumerable<KeyValuePair<PropertyInfo, TableOptionsAttribute>> tableColumns)
        {
            return new XmlExport<T> { Data = table as List<T> ?? table.ToList() }.Serialize();
        }

        [DataContract(Name = "ExportData", Namespace = "")]
        internal class XmlExport<T>
        {
            [DataMember]
            public DateTime CreateDate { get; set; } = DateTime.Now;

            [DataMember]
            public IEnumerable<T> Data { get; set; }
        }
    }
}
