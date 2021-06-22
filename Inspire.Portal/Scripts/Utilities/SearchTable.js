/// <reference path="../Utilities/Core.ts" />
/// <reference path="../Utilities/Notification.ts" />
var SearchTableClass = /** @class */ (function () {
    function SearchTableClass() {
        this.init();
    }
    SearchTableClass.prototype.getSelectedItem = function (element) {
        var grid = $(element).closest("div.tableWrapper").find("div.k-widget.k-grid:first").data("kendoGrid");
        if (!grid) {
            return null;
        }
        var selectedRows = grid.select();
        if (selectedRows.length !== 1) {
            notification.displayMessage(resources.NotSelectedRowMessage, "warning");
            return null;
        }
        var selectedItem = grid.dataItem(selectedRows);
        if (!selectedItem) {
            notification.displayMessage(resources.NotSelectedRowMessage, "warning");
            return null;
        }
        return selectedItem;
    };
    SearchTableClass.prototype.getGrid = function (element) {
        return $(element).closest("div.tableWrapper").find("div.k-widget.k-grid:first").data("kendoGrid");
    };
    SearchTableClass.prototype.getSelectedRow = function (element) {
        var grid = $(element).closest("div.tableWrapper").find("div.k-widget.k-grid:first").data("kendoGrid");
        if (!grid) {
            return null;
        }
        return grid.select();
    };
    SearchTableClass.prototype.getSelectedItemByTr = function (element) {
        if (!element) {
            return null;
        }
        var grid = $(element).closest("div.k-widget.k-grid").data("kendoGrid");
        if (!grid) {
            return null;
        }
        var selectedItem = grid.dataItem(element);
        if (!selectedItem) {
            notification.displayMessage(resources.NotSelectedRowMessage, "warning");
            return null;
        }
        return selectedItem;
    };
    SearchTableClass.prototype.getCheckedItems = function (grid) {
        var dataItems = grid.dataItems();
        var checkedItems = [];
        for (var i = 0; i < dataItems.length; i++) {
            if (dataItems[i].IsChecked == true) {
                checkedItems.push(dataItems[i]);
            }
        }
        return checkedItems;
    };
    SearchTableClass.prototype.deleteItemByElement = function (sender, controller) {
        var dataItem = searchTable.getSelectedItemByTr(sender.closest("tr"));
        if (!dataItem) {
            return;
        }
        var actions = [
            {
                text: resources.Yes,
                action: function () {
                    core.requestOptional("Delete", controller, {
                        type: "DELETE",
                        data: {
                            id: dataItem["Id"]
                        },
                        success: function () {
                            var grid = searchTable.getGrid(sender);
                            grid.dataSource.remove(dataItem);
                        }
                    });
                    return true;
                },
                primary: true
            },
            {
                text: resources.No
            }
        ];
        var dialog = core.createKendoDialog({ actions: actions });
        dialog.title(resources.Warning);
        dialog.content(resources.ConfirmRemoveQuestion);
        dialog.open();
    };
    SearchTableClass.prototype.bindEvents = function () {
        core.rebindEvent("change", "input[type=checkbox].checkAll-js, input[type=checkbox].checkItem-js", function (e) {
            e.preventDefault();
            var sender = $(e.currentTarget);
            var grid = searchTable.getGrid(sender);
            if (!grid) {
                return;
            }
            var checked = $(this).is(':checked');
            var searchQueryId = grid.element.data("searchqueryid");
            var controller = grid.element.data("controller");
            var area = grid.element.data("area");
            var selectOperationType;
            var selectedObjects = [];
            if ($(this).hasClass("checkItem-js")) {
                var tr = sender.closest("tr");
                selectOperationType = checked ? "Add" : "Remove";
                var dataItem = grid.dataItem(tr);
                if (dataItem) {
                    selectedObjects.push(dataItem);
                }
                if (selectedObjects.length < 1) {
                    return;
                }
            }
            else {
                selectOperationType = checked ? "AddAll" : "RemoveAll";
            }
            core.requestOptional("ChangeSelectedItems", controller, {
                type: "POST",
                area: area,
                data: {
                    searchQueryId: searchQueryId,
                    selectOperationType: selectOperationType,
                    selectedObjects: selectedObjects
                },
                success: function () {
                    if (selectOperationType.indexOf("All") !== -1) {
                        grid.dataSource.read();
                        grid.refresh();
                    }
                    else {
                        var checkAllElement = grid.element.find("input[type=checkbox].checkAll-js:first");
                        if (checkAllElement.is(':checked') && !checked) {
                            checkAllElement.prop('checked', checked);
                        }
                    }
                }
            });
        });
    };
    SearchTableClass.prototype.init = function () {
        this.bindEvents();
    };
    return SearchTableClass;
}());
var searchTable = new SearchTableClass();
