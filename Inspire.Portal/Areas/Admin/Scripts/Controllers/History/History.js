var ObjectHistory = /** @class */ (function () {
    function ObjectHistory() {
        var _this = this;
        this.init = function () {
            var self = _this;
            core.rebindEvent("click", ".view-history-js", self.onViewHistoryClick);
        };
        this.onViewHistoryClick = function (e) {
            e.preventDefault();
            var sender = $(e.currentTarget);
            var objectId = sender.data("objectid");
            var objectType = sender.data("objecttype");
            if (!objectType) {
                return;
            }
            core.openKendoWindow("Index", "History", {
                area: "Admin",
                type: "GET",
                data: {
                    objectId: objectId,
                    objectType: objectType
                }
            }, {
                title: resources.History
            });
        };
        this.init();
    }
    return ObjectHistory;
}());
var objectHistory = new ObjectHistory();
