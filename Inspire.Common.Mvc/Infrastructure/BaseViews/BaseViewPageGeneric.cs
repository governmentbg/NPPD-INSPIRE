#pragma warning disable SA1649 // SA1649FileNameMustMatchTypeName
namespace Inspire.Common.Mvc.Infrastructure.BaseViews
{
    using System.Diagnostics;
    using System.Globalization;
    using System.Threading;
    using System.Web.Mvc;

    using Inspire.Common.Mvc.Extensions;
    using Inspire.Core.Infrastructure.Membership;

    public abstract class BaseViewPage<TModel> : WebViewPage<TModel>
    {
        public string Area => ViewBag.Area as string;

        public new virtual IUserPrincipal User => base.User as IUserPrincipal;

        public CultureInfo CurrentCulture => Thread.CurrentThread.CurrentCulture;

        public FileVersionInfo ApplicationVersion => this.GetApplicationVersion();
    }
}
#pragma warning restore SA1649 // SA1649FileNameMustMatchTypeName