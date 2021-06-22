namespace Inspire.Common.Mvc.Infrastructure.BaseViews
{
    using System.Diagnostics;
    using System.Globalization;
    using System.Threading;
    using System.Web.Mvc;

    using Inspire.Common.Mvc.Extensions;
    using Inspire.Core.Infrastructure.Membership;

    public abstract class BaseViewPage : WebViewPage
    {
        public string Area => ViewBag.Area as string;

        public new virtual IUserPrincipal User => base.User as IUserPrincipal;

        public CultureInfo CurrentCulture => Thread.CurrentThread.CurrentCulture;

        public FileVersionInfo ApplicationVersion => this.GetApplicationVersion();
    }
}