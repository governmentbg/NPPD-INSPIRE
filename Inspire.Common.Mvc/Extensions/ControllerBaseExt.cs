namespace Inspire.Common.Mvc.Extensions
{
    using System;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;

    using Inspire.Common.Mvc.Enums;

    public static class ControllerBaseExt
    {
        public static ActionResult RedirectToRequestPage(this ControllerBase controller)
        {
            return new RedirectResult(controller.GetRequestPageAbsoluteUrl());
        }

        public static string GetRequestPageAbsoluteUrl(this ControllerBase controller)
        {
            return controller.ControllerContext.HttpContext.Request.UrlReferrer?.AbsoluteUri ?? "~/";
        }

        public static void ShowMessage(this ControllerBase controller, MessageType messageType, string message)
        {
            var messageTypeKey = messageType.ToString();
            controller.TempData[messageTypeKey] = message;
        }

        public static void ClearOldMessages(this ControllerBase controller)
        {
            foreach (var messageTypeKey in Enum.GetNames(typeof(MessageType))
                                               .Where(
                                                   messageTypeKey => controller.TempData.ContainsKey(messageTypeKey)))
            {
                controller.TempData.Remove(messageTypeKey);
            }
        }

        public static void SendMessageData(
            this ControllerBase controller,
            HttpContextBase httpContext,
            bool isResultRedirect = false)
        {
            var messageData = controller.TempData.Where(item => Enum.GetNames(typeof(MessageType)).Contains(item.Key))
                                        .ToDictionary(x => x.Key, y => y.Value);
            foreach (var messageType in messageData)
            {
                if (messageType.Value == null || string.IsNullOrEmpty(messageType.Value.ToString()))
                {
                    continue;
                }

                // Check request type
                if (httpContext.Request.IsAjaxRequest())
                {
                    // Get response and check headers
                    var response = httpContext.Response;
                    if (response.Headers["X-Redirect-Force"] == null && response.Headers["X-Message-Type"] == null)
                    {
                        response.AddHeader("X-Message-Type", messageType.Key.ToLower());
                        response.AddHeader("X-Message", HttpUtility.UrlEncode(messageType.Value.ToString()));

                        controller.TempData.Remove(messageType.Key);
                    }
                    else
                    {
                        // Save message data to ajax redirect
                        controller.TempData.Keep(messageType.Key);
                    }

                    // Show only first ajax request message
                    break;
                }

                // Check if request is redirect - remove tempData
                if (!httpContext.Response.IsRequestBeingRedirected && !isResultRedirect)
                {
                    controller.TempData.Remove(messageType.Key);
                    controller.ViewData[messageType.Key] = messageType.Value;
                }
            }
        }

        public static void SetAjaxResponseRedirectUrl(
            this ControllerBase controller,
            string redirectUrl,
            bool forceRedirect = false)
        {
            if (!controller.ControllerContext.HttpContext.Request.IsAjaxRequest())
            {
                return;
            }

            // Reinit headers if exist
            controller.ControllerContext.HttpContext.Response.Headers["X-Redirect-Url"] = redirectUrl;
            controller.ControllerContext.HttpContext.Response.Headers["X-Redirect-Force"] = forceRedirect.ToString();
        }

        public static void SetAjaxResponseRedirectUrl(
            this ControllerBase controllerBase,
            string action,
            string controller,
            object routeValues = null,
            bool forceRedirect = false)
        {
            var redirectUrl =
                new UrlHelper(controllerBase.ControllerContext.HttpContext.Request.RequestContext).AbsoluteAction(
                    action,
                    controller,
                    routeValues);
            controllerBase.SetAjaxResponseRedirectUrl(redirectUrl, forceRedirect);
        }

        public static void SetAjaxBadRequest(this Controller controller, int code, bool skipIisValidation = false)
        {
            controller.Response.StatusCode = code;
            controller.Response.TrySkipIisCustomErrors = skipIisValidation;
        }
    }
}