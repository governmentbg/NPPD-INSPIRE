namespace Inspire.Model.QueryModels
{
    using System;

    public class UserLoginQueryModel
    {
        public bool IsFromGN { get; set; }

        public string Username { get; set; }

        public DateTime? LoginDateFrom { get; set; }

        public DateTime? LoginDateTo { get; set; }

        public string LoginIp { get; set; }
    }
}
