var Cms = /** @class */ (function () {
    function Cms() {
        this.getPageTypeIcon = function (type) {
            switch (type.toString()) {
                case "2":
                case "Content":
                    {
                        return "k-i-file-programming";
                    }
                case "1":
                case "Link":
                    {
                        return "k-i-hyperlink";
                    }
                default:
                    {
                        return "";
                    }
            }
        };
        this.getPageVisibilityTypeIcon = function (type) {
            switch (type.toString()) {
                case "1":
                case "Hide":
                    {
                        return "k-i-minus-circle";
                    }
                case "2":
                case "AuthenticatedUsed":
                    {
                        return "k-i-lock";
                    }
                case "3":
                case "Public":
                    {
                        return "k-i-check-circle";
                    }
                default: {
                    return "";
                }
            }
        };
        this.onCopyLinkPageClick = function (e) {
            var sender = $(e.currentTarget);
            var tr = sender.closest("tr");
            var treeList = tr.closest(".k-widget.k-treelist").data("kendoTreeList");
            var item = treeList.dataItem(tr);
            var copyLink = item["PermanentLink"];
            core.copyToClipboard(copyLink);
            notification.displayMessage(resources.CopyLinkMessage, "info");
        };
        this.onRemovePageClick = function (e) {
            var sender = $(e.currentTarget);
            var treeList = sender.closest(".k-widget.k-treelist").data("kendoTreeList");
            var dataItem = treeList.dataItem(sender.closest("tr"));
            if (!dataItem) {
                return;
            }
            core.confirmDelete(function () {
                core.requestOptional("Delete", "Cms", {
                    type: "DELETE",
                    useArea: false,
                    data: {
                        id: dataItem["Id"]
                    },
                    success: function () {
                        treeList.dataSource.read();
                        notification.displayMessage(resources.DeleteSuccess, "success");
                    }
                });
                return true;
            });
        };
        this.init();
    }
    Cms.prototype.onDrop = function (e) {
        if (e.source["Id"] == e.destination["Id"]) {
            e.setValid(false);
            return;
        }
        core.requestOptional("ChangePosition", "Cms", {
            type: "POST",
            useArea: false,
            data: {
                sourceId: e.source["Id"],
                destinationId: e.destination["Id"],
                position: e.position
            },
            async: false,
            success: function () {
                e.setValid(true);
            },
            error: function () {
                e.setValid(false);
            }
        });
    };
    Cms.prototype.init = function () {
        var _this = this;
        var self = this;
        core.bindBodyClick();
        core.rebindEvent("click", ".page-remove-js", self.onRemovePageClick);
        core.rebindEvent("click", ".copy-link-js", self.onCopyLinkPageClick);
        $(document).ready(function () {
            var self = _this;
            var pagesTreeList = $("#pages").data("kendoTreeList");
            if (pagesTreeList) {
                pagesTreeList.setOptions({
                    toolbar: kendo.template($("#pageToolbarTemplate").html())
                });
            }
            var pageType = $("#pageType").data("kendoDropDownList");
            if (pageType) {
                pageType.bind("change", self.onPageTypeChange);
                pageType.one("dataBound", function () { pageType.trigger("change"); });
            }
            var locationType = $("#locationType").data("kendoDropDownList");
            if (locationType) {
                locationType.bind("change", self.onLocationTypeChange);
                locationType.one("dataBound", function () { locationType.trigger("change"); });
            }
        });
    };
    ;
    Cms.prototype.onLocationTypeChange = function (e) {
        var form = $(e.sender.wrapper).closest("form");
        var elements = form.find(".menuTitle-js");
        if (e.sender.value() != e.sender.element.data("location-type-id")) {
            elements.show();
        }
        else {
            elements.hide();
        }
    };
    Cms.prototype.onPageTypeChange = function (e) {
        var form = $(e.sender.wrapper).closest("form");
        var elements = form.find(".content-js");
        var urlName;
        if (e.sender.value() == e.sender.element.data("content-type-id")) {
            elements.show();
            urlName = resources.UrlAddressName;
        }
        else {
            elements.hide();
            urlName = resources.Link;
        }
        form.find("label[for='PermanentLink']:first").text(urlName);
        form.find("input[name='PermanentLink']:first").attr("data-val-required", kendo.format(resources.Required, urlName));
    };
    return Cms;
}());
var cms = new Cms();
