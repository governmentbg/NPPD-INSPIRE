var Faq = /** @class */ (function () {
    function Faq() {
        var _this = this;
        this.onFaqCategoryUpsertSuccessfully = function (e) {
            var categoriesDropDownList = $("#Category_Id").data("kendoDropDownList");
            categoriesDropDownList.dataSource.read();
            $("#CategoriesGrid").data("kendoGrid").dataSource.read();
            var window = $(".k-window-content:last").data("kendoWindow");
            if (!window) {
                return;
            }
            window.close();
        };
        this.onChangeOrder = function (e) {
            var oldIndex = e.oldIndex;
            var newIndex = e.newIndex;
            core.requestOptional("Rearrange", "Faq", {
                type: "POST",
                useArea: false,
                data: {
                    oldIndex: oldIndex,
                    newIndex: newIndex
                }
            });
        };
        this.Init = function () {
            var self = _this;
            self.RebindEvents();
        };
        this.RebindEvents = function () {
            var self = _this;
            core.rebindEvent("click", ".manageFAQCategory-js", self.ManageCategory);
            core.rebindEvent("click", ".faqDelete-js", self.DeleteFAQ);
            core.rebindEvent("click", ".remove-category-js", self.DeleteFAQCategory);
            core.rebindEvent("click", ".add-category-js,.edit-category-js", self.UpsertCategory);
            core.rebindEvent("click", ".searchByWord-js", self.searchFaq);
            core.bindBodyClick();
        };
        this.searchFaq = function (e) {
            e.preventDefault();
            var word = $("#SearchWord").val();
            var categoryid = $("#CategoryId").val();
            core.requestOptional("SearchFaq", "Home", {
                type: "POST",
                useArea: false,
                data: {
                    categoryid: categoryid,
                    searchword: word
                },
                success: function (res) {
                    $("#faq-result").html(res);
                }
            });
        };
        this.UpsertCategory = function (e) {
            e.preventDefault();
            var grid = $("#CategoriesGrid").data("kendoGrid");
            var tr = $(e.currentTarget).closest("tr");
            var dataIem = null;
            if (tr) {
                dataIem = grid.dataItem(tr);
            }
            core.openKendoWindow("UpsertCategory", "Faq", {
                type: "GET",
                useArea: false,
                data: {
                    id: dataIem != null ? dataIem["Id"] : ''
                }
            }, {
                close: function (e) {
                    if (e.userTriggered) {
                        return;
                    }
                },
                title: dataIem != null ? dataIem["Name"] : resources.CreateCategory
            });
        };
        this.DeleteFAQ = function (e) {
            e.preventDefault();
            var sender = $(e.currentTarget);
            var grid = sender.closest(".k-widget.k-grid").data("kendoGrid");
            var dataItem = searchTable.getSelectedItemByTr(sender.closest("tr"));
            var searchQueryId = grid.wrapper.data("searchqueryid");
            if (!dataItem) {
                return;
            }
            core.confirmDelete(function () {
                core.requestOptional("Delete", "Faq", {
                    type: "POST",
                    useArea: false,
                    data: {
                        id: dataItem["Id"],
                        searchQueryId: searchQueryId
                    },
                    success: function () {
                        core.onDeleteMvcTableItem(grid, dataItem);
                        notification.displayMessage(resources.DeleteSuccess, "success");
                    }
                });
                return true;
            });
        };
        this.ManageCategory = function (e) {
            e.preventDefault();
            core.openKendoWindow("ManageCategory", "Faq", {
                type: "GET",
                useArea: false
            }, {
                close: function (e) {
                    if (e.userTriggered) {
                        return;
                    }
                },
                title: resources.ManageCategory
            });
        };
        this.DeleteFAQCategory = function (e) {
            e.preventDefault();
            var sender = $(e.currentTarget);
            var faqCategoryId = sender.data("id");
            var actions = [
                {
                    text: resources.Yes,
                    action: function () {
                        core.requestOptional("DeleteCategory", "Faq", {
                            type: "DELETE",
                            useArea: false,
                            data: {
                                id: faqCategoryId,
                                newCategoryId: $("#newCategory").val()
                            },
                            success: function () {
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
            var categoryName = $("#CategoriesGrid").data("kendoGrid").dataItem(sender.closest("tr"))["Name"];
            var dialogContent = resources.ConfirmDeletingCategory.replace("{0}", categoryName) +
                "<div class='form-input'>" +
                ("<label>" + resources.TransferCategory + "</label>") +
                "<input id='newCategory'/>" +
                "</div>";
            core.createKendoDialog({
                title: resources.Deleting,
                content: dialogContent,
                visible: true,
                actions: actions
            });
            var url = core.getPathToActionMethod("GetFaqFilteredCategories", "Faq") + "?categoryId=" + faqCategoryId;
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
        };
        this.Init();
    }
    return Faq;
}());
var faq = new Faq();
