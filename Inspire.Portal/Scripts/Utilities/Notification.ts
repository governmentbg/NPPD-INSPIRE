/// <reference path="../typings/jquery/jquery.d.ts" />
/// <reference path="../typings/kendo-ui/kendo.all.d.ts" />
/// <reference path="../Utilities/Core.ts" />

class CustomNotification {

    constructor() {
        this.init();
    }

    private types: string[] = ["info", "success", "warning", "error"];
    private selectorHandle = "#notification";
    private handle: any;

    displayMessage = (message: string, messageType: string) => {
        let self = this;
        if (!message || !messageType || self.types.indexOf(messageType) < 0) {
            return;
        }

        self.handle = self.handle || $(self.selectorHandle);
        let notificationElement = self.handle.data("kendoNotification") as kendo.ui.Notification;

        if (notificationElement) {
            if (messageType.toLowerCase() === "info" || messageType.toLowerCase() === "success") {
                notificationElement.options.autoHideAfter = 5000;
            } else {
                notificationElement.options.autoHideAfter = 0;
            }

            notificationElement.show(
                {
                    title: messageType,
                    message: message
                },
                messageType.toLowerCase());
        }
    }

    onShowNotification(e: kendo.ui.NotificationShowEvent) {
        if (e.sender.getNotifications().length == 1) {
            let element = e.element.parent(),
                eWidth = element.width(),
                eHeight = element.height(),
                wWidth = $(window).width(),
                wHeight = $(window).height(),
                newTop, newLeft;

            newLeft = Math.floor(wWidth / 2 - eWidth / 2);
            newTop = Math.floor(wHeight / 2 - eHeight / 2);

            e.element.parent().css({ top: newTop, left: newLeft, zIndex: 33333 });
        }
    }

    private init = () => {
        let self = this;
        $(document).ready(function () {
            self.handleAjaxMessages();
            self.bindNotificationCloseHandler();
        });
    }

    private bindNotificationCloseHandler() {
        $(document).on(
            "click",
            ".k-widget.k-notification",
            function (e) {
                e.stopPropagation();

                if ($(e.target).hasClass("close-js")) {
                    $(e.currentTarget).closest(".k-animation-container").remove();
                }
            });
    }

    private handleAjaxMessages = () => {
        let self = this;
        $(document)
            .ajaxSuccess(function (event, response) {
                if (self.checkRedirectUrl(response)) {
                    return;
                }

                self.checkAndHandleMessageFromHeader(response);
            })
            .ajaxError(function (event, response) {
                self.showError(response);
            });
    }

    private checkRedirectUrl = (response: any) => {
        let redirectUrl = response.getResponseHeader("X-Redirect-Url");
        let forceRedirect = response.getResponseHeader("X-Redirect-Force");
        if (redirectUrl) {
            window.location.href = redirectUrl;
            return forceRedirect === "True";
        }

        return false;
    }

    private checkAndHandleMessageFromHeader = (response: any) => {
        let self = this;
        let msg = response.getResponseHeader('X-Message');
        if (msg) {
            msg = decodeURIComponent(decodeURIComponent(msg.replace(/\+/g, " ")));
            let messageType = response.getResponseHeader('X-Message-Type');
            self.displayMessage(msg, messageType);
            return true;
        }

        return false;
    }

    private showError = (response: any) => {
        let self = this;
        let message = resources.InternalServerError;
        let messageType = "error";
        if (response) {

            if (self.checkRedirectUrl(response)) {
                return;
            }

            if (self.checkAndHandleMessageFromHeader(response)) {
                return;
            }

            switch (response.status) {
                case 401:
                    {
                        location.reload();
                        return;
                    }
                case 403:
                    {
                        message = resources.ForbiddenError;
                        messageType = "warning";
                        break;
                    }
                case 404:
                    {
                        message = resources.NotFoundError;
                        messageType = "warning";
                        break;
                    }
            }
        }

        self.displayMessage(message, messageType);
    }
}

let notification = new CustomNotification();