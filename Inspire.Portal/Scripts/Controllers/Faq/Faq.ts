class Faq {
    constructor() {
        this.Init();
    }

    public onFaqCategoryUpsertSuccessfully = (e) => {
        let categoriesDropDownList = $("#Category_Id").data("kendoDropDownList") as kendo.ui.DropDownList;

        categoriesDropDownList.dataSource.read();
        $("#CategoriesGrid").data("kendoGrid").dataSource.read();

        let window = $(".k-window-content:last").data("kendoWindow");
        if (!window) {
            return;
        }
        window.close();
    }

    public onChangeOrder = (e) => {
        let oldIndex = e.oldIndex;
        let newIndex = e.newIndex;

        core.requestOptional("Rearrange", "Faq",
            {
                type: "POST",
                useArea: false,
                data: {
                    oldIndex,
                    newIndex
                }
            });
    }

    private Init = () => {
        let self = this;
        self.RebindEvents();
    }

    private RebindEvents = () => {
        let self = this;
        core.rebindEvent("click", ".manageFAQCategory-js", self.ManageCategory);
        core.rebindEvent("click", ".faqDelete-js", self.DeleteFAQ);
        core.rebindEvent("click", ".remove-category-js", self.DeleteFAQCategory);
        core.rebindEvent("click", ".add-category-js,.edit-category-js", self.UpsertCategory);
        core.rebindEvent("click", ".searchByWord-js", self.searchFaq);
        core.bindBodyClick();
    }

    private searchFaq = (e) => {
        e.preventDefault();
        let word = $("#SearchWord").val();
        let categoryid = $("#CategoryId").val();

        core.requestOptional(
            "SearchFaq",
            "Home",
            {
                type: "POST",
                useArea: false,
                data: {
                    categoryid: categoryid,
                    searchword: word
                },
                success: (res) => {
                    $("#faq-result").html(res);
                }
            });
    }

    private UpsertCategory = (e) => {
        e.preventDefault();

        let grid = $("#CategoriesGrid").data("kendoGrid");
        let tr = $(e.currentTarget).closest("tr");
        let dataIem = null;
        if (tr) {
            dataIem = grid.dataItem(tr);
        }

        core.openKendoWindow(
            "UpsertCategory",
            "Faq",
            {
                type: "GET",
                useArea: false,
                data: {
                    id: dataIem != null ? dataIem["Id"] : ''
                }
            },
            {
                close: (e) => {
                    if (e.userTriggered) {
                        return;
                    }
                },
                title: dataIem != null ? dataIem["Name"] : resources.CreateCategory
            });
    }

    private DeleteFAQ = (e) => {
        e.preventDefault();
        let sender = $(e.currentTarget);
        let grid = sender.closest(".k-widget.k-grid").data("kendoGrid") as kendo.ui.Grid;
        let dataItem = searchTable.getSelectedItemByTr(sender.closest("tr"));
        let searchQueryId = grid.wrapper.data("searchqueryid");

        if (!dataItem) {
            return;
        }

        core.confirmDelete(
            () => {
                core.requestOptional(
                    "Delete",
                    "Faq",
                    {
                        type: "POST",
                        useArea: false,
                        data: {
                            id: dataItem["Id"],
                            searchQueryId: searchQueryId
                        },
                        success: () => {
                            core.onDeleteMvcTableItem(grid, dataItem);
                            notification.displayMessage(resources.DeleteSuccess, "success");
                        }
                    });
                return true;
            });
    }

    private ManageCategory = (e) => {
        e.preventDefault();

        core.openKendoWindow(
            "ManageCategory",
            "Faq",
            {
                type: "GET",
                useArea: false
            },
            {
                close: (e) => {
                    if (e.userTriggered) {
                        return;
                    }
                },
                title: resources.ManageCategory
            });
    }

    private DeleteFAQCategory = (e) => {
        e.preventDefault();

        let sender = $(e.currentTarget);
        let faqCategoryId = sender.data("id");

        const actions = [
            {
                text: resources.Yes,
                action: () => {
                    core.requestOptional(
                        "DeleteCategory",
                        "Faq",
                        {
                            type: "DELETE",
                            useArea: false,
                            data: {
                                id: faqCategoryId,
                                newCategoryId: $("#newCategory").val()
                },
                            success: () => {
                                notification.displayMessage(resources.DeleteSuccess, "success");
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

        let categoryName: string = $("#CategoriesGrid").data("kendoGrid").dataItem(sender.closest("tr"))["Name"];

        let dialogContent: string = resources.ConfirmDeletingCategory.replace("{0}", categoryName) +
            "<div class='form-input'>" +
            `<label>${resources.TransferCategory}</label>` +
            "<input id='newCategory'/>" +
            "</div>";

        core.createKendoDialog(
            {
                title: resources.Deleting,
                content: dialogContent,
                visible: true,
                actions: actions
            });

        var url = `${core.getPathToActionMethod("GetFaqFilteredCategories", "Faq")}?categoryId=${faqCategoryId}`;

        $("#newCategory").kendoDropDownList({
            dataTextField: "Value",
            dataValueField: "Key",
            optionLabel: resources.CategoryAndQuestionsDelete,
            dataSource: {
                type: "json",
                serverFiltering: true,
                transport: {
                    read: url,
                }
            }
        }).data("kendoDropDownList");
    }
}

let faq = new Faq();