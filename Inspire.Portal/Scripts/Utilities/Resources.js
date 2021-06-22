/// <reference path="../typings/jquery/jquery.d.ts" />
/// <reference path="../Utilities/Core.ts" />
var Resources = /** @class */ (function () {
    function Resources() {
        this.readResource = function () {
            core.requestOptional("Resources", "Home", {
                success: function (data) {
                    resources = data;
                    $.DirtyForms.message = resources.ConfirmPageLeave;
                    gdpr.check();
                },
                useArea: false,
                cache: true,
                async: false
            });
        };
    }
    return Resources;
}());
var resources;
new Resources().readResource();
