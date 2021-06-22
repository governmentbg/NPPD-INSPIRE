namespace Inspire.Model.Base
{
    using System;
    using System.Runtime.Serialization;

    using Inspire.Core.Infrastructure.Attribute;

    [DataContract]
    public abstract class BaseDbModel : IBaseDbModel
    {
        private string uniqueId;

        public virtual Guid? Id { get; set; }

        [Ignore]
        public virtual string UniqueId
        {
            get => uniqueId ?? (uniqueId = Guid.NewGuid().ToString());

            set => uniqueId = value;
        }
    }
}