namespace Inspire.Model.Base
{
    using System;

    using Inspire.Model.User;

    public class TransactionDbData
    {
        public virtual DateTime? Date { get; set; }

        public virtual IUser User { get; set; }
    }
}