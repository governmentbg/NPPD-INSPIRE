/// <reference path="../typings/jquery/jquery.d.ts" />
/// <reference path="../Utilities/Core.ts" />

class Resources {
    readResource = (): void => {
        core.requestOptional(
            "Resources",
            "Home",
            {
                success: function (data) {
                    resources = data;
                    $.DirtyForms.message = resources.ConfirmPageLeave;
                    gdpr.check();
                },
                useArea: false,
                cache: true,
                async: false
            });
    }
}

let resources: any;
new Resources().readResource();