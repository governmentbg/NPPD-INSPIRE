/// <reference path="../Utilities/Core.ts" />
/// <reference path="../Utilities/Notification.ts" />


class SearchTableClass {
    constructor() {
        this.init();
    }

    getSelectedItem(element) {
        let grid = $(element).closest("div.tableWrapper").find("div.k-widget.k-grid:first").data("kendoGrid");
        if (!grid) {
            return null;
        }

        let selectedRows = grid.select();
        if (selectedRows.length !== 1) {
            notification.displayMessage(resources.NotSelectedRowMessage, "warning");
            return null;
        }

        let selectedItem = grid.dataItem(selectedRows);
        if (!selectedItem) {
            notification.displayMessage(resources.NotSelectedRowMessage, "warning");
            return null;
        }

        return selectedItem;
    }

    getGrid(element) {
        return $(element).closest("div.tableWrapper").find("div.k-widget.k-grid:first").data("kendoGrid");
    }

    getSelectedRow(element) {
        let grid = $(element).closest("div.tableWrapper").find("div.k-widget.k-grid:first").data("kendoGrid");
        if (!grid) {
            return null;
        }

        return grid.select();
    }

    getSelectedItemByTr(element) {
        if (!element) {
            return null;
        }

        let grid = $(element).closest("div.k-widget.k-grid").data("kendoGrid");
        if (!grid) {
            return null;
        }

        let selectedItem = grid.dataItem(element);
        if (!selectedItem) {
            notification.displayMessage(resources.NotSelectedRowMessage, "warning");
            return null;
        }

        return selectedItem;
    }

    getCheckedItems(grid) {
        let dataItems = grid.dataItems();
        let checkedItems = [];

        for (let i = 0; i < dataItems.length; i++) {
            if (dataItems[i].IsChecked == true) {
                checkedItems.push(dataItems[i])
            }
        }

        return checkedItems;
    }

    deleteItemByElement(sender: JQuery, controller: string) {

        let dataItem = searchTable.getSelectedItemByTr(sender.closest("tr"));
        if (!dataItem) {
            return;
        }

        let actions = [
            {
                text: resources.Yes,
                action: function () {
                    core.requestOptional(
                        "Delete",
                        controller,
                        {
                            type: "DELETE",
                            data: {
                                id: dataItem["Id"]
                            },
                            success: () => {
                                let grid = searchTable.getGrid(sender);
                                grid.dataSource.remove(dataItem);
                            }
                        })

                    return true;
                },
                primary: true
            },
            {
                text: resources.No
            }
        ];

        let dialog = core.createKendoDialog({ actions: actions });
        dialog.title(resources.Warning);
        dialog.content(resources.ConfirmRemoveQuestion);
        dialog.open();
    }

    private bindEvents() {
        core.rebindEvent("change", "input[type=checkbox].checkAll-js, input[type=checkbox].checkItem-js", function (e) {
            e.preventDefault();
            let sender = $(e.currentTarget);
            let grid = searchTable.getGrid(sender);
            if (!grid) {
                return;
            }

            let checked = $(this).is(':checked');
            let searchQueryId = grid.element.data("searchqueryid");
            let controller = grid.element.data("controller");
            let area = grid.element.data("area");

            let selectOperationType;
            let selectedObjects = [];

            if ($(this).hasClass("checkItem-js")) {
                let tr = sender.closest("tr");
                selectOperationType = checked ? "Add" : "Remove";
                let dataItem = grid.dataItem(tr);
                if (dataItem) {
                    selectedObjects.push(dataItem);
                }

                if (selectedObjects.length < 1) {
                    return;
                }
            } else {
                selectOperationType = checked ? "AddAll" : "RemoveAll";
            }

            core.requestOptional(
                "ChangeSelectedItems",
                controller,
                {
                    type: "POST",
                    area: area,
                    data: {
                         searchQueryId: searchQueryId, 
                         selectOperationType: selectOperationType, 
                         selectedObjects: selectedObjects
                    }, //// aко няма името на пропъртито две точки стойността му - ИЕ се чупи
                    success:
                        function () {
                            if (selectOperationType.indexOf("All") !== -1) {
                                grid.dataSource.read();
                                grid.refresh();
                            } else {
                                let checkAllElement = grid.element.find("input[type=checkbox].checkAll-js:first");
                                if (checkAllElement.is(':checked') && !checked) {
                                    checkAllElement.prop('checked', checked);
                                }
                            }
                        }
                });
        });
    }

    private init() {
        this.bindEvents();
    }
}

let searchTable = new SearchTableClass();