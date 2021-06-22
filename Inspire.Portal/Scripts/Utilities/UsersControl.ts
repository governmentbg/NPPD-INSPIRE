class UsersControl {
    constructor() {
        let self = this;
        this.init();
    }


    private init() {
        let self = this;

        core.rebindEvent("click", "#addUserOrRole", self.onAddUserOrRoleClick);
        core.rebindEvent("click", ".remove-item-js", core.removeListViewItem);
    }

    private onAddUserOrRoleClick = (e) => {
        e.preventDefault();

        let listView = $("#controlItems").data("kendoListView") as kendo.ui.ListView,
            rolesDropDown = $("#rolesDropDown").data("kendoDropDownList") as kendo.ui.DropDownList,
            usersDropDown = $("#usersDropDown").data("kendoDropDownList") as kendo.ui.DropDownList,
            role = rolesDropDown && rolesDropDown.dataItem() ? rolesDropDown.dataItem() : null,
            user = usersDropDown && usersDropDown.dataItem() ? usersDropDown.dataItem() : null;
            
        if (!listView) {
            return;
        }

        let exist = listView.dataSource.data().some((item) => {
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
    }
}

let usersControl = new UsersControl();