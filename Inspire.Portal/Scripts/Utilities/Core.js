/// <reference path="../typings/jquery/jquery.d.ts" />
/// <reference path="../typings/kendo-ui/kendo.all.d.ts" />
/// <reference path="../typings/globalize/globalize.d.ts" />
/// <reference path="../typings/globalize/globalize-0.1.3.d.ts" />
/// <reference path="../typings/jquery.validation/jquery.validation.d.ts" />
/// <reference path="../typings/jquery-validation-unobtrusive/jquery-validation-unobtrusive.d.ts"/>
var Core = /** @class */ (function () {
    function Core() {
        var _this = this;
        this.confirmDelete = function (action) {
            var actions = [
                {
                    text: resources.Yes,
                    action: action,
                    primary: true
                },
                {
                    text: resources.No
                }
            ];
            core.createKendoDialog({
                title: resources.Deleting,
                content: resources.ConfirmDeleting,
                visible: true,
                actions: actions
            });
        };
        this.copyToClipboard = function (str) {
            var el = document.createElement('textarea');
            el.value = str;
            el.setAttribute('readonly', '');
            el.style.position = 'absolute';
            el.style.left = '-9999px';
            document.body.appendChild(el);
            el.select();
            document.execCommand('copy');
            document.body.removeChild(el);
        };
        this.onValueChangeSetDirtyForm = function (e) {
            var sender = e.sender
                ? e.sender.element
                : $(e.currentTarget);
            var form = sender.closest("form");
            if (!form || !form.hasClass($.DirtyForms.listeningClass)) {
                return;
            }
            form.dirtyForms('rescan');
        };
        this.onRemoveFileClick = function (e) {
            var sender = $(e.currentTarget);
            var element = sender.closest("[role='listitem']");
            var listView = sender.closest(".k-widget.k-listview").data("kendoListView");
            var selectedItem = listView.dataItem(element);
            if (!selectedItem) {
                return;
            }
            core.confirmDelete(function () {
                listView.dataSource.remove(selectedItem);
                return true;
            });
        };
        this.initImageMagnificPopup = function (selector) {
            if (selector === void 0) { selector = null; }
            selector = selector ? selector : ".magnet, .p-img a";
            $(selector).magnificPopup({ type: "image", gallery: { enabled: true } });
        };
        this.onChangeDropDownType = function (e) {
            var dropDownList = e.sender;
            var dropDownItem = dropDownList.dataItem();
            if (!dropDownItem) {
                return;
            }
            var templateWrapper = dropDownList.wrapper.closest(".offerFile");
            var uniqueId = templateWrapper.find("[name$='.UniqueId']").val();
            var listView = templateWrapper.closest(".k-widget.k-listview").data("kendoListView");
            var listDataItem = listView.dataSource.get(uniqueId);
            if (listDataItem) {
                listDataItem.set("Type", {
                    Id: dropDownItem.Id,
                    Name: dropDownItem.Name
                });
            }
        };
        this.requestOptional = function (action, controller, optional) {
            var self = _this;
            if (optional == null) {
                optional = {};
            }
            var isPost = typeof optional.type !== 'undefined' && optional.type != null && ["POST", "DELETE"].indexOf(optional.type.toUpperCase()) > -1;
            var ajaxSettings = {
                data: isPost
                    ? JSON.stringify(optional.data)
                    : optional.data,
                contentType: optional.contentType ? optional.contentType : "application/json; charset=utf-8",
                headers: isPost
                    ? { "__RequestVerificationToken": $("input[name='__RequestVerificationToken']:first").val() }
                    : null,
                success: function (d, e, t) {
                    if (optional.success) {
                        optional.success(d, e, t);
                    }
                },
                error: function (d, e, t) {
                    if (optional.error) {
                        optional.error(d, e, t);
                    }
                },
                complete: function (d, e) {
                    if (optional.complete) {
                        optional.complete(d, e);
                    }
                },
                async: (typeof optional.async === "undefined" || optional.async == null) ? true : optional.async,
                type: optional.type || "GET",
                traditional: optional.traditional === true,
                global: (typeof optional.global === "undefined" || optional.global == null) ? true : optional.global,
                cache: optional.cache
            };
            return $.ajax(self.getPathToActionMethod(action, controller, {
                area: optional.area,
                useArea: optional.useArea
            }), ajaxSettings);
        };
        this.openKendoWindow = function (action, controller, optional, windowOptions, enableInputs) {
            if (enableInputs === void 0) { enableInputs = true; }
            var self = _this;
            if (!optional) {
                optional = {};
            }
            var oldSuccessCallback = optional.success;
            if (!oldSuccessCallback) {
                optional.success = function (data) {
                    if (!enableInputs) {
                        data = self.enableInputs($(data), enableInputs);
                    }
                    self.openKendoWindowContent(data, windowOptions);
                };
            }
            self.requestOptional(action, controller, optional);
        };
        this.goToCorrectGridPage = function (grid) {
            var dataSource = grid.dataSource;
            var currentPage = dataSource.page();
            var totalPages = dataSource.totalPages();
            if (currentPage > totalPages && dataSource.total() > 0) {
                dataSource.page(totalPages);
                return true;
            }
            return false;
        };
        this.createKendoDialog = function (options, divClass) {
            if (options === void 0) { options = null; }
            if (divClass === void 0) { divClass = null; }
            options = options != null ? options : {};
            options.visible = options.visible
                ? options.visible
                : false;
            options.closable = options.closable
                ? options.closable
                : true;
            options.modal = options.modal
                ? options.modal
                : true;
            options.close = options.close
                ? options.close
                : function () {
                    dialog.destroy();
                };
            var dialog = $(divClass ? '<div class="' + divClass + '"/>' : '<div />')
                .appendTo('body')
                .kendoDialog(options)
                .data("kendoDialog");
            return dialog;
        };
        this.onAjaxFormSuccess = function (data, selector) {
            if (data) {
                $(selector).html(data);
            }
        };
        this.removeListViewItem = function (e) {
            var sender = $(e.currentTarget);
            var li = sender.closest("li");
            var list = sender.closest(".k-widget.k-listview").data("kendoListView");
            var selectedItem = list.dataItem(li);
            if (!selectedItem) {
                return;
            }
            var actions = [
                {
                    text: resources.Yes,
                    action: function () {
                        list.dataSource.remove(selectedItem);
                        return true;
                    },
                    primary: true
                },
                {
                    text: resources.No
                }
            ];
            core.createKendoDialog({
                title: resources.DeleteTitle,
                content: resources.ConfirmDeleteSelectedItem,
                visible: true,
                actions: actions
            });
        };
        this.bindBodyClick = function () {
            var self = _this;
            self.rebindEvent("click", "body", self.onBodyClick);
        };
        this.onGridFilterOnClient = function (e) {
            if (e.filter && e.filter.filters.length > 0) {
                for (var i = 0; i < e.filter.filters.length; i++) {
                    e.filter.filters[i].value = e.filter.filters[i].value
                        ? e.filter.filters[i].value.trim()
                        : e.filter.filters[i].value;
                }
            }
        };
        this.initUploadTemplate = function (upload, file) {
            var self = _this;
            var size = self.getFileSizeMessage(file);
            file.url = file.url ? file.url : '';
            file.description = file.description ? file.description : '';
            var name = upload.element.attr("Name");
            var template;
            if (upload.options.multiple) {
                template =
                    "<input type=\"hidden\" name=\"" + name + ".Index\" value=\"#: uid #\" />" +
                        ("<input type=\"hidden\" name=\"" + name + "[#: uid #].Name\" value=\"#: name #\" />") +
                        ("<input type=\"hidden\" name=\"" + name + "[#: uid #].Size\" value=\"#: size #\" />") +
                        ("<input type=\"hidden\" name=\"" + name + "[#: uid #].Url\" value=\"#: url #\" />") +
                        ("<input type=\"hidden\" name=\"" + name + "[#: uid #].Description\" value=\"#: description #\" />");
            }
            else {
                template =
                    "<input type=\"hidden\" name=\"" + name + ".Name\" value=\"#: name #\" />" +
                        ("<input type=\"hidden\" name=\"" + name + ".Size\" value=\"#: size #\" />") +
                        ("<input type=\"hidden\" name=\"" + name + ".Url\" value=\"#: url #\" />") +
                        ("<input type=\"hidden\" name=\"" + name + ".Description\" value=\"#: description #\" />");
            }
            template = '<span class="k-progress"></span>' +
                '<span class="k-file-extension-wrapper">' +
                '<span class="k-file-extension">#: extension #</span>' +
                '<span class="k-file-state"></span>' +
                '</span>' +
                '<span class="k-file-name-size-wrapper">' +
                ("" + template) +
                '<a href="#: url #" target="_blank" class="k-file-name" title="#: name #">#: name #</a>' +
                ("<span class=\"k-file-size\">" + size + "</span>") +
                '</span>' +
                '<strong class="k-upload-status">' +
                '<button type="button" class="k-upload-action"></button>' +
                '<button type="button" class="k-upload-action"></button>' +
                '</strong>';
            return kendo.template(template)(file);
        };
        this.onUploadSuccess = function (e) {
            var self = _this;
            if (e.operation === "upload") {
                for (var i = 0; i < e.files.length; i++) {
                    var attachment = e.response.files[i];
                    if (attachment) {
                        e.files[i]["url"] = attachment["Url"] ? attachment["Url"] : "#";
                        e.files[i]["description"] = attachment["Description"];
                        $("li[data-uid='" + e.files[i]["uid"] + "']").html(self.initUploadTemplate(e.sender, e.files[i]));
                    }
                }
            }
        };
        this.onUploadRemove = function (e) {
            var urls = [];
            for (var i = 0; i < e.files.length; i++) {
                if (e.files[i]["url"]) {
                    urls.push(e.files[i]["url"]);
                }
            }
            e.data = {
                urls: urls
            };
        };
        this.renderMenuButton = function (templateId, item) {
            var templateContent = $("#" + templateId).html();
            var template = kendo.template(templateContent);
            var html = template(item);
            return $(html).find("li:first").length > 0
                ? html
                : "";
        };
        this.persistGridPageSize = function (gridSelector) {
            var grid = $(gridSelector).data("kendoGrid");
            if (!grid) {
                return;
            }
            window.addEventListener("beforeunload", function (e) {
                var pageSizesDropDownList = grid.element.find(".k-pager-sizes:first select:first").data("kendoDropDownList");
                if (pageSizesDropDownList) {
                    localStorage["grid-page-size"] = pageSizesDropDownList.value();
                }
                return;
            });
            var pageSize = parseInt(localStorage["grid-page-size"])
                ? parseInt(localStorage["grid-page-size"])
                : localStorage["grid-page-size"];
            if (pageSize && grid.options.pageable) {
                grid.dataSource.pageSize(pageSize === 'all' ? grid.dataSource.total() : pageSize);
                grid.refresh();
            }
        };
        this.onPanelBarSelectExpandCollapse = function (e) {
            if ($(e.item).is(".k-state-active")) {
                window.setTimeout(function () {
                    e.sender.collapse(e.item, true);
                }, 1);
            }
        };
        this.getFileSizeMessage = function (file) {
            var totalSize = 0;
            if (typeof file.size == 'number') {
                totalSize = file.size;
            }
            else {
                return '';
            }
            totalSize /= 1024;
            if (totalSize < 1024) {
                return totalSize.toFixed(2) + ' KB';
            }
            else {
                return (totalSize / 1024).toFixed(2) + ' MB';
            }
        };
        this.init = function () {
            var self = _this;
            self.initCulture();
            self.predefineMethods();
            self.bindAjaxAction();
            self.customBinding();
            $.fn.andSelf = function () { return this.addBack.apply(this, arguments); };
            // Override the default ignore setting after including the scripts to enable validation of hidden elements. For instance, widgets like the Kendo UI DropDownList and NumericTextBox have a hidden input to keep the value.
            // http://docs.telerik.com/kendo-ui/aspnet-mvc/validation#use-dataAnnotation-attributes
            $.validator.setDefaults({
                ignore: ".ignore, :hidden:not(:visible.k-widget input)"
            });
            $.validator.unobtrusive.adapters.add("regexwithoptions", ["pattern", "flags"], function (options) {
                options.messages['regexwithoptions'] = options.message;
                options.rules['regexwithoptions'] = options.params;
            });
            // Globalized Dates and Numbers Are Not Recognized As Valid When Using jQuery Validation
            // http://docs.telerik.com/kendo-ui/aspnet-mvc/troubleshoot/troubleshooting-validation
            $.extend($.validator.methods, {
                date: function (value, element) {
                    return this.optional(element) || kendo.parseDate(value) != null || parseInt(value) != null;
                },
                number: function (value, element) {
                    return this.optional(element) || kendo.parseFloat(value) != null;
                },
                regexwithoptions: function (value, element, params) {
                    var match;
                    if (this.optional(element)) {
                        return true;
                    }
                    var reg = new RegExp(params.pattern, params.flags);
                    match = reg.exec(value);
                    return (match && (match.index === 0) && (match[0].length === value.length));
                }
            });
            $(document).ready(function () {
                // Use kendo for select inputs
                $("select").each(function () {
                    var element = $(this);
                    if (!element.closest(".k-widget")) {
                        element.kendoDropDownList();
                    }
                });
                // Widgets Are Hidden after Postbacks When Using jQuery Validation
                // http://docs.telerik.com/kendo-ui/aspnet-mvc/troubleshoot/troubleshooting-validation
                $(".k-widget").removeClass("input-validation-error");
                $(document).on("submit", "form", function (e) {
                    var sender = $(e.currentTarget);
                    if (sender.find('.input-validation-error').length < 1 && sender.data("ajax") !== true) {
                        if (sender.data("submitted") === true) {
                            e.preventDefault();
                            return false;
                        }
                        sender.data("submitted", true);
                        return true;
                    }
                });
            });
            self.rebindEvent("blur", "input[type='text']", function () {
                var element = $(this);
                var value = element.val();
                if (value) {
                    element.val(value.replace(/\s+/g, ' ').trim());
                }
            });
            self.rebindEvent("click", ".logout-js", self.onLogOutClick);
            self.rebindEvent("click", ".saveButton", self.validatePanelBarItems);
            self.rebindEvent("submit", "#searchRecordsForm", self.onGeonetworkSearchFormSubmit);
            self.initImageMagnificPopup();
            self.initDirtyForm();
        };
        this.initCulture = function () {
            var currentCulture = $('html').attr('lang');
            if (currentCulture == "bg-BG") {
                var globalisationCulture = Globalize.cultures[currentCulture];
                var customBg = $.extend(true, {}, globalisationCulture, {
                    numberFormat: {
                        ",": " ",
                        ".": ".",
                        currency: {
                            ",": " ",
                            ".": ".",
                        },
                        percent: {
                            ",": " ",
                            ".": ".",
                        }
                    }
                });
                Globalize.cultures[currentCulture] = customBg;
                var customKendoBg = $.extend(true, {}, kendo.cultures[currentCulture], {
                    name: currentCulture,
                    numberFormat: {
                        ",": " ",
                        ".": ".",
                        currency: {
                            ",": " ",
                            ".": ".",
                        },
                        percent: {
                            ",": " ",
                            ".": ".",
                        }
                    }
                });
                kendo.cultures[currentCulture] = customKendoBg;
                currentCulture = currentCulture;
            }
            Globalize.culture(currentCulture);
            kendo.culture(currentCulture);
            var kendoCulture = kendo.culture();
            if (currentCulture != currentCulture) {
                var jQueryCulture = Globalize.cultures[currentCulture];
                jQueryCulture.numberFormat.currency.symbol = "bgn";
                jQueryCulture.numberFormat.currency.pattern[0] = "-n $";
                jQueryCulture.numberFormat.currency.pattern[1] = "n $";
                kendoCulture.numberFormat.currency.symbol = "bgn";
                kendoCulture.numberFormat.currency.pattern[0] = "-n%";
                kendoCulture.numberFormat.currency.pattern[1] = " n%";
            }
            var decimals = 6;
            //http://docs.telerik.com/kendo-ui/controls/editors/numerictextbox/overview#known-limitations
            // Set precision limitation
            kendoCulture.numberFormat.decimals = decimals;
        };
        this.onGeonetworkSearchFormSubmit = function (e) {
            var form = $(e.currentTarget);
            form.attr("action", kendo.format(form.data("url"), form.find("#any:first").val()));
        };
        this.getProgressElement = function () {
            return $('body');
        };
        this.customBinding = function () {
            var self = _this;
            self.rebindEvent("click", ".closeKendoWindow-js", function (e) {
                e.preventDefault();
                var window = $(this).closest(".k-window-content").data("kendoWindow");
                if (window) {
                    window.close();
                }
            });
        };
        this.onBodyClick = function (e) {
            var target = $(e.target);
            if ((target.hasClass("dropdown-trigger") || target.hasClass("icon currentColor"))
                && !target.parents('.btn-with-content').hasClass('openeddropdown')) {
                e.preventDefault();
                e.stopPropagation();
                $('.openeddropdown').removeClass('openeddropdown');
                target.parents('.btn-with-content').addClass('openeddropdown');
                var overflowcompensation = target.parents('.overwrite-table');
                if (overflowcompensation) {
                    var element = overflowcompensation.get(0);
                    if (element && element.clientHeight != element.scrollHeight) {
                        overflowcompensation.css({ paddingBottom: (target.parents('.btn-with-content').find('ul').innerHeight() - 53) + 'px' });
                    }
                }
            }
            else {
                $('.openeddropdown').removeClass('openeddropdown');
                $('.overwrite-table').attr('style', '');
            }
        };
        this.onLogOutClick = function (e) {
            e.preventDefault();
            var sender = $(e.currentTarget);
            var actions = [
                {
                    text: resources.Exit,
                    action: function () {
                        $.ajax(sender.prop("href"), { type: "POST" });
                        return true;
                    },
                    primary: true
                },
                {
                    text: resources.Stay
                }
            ];
            var dialog = core.createKendoDialog({ actions: actions });
            dialog.title(resources.Exit);
            dialog.content(resources.ConfirmProfileLogout);
            dialog.open();
        };
        this.initDirtyForm = function () {
            var self = _this;
            $.DirtyForms.dialog = {
                open: function (choice, message) {
                    var actions = [
                        {
                            text: resources.Yes,
                            action: function () {
                                choice.proceed = true;
                                return true;
                            },
                            primary: true
                        },
                        {
                            text: resources.No
                        }
                    ];
                    var dialog = self.createKendoDialog({
                        actions: actions,
                        close: function (e) {
                            dialog.destroy();
                            choice.commit(new Event("commit"));
                        }
                    });
                    dialog.title(resources.Warning);
                    dialog.content(message);
                    dialog.open();
                }
            };
        };
        this.init();
    }
    Core.prototype.filterData = function (e) {
        var data = {};
        if (e.filter.filters && e.filter.filters.length > 0) {
            for (var i = 0; i < e.filter.filters.length; i++) {
                var fieldName = e.filter.filters[i].field.toUpperCase() === "name".toUpperCase()
                    || e.filter.filters[i].field.toUpperCase() === "fullname".toUpperCase()
                    ? "text"
                    : e.filter.filters[i].field;
                data[fieldName] = e.filter.filters[i].value;
            }
        }
        return data;
    };
    Core.prototype.getBaseUrl = function () {
        var siteBase = globalVariables.SiteBase;
        var href = window.location.href;
        var b = href.split('/');
        href = b[0] + '//' + b[2] + siteBase;
        return href;
    };
    Core.prototype.getPathToActionMethod = function (action, controller, areaSettings) {
        var controllerArea = globalVariables ? globalVariables.ControllerArea : null;
        var area, useArea;
        if (areaSettings != null) {
            area = areaSettings.area;
            useArea = areaSettings.useArea;
        }
        var base = this.getBaseUrl();
        var areaData = "";
        area = !area || area.length === 0
            ? typeof controllerArea !== "undefined" ? controllerArea : ""
            : area;
        if ((useArea == undefined || useArea === true) && area) {
            areaData = area + "/";
        }
        var currentCulture = $('html').attr('lang').split("-")[0];
        var currentCultureData = "";
        if (typeof currentCulture !== "undefined" && currentCulture) {
            currentCultureData = currentCulture + "/";
        }
        return base + currentCultureData + areaData + controller + "/" + action + "/";
    };
    Core.prototype.rebindEvent = function (event, selector, callback) {
        $(document).off(event, selector);
        $(document).on(event, selector, callback);
    };
    Core.prototype.onKendoWindowSuccessCallback = function (data) {
        var window = $(this).closest(".k-window-content").data("kendoWindow");
        if (!window) {
            return;
        }
        var success = !core.isEmpty(data) && data.success === true;
        if (success === false) {
            window.content(data);
        }
        else {
            var keys = Object.keys(data);
            for (var i = 0; i < keys.length; i++) {
                var key = keys[i];
                $(window.element).data(key, data[key]);
            }
            if (data.refreshgrid && data.searchqueryid) {
                var grid = $(kendo.format("div[data-searchqueryid='{0}']", data.searchqueryid)).data("kendoGrid");
                if (grid) {
                    grid.dataSource.read();
                }
            }
            window.close();
        }
    };
    Core.prototype.getSelectedRowId = function (element) {
        var selectedItem = searchTable.getSelectedItem(element).toJSON();
        if (!selectedItem) {
            return;
        }
        return selectedItem["Id"];
    };
    Core.prototype.enableInputs = function (wrapper, enable) {
        if (!enable) {
            wrapper.find("form").each(function (i, form) {
                var formElement = $(form);
                formElement.replaceWith(formElement.html());
            });
        }
        wrapper.find("input, textarea, select").each(function (i, item) {
            var jqueryElement = $(item);
            var role = jqueryElement.data("role");
            var kendoType = "";
            var cascadeFrom = "";
            if (role) {
                switch (role) {
                    case "autocomplete":
                        {
                            kendoType = "kendoAutoComplete";
                            break;
                        }
                    case "datepicker":
                        {
                            kendoType = "kendoDatePicker";
                            break;
                        }
                    case "dropdownlist":
                        {
                            kendoType = "kendoDropDownList";
                            cascadeFrom = jqueryElement.data(kendoType).options.cascadeFrom;
                            break;
                        }
                    case "timepicker":
                        {
                            kendoType = "kendoTimePicker";
                            break;
                        }
                    case "numerictextbox":
                        {
                            kendoType = "kendoNumericTextBox";
                            break;
                        }
                    case "checkbox":
                        {
                            kendoType = "kendoCheckBox";
                            break;
                        }
                    case "datetimepicker":
                        {
                            kendoType = "kendoDateTimePicker";
                            break;
                        }
                }
                if (kendoType) {
                    if (cascadeFrom == "") {
                        jqueryElement.data(kendoType).enable(enable);
                    }
                }
            }
            else {
                if (jqueryElement.attr("type") === "submit" && !enable) {
                    jqueryElement.remove();
                }
                else {
                    jqueryElement.prop('disabled', !enable);
                    if (jqueryElement.hasClass("k-textbox")) {
                        if (enable) {
                            jqueryElement.removeClass("k-state-disabled");
                        }
                        else {
                            jqueryElement.addClass("k-state-disabled");
                        }
                    }
                }
            }
        });
        return wrapper;
    };
    Core.prototype.onDeleteMvcTableItem = function (grid, items) {
        var dataSource = grid.dataSource;
        grid.dataSource.remove(items);
        var currentPage = dataSource.page();
        var totalPages = dataSource.totalPages();
        if (currentPage > totalPages) {
            dataSource.page(totalPages);
        }
        else {
            dataSource.read();
        }
    };
    Core.prototype.isEmpty = function (val) {
        return (val === undefined || val == null || val.length <= 0) ? true : false;
    };
    Core.prototype.openKendoWindowContent = function (content, options) {
        if (!content) {
            return;
        }
        if (!options) {
            options = {};
        }
        options.modal = options.modal ? options.modal : true;
        options.deactivate = function () {
            this.destroy();
        };
        var kendoWindow = $('<div/>').appendTo('body').kendoWindow(options).data("kendoWindow");
        kendoWindow.content(content).center().open();
        core.refreshKendoEditor();
    };
    ////////fix problem with kendo editor in mozilla firefox in kendo popup 
    ////////https://docs.telerik.com/kendo-ui/controls/editors/editor/troubleshoot/troubleshooting#editor-in-popup-is-read-only-in-firefox
    Core.prototype.refreshKendoEditor = function () {
        if (navigator.userAgent.indexOf("Firefox") > 0) {
            $(document).find(".k-widget.k-editor textarea").each(function (index, elem) {
                $(elem).data("kendoEditor").refresh();
            });
        }
    };
    Core.prototype.validatePanelBarItems = function () {
        setTimeout(function () {
            var errorElement = $(".field-validation-error");
            if (errorElement.length) {
                var panels = errorElement.closest(".k-content").toArray();
                panels.forEach(function (p) {
                    p.style.display = "block";
                });
            }
        }, 100);
    };
    Core.prototype.bindAjaxAction = function () {
        var _this = this;
        // Hook global ajax start - show loading progress
        $(document).ajaxStart(function () {
            var self = _this;
            var element = self.getProgressElement();
            kendo.ui.progress(element, true);
        });
        // Hook global ajax complete - hide loading progress
        $(document).ajaxComplete(function () {
            var self = _this;
            var element = self.getProgressElement();
            kendo.ui.progress(element, false);
            $.validator.unobtrusive.parse(document);
        });
        // Add __RequestVerificationToken for POST ajax request
        $.ajaxPrefilter(function (options, originalOptions, jqXHR) {
            var type;
            if (originalOptions.type !== undefined) {
                type = originalOptions.type;
            }
            else {
                type = options.type;
            }
            if (!type || type.toUpperCase() !== 'POST') {
                return;
            }
            if (!options.headers) {
                options.headers = {};
            }
            if (!options.headers["__RequestVerificationToken"]) {
                options.headers["__RequestVerificationToken"] = $("input[name='__RequestVerificationToken']:first").val();
            }
        });
    };
    Core.prototype.predefineMethods = function () {
        if (!String.prototype.trim) {
            String.prototype.trim = function () {
                return this.replace(/^\s+|\s+$/g, '');
            };
        }
        if (!String.prototype.format) {
            String.prototype.format = function () {
                var formatted = this;
                for (var i = 0; i < arguments.length; i++) {
                    var regexp = new RegExp('\\{' + i + '\\}', 'gi');
                    formatted = formatted.replace(regexp, arguments[i]);
                }
                return formatted;
            };
        }
        if (!String.prototype.endsWith) {
            String.prototype.endsWith = function (searchString, position) {
                var subjectString = this.toString();
                if (position === undefined || position > subjectString.length) {
                    position = subjectString.length;
                }
                position -= searchString.length;
                var lastIndex = subjectString.indexOf(searchString, position);
                return lastIndex !== -1 && lastIndex === position;
            };
        }
        if (!Array.prototype.indexOf) {
            Array.prototype.indexOf = function (what, i) {
                i = i || 0;
                var L = this.length;
                while (i < L) {
                    if (this[i] === what)
                        return i;
                    ++i;
                }
                return -1;
            };
        }
        if (!Array.prototype.remove) {
            Array.prototype.remove = function () {
                var what, a = arguments, L = a.length, ax;
                while (L > 1 && this.length) {
                    what = a[--L];
                    while ((ax = this.indexOf(what)) !== -1) {
                        this.splice(ax, 1);
                    }
                }
                return this;
            };
        }
    };
    return Core;
}());
// Used for JS object inheritance
var __extends = function (d, b) {
    for (var p in b)
        if (b.hasOwnProperty(p))
            d[p] = b[p];
    function __() { this.constructor = d; }
    d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
};
var core = new Core();
