var Publication = /** @class */ (function () {
    function Publication() {
        var _this = this;
        this.placeholderAttachmentPositionChange = function (element) {
            return element.clone().css("opacity", 0.1);
        };
        this.hintAttachmentPositionChange = function (element) {
            return element.clone().removeClass("k-state-selected");
        };
        this.onAttachmentPositionChange = function (e) {
            var listView = $(e.sender.element).data("kendoListView");
            if (!listView) {
                return;
            }
            var dataSource = listView.dataSource, newIndex = e.newIndex, dataItem = dataSource.getByUid(e.item.data("uid"));
            dataSource.remove(dataItem);
            dataSource.insert(newIndex, dataItem);
        };
        this.onPublicationAttachmentSuccessUpload = function (e) {
            var upload = e.sender, listView = $(upload.options.dropZone).data("kendoListView");
            if (e.operation == "upload" && e.response && e.response.files && e.response.files.length > 0) {
                for (var i = 0; i < e.response.files.length; i++) {
                    if (listView.dataSource.data().length < 30) {
                        listView.dataSource.add(e.response.files[i]);
                    }
                    else {
                        notification.displayMessage(resources.MaximumImageCount, "error");
                        return false;
                    }
                }
            }
        };
        this.onImageSelect = function (e) {
            var self = _this;
            // A timeout so you do not break the built-in features and throw errors.
            // Note that the file size that is shown by the Upload will therefore be wrong.
            // Consider using a template to hide it: https://demos.telerik.com/aspnet-mvc/upload/templates.
            setTimeout(function () {
                self.openResizeImage(e);
            });
        };
        this.onRemovePublicationClick = function (e) {
            var sender = $(e.currentTarget);
            var grid = sender.closest(".k-widget.k-grid").data("kendoGrid");
            var dataItem = searchTable.getSelectedItemByTr(sender.closest("tr"));
            if (!dataItem) {
                return;
            }
            var searchQueryId = grid.wrapper.data("searchqueryid");
            core.confirmDelete(function () {
                core.requestOptional("Delete", "Publication", {
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
        this.onMainPictureCheckBoxChange = function (e) {
            var sender = $(e.currentTarget);
            if (!sender) {
                return;
            }
            var listView = $(e.currentTarget).closest(".k-widget.k-listview").data("kendoListView");
            if (!listView) {
                return;
            }
            var dataItem = listView.dataSource.get(sender.data("uid"));
            if (!dataItem) {
                return;
            }
            dataItem["IsMain"] = e.currentTarget.checked;
            var checkboxes = $(".imageDiv>label>input[type='checkbox']");
            if (!checkboxes) {
                return;
            }
            checkboxes.each(function (index, elem) {
                if (elem != e.currentTarget) {
                    $(elem).prop("checked", false);
                }
            });
            listView.dataItems().forEach(function (item) {
                if (item != dataItem) {
                    item["IsMain"] = false;
                }
            });
            listView.refresh();
        };
        // https://docs.telerik.com/aspnet-core/knowledge-base/upload-resize-image-before-upload
        this.openResizeImage = function (e) {
            var uploader = e.sender;
            var file = e.files[0];
            var id = kendo.guid(), buttonId = kendo.guid();
            var content = '<div class="form-control fullwidth">' +
                ("<img id='" + id + "'></img>") +
                '</div>' +
                '<div class="clear"></div>' +
                '<div class="form-control fullwidth">' +
                ("<button id='" + buttonId + "' class='bttn main caps right' type='button'>" + resources.Save + "</button>") +
                '</div>';
            core.openKendoWindowContent(content, { title: resources.ResizeImage, width: '80%', height: '80%' });
            $("#" + buttonId).one("click", function (evt) {
                var kendoWindow = $(evt.currentTarget).closest(".k-window-content").data("kendoWindow");
                var cropper = $("#" + id).data("cropper");
                if (cropper.cropped === true) {
                    var isIE = navigator.userAgent.indexOf("MSIE ") > -1 || navigator.userAgent.indexOf("Trident/") > -1;
                    if (isIE) {
                        if (!HTMLCanvasElement.prototype.toBlob) {
                            Object.defineProperty(HTMLCanvasElement.prototype, 'toBlob', {
                                value: function (callback, type, quality) {
                                    var dataURL = this.toDataURL(type, quality).split(',')[1];
                                    setTimeout(function () {
                                        var binStr = atob(dataURL), len = binStr.length, arr = new Uint8Array(len);
                                        for (var i = 0; i < len; i++) {
                                            arr[i] = binStr.charCodeAt(i);
                                        }
                                        callback(new Blob([arr], { type: type || 'image/png' }));
                                    });
                                }
                            });
                        }
                    }
                    var croppedCanvas = cropper.getCroppedCanvas();
                    croppedCanvas.toBlob(function (blob) {
                        blob.lastModifiedDate = new Date();
                        blob.name = file.name;
                        // Replace the original files with the new files.
                        try {
                            file.size = blob.size;
                            file.rawFile = new File([blob], file.name);
                        }
                        catch (err) {
                            var textFileAsBlob = new Blob([blob], { type: 'application/xml' });
                            window.navigator.msSaveBlob(textFileAsBlob, file.name);
                        }
                        // Force to upload modify image - async.autoUpload must be set to false
                        uploader.upload();
                        if (kendoWindow) {
                            kendoWindow.close();
                        }
                    });
                }
            });
            var reader = new FileReader();
            reader.onload = function (e) {
                var image = $("#" + id);
                image.attr("src", this.result);
                image.cropper({
                    dragMode: 'move',
                    cropBoxResizable: false,
                    data: {
                        width: uploader.element.data("width"),
                        height: uploader.element.data("height")
                    }
                });
            };
            reader.readAsDataURL(file.rawFile);
        };
        this.init();
    }
    Publication.prototype.init = function () {
        var self = this;
        core.rebindEvent("change", ".imageDiv>label>input[type='checkbox']", self.onMainPictureCheckBoxChange);
        core.rebindEvent("click", ".file-remove-js", core.onRemoveFileClick);
        core.rebindEvent("click", ".publication-remove-js", self.onRemovePublicationClick);
        core.bindBodyClick();
    };
    ;
    return Publication;
}());
var publication = new Publication();
