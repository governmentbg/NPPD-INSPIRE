namespace Inspire.Model.History
{
    using System;

    public class Action
    {
        public string Name { get; set; }

        public DateTime Date { get; set; }

        public string Reason { get; set; }

        public Guid? UserId { get; set; }

        public string UserName { get; set; }
    }
}