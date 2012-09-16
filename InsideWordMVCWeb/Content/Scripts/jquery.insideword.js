/// <reference path="/Scripts/thirdparty/jquery-1.4.4.min.js" />
/* jQuery InsideWord library
 *
 * Copyright (c) 2010 - 2011 InsideWord
 *
 */

/* function that recursively creates a list based on the passed in json object 
 *  and the li construction ojbect.
 *
 * jsonTreeList - json object to be recursively traversed to create the list. 
 *                MUST have the attribute Children, which is a list of the 
 *                child objects.
 * 
 * liObject - object that will parse the jsonTreeList and create the li object
 *
 * returns the root <ul> of the generated list
 */
 
(function ($) {
    $.fn.createList = function (jsonTreeList, liBuilderObject) {
        if (jsonTreeList.length > 0) {

            var ul = $(document.createElement("ul"));

            $.each(jsonTreeList, function (index, jsonObject) {
                liBuilderObject.liObject = jsonObject;
                ul.append(liBuilderObject.createLi()).createList(jsonObject.Children, liBuilderObject);
            });
            return this.append(ul);
        }
        return this;
    };
})(jQuery);

/* changes the background color for a flash like effect in order to visually highlight an element
 * example: $("div").animateHighlight("#dd0000", 1000);
 */
(function ($) {
    $.fn.animateHighlight = function (highlightColor, duration) {
        var highlightBg = highlightColor || "#FFFF9C";
        var animateMs = duration || 1500;
        var originalBg = this.css("backgroundColor");
        this.stop().css("background-color", highlightBg).animate({ backgroundColor: originalBg }, animateMs);
        return this;
    };
})(jQuery);