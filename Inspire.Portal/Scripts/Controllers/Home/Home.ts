class Home {
    constructor() {
        this.init();
    }

    private init = () => {
        let self = this;
        $(document).ready(() => {
            self.rebindEvents();
            self.initBackgroundImagesCarousel();
        });
    }

    private rebindEvents = () => {
        let self = this;
        let calendar = $("#calendar").data("kendoCalendar");
        if (calendar) {
            calendar.bind("change", self.onCalendarChange);
        }
        core.rebindEvent("click",".showPassword", self.onShowPassword);
    }

    private onShowPassword = (e) => {
        e.preventDefault();
        let x = document.getElementById("Password");
        if ($(e.currentTarget).data("type") == "old") {
            x = document.getElementById("OldPassword")
        } else if ($(e.currentTarget).data("type") == "confirm") {
            x = document.getElementById("ConfirmPassword")
        }
        if (x['type'] === "password") {
            x['type'] = "text";
            e.currentTarget.text = resources.HidePassword
        } else {
            x['type'] = "password";
            e.currentTarget.text = resources.ShowPassword
        }
    }

    private onCalendarChange = (e: kendo.ui.CalendarEvent) => {
        let value = e.sender.value()
        if (!value || !e.sender.options.dates || e.sender.options.dates.length < 1 || e.sender.options.dates.map(Number).indexOf(+value) < 0) {
            return;
        }

        let url = core.getPathToActionMethod(
            "Events",
            "Publication",
            {
                area: "",
                useArea: false
            });

        let params = $.param({
            date: kendo.toString(value, "d")
        });

        location.href = `${url}?${params}`;
    }


    private initBackgroundImagesCarousel = () => {
        $('.background-images').owlCarousel(
            {
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
            })
    }
}

let home = new Home();