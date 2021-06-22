namespace Inspire.Common.Mvc.Filters
{
    using System;
    using System.ComponentModel;
    using System.Net;
    using System.Web.Mvc;

    using Inspire.Common.Mvc.Enums;
    using Inspire.Common.Mvc.Extensions;
    using Inspire.Core.Infrastructure.Logger;
    using Inspire.Utilities.Exception;

    public abstract class CustomErrorHandler : HandleErrorAttribute
    {
        private ILogger logger;

        protected abstract string DbErrorMessage { get; }

        private ILogger Logger => logger ?? (logger = DependencyResolver.Current.GetService<ILogger>());

        public override void OnException(ExceptionContext context)
        {
            // Log exceptions
            LogException(context.Exception);

            // If exception is child - catch it and return null result
            if (context.IsChildAction)
            {
                context.ExceptionHandled = true;
                return;
            }

            // Catch user exception and warnings
            if (context.Exception is UserException
                || context.Exception is WarningException)
            {
                context.ExceptionHandled = true;
                var message = context.Exception is UserDbException
                    ? DbErrorMessage
                    : context.Exception.Message;
                var messageType = context.Exception is UserException
                    ? MessageType.Error
                    : MessageType.Warning;
                HandleUserException(context, message, messageType);
            }
        }

        private void HandleUserException(ExceptionContext context, string message, MessageType type)
        {
            context.HttpContext.Response.StatusCode = HttpStatusCode.InternalServerError.GetHashCode();

            // Check if request is not ajax -> show error message and redirect to request page
            if (!context.HttpContext.Request.IsAjaxRequest())
            {
                context.Result = context.Controller.RedirectToRequestPage();
            }

            var isResultRedirect = context.Result is RedirectToRouteResult || context.Result is RedirectResult;
            context.Controller.ShowMessage(type, message);
            context.Controller.SendMessageData(context.HttpContext, isResultRedirect);
        }

        private void LogException(Exception exception)
        {
            if (exception == null || exception.GetType() == typeof(UserException))
            {
                return;
            }

            Logger.Error(exception);
            LogException(exception.InnerException);
        }
    }
}