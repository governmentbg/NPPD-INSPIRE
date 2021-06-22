let questionTypes: any;

class Poll {
    constructor() {
        this.init();
    }

    private init = () => {
        let self = this;
        self.rebindEvents();

        this.getQuestionTypes();
    }

    private rebindEvents = () => {
        let self = this;
        core.rebindEvent("click", ".addQuestion-js, .editQuestion-js", self.addQuestion);
        core.rebindEvent("click", ".addOption-js, .editOption-js", self.addOptionToQuestion);
        core.rebindEvent("click", ".deleteOption-js", self.deleteOption);
        core.rebindEvent("click", ".deleteQuestion-js", self.deleteQuestion);
        core.rebindEvent("click", ".pollDelete-js", self.deletePoll);
        core.rebindEvent("click", ".pollChangeDates-js", self.changeDates);
        core.rebindEvent("click", ".sendPoll", this.sendPoll);
    }

    private getQuestionTypes = (): void => {
        core.requestOptional("GetQuestionTypes", "Poll",
            {
                type: "GET",
                useArea: false,
                success: (data) => {
                    questionTypes = data;
                },
                cache: true
            })
    }

    private sendPoll = (e) => {
        e.preventDefault();

        let actions = [
            {
                text: resources.Yes,
                action: () => {
                    $("form").submit();
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
                title: resources.Send,
                content: resources.ConfirmPollSend,
                visible: true,
                actions: actions
            });
    }

    public hideOptions = (e) => {
        var typeId = e.sender.value();
        if (questionTypes.includes(typeId)) {
            $(".optionsWrapper").hide();
            var listView = $("#optionsList").data("kendoListView");
            if (listView.dataSource.data().length === 0) {
                core.requestOptional("AddDefaultOption",
                    "Poll",
                    {
                        type: "GET",
                        useArea: false,
                        success: function (data) {
                            listView.dataSource.add(data.model);
                        }
                    });
            }
        }
    }

    public onQuestionTypeChange = (e) => {
        let optionsList = $("#optionsList").data("kendoListView");

        if (optionsList.dataSource.data().length > 0) {
            optionsList.dataSource.data([]);
            optionsList.refresh();
        }

        let questionType = e.sender.value();

        if (questionTypes.includes(questionType)) {
            $(".optionsWrapper").hide();
            if (optionsList.dataSource.data().length === 0) {
                core.requestOptional("AddDefaultOption",
                    "Poll",
                    {
                        type: "GET",
                        useArea: false,
                        success: (data) => {
                            optionsList.dataSource.add(data.model);
                        }
                    });
            }
        } else {
            $(".optionsWrapper").show();
        }
    }

    public questionPlaceholder = (element) => {
        return element.clone().css("opacity", 0.1);
    }

    public questionHint = (element) => {
        return element.clone().removeClass("k-state-selected");
    }

    public onQuestionPositionChange = (e) => {
        let listView = $(e.sender.element).data("kendoListView");
        if (!listView) {
            return;
        }

        let dataSource = listView.dataSource,
            newIndex = e.newIndex,
            dataItem = dataSource.getByUid(e.item.data("uid"));

        dataSource.remove(dataItem);
        dataSource.insert(newIndex, dataItem);
    }

    public addQuestionToPoll = (e) => {
        let window = $(".k-window-content:last").data("kendoWindow");
        if (!window) {
            return;
        }

        let success = !core.isEmpty(e) && e.success === true;
        if (success === false) {
            window.content(e as string);
        } else {


            let pollQuestion = $("#Questions").data("kendoListView");

            let question = pollQuestion.dataSource.get(e.model.UniqueId);
            let index = 0;
            if (question) {
                index = pollQuestion.dataSource.indexOf(question);
                pollQuestion.dataSource.remove(question);
                pollQuestion.dataSource.insert(index, e.model);
            } else {
                pollQuestion.dataSource.add(e.model);
            }
            notification.displayMessage(resources.Success, "success");
            window.close();
        }
    }

    private changeDates = (e) => {
        e.preventDefault();

        let poll = searchTable.getSelectedItemByTr($(e.currentTarget).closest("tr"));
        let pollId = poll["Id"];
        let searchQueryId = $(e.currentTarget).closest(".k-grid").data("kendoGrid").wrapper.data("searchqueryid");
        core.openKendoWindow(
            "ChangeDates",
            "Poll",
            {
                type: "GET",
                useArea: false,
                data: { id: pollId, searchQueryId: searchQueryId }
            },
            {
                close: (e) => {
                    if (e.userTriggered) {
                        return;
                    }
                },
                title: resources.ChangeDates
            });
    }

    private deletePoll = (e) => {
        e.preventDefault();

        let poll = searchTable.getSelectedItemByTr($(e.currentTarget).closest("tr"));
        let pollId = poll["Id"];
        let grid = $(e.currentTarget).closest(".k-grid").data("kendoGrid");

        core.confirmDelete(
            () => {
                core.requestOptional(
                    "DeletePoll",
                    "Poll",
                    {
                        type: "DELETE",
                        useArea: false,
                        data: {
                            pollId: pollId
                        },
                        success: () => {
                            grid.dataSource.remove(poll);
                            notification.displayMessage(resources.DeleteSuccess, "success");
                        }
                    });
                return true;
            });
    }

    private deleteQuestion = (e) => {
        e.preventDefault();

        let sender = $(e.currentTarget);
        let listView = sender.closest(".k-widget.k-listview").data("kendoListView") as kendo.ui.ListView;
        if (!listView) {
            return;
        }

        let id = sender.closest("li").find("#questionId").val();
        let uId = sender.closest("li").find("#questionUniqueId").val();
        core.confirmDelete(
            () => {
                core.requestOptional(
                    id ? "DeleteQuestion" : "DeleteQuestionFromSession",
                    "Poll",
                    {
                        type: "DELETE",
                        useArea: false,
                        data: id 
                            ? {
                                questionId: id
                            }
                            : {
                                uniqueId: uId
                            },
                        success: () => {
                            let item = listView.dataSource.get(uId);
                            listView.dataSource.remove(item);
                            listView.refresh();
                            notification.displayMessage(resources.DeleteSuccess, "success");
                        }
                    });
                return true;
            });
    }

    private deleteOption = (e) => {
        e.preventDefault();

        let sender = $(e.currentTarget);
        let listView = $("#optionsList").data("kendoListView") as kendo.ui.ListView;
        if (!listView) {
            return;
        }

        let uniqueId = sender.closest("li").find("#optionUniqueId").val();
        if (!uniqueId) {
            return;
        }

        core.confirmDelete(
            () => {
                core.requestOptional(
                    "DeleteOption",
                    "Poll",
                    {
                        type: "DELETE",
                        useArea: false,
                        data: {
                            uniqueId: uniqueId
                        },
                        success: () => {
                            let item = listView.dataSource.get(uniqueId);
                            listView.dataSource.remove(item);
                            listView.refresh();
                            notification.displayMessage(resources.DeleteSuccess, "success");
                        }
                    });
                return true;
            });
    }

    private addOptionToQuestion = (e) => {
        e.preventDefault();

        let sender = $(e.currentTarget);
        let listView = $("#optionsList").data("kendoListView") as kendo.ui.ListView;
        if (!listView) {
            return;
        }

        let uniqueId;
        if (sender.hasClass("editOption-js")) {
            uniqueId = sender.closest("li").find("#optionUniqueId").val();
        }

        core.openKendoWindow(
            "EditOption",
            "Poll",
            {
                data: {
                    uniqueId: uniqueId
                },
                useArea: false
            },
            {
                title: resources.AddOption,
                modal: true,
                visible: false,
                close: function (e) {
                    if (e.userTriggered) {
                        return;
                    }

                    let success = e.sender.element.data("success");
                    if (success === true) {
                        let entry = e.sender.element.data("result");
                        let existItem = listView.dataSource.get(uniqueId);
                        if (existItem) {
                            listView.dataSource.remove(existItem);
                        }
                        listView.dataSource.add(entry);
                    }
                }
            });
    }

    private addQuestion = (e) => {
        e.preventDefault();

        let sender = $(e.currentTarget);
        let uniqueId = sender.closest("li").find("#questionUniqueId").val();

        core.openKendoWindow("UpsertQuestion",
            "Poll",
            {
                type: "GET",
                useArea: false,
                data: { uniqueId: uniqueId }
            },
            {
                close: (e) => {
                    if (e.userTriggered) {
                        return;
                    }
                },
                title: resources.AddQuestion
            });
    }
}

let poll = new Poll();