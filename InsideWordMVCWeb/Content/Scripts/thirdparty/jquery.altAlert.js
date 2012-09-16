/// <reference path="jquery-1.4.1.js" />

/* http: //roosteronacid.com/blog/index.php/2010/01/20/jquery-plug-in-personalized-alert-messages/
call this in global to activate for all alerts:
$(function ()  
{  
    $.altAlert();  
});
*/

jQuery.altAlert = function (options) {
    var defaults = {
        title: "Alert",
        buttons: {
            "Ok": function () {
                jQuery(this).dialog("close");
            }
        }
    };

    jQuery.extend(defaults, options);

    delete defaults.autoOpen;

    window.alert = function () {
        jQuery("<div />", { html: arguments[0].replace(/\n/, "<br />") }).dialog(defaults);
    };
};