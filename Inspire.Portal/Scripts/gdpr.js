var GDPR = /** @class */ (function () {
    function GDPR() {
        var _this = this;
        this.gdpr_link = "#";
        this.gdpr_link2 = "#";
        this.check = function () {
            var self = _this;
            var gdpr_htmlstring = "<div id=\"gdpr_wrapper\"><div class=\"center\"><p class=\"gdpr_rm\">" + resources.CookiesMessage + " <a href=\"" + self.gdpr_link + "\" style=\"color:#fff; text-decoration: underline;\"><em>" + resources.PersonalDataPoliticsMessage + "</em></a> " + resources.And + " <a href=\"" + self.gdpr_link2 + "\"  style=\"color:#fff;text-decoration: underline;\"><em>" + resources.GeneralTermsMessage + "</em></a></p><div class=\"gdpr_bttns\"><a id=\"gdpr_agree\" href=\"#\" class=\"bttn main\" >" + resources
                .ReadAndAcceptMessage + "</a></div></div>";
            if ($("#privacystate").length) {
                $("#privacystate h2")
                    .after("&nbsp;<br><a href=\"#\" class=\"gdpr_disagree\">" + resources.RemoveCookiesMessage + "</a>");
                $("#privacystate h2")
                    .after("&nbsp;<div class=\"success privacybtn\" >" + resources.ReadAndAcceptConditions + "</div>");
                $("#privacystate .privacybtn2").remove();
                var status_1;
                if (self.getCookie("cookiesgdpr")) {
                    switch (self.getCookie("cookiesgdpr")) {
                        case "true":
                            status_1 = "<div class=\"note\">" + resources.AcceptedCookiesMessage + "</div>";
                            break;
                        case "false":
                            status_1 = "<div class=\"note\">" + resources.DeclinedCookiesMessage + "</div>";
                            break;
                    }
                }
                else {
                    status_1 = "<div class=\"note\">" + resources.NotAcceptedYetCookiesMessage + "</div>";
                }
                $("#privacystate h2").append(status_1);
            }
            if (!self.getCookie("cookiesgdpr")) {
                $("header").before(gdpr_htmlstring);
            }
        };
        this.init = function () {
            var self = _this;
            $(document).ready(function () {
                core.rebindEvent("click", "#gdpr_agree,.gdpr_agree,.privacybtn1,.success.privacybtn", self.onGdprAgree);
                core.rebindEvent("click", "#gdpr_disagree, .gdpr_disagree", self.onGdprDisagree);
                core.rebindEvent("click", "#gdpr_close, .gdpr_close", self.onGdprClose);
            });
        };
        this.onGdprClose = function (e) {
            var self = _this;
            e.preventDefault();
            self.setCookie("cookiesgdpr", "false", 0.04166666);
            $("#gdprconsent").fadeOut(200);
        };
        this.onGdprDisagree = function (e) {
            var self = _this;
            e.preventDefault();
            self.removeCookies();
            self.setCookie("cookiesgdpr", "false", 0.0416666);
            $("#gdprconsent").fadeOut(200);
            setTimeout(function () {
                this.window.location = self.gdpr_link;
            }, 200);
        };
        this.onGdprAgree = function (e) {
            var self = _this;
            e.preventDefault();
            self.setCookie("cookiesgdpr", "true", 365);
            $("#gdprconsent").fadeOut(200);
            window.location.reload();
        };
        this.setCookie = function (cname, cvalue, exdays) {
            var d = new Date();
            d.setTime(d.getTime() + (exdays * 24 * 60 * 60 * 1000));
            var expires = "expires=" + d;
            document.cookie = cname + "=" + cvalue + "; " + expires + "; path=/";
        };
        this.getCookie = function (cname) {
            var name = cname + "=";
            var ca = document.cookie.split(";");
            for (var i = 0; i < ca.length; i++) {
                var c = ca[i];
                while (c.charAt(0) == " ")
                    c = c.substring(1);
                if (c.indexOf(name) == 0)
                    return c.substring(name.length, c.length);
            }
            return "";
        };
        this.removeCookies = function () {
            document.cookie.split(";").forEach(function (c) {
                document.cookie =
                    c.replace(/^ +/, "").replace(/=.*/, "=;expires=" + new Date().toUTCString() + ";path=/");
            });
        };
        this.init();
    }
    return GDPR;
}());
var gdpr = new GDPR();
