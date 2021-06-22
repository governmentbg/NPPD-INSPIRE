namespace Inspire.Common.Mvc.Infrastructure.CustomResult
{
    using System.Web.Mvc;

    /// <summary>
    ///     A custom override of the JsonResult
    ///     to provide a public constructor allowing
    ///     to set custom values for some important
    ///     properties.
    /// </summary>
    public class JsonResultMaxLength : JsonResult
    {
        /// <summary>
        ///     Custom wrapper of the standart JsonResult class to expose public c-tor for some important settings
        /// </summary>
        /// <param name="data"> The data of the json</param>
        /// <param name="behaviour"> The Json request behaviour - defaults to AllowGet </param>
        /// <param name="length"> The max allowed length for the json - defaults to int.MaxValue if not specified </param>
        /// <param name="contentType"> The content type of the json - defaults to "application/json" </param>
        public JsonResultMaxLength(
            object data,
            JsonRequestBehavior behaviour = JsonRequestBehavior.AllowGet,
            int length = int.MaxValue,
            string contentType = "application/json")
        {
            Data = data;
            JsonRequestBehavior = behaviour;
            MaxJsonLength = length;
            ContentType = contentType;
        }
    }
}