/// <reference path="../typings/jquery/jquery.d.ts" />
/// <reference path="../typings/kendo-ui/kendo.all.d.ts" />
/// <reference path="../typings/globalize/globalize.d.ts" />
/// <reference path="../typings/globalize/globalize-0.1.3.d.ts" />
/// <reference path="../typings/jquery.validation/jquery.validation.d.ts" />
/// <reference path="../typings/jquery-validation-unobtrusive/jquery-validation-unobtrusive.d.ts"/>

declare let globalVariables: any;
declare let SiteBase: any;
declare let currentCulture: any;
declare let controllerArea: any;

interface JQueryStatic {
    CookiesMessage(options: any): JQuery;
    DirtyForms: any;
}

interface String {
    format(...replacements: string[]): string;
    endsWith(...replacements: string[]): any;
}

interface Array<T> {
    remove(start: T, item: T): any;
}

interface AreaSettings {
    area?: string;
    useArea?: boolean;
}

interface RequestOptionalParameters {
    success?: Function;
    error?: Function;
    complete?: Function;
    data?: any;
    type?: string;
    traditional?: boolean;
    async?: boolean;
    global?: boolean;
    area?: string;
    useArea?: boolean;
    contentType?: string;
    cache?: boolean;
}

class Core {

    constructor() {
        this.init();
    }

    public confirmDelete = (action: Function) => {
        let actions = [
            {
                text: resources.Yes,
                action: action,
                primary: true
            },
            {
                text: resources.No
            }
        ];

        core.createKendoDialog(
            {
                title: resources.Deleting,
                content: resources.ConfirmDeleting,
                visible: true,
                actions: actions
            });
    }

    public copyToClipboard = (str: string) => {
        const el = document.createElement('textarea');
        el.value = str;
        el.setAttribute('readonly', '');
        el.style.position = 'absolute';
        el.style.left = '-9999px';
        document.body.appendChild(el);
        el.select();
        document.execCommand('copy');
        document.body.removeChild(el);
    };

    public onValueChangeSetDirtyForm = (e) => {
        let sender = e.sender
            ? e.sender.element
            : $(e.currentTarget);
        let form = sender.closest("form");
        if (!form || !form.hasClass($.DirtyForms.listeningClass)) {
            return;
        }

        form.dirtyForms('rescan');
    }

    public onRemoveFileClick = (e) => {
        let sender = $(e.currentTarget);
        let element = sender.closest("[role='listitem']");
        let listView = sender.closest(".k-widget.k-listview").data("kendoListView");
        let selectedItem = listView.dataItem(element);

        if (!selectedItem) {
            return;
        }

        core.confirmDelete(
            () => {
                listView.dataSource.remove(selectedItem);
                return true;
            });
    }

    public initImageMagnificPopup = (selector: string = null) => {
        selector = selector ? selector : ".magnet, .p-img a";
        $(selector).magnificPopup({ type: "image", gallery: { enabled: true } });
    }

    public onChangeDropDownType = (e: kendo.ui.DropDownListChangeEvent) => {
        let dropDownList = e.sender;
        let dropDownItem = dropDownList.dataItem();

        if (!dropDownItem) {
            return;
        }

        let templateWrapper = dropDownList.wrapper.closest(".offerFile");
        let uniqueId = templateWrapper.find("[name$='.UniqueId']").val();
        let listView = templateWrapper.closest(".k-widget.k-listview").data("kendoListView") as kendo.ui.ListView;
        let listDataItem = listView.dataSource.get(uniqueId);

        if (listDataItem) {
            listDataItem.set(
                "Type",
                {
                    Id: dropDownItem.Id,
                    Name: dropDownItem.Name
                });
        }
    }

    public filterData(e) {
        let data = {};
        if (e.filter.filters && e.filter.filters.length > 0) {
            for (let i = 0; i < e.filter.filters.length; i++) {
                let fieldName = e.filter.filters[i].field.toUpperCase() === "name".toUpperCase()
                    || e.filter.filters[i].field.toUpperCase() === "fullname".toUpperCase()
                    ? "text"
                    : e.filter.filters[i].field;
                data[fieldName] = e.filter.filters[i].value;
            }
        }

        return data;
    }

    public getBaseUrl() {
        let siteBase = globalVariables.SiteBase;

        let href = window.location.href;
        let b = href.split('/');
        href = b[0] + '//' + b[2] + siteBase;

        return href;
    }

    public getPathToActionMethod(action: string, controller: string, areaSettings?: AreaSettings) {
        let controllerArea: string = globalVariables ? globalVariables.ControllerArea : null;
        let area: string,
            useArea: boolean;

        if (areaSettings != null) {
            area = areaSettings.area;
            useArea = areaSettings.useArea;
        }

        let base = this.getBaseUrl();

        let areaData: string = "";
        area = !area || area.length === 0
            ? typeof controllerArea !== "undefined" ? controllerArea : ""
            : area;

        if ((useArea == undefined || useArea === true) && area) {
            areaData = area + "/";
        }

        let currentCulture = $('html').attr('lang').split("-")[0];
        let currentCultureData: string = "";
        if (typeof currentCulture !== "undefined" && currentCulture) {
            currentCultureData = currentCulture + "/";
        }

        return base + currentCultureData + areaData + controller + "/" + action + "/";
    }

    public rebindEvent(event: string, selector: string, callback: (eventObject: JQueryEventObject, ...eventData: any[]) => any) {
        $(document).off(event, selector);
        $(document).on(event, selector, callback);
    }

    public requestOptional = (action: string, controller: string, optional?: RequestOptionalParameters): JQueryXHR => {
        let self = this;

        if (optional == null) {
            optional = {};
        }

        let isPost = typeof optional.type !== 'undefined' && optional.type != null && ["POST", "DELETE"].indexOf(optional.type.toUpperCase()) > -1;
        let ajaxSettings: JQueryAjaxSettings = {
            data: isPost
                ? JSON.stringify(optional.data)
                : optional.data,
            contentType: optional.contentType ? optional.contentType : "application/json; charset=utf-8",
            headers: isPost
                ? { "__RequestVerificationToken": $("input[name='__RequestVerificationToken']:first").val() }
                : null, // Add __RequestVerificationToken for POST ajax request
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

        return $.ajax(
            self.getPathToActionMethod(
                action,
                controller,
                {
                    area: optional.area,
                    useArea: optional.useArea
                }),
            ajaxSettings
        );
    }

    public openKendoWindow = (action: string, controller: string, optional?: RequestOptionalParameters, windowOptions?: kendo.ui.WindowOptions, enableInputs: boolean = true) => {
        let self = this;

        if (!optional) {
            optional = {};
        }

        let oldSuccessCallback = optional.success;
        if (!oldSuccessCallback) {
            optional.success = function (data) {
                if (!enableInputs) {
                    data = self.enableInputs($(data), enableInputs);
                }

                self.openKendoWindowContent(data, windowOptions);
            };
        }

        self.requestOptional(action, controller, optional);
    }

    public onKendoWindowSuccessCallback(data: any) {
        let window = $(this).closest(".k-window-content").data("kendoWindow");
        if (!window) {
            return;
        }

        let success = !core.isEmpty(data) && data.success === true;
        if (success === false) {
            window.content(data as string);
        } else {
            let keys = Object.keys(data);
            for (let i = 0; i < keys.length; i++) {
                let key = keys[i];
                $(window.element).data(key, data[key]);
            }

            if (data.refreshgrid && data.searchqueryid) {
                let grid = $(kendo.format("div[data-searchqueryid='{0}']", data.searchqueryid)).data("kendoGrid") as kendo.ui.Grid;
                if (grid) {
                    grid.dataSource.read();
                }
            }

            window.close();
        }
    }

    public getSelectedRowId(element: any) {
        let selectedItem = searchTable.getSelectedItem(element).toJSON();
        if (!selectedItem) {
            return;
        }
        return selectedItem["Id"];
    }

    public enableInputs(wrapper: JQuery, enable: boolean): any {

        if (!enable) {
            wrapper.find("form").each(
                (i, form) => {
                    let formElement = $(form);
                    formElement.replaceWith(formElement.html());
                });
        }

        wrapper.find("input, textarea, select").each(
            (i, item) => {
                let jqueryElement = $(item);
                let role = jqueryElement.data("role");
                let kendoType: string = "";
                let cascadeFrom: string = "";
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
                                cascadeFrom = jqueryElement.data(kendoType).options.cascadeFrom
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
                } else {
                    if (jqueryElement.attr("type") === "submit" && !enable) {
                        jqueryElement.remove();
                    } else {
                        jqueryElement.prop('disabled', !enable);
                        if (jqueryElement.hasClass("k-textbox")) {
                            if (enable) {
                                jqueryElement.removeClass("k-state-disabled");
                            } else {
                                jqueryElement.addClass("k-state-disabled");
                            }
                        }
                    }
                }
            });

        return wrapper;
    }

    public onDeleteMvcTableItem(grid: kendo.ui.Grid, items) {
        let dataSource = grid.dataSource;

        grid.dataSource.remove(items);

        let currentPage = dataSource.page();
        let totalPages = dataSource.totalPages();
        if (currentPage > totalPages) {
            dataSource.page(totalPages);
        } else {
            dataSource.read();
        }
    }

    public goToCorrectGridPage = (grid: kendo.ui.Grid): boolean => {
        let dataSource = grid.dataSource;
        let currentPage = dataSource.page();
        let totalPages = dataSource.totalPages();
        if (currentPage > totalPages && dataSource.total() > 0) {
            dataSource.page(totalPages);
            return true;
        }

        return false;
    }

    public createKendoDialog = (options: kendo.ui.DialogOptions = null, divClass: string = null): kendo.ui.Dialog => {
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
            : () => {
                dialog.destroy();
            };

        let dialog = $(divClass ? '<div class="' + divClass + '"/>' : '<div />')
            .appendTo('body')
            .kendoDialog(options)
            .data("kendoDialog");
        return dialog;
    }

    public onAjaxFormSuccess = (data: any, selector: string) => {
        if (data) {
            $(selector).html(data);
        }
    }

    public isEmpty(val: any) {
        return (val === undefined || val == null || val.length <= 0) ? true : false;
    }

    public removeListViewItem = (e) => {
        let sender = $(e.currentTarget);
        let li = sender.closest("li");
        let list = sender.closest(".k-widget.k-listview").data("kendoListView");
        let selectedItem = list.dataItem(li);

        if (!selectedItem) {
            return;
        }

        let actions = [
            {
                text: resources.Yes,
                action: () => {
                    list.dataSource.remove(selectedItem);
                    return true;
                },
                primary: true
            },
            {
                text: resources.No
            }
        ];

        core.createKendoDialog(
            {
                title: resources.DeleteTitle,
                content: resources.ConfirmDeleteSelectedItem,
                visible: true,
                actions: actions
            });
    }

    public bindBodyClick = () => {
        let self = this;
        self.rebindEvent("click", "body", self.onBodyClick);
    }

    public openKendoWindowContent(content: string, options?: kendo.ui.WindowOptions) {

        if (!content) {
            return;
        }

        if (!options) {
            options = {};
        }
        options.modal = options.modal ? options.modal : true;
        options.deactivate = function () {
            this.destroy();
        }

        let kendoWindow = $('<div/>').appendTo('body').kendoWindow(options).data("kendoWindow");
        kendoWindow.content(content).center().open();

        core.refreshKendoEditor();
    }

    public onGridFilterOnClient = (e: kendo.ui.GridFilterEvent) => {
        if (e.filter && e.filter.filters.length > 0) {
            for (let i = 0; i < e.filter.filters.length; i++) {
                e.filter.filters[i].value = e.filter.filters[i].value
                    ? e.filter.filters[i].value.trim()
                    : e.filter.filters[i].value;
            }
        }
    }

    public initUploadTemplate = (upload: kendo.ui.Upload, file: any): string => {
        let self = this;
        let size = self.getFileSizeMessage(file);

        file.url = file.url ? file.url : ''
        file.description = file.description ? file.description : '';

        let name = upload.element.attr("Name");
        let template;
        if (upload.options.multiple) {
            template =
                `<input type="hidden" name="${name}.Index" value="#: uid #" />` +
                `<input type="hidden" name="${name}[#: uid #].Name" value="#: name #" />` +
                `<input type="hidden" name="${name}[#: uid #].Size" value="#: size #" />` +
                `<input type="hidden" name="${name}[#: uid #].Url" value="#: url #" />` +
                `<input type="hidden" name="${name}[#: uid #].Description" value="#: description #" />`;
        } else {
            template =
                `<input type="hidden" name="${name}.Name" value="#: name #" />` +
                `<input type="hidden" name="${name}.Size" value="#: size #" />` +
                `<input type="hidden" name="${name}.Url" value="#: url #" />` +
                `<input type="hidden" name="${name}.Description" value="#: description #" />`;
        }

        template = '<span class="k-progress"></span>' +
            '<span class="k-file-extension-wrapper">' +
            '<span class="k-file-extension">#: extension #</span>' +
            '<span class="k-file-state"></span>' +
            '</span>' +
            '<span class="k-file-name-size-wrapper">' +
            `${template}` +
            '<a href="#: url #" target="_blank" class="k-file-name" title="#: name #">#: name #</a>' +
            `<span class="k-file-size">${size}</span>` +
            '</span>' +
            '<strong class="k-upload-status">' +
            '<button type="button" class="k-upload-action"></button>' +
            '<button type="button" class="k-upload-action"></button>' +
            '</strong>';
        return kendo.template(template)(file);
    }

    public onUploadSuccess = (e: kendo.ui.UploadSuccessEvent) => {
        let self = this;
        if (e.operation === "upload") {
            for (let i = 0; i < e.files.length; i++) {
                let attachment = e.response.files[i];
                if (attachment) {
                    e.files[i]["url"] = attachment["Url"] ? attachment["Url"] : "#";
                    e.files[i]["description"] = attachment["Description"];

                    $(`li[data-uid='${e.files[i]["uid"]}']`).html(self.initUploadTemplate(e.sender, e.files[i]));
                }
            }
        }
    }

    public onUploadRemove = (e: kendo.ui.UploadRemoveEvent) => {
        let urls = [];
        for (let i = 0; i < e.files.length; i++) {
            if (e.files[i]["url"]) {
                urls.push(e.files[i]["url"]);
            }
        }

        e.data = {
            urls: urls
        };
    }

    public renderMenuButton = (templateId: string, item: any) => {
        let templateContent = $(`#${templateId}`).html();
        let template = kendo.template(templateContent);
        let html = template(item);
        return $(html).find("li:first").length > 0
            ? html
            : "";
    }

    public persistGridPageSize = (gridSelector) => {
        let grid = $(gridSelector).data("kendoGrid") as kendo.ui.Grid;
        if (!grid) {
            return;
        }

        window.addEventListener(
            "beforeunload",
            function (e) {
                let pageSizesDropDownList = grid.element.find(".k-pager-sizes:first select:first").data("kendoDropDownList") as kendo.ui.DropDownList;
                if (pageSizesDropDownList) {
                    localStorage["grid-page-size"] = pageSizesDropDownList.value();
                }

                return;
            });

        let pageSize = parseInt(localStorage["grid-page-size"])
            ? parseInt(localStorage["grid-page-size"])
            : localStorage["grid-page-size"];
        if (pageSize && grid.options.pageable) {
            grid.dataSource.pageSize(pageSize === 'all' ? grid.dataSource.total() : pageSize);
            grid.refresh();
        }
    }

    public onPanelBarSelectExpandCollapse = (e: kendo.ui.PanelBarSelectEvent) => {
        if ($(e.item).is(".k-state-active")) {
            window.setTimeout(
                () => {
                    e.sender.collapse(e.item, true);
                },
                1);
        }
    }

    ////////fix problem with kendo editor in mozilla firefox in kendo popup 
    ////////https://docs.telerik.com/kendo-ui/controls/editors/editor/troubleshoot/troubleshooting#editor-in-popup-is-read-only-in-firefox
    public refreshKendoEditor() {
        if (navigator.userAgent.indexOf("Firefox") > 0) {
            $(document).find(".k-widget.k-editor textarea").each((index, elem) => {
                $(elem).data("kendoEditor").refresh();
            });
        }
    }

    private getFileSizeMessage = (file) => {
        let totalSize = 0;
        if (typeof file.size == 'number') {
            totalSize = file.size;
        } else {
            return '';
        }

        totalSize /= 1024;
        if (totalSize < 1024) {
            return totalSize.toFixed(2) + ' KB';
        } else {
            return (totalSize / 1024).toFixed(2) + ' MB';
        }
    }

    private init = () => {
        let self = this;

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
        $.extend(
            $.validator.methods, {
                date(value, element) {
                    return this.optional(element) || kendo.parseDate(value) != null || parseInt(value) != null;
                },
                number(value, element) {
                    return this.optional(element) || kendo.parseFloat(value) != null;
                },
                regexwithoptions(value, element, params) {
                    let match;
                    if (this.optional(element)) {
                        return true;
                    }

                    let reg = new RegExp(params.pattern, params.flags);
                    match = reg.exec(value);
                    return (match && (match.index === 0) && (match[0].length === value.length));
                }
        });

        $(document).ready(() => {
            // Use kendo for select inputs
            $("select").each(function () {
                let element = $(this);
                if (!element.closest(".k-widget")) {
                    element.kendoDropDownList();
                }
            });

            // Widgets Are Hidden after Postbacks When Using jQuery Validation
            // http://docs.telerik.com/kendo-ui/aspnet-mvc/troubleshoot/troubleshooting-validation
            $(".k-widget").removeClass("input-validation-error");

            $(document).on("submit", "form", function (e) {
                let sender = $(e.currentTarget);
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
            let element = $(this);
            let value = element.val();
            if (value) {
                element.val(value.replace(/\s+/g, ' ').trim());
            }
        });
        self.rebindEvent("click", ".logout-js", self.onLogOutClick);
        self.rebindEvent("click", ".saveButton", self.validatePanelBarItems);
        self.rebindEvent("submit", "#searchRecordsForm", self.onGeonetworkSearchFormSubmit);
        self.initImageMagnificPopup();
        self.initDirtyForm();
    }

    private initCulture = () => {
        let currentCulture = $('html').attr('lang');
        if (currentCulture == "bg-BG") {
            let globalisationCulture = Globalize.cultures[currentCulture];
            let customBg = $.extend(true, {}, globalisationCulture, {
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

            let customKendoBg = $.extend(true, {}, kendo.cultures[currentCulture], {
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
        let kendoCulture = kendo.culture();
        if (currentCulture != currentCulture) {

            let jQueryCulture = Globalize.cultures[currentCulture];
            jQueryCulture.numberFormat.currency.symbol = "bgn";
            jQueryCulture.numberFormat.currency.pattern[0] = "-n $";
            jQueryCulture.numberFormat.currency.pattern[1] = "n $";

            kendoCulture.numberFormat.currency.symbol = "bgn";
            kendoCulture.numberFormat.currency.pattern[0] = "-n%";
            kendoCulture.numberFormat.currency.pattern[1] = " n%";
        }

        let decimals = 6;
        //http://docs.telerik.com/kendo-ui/controls/editors/numerictextbox/overview#known-limitations
        // Set precision limitation
        kendoCulture.numberFormat.decimals = decimals
    }

    private onGeonetworkSearchFormSubmit = (e: Event) => {
        let form = $(e.currentTarget);
        form.attr("action", kendo.format(form.data("url"), form.find("#any:first").val()));
    }

    private validatePanelBarItems() {
        setTimeout(() => {
            let errorElement = $(".field-validation-error");
            if (errorElement.length) {
                let panels = errorElement.closest(".k-content").toArray();
                panels.forEach((p) => {
                    p.style.display = "block";
                });
            }
        },
            100);
    }

    private getProgressElement = (): JQuery => {
        return $('body');
    }

    private bindAjaxAction() {
        // Hook global ajax start - show loading progress
        $(document).ajaxStart(() => {
            let self = this;
            let element = self.getProgressElement();
            kendo.ui.progress(element, true);
        });

        // Hook global ajax complete - hide loading progress
        $(document).ajaxComplete(() => {
            let self = this;
            let element = self.getProgressElement();
            kendo.ui.progress(element, false);
            $.validator.unobtrusive.parse(document);
        });

        // Add __RequestVerificationToken for POST ajax request
        $.ajaxPrefilter((options, originalOptions, jqXHR) => {
            let type;
            if (originalOptions.type !== undefined) {
                type = originalOptions.type;
            } else {
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
    }

    private predefineMethods() {

        if (!String.prototype.trim) {
            String.prototype.trim = function () {
                return this.replace(/^\s+|\s+$/g, '');
            };
        }

        if (!String.prototype.format) {
            String.prototype.format = function () {
                let formatted = this;
                for (let i = 0; i < arguments.length; i++) {
                    let regexp = new RegExp('\\{' + i + '\\}', 'gi');
                    formatted = formatted.replace(regexp, arguments[i]);
                }
                return formatted;
            };
        }

        if (!String.prototype.endsWith) {
            String.prototype.endsWith = function (searchString, position: any) {
                let subjectString = this.toString();
                if (position === undefined || position > subjectString.length) {
                    position = subjectString.length;
                }
                position -= searchString.length;
                let lastIndex = subjectString.indexOf(searchString, position);
                return lastIndex !== -1 && lastIndex === position;
            };
        }

        if (!Array.prototype.indexOf) {
            Array.prototype.indexOf = function (what, i) {
                i = i || 0;
                let L = this.length;
                while (i < L) {
                    if (this[i] === what) return i;
                    ++i;
                }
                return -1;
            };
        }

        if (!Array.prototype.remove) {
            Array.prototype.remove = function () {
                let what, a = arguments, L = a.length, ax;
                while (L > 1 && this.length) {
                    what = a[--L];
                    while ((ax = this.indexOf(what)) !== -1) {
                        this.splice(ax, 1);
                    }
                }

                return this;
            }
        }
    }

    private customBinding = () => {
        let self = this;

        self.rebindEvent(
            "click",
            ".closeKendoWindow-js",
            function (e) {
                e.preventDefault();
                let window = $(this).closest(".k-window-content").data("kendoWindow");

                if (window) {
                    window.close();
                }
            });
    }

    private onBodyClick = (e) => {
        let target = $(e.target);
        if ((target.hasClass("dropdown-trigger") || target.hasClass("icon currentColor"))
            && !target.parents('.btn-with-content').hasClass('openeddropdown')
        ) {
            e.preventDefault();
            e.stopPropagation();
            $('.openeddropdown').removeClass('openeddropdown');
            target.parents('.btn-with-content').addClass('openeddropdown');
            let overflowcompensation = target.parents('.overwrite-table');
            if (overflowcompensation) {
                let element = overflowcompensation.get(0);
                if (element && element.clientHeight != element.scrollHeight) {
                    overflowcompensation.css({ paddingBottom: (target.parents('.btn-with-content').find('ul').innerHeight() - 53) + 'px' })
                }
            }

        } else {
            $('.openeddropdown').removeClass('openeddropdown');
            $('.overwrite-table').attr('style', '');
        }
    }

    private onLogOutClick = (e) => {
        e.preventDefault();

        let sender = $(e.currentTarget);
        let actions = [
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

        let dialog = core.createKendoDialog({ actions: actions });
        dialog.title(resources.Exit);
        dialog.content(resources.ConfirmProfileLogout);
        dialog.open();
    }

    private initDirtyForm = () => {
        let self = this;
        $.DirtyForms.dialog = {
            open: function (choice, message) {
                let actions = [
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

                let dialog = self.createKendoDialog(
                    {
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
    }
}

// Used for JS object inheritance
let __extends = function (d, b) {
    for (let p in b) if (b.hasOwnProperty(p)) d[p] = b[p];
    function __() { this.constructor = d; }
    d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
};

let core = new Core();