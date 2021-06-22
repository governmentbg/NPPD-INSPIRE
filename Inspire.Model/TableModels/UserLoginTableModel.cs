namespace Inspire.Model.TableModels
{
    using System;

    public class UserLoginTableModel
    {
        public string Username { get; set; }

        public DateTime? LoginDate { get; set; }

        public string LoginIp { get; set; }
    }
}
