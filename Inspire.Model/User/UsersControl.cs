namespace Inspire.Model.User
{
    using System;
    using System.Collections.Generic;

    public class UsersControl
    {
        public Guid ItemId { get; set; }

        public Guid ItemType { get; set; }

        public List<UserControlItem> Items { get; set; }
    }
}