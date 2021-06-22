namespace Inspire.Portal.Infrastructure.RequestData
{
    using System;
    using System.Web;

    using Inspire.Core.Infrastructure.Membership;
    using Inspire.Core.Infrastructure.RequestData;
    using Inspire.Portal.Utilities;

    public class RequestData : IRequestData
    {
        private readonly IUserPrincipal user;

        public RequestData()
        {
            user = HttpContext.Current?.User as IUserPrincipal;
        }

        public RequestData(IUserPrincipal user)
        {
            this.user = user;
        }

        public string Host => Address;

        public string Address
        {
            get
            {
                var ip = Request?.ServerVariables["HTTP_X_FORWARDED_FOR"];
                return ip == null
                    ? Request?.ServerVariables["REMOTE_ADDR"]
                    : ip.Split(',')[0];
            }
        }

        public string Browser => Request?.UserAgent;

        public Guid LanguageId => Guid.Parse(LocalizationHelper.GetCurrentCultureId());

        public Guid? UserId => user?.Id ?? ConfigurationReader.AutomationUserId;

        public string UserName => user?.UserName ?? "AutomationUser";

        private HttpRequest Request => HttpContext.Current?.Request;
    }
}