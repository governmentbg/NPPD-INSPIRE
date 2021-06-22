var Role = /** @class */ (function () {
    function Role() {
        this.onSelectAllActivitiesClick = function (e) {
            var sender = $(e.currentTarget);
            var checked = sender.prop("checked");
            $("#ActivitiesList").find("input[type='checkbox']").each(function (index, item) {
                $(item).prop("checked", checked);
            });
        };
        this.onDeleteRoleClick = function (e) {
            e.preventDefault();
            var sender = $(e.currentTarget);
            searchTable.deleteItemByElement(sender, "Role");
        };
        this.init();
    }
    Role.prototype.init = function () {
        var self = this;
        core.bindBodyClick();
        core.rebindEvent("click", ".selectAllActivities-js", self.onSelectAllActivitiesClick);
        core.rebindEvent("click", ".deleteRole-js", self.onDeleteRoleClick);
    };
    return Role;
}());
var role = new Role();
