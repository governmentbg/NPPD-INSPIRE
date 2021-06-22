var User = /** @class */ (function () {
    function User() {
        this.onIsAdminCheckboxChange = function (e) {
            var checked = $(e.currentTarget).prop("checked");
            if (checked) {
                $(".gntwrk-groups").hide();
            }
            else {
                $(".gntwrk-groups").show();
            }
        };
        this.onUserInfoClick = function (e) {
            e.preventDefault();
            var selectedItem = searchTable.getSelectedItemByTr($(e.currentTarget).closest("tr"));
            core.openKendoWindow("Info", "User", {
                type: "GET",
                area: "Admin",
                useArea: true,
                data: {
                    id: selectedItem["Id"]
                }
            }, {
                title: kendo.format("{0}: {1}", resources.Info, selectedItem["Name"] ? selectedItem["Name"] : ""),
            });
        };
        this.onChangeStatusClick = function (e) {
            e.preventDefault();
            var sender = $(e.currentTarget);
            var grid = searchTable.getGrid(sender);
            if (!grid) {
                return;
            }
            core.openKendoWindow("ChangeStatus", "User", {
                useArea: true,
                area: "admin",
                data: {
                    id: sender.data("userid"),
                    searchQueryId: grid.element.data("searchqueryid")
                }
            }, {
                close: function (e) {
                    if (e.userTriggered) {
                        return;
                    }
                },
                title: resources.ChangeStatus
            });
        };
        this.onSetUserRoleClick = function (e) {
            e.preventDefault();
            var sender = $(e.currentTarget);
            var grid = searchTable.getGrid(sender);
            if (!grid) {
                return;
            }
            core.openKendoWindow("SetRole", "User", {
                useArea: true,
                area: "admin",
                data: {
                    id: sender.data("userid"),
                    searchQueryId: grid.element.data("searchqueryid")
                }
            }, {
                close: function (e) {
                    if (e.userTriggered) {
                        return;
                    }
                },
                title: resources.RolesManagement
            });
        };
        var self = this;
        self.init();
        core.bindBodyClick();
    }
    User.prototype.init = function () {
        var self = this;
        core.rebindEvent("click", ".change-status-js", self.onChangeStatusClick);
        core.rebindEvent("click", ".set-role-js", self.onSetUserRoleClick);
        core.rebindEvent("click", ".user-info-js", self.onUserInfoClick);
        core.rebindEvent("change", ".isAdmin-js", self.onIsAdminCheckboxChange);
    };
    return User;
}());
var user = new User();
