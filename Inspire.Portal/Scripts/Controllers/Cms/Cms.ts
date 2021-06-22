class Cms {
    onDrop(e) {
        if (e.source["Id"] == e.destination["Id"]) {
            e.setValid(false);
            return;
        }

        core.requestOptional(
            "ChangePosition",
            "Cms",
            {
                type: "POST",
                useArea: false,
                data: {
                    sourceId: e.source["Id"],
                    destinationId: e.destination["Id"],
                    position: e.position
                },
                async: false,
                success: () => {
                    e.setValid(true);
                },
                error: () => {
                    e.setValid(false);
                }
            });
    }

    getPageTypeIcon = (type) => {
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
    }

    getPageVisibilityTypeIcon = (type: string) => {
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
    }

    constructor() {
        this.init();
    }

    private init() {
        let self = this;

        core.bindBodyClick();

        core.rebindEvent("click", ".page-remove-js", self.onRemovePageClick);

        core.rebindEvent("click", ".copy-link-js", self.onCopyLinkPageClick);

        $(document).ready(() => {
            let self = this;
            let pagesTreeList = $("#pages").data("kendoTreeList") as kendo.ui.TreeList;
            if (pagesTreeList) {
                pagesTreeList.setOptions({
                    toolbar: kendo.template($("#pageToolbarTemplate").html())
                });
            }

            let pageType = $("#pageType").data("kendoDropDownList") as kendo.ui.DropDownList;
            if (pageType) {
                pageType.bind("change", self.onPageTypeChange);
                pageType.one("dataBound", () => { pageType.trigger("change"); });
            }

            let locationType = $("#locationType").data("kendoDropDownList") as kendo.ui.DropDownList;
            if (locationType) {
                locationType.bind("change", self.onLocationTypeChange);
                locationType.one("dataBound", () => { locationType.trigger("change"); });
            }
        });
    };

    private onLocationTypeChange(e: kendo.ui.DropDownListChangeEvent) {
        let form = $(e.sender.wrapper).closest("form");
        let elements = form.find(".menuTitle-js");
        if (e.sender.value() != e.sender.element.data("location-type-id")) {
            elements.show();
        } else {
            elements.hide();
        }
    }

    private onPageTypeChange(e: kendo.ui.DropDownListChangeEvent) {
        let form = $(e.sender.wrapper).closest("form");
        let elements = form.find(".content-js");
        let urlName: string;
        if (e.sender.value() == e.sender.element.data("content-type-id")) {
            elements.show();
            urlName = resources.UrlAddressName;
        } else {
            elements.hide();
            urlName = resources.Link;
        }

        form.find("label[for='PermanentLink']:first").text(urlName);
        form.find("input[name='PermanentLink']:first").attr("data-val-required", kendo.format(resources.Required, urlName));
    }

    private onCopyLinkPageClick = (e) => {
        let sender = $(e.currentTarget);
        let tr = sender.closest("tr");
        let treeList = tr.closest(".k-widget.k-treelist").data("kendoTreeList") as kendo.ui.TreeList;
        let item = treeList.dataItem(tr);
        let copyLink = item["PermanentLink"];
        core.copyToClipboard(copyLink);
        notification.displayMessage(resources.CopyLinkMessage, "info");
    }

    private onRemovePageClick = (e) => {
        let sender = $(e.currentTarget);
        let treeList = sender.closest(".k-widget.k-treelist").data("kendoTreeList") as kendo.ui.TreeList;
        let dataItem = treeList.dataItem(sender.closest("tr"));
        if (!dataItem) {
            return;
        }

        core.confirmDelete(
            () => {
                core.requestOptional(
                    "Delete",
                    "Cms",
                    {
                        type: "DELETE",
                        useArea: false,
                        data: {
                            id: dataItem["Id"]
                        },
                        success: () => {
                            treeList.dataSource.read();
                            notification.displayMessage(resources.DeleteSuccess, "success");
                        }
                    });
                return true;
            });
    }
}

let cms = new Cms();