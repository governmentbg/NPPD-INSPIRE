var BackgroundImages = /** @class */ (function () {
    function BackgroundImages() {
        var _this = this;
        this.init = function () {
            var self = _this;
            core.rebindEvent("click", ".openUploader-js", self.openUploader);
            core.rebindEvent("click", ".delete-background-image-js", self.onDeleteBackgroundImageClick);
        };
        this.onDeleteBackgroundImageClick = function (e) {
            var sender = $(e.currentTarget);
            var element = sender.closest("[role='listitem']");
            var listView = sender.closest(".k-widget.k-listview").data("kendoListView");
            var selectedItem = listView.dataItem(element);
            if (!selectedItem) {
                return;
            }
            var image = selectedItem.toJSON();
            core.confirmDelete(function () {
                core.requestOptional("Delete", "BackgroundImages", {
                    area: "Admin",
                    useArea: true,
                    type: "DELETE",
                    data: {
                        url: image["Url"]
                    },
                    success: function () {
                        listView.dataSource.remove(selectedItem);
                        notification.displayMessage(resources.DeleteSuccess, "success");
                    }
                });
                return true;
            });
        };
        this.onEditorSaveButtonClick = function (e) {
            var imageEditor = $("#imageEditor").data("kendoImageEditor");
            var base64Image = imageEditor['_image'];
            if (!base64Image) {
                return;
            }
            var base64ImageSrc = base64Image.currentSrc;
            $("head").append("<img id='hiddenImage' src='" + base64ImageSrc + "' />");
            var element = $("#hiddenImage")[0]['height'];
            var minimumHeight = 570;
            $('#hiddenImage').remove();
            if (element < minimumHeight) {
                notification.displayMessage(resources.MinimalHeightRequirement, "warning");
                return;
            }
            var blob = _this.getBlob(base64ImageSrc);
            var file = new Blob(blob, { type: 'image/jpeg' });
            var filename = "backgroundimage.jpg";
            var formData = new FormData();
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
                        var oldImagePath = $('input:hidden[name=oldImagePath]').val();
                        core.requestOptional("Upsert", "BackgroundImages", {
                            area: "Admin",
                            useArea: true,
                            type: "POST",
                            data: {
                                backgroundImage: response.files[0],
                                editedImgePath: oldImagePath
                            },
                            success: function (res) {
                                if (res.success) {
                                    var kwindow = $(".k-window-content").data("kendoWindow");
                                    kwindow.close();
                                    var listView = $("#imageListView").data("kendoListView");
                                    listView.dataSource.data(res.images);
                                    listView.refresh();
                                    notification.displayMessage(resources.Success, "success");
                                }
                            }
                        });
                    }
                }
            });
        };
        this.openUploader = function (e) {
            var path = $(e.currentTarget).data("url");
            core.openKendoWindow("Upsert", "BackgroundImages", {
                area: "Admin",
                useArea: true,
                data: {
                    path: path
                }
            }, {
                width: 1024,
                height: 640,
                title: resources.BackgroundImage
            });
        };
        this.getBlob = function (base64string) {
            //// ако се редактира background image-a, но все още няма промени по самата снимка, тук вместо base64 стринг е url пътя към снимката
            if (base64string.substr(0, 4) === "http") {
                notification.displayMessage(resources.NoChangesDetected, "warning");
                return;
            }
            var byteString = atob(base64string.split(',')[1]);
            var ab = new ArrayBuffer(byteString.length);
            var ia = new Uint8Array(ab);
            for (var i = 0; i < byteString.length; i++) {
                ia[i] = byteString.charCodeAt(i);
            }
            return [ab];
        };
        this.init();
    }
    return BackgroundImages;
}());
var backgroundImages = new BackgroundImages();
