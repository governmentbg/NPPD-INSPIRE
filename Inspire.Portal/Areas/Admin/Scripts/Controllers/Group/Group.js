var Group = /** @class */ (function () {
    function Group() {
        this.onEnableAllowedCategoriesClick = function (e) {
            var checked = $(e.currentTarget).prop("checked");
            var multiSelect = $("#SelectedAllowedCategories").data("kendoMultiSelect");
            multiSelect.enable(checked);
        };
        this.onGroupInfoClick = function (e) {
            e.preventDefault();
            var selectedItem = searchTable.getSelectedItemByTr($(e.currentTarget).closest("tr"));
            core.openKendoWindow("Info", "GroupsView", {
                type: "GET",
                area: null,
                useArea: false,
                data: {
                    id: selectedItem["Id"]
                }
            }, {
                title: kendo.format("{0}: {1}", resources.Info, selectedItem["Name"]),
            });
        };
        var self = this;
        self.init();
        core.bindBodyClick();
    }
    Group.prototype.init = function () {
        var self = this;
        core.rebindEvent("click", ".group-info-js", self.onGroupInfoClick);
        core.rebindEvent("click", "#EnableAllowedCategories", self.onEnableAllowedCategoriesClick);
    };
    return Group;
}());
var group = new Group();
