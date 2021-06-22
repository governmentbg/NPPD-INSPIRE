var Provider = /** @class */ (function () {
    function Provider() {
        this.onChangeOrder = function (e) {
            var oldIndex = e.oldIndex;
            var newIndex = e.newIndex;
            core.requestOptional("Rearrange", "Provider", {
                type: "POST",
                useArea: false,
                data: {
                    oldIndex: oldIndex,
                    newIndex: newIndex
                }
            });
        };
        this.onRemoveProviderClick = function (e) {
            var sender = $(e.currentTarget);
            var grid = sender.closest(".k-widget.k-grid").data("kendoGrid");
            var dataItem = searchTable.getSelectedItemByTr(sender.closest("tr"));
            var searchQueryId = grid.wrapper.data("searchqueryid");
            if (!dataItem) {
                return;
            }
            core.confirmDelete(function () {
                core.requestOptional("Delete", "Provider", {
                    type: "DELETE",
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
        this.onProviderAttachmentSuccessUpload = function (e) {
            if (e.response != "") {
                var response = e.response.files[0];
                $("#MainPicture_Name").val(response.Name);
                $("#MainPicture_Url").val(response.Url);
                $("#MainPicture_Size").val(response.Size);
                $("#MainPicture_MimeType").val(response.MimeType);
                $("#imageDiv").show();
                var listview = $("#imageListView").data("kendoListView");
                listview.dataSource.data().remove(listview.dataSource.data()[0]);
                listview.dataSource.add(response);
            }
        };
        this.init();
    }
    Provider.prototype.init = function () {
        var self = this;
        core.rebindEvent("click", ".provider-remove-js", self.onRemoveProviderClick);
        core.bindBodyClick();
    };
    ;
    return Provider;
}());
var provider = new Provider();
