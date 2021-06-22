var UsersControl = /** @class */ (function () {
    function UsersControl() {
        this.onAddUserOrRoleClick = function (e) {
            e.preventDefault();
            var listView = $("#controlItems").data("kendoListView"), rolesDropDown = $("#rolesDropDown").data("kendoDropDownList"), usersDropDown = $("#usersDropDown").data("kendoDropDownList"), role = rolesDropDown && rolesDropDown.dataItem() ? rolesDropDown.dataItem() : null, user = usersDropDown && usersDropDown.dataItem() ? usersDropDown.dataItem() : null;
            if (!listView) {
                return;
            }
            var exist = listView.dataSource.data().some(function (item) {
                return item["Role"].Id === role.Id && user["Id"] === null
                    || user["Id"] != null && item["User"].Id === user.Id;
            });
            if (exist) {
                notification.displayMessage(resources.RoleOrUserAlreadyAdded, "warning");
                return;
            }
            listView.dataSource.add({
                Id: null,
                Role: role,
                User: user
            });
        };
        var self = this;
        this.init();
    }
    UsersControl.prototype.init = function () {
        var self = this;
        core.rebindEvent("click", "#addUserOrRole", self.onAddUserOrRoleClick);
        core.rebindEvent("click", ".remove-item-js", core.removeListViewItem);
    };
    return UsersControl;
}());
var usersControl = new UsersControl();
