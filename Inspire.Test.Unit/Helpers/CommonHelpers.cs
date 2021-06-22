namespace Inspire.Test.Unit.Helpers
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.SessionState;

    public class CommonHelpers
    {
        public static T GetValueFromJsonResult<T>(JsonResult jsonResult, string propertyName)
        {
            var property =
                jsonResult.Data.GetType().GetProperties()
                          .FirstOrDefault(p => string.Compare(p.Name, propertyName) == 0);

            if (property == null)
            {
                throw new ArgumentException("propertyName not found", "propertyName");
            }

            return (T)property.GetValue(jsonResult.Data, null);
        }

        public static HttpContext GetHttpContext()
        {
            // We need to setup the Current HTTP Context as follows:

            // Step 1: Setup the HTTP Request
            var httpRequest = new HttpRequest(string.Empty, "http://localhost/", string.Empty);

            // Step 2: Setup the HTTP Response
            var httpResponse = new HttpResponse(new StringWriter());

            // Step 3: Setup the Http Context
            var httpContext = new HttpContext(httpRequest, httpResponse);
            var sessionContainer =
                new HttpSessionStateContainer(
                    "id",
                    new SessionStateItemCollection(),
                    new HttpStaticObjectsCollection(),
                    10,
                    true,
                    HttpCookieMode.AutoDetect,
                    SessionStateMode.InProc,
                    false);
            httpContext.Items["AspSession"] =
                typeof(HttpSessionState)
                    .GetConstructor(
                        BindingFlags.NonPublic | BindingFlags.Instance,
                        null,
                        CallingConventions.Standard,
                        new[] { typeof(HttpSessionStateContainer) },
                        null)
                    .Invoke(new object[] { sessionContainer });
            return httpContext;
        }
    }
}