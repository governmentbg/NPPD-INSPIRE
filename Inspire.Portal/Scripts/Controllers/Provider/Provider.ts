class Provider {
    constructor() {
        this.init()
    }

    private init() {
        let self = this;
        core.rebindEvent("click", ".provider-remove-js", self.onRemoveProviderClick);
        core.bindBodyClick();
    };


    public onChangeOrder = (e) => {
        let oldIndex = e.oldIndex;
        let newIndex = e.newIndex;

        core.requestOptional("Rearrange", "Provider",
            {
                type: "POST",
                useArea: false,
                data: {
                    oldIndex,
                    newIndex
                }
            });
    }

    private onRemoveProviderClick = (e) => {
        let sender = $(e.currentTarget);
        let grid = sender.closest(".k-widget.k-grid").data("kendoGrid") as kendo.ui.Grid;
        let dataItem = searchTable.getSelectedItemByTr(sender.closest("tr"));
        let searchQueryId = grid.wrapper.data("searchqueryid")

        if (!dataItem) {
            return;
        }

        core.confirmDelete(
            () => {
                core.requestOptional(
                    "Delete",
                    "Provider",
                    {
                        type: "DELETE",
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

    public onProviderAttachmentSuccessUpload = (e) => {
        if (e.response != "") {
            let response = e.response.files[0];
            $("#MainPicture_Name").val(response.Name);
            $("#MainPicture_Url").val(response.Url);
            $("#MainPicture_Size").val(response.Size);
            $("#MainPicture_MimeType").val(response.MimeType);

            $("#imageDiv").show();
            let listview = $("#imageListView").data("kendoListView") as kendo.ui.ListView;
            listview.dataSource.data().remove(listview.dataSource.data()[0]);
            listview.dataSource.add(response);
        }
    }
}

let provider = new Provider();