/// <reference path="../typings/jquery/jquery.d.ts" />
/// <reference path="../typings/kendo-ui/kendo.all.d.ts" />
/// <reference path="../Utilities/Core.ts" />
var CustomNotification = /** @class */ (function () {
    function CustomNotification() {
        var _this = this;
        this.types = ["info", "success", "warning", "error"];
        this.selectorHandle = "#notification";
        this.displayMessage = function (message, messageType) {
            var self = _this;
            if (!message || !messageType || self.types.indexOf(messageType) < 0) {
                return;
            }
            self.handle = self.handle || $(self.selectorHandle);
            var notificationElement = self.handle.data("kendoNotification");
            if (notificationElement) {
                if (messageType.toLowerCase() === "info" || messageType.toLowerCase() === "success") {
                    notificationElement.options.autoHideAfter = 5000;
                }
                else {
                    notificationElement.options.autoHideAfter = 0;
                }
                notificationElement.show({
                    title: messageType,
                    message: message
                }, messageType.toLowerCase());
            }
        };
        this.init = function () {
            var self = _this;
            $(document).ready(function () {
                self.handleAjaxMessages();
                self.bindNotificationCloseHandler();
            });
        };
        this.handleAjaxMessages = function () {
            var self = _this;
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
        };
        this.checkRedirectUrl = function (response) {
            var redirectUrl = response.getResponseHeader("X-Redirect-Url");
            var forceRedirect = response.getResponseHeader("X-Redirect-Force");
            if (redirectUrl) {
                window.location.href = redirectUrl;
                return forceRedirect === "True";
            }
            return false;
        };
        this.checkAndHandleMessageFromHeader = function (response) {
            var self = _this;
            var msg = response.getResponseHeader('X-Message');
            if (msg) {
                msg = decodeURIComponent(decodeURIComponent(msg.replace(/\+/g, " ")));
                var messageType = response.getResponseHeader('X-Message-Type');
                self.displayMessage(msg, messageType);
                return true;
            }
            return false;
        };
        this.showError = function (response) {
            var self = _this;
            var message = resources.InternalServerError;
            var messageType = "error";
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
        };
        this.init();
    }
    CustomNotification.prototype.onShowNotification = function (e) {
        if (e.sender.getNotifications().length == 1) {
            var element = e.element.parent(), eWidth = element.width(), eHeight = element.height(), wWidth = $(window).width(), wHeight = $(window).height(), newTop = void 0, newLeft = void 0;
            newLeft = Math.floor(wWidth / 2 - eWidth / 2);
            newTop = Math.floor(wHeight / 2 - eHeight / 2);
            e.element.parent().css({ top: newTop, left: newLeft, zIndex: 33333 });
        }
    };
    CustomNotification.prototype.bindNotificationCloseHandler = function () {
        $(document).on("click", ".k-widget.k-notification", function (e) {
            e.stopPropagation();
            if ($(e.target).hasClass("close-js")) {
                $(e.currentTarget).closest(".k-animation-container").remove();
            }
        });
    };
    return CustomNotification;
}());
var notification = new CustomNotification();
