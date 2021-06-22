var questionTypes;
var Poll = /** @class */ (function () {
    function Poll() {
        var _this = this;
        this.init = function () {
            var self = _this;
            self.rebindEvents();
            _this.getQuestionTypes();
        };
        this.rebindEvents = function () {
            var self = _this;
            core.rebindEvent("click", ".addQuestion-js, .editQuestion-js", self.addQuestion);
            core.rebindEvent("click", ".addOption-js, .editOption-js", self.addOptionToQuestion);
            core.rebindEvent("click", ".deleteOption-js", self.deleteOption);
            core.rebindEvent("click", ".deleteQuestion-js", self.deleteQuestion);
            core.rebindEvent("click", ".pollDelete-js", self.deletePoll);
            core.rebindEvent("click", ".pollChangeDates-js", self.changeDates);
            core.rebindEvent("click", ".sendPoll", _this.sendPoll);
        };
        this.getQuestionTypes = function () {
            core.requestOptional("GetQuestionTypes", "Poll", {
                type: "GET",
                useArea: false,
                success: function (data) {
                    questionTypes = data;
                },
                cache: true
            });
        };
        this.sendPoll = function (e) {
            e.preventDefault();
            var actions = [
                {
                    text: resources.Yes,
                    action: function () {
                        $("form").submit();
                        return true;
                    },
                    primary: true
                },
                {
                    text: resources.No
                }
            ];
            core.createKendoDialog({
                title: resources.Send,
                content: resources.ConfirmPollSend,
                visible: true,
                actions: actions
            });
        };
        this.hideOptions = function (e) {
            var typeId = e.sender.value();
            if (questionTypes.includes(typeId)) {
                $(".optionsWrapper").hide();
                var listView = $("#optionsList").data("kendoListView");
                if (listView.dataSource.data().length === 0) {
                    core.requestOptional("AddDefaultOption", "Poll", {
                        type: "GET",
                        useArea: false,
                        success: function (data) {
                            listView.dataSource.add(data.model);
                        }
                    });
                }
            }
        };
        this.onQuestionTypeChange = function (e) {
            var optionsList = $("#optionsList").data("kendoListView");
            if (optionsList.dataSource.data().length > 0) {
                optionsList.dataSource.data([]);
                optionsList.refresh();
            }
            var questionType = e.sender.value();
            if (questionTypes.includes(questionType)) {
                $(".optionsWrapper").hide();
                if (optionsList.dataSource.data().length === 0) {
                    core.requestOptional("AddDefaultOption", "Poll", {
                        type: "GET",
                        useArea: false,
                        success: function (data) {
                            optionsList.dataSource.add(data.model);
                        }
                    });
                }
            }
            else {
                $(".optionsWrapper").show();
            }
        };
        this.questionPlaceholder = function (element) {
            return element.clone().css("opacity", 0.1);
        };
        this.questionHint = function (element) {
            return element.clone().removeClass("k-state-selected");
        };
        this.onQuestionPositionChange = function (e) {
            var listView = $(e.sender.element).data("kendoListView");
            if (!listView) {
                return;
            }
            var dataSource = listView.dataSource, newIndex = e.newIndex, dataItem = dataSource.getByUid(e.item.data("uid"));
            dataSource.remove(dataItem);
            dataSource.insert(newIndex, dataItem);
        };
        this.addQuestionToPoll = function (e) {
            var window = $(".k-window-content:last").data("kendoWindow");
            if (!window) {
                return;
            }
            var success = !core.isEmpty(e) && e.success === true;
            if (success === false) {
                window.content(e);
            }
            else {
                var pollQuestion = $("#Questions").data("kendoListView");
                var question = pollQuestion.dataSource.get(e.model.UniqueId);
                var index = 0;
                if (question) {
                    index = pollQuestion.dataSource.indexOf(question);
                    pollQuestion.dataSource.remove(question);
                    pollQuestion.dataSource.insert(index, e.model);
                }
                else {
                    pollQuestion.dataSource.add(e.model);
                }
                notification.displayMessage(resources.Success, "success");
                window.close();
            }
        };
        this.changeDates = function (e) {
            e.preventDefault();
            var poll = searchTable.getSelectedItemByTr($(e.currentTarget).closest("tr"));
            var pollId = poll["Id"];
            var searchQueryId = $(e.currentTarget).closest(".k-grid").data("kendoGrid").wrapper.data("searchqueryid");
            core.openKendoWindow("ChangeDates", "Poll", {
                type: "GET",
                useArea: false,
                data: { id: pollId, searchQueryId: searchQueryId }
            }, {
                close: function (e) {
                    if (e.userTriggered) {
                        return;
                    }
                },
                title: resources.ChangeDates
            });
        };
        this.deletePoll = function (e) {
            e.preventDefault();
            var poll = searchTable.getSelectedItemByTr($(e.currentTarget).closest("tr"));
            var pollId = poll["Id"];
            var grid = $(e.currentTarget).closest(".k-grid").data("kendoGrid");
            core.confirmDelete(function () {
                core.requestOptional("DeletePoll", "Poll", {
                    type: "DELETE",
                    useArea: false,
                    data: {
                        pollId: pollId
                    },
                    success: function () {
                        grid.dataSource.remove(poll);
                        notification.displayMessage(resources.DeleteSuccess, "success");
                    }
                });
                return true;
            });
        };
        this.deleteQuestion = function (e) {
            e.preventDefault();
            var sender = $(e.currentTarget);
            var listView = sender.closest(".k-widget.k-listview").data("kendoListView");
            if (!listView) {
                return;
            }
            var id = sender.closest("li").find("#questionId").val();
            var uId = sender.closest("li").find("#questionUniqueId").val();
            core.confirmDelete(function () {
                core.requestOptional(id ? "DeleteQuestion" : "DeleteQuestionFromSession", "Poll", {
                    type: "DELETE",
                    useArea: false,
                    data: id
                        ? {
                            questionId: id
                        }
                        : {
                            uniqueId: uId
                        },
                    success: function () {
                        var item = listView.dataSource.get(uId);
                        listView.dataSource.remove(item);
                        listView.refresh();
                        notification.displayMessage(resources.DeleteSuccess, "success");
                    }
                });
                return true;
            });
        };
        this.deleteOption = function (e) {
            e.preventDefault();
            var sender = $(e.currentTarget);
            var listView = $("#optionsList").data("kendoListView");
            if (!listView) {
                return;
            }
            var uniqueId = sender.closest("li").find("#optionUniqueId").val();
            if (!uniqueId) {
                return;
            }
            core.confirmDelete(function () {
                core.requestOptional("DeleteOption", "Poll", {
                    type: "DELETE",
                    useArea: false,
                    data: {
                        uniqueId: uniqueId
                    },
                    success: function () {
                        var item = listView.dataSource.get(uniqueId);
                        listView.dataSource.remove(item);
                        listView.refresh();
                        notification.displayMessage(resources.DeleteSuccess, "success");
                    }
                });
                return true;
            });
        };
        this.addOptionToQuestion = function (e) {
            e.preventDefault();
            var sender = $(e.currentTarget);
            var listView = $("#optionsList").data("kendoListView");
            if (!listView) {
                return;
            }
            var uniqueId;
            if (sender.hasClass("editOption-js")) {
                uniqueId = sender.closest("li").find("#optionUniqueId").val();
            }
            core.openKendoWindow("EditOption", "Poll", {
                data: {
                    uniqueId: uniqueId
                },
                useArea: false
            }, {
                title: resources.AddOption,
                modal: true,
                visible: false,
                close: function (e) {
                    if (e.userTriggered) {
                        return;
                    }
                    var success = e.sender.element.data("success");
                    if (success === true) {
                        var entry = e.sender.element.data("result");
                        var existItem = listView.dataSource.get(uniqueId);
                        if (existItem) {
                            listView.dataSource.remove(existItem);
                        }
                        listView.dataSource.add(entry);
                    }
                }
            });
        };
        this.addQuestion = function (e) {
            e.preventDefault();
            var sender = $(e.currentTarget);
            var uniqueId = sender.closest("li").find("#questionUniqueId").val();
            core.openKendoWindow("UpsertQuestion", "Poll", {
                type: "GET",
                useArea: false,
                data: { uniqueId: uniqueId }
            }, {
                close: function (e) {
                    if (e.userTriggered) {
                        return;
                    }
                },
                title: resources.AddQuestion
            });
        };
        this.init();
    }
    return Poll;
}());
var poll = new Poll();
