class User {

    constructor() {
        let self = this;

        self.init();
        core.bindBodyClick();
    }

    private init() {
        let self = this;
        core.rebindEvent("click", ".change-status-js", self.onChangeStatusClick);
        core.rebindEvent("click", ".set-role-js", self.onSetUserRoleClick);
        core.rebindEvent("click", ".user-info-js", self.onUserInfoClick);
        core.rebindEvent("change", ".isAdmin-js", self.onIsAdminCheckboxChange);
    }

    private onIsAdminCheckboxChange = (e) => {
        let checked = $(e.currentTarget).prop("checked");
        if (checked) {
            $(".gntwrk-groups").hide();
        } else {
            $(".gntwrk-groups").show();
        }
    }

    private onUserInfoClick = (e) => {
        e.preventDefault();

        let selectedItem = searchTable.getSelectedItemByTr($(e.currentTarget).closest("tr"));

        core.openKendoWindow(
            "Info",
            "User",
            {
                type: "GET",
                area: "Admin",
                useArea: true,
                data: {
                    id: selectedItem["Id"]
                }
            },
            {
                title: kendo.format("{0}: {1}", resources.Info, selectedItem["Name"] ? selectedItem["Name"] : ""),
            });
    };

    private onChangeStatusClick = (e) => {
        e.preventDefault();
        let sender = $(e.currentTarget);
        let grid = searchTable.getGrid(sender);
        if (!grid) {
            return;
        }

        core.openKendoWindow(
            "ChangeStatus",
            "User",
            {
                useArea: true,
                area: "admin",
                data: {
                    id: sender.data("userid"),
                    searchQueryId: grid.element.data("searchqueryid")
                }
            },
            {
                close: (e) => {
                    if (e.userTriggered) {
                        return;
                    }
                },
                title: resources.ChangeStatus
            });
    };

    private onSetUserRoleClick = (e) => {
        e.preventDefault();
        let sender = $(e.currentTarget);
        let grid = searchTable.getGrid(sender);
        if (!grid) {
            return;
        }

        core.openKendoWindow("SetRole",
            "User",
            {
                useArea: true,
                area: "admin",
                data: {
                    id: sender.data("userid"),
                    searchQueryId: grid.element.data("searchqueryid")
                }
            },
            {
                close: (e) => {
                    if (e.userTriggered) {
                        return;
                    }
                },
                title: resources.RolesManagement
            });
    };
}

let user = new User();