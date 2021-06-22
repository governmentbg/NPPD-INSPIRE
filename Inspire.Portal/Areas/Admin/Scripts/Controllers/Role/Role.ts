class Role {
    constructor() {
        this.init();
    }

    private init() {
        let self = this;
        core.bindBodyClick();

        core.rebindEvent("click", ".selectAllActivities-js", self.onSelectAllActivitiesClick);
        core.rebindEvent("click", ".deleteRole-js", self.onDeleteRoleClick);
    }

    private onSelectAllActivitiesClick = (e) => {
        let sender = $(e.currentTarget);
        let checked = sender.prop("checked");
        $("#ActivitiesList").find("input[type='checkbox']").each(function (index, item) {
            $(item).prop("checked", checked);
        });
    }

    private onDeleteRoleClick = (e) => {
        e.preventDefault();
        let sender = $(e.currentTarget);
        searchTable.deleteItemByElement(sender, "Role");
    }
}

let role = new Role();