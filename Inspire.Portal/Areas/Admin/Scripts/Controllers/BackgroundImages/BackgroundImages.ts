class BackgroundImages {
    constructor() {
        this.init();
    }

    private init = () => {
        let self = this;

        core.rebindEvent("click", ".openUploader-js", self.openUploader);
        core.rebindEvent("click", ".delete-background-image-js", self.onDeleteBackgroundImageClick)
    }

    public onDeleteBackgroundImageClick = (e) => {
        let sender = $(e.currentTarget);
        let element = sender.closest("[role='listitem']");
        let listView = sender.closest(".k-widget.k-listview").data("kendoListView");
        let selectedItem = listView.dataItem(element);
        if (!selectedItem) {
            return;
        }

        let image = selectedItem.toJSON();
        core.confirmDelete(
            () => {
                core.requestOptional(
                    "Delete",
                    "BackgroundImages",
                    {
                        area: "Admin",
                        useArea: true,
                        type: "DELETE",
                        data: {
                            url: image["Url"]
                        },
                        success: () => {
                            listView.dataSource.remove(selectedItem);
                            notification.displayMessage(resources.DeleteSuccess, "success");
                        }
                    })
                return true;
            });
    }

    public onEditorSaveButtonClick = (e) => {
        let imageEditor = $("#imageEditor").data("kendoImageEditor");
        let base64Image = imageEditor['_image'];
        if (!base64Image) {
            return;
        }
        let base64ImageSrc = base64Image.currentSrc;
        $("head").append("<img id='hiddenImage' src='" + base64ImageSrc + "' />");
        let element: number = $("#hiddenImage")[0]['height'];
        let minimumHeight = 570;
        $('#hiddenImage').remove();
        if (element < minimumHeight) {
            notification.displayMessage(resources.MinimalHeightRequirement, "warning");
            return;
        }
        let blob = this.getBlob(base64ImageSrc)
        let file = new Blob(blob, { type: 'image/jpeg' });
        let filename = "backgroundimage.jpg";
        let formData = new FormData();
        formData.append("imageUploader", file, filename);
        $.ajax({
            type: "POST",
            url: core.getPathToActionMethod("Upload", "Attachment", {
                area: "",
                useArea: false
            }),
            data: formData,
            dataType: "json",
            contentType: false,
            processData: false,
            success: function (response) {
                if (response && response.files && response.files[0].Url) {
                    let oldImagePath = $('input:hidden[name=oldImagePath]').val()
                    core.requestOptional("Upsert", "BackgroundImages", {
                        area: "Admin",
                        useArea: true,
                        type: "POST",
                        data: {
                            backgroundImage: response.files[0],
                            editedImgePath: oldImagePath
                        },
                        success: (res) => {
                            if (res.success) {
                                let kwindow = $(".k-window-content").data("kendoWindow");
                                kwindow.close();
                                let listView = $("#imageListView").data("kendoListView");
                                listView.dataSource.data(res.images);
                                listView.refresh();
                                notification.displayMessage(resources.Success, "success");
                            }
                        }
                    });
                }
            }
        });
    }

    private openUploader = (e) => {
        let path = $(e.currentTarget).data("url");
        core.openKendoWindow(
            "Upsert",
            "BackgroundImages",
            {
                area: "Admin",
                useArea: true,
                data: {
                    path: path
                }
            },
            {
                width: 1024,
                height: 640,
                title: resources.BackgroundImage
            });
    }


    private getBlob = (base64string: string) => {
        //// ако се редактира background image-a, но все още няма промени по самата снимка, тук вместо base64 стринг е url пътя към снимката
        if (base64string.substr(0, 4) === "http") {
            notification.displayMessage(resources.NoChangesDetected, "warning");
            return;
        }

        let byteString = atob(base64string.split(',')[1]);
        let ab = new ArrayBuffer(byteString.length);
        let ia = new Uint8Array(ab);

        for (let i = 0; i < byteString.length; i++) {
            ia[i] = byteString.charCodeAt(i);
        }

        return [ab];
    }
}

let backgroundImages = new BackgroundImages();