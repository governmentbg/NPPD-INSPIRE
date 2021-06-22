class ObjectHistory {
    constructor() {
        this.init();
    }

    private init = () => {
        let self = this;
        core.rebindEvent("click", ".view-history-js", self.onViewHistoryClick);
    }

    private onViewHistoryClick = (e) => {
        e.preventDefault();
        let sender = $(e.currentTarget);

        let objectId = sender.data("objectid");
        let objectType = sender.data("objecttype");
        if (!objectType) {
            return;
        }

        core.openKendoWindow(
            "Index",
            "History",
            {
                area: "Admin",
                type: "GET",
                data: {
                    objectId: objectId,
                    objectType: objectType
                }
            },
            {
                title: resources.History
            });
    }
}

let objectHistory = new ObjectHistory();