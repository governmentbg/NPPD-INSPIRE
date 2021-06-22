var Home = /** @class */ (function () {
    function Home() {
        var _this = this;
        this.init = function () {
            var self = _this;
            $(document).ready(function () {
                self.rebindEvents();
                self.initBackgroundImagesCarousel();
            });
        };
        this.rebindEvents = function () {
            var self = _this;
            var calendar = $("#calendar").data("kendoCalendar");
            if (calendar) {
                calendar.bind("change", self.onCalendarChange);
            }
            core.rebindEvent("click", ".showPassword", self.onShowPassword);
        };
        this.onShowPassword = function (e) {
            e.preventDefault();
            var x = document.getElementById("Password");
            if ($(e.currentTarget).data("type") == "old") {
                x = document.getElementById("OldPassword");
            }
            else if ($(e.currentTarget).data("type") == "confirm") {
                x = document.getElementById("ConfirmPassword");
            }
            if (x['type'] === "password") {
                x['type'] = "text";
                e.currentTarget.text = resources.HidePassword;
            }
            else {
                x['type'] = "password";
                e.currentTarget.text = resources.ShowPassword;
            }
        };
        this.onCalendarChange = function (e) {
            var value = e.sender.value();
            if (!value || !e.sender.options.dates || e.sender.options.dates.length < 1 || e.sender.options.dates.map(Number).indexOf(+value) < 0) {
                return;
            }
            var url = core.getPathToActionMethod("Events", "Publication", {
                area: "",
                useArea: false
            });
            var params = $.param({
                date: kendo.toString(value, "d")
            });
            location.href = url + "?" + params;
        };
        this.initBackgroundImagesCarousel = function () {
            $('.background-images').owlCarousel({
                singleItem: true,
                responsive: {
                    0: {
                        items: 1,
                    },
                },
                autoplay: true,
                autoplayTimeout: 5000,
                autoplayHoverPause: true,
                dots: false,
                navigation: false,
                slideSpeed: 300,
                paginationSpeed: 400,
                nav: false,
                loop: true
            });
        };
        this.init();
    }
    return Home;
}());
var home = new Home();
