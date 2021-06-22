class Group {

    constructor() {
        let self = this;

        self.init();
        core.bindBodyClick();
    }

    private init() {
        let self = this;
        core.rebindEvent("click", ".group-info-js", self.onGroupInfoClick);
        core.rebindEvent("click","#EnableAllowedCategories", self.onEnableAllowedCategoriesClick);
    }

    private onEnableAllowedCategoriesClick = (e) => {

        let checked = $(e.currentTarget).prop("checked");
        let multiSelect = $("#SelectedAllowedCategories").data("kendoMultiSelect") as kendo.ui.MultiSelect;
        multiSelect.enable(checked);
    }

    private onGroupInfoClick = (e) => {
        e.preventDefault();

        let selectedItem = searchTable.getSelectedItemByTr($(e.currentTarget).closest("tr"));

        core.openKendoWindow(
            "Info",
            "GroupsView",
            {
                type: "GET",
                area: null,
                useArea: false,
                data: {
                    id: selectedItem["Id"]
                }
            },
            {
                title: kendo.format("{0}: {1}", resources.Info, selectedItem["Name"]),
            });
    };
}

let group = new Group();