/// <reference path="jquery-1.4.1.js" />

(function ($) {
    $.getAntiForgeryToken = function (tokenWindow, appPath) {
        // HtmlHelper.AntiForgeryToken() must be invoked to print the token.
        tokenWindow = tokenWindow && typeof tokenWindow === typeof window ? tokenWindow : window;

        appPath = appPath && typeof appPath === "string" ? "_" + appPath.toString() : "";
        // The name attribute is either __RequestVerificationToken,
        // or __RequestVerificationToken_{appPath}.
        var tokenName = "__RequestVerificationToken" + appPath;

        // Finds the <input type="hidden" name={tokenName} value="..." /> from the specified window.
        // var inputElements = tokenWindow.$("input[type='hidden'][name=' + tokenName + "']");
        var inputElements = tokenWindow.document.getElementsByTagName("input");
        for (var i = 0; i < inputElements.length; i++) {
            var inputElement = inputElements[i];
            if (inputElement.type === "hidden" && inputElement.name === tokenName) {
                return {
                    name: tokenName,
                    value: inputElement.value
                };
            }
        }
    };

    $.appendAntiForgeryToken = function (data, token) {
        // Converts data if not already a string.
        if (data && typeof data !== "string") {
            data = $.param(data);
        }

        // Gets token from current window by default.
        token = token ? token : $.getAntiForgeryToken(); // $.getAntiForgeryToken(window).

        data = data ? data + "&" : "";
        // If token exists, appends {token.name}={token.value} to data.
        return token ? data + encodeURIComponent(token.name) + "=" + encodeURIComponent(token.value) : data;
    };

    // Wraps $.post(url, data, callback, type) for most common scenarios.
    $.postAntiForgery = function (url, data, callback, type) {
        return $.post(url, $.appendAntiForgeryToken(data), callback, type);
    };

    // Wraps $.ajax(settings).
    $.ajaxAntiForgery = function (settings) {
        // Supports more options than $.ajax(): 
        // settings.token, settings.tokenWindow, settings.appPath.
        var token = settings.token ? settings.token : $.getAntiForgeryToken(settings.tokenWindow, settings.appPath);
        settings.data = $.appendAntiForgeryToken(settings.data, token);
        return $.ajax(settings);
    };
})(jQuery);

/* Example Usage */
/*

In most of the scenarios, it is Ok to just replace $.post() invocation with $.postAntiForgery(), and replace $.ajax() with $.ajaxAntiForgery():

$.postAntiForgery(url, {
    productName: "Tofu",
    categoryId: 1
}, callback); // The same usage as $.post(), but token is posted. 
There might be some scenarios of custom token, where $.appendAntiForgeryToken() is useful:

data = $.appendAntiForgeryToken(data, token);
// Token is already in data. No need to invoke $.postAntiForgery().
$.post(url, data, callback);
or $.ajaxAntiForgery() can be used:

$.ajaxAntiForgery({
    type: "POST",
    url: url,
    data: {
        productName: "Tofu",
        categoryId: 1
    },
    success: callback, // The same usage as $.ajax(), supporting more options.
    token: token // Custom token.
});
And there are special scenarios that the token is not in the current window. For example:

An HTTP POST request can be sent from an iframe, while the token is in the parent window or top window;
An HTTP POST request can be sent from an popup window or a dialog, while the token is in the opener window;
etc. Here, token's container window can be specified for $.getAntiForgeryToken():

data = $.appendAntiForgeryToken(data, $.getAntiForgeryToken(window.parent));
// Token is already in data. No need to invoke $.postAntiForgery().
$.post(url, data, callback);
or $.ajaxAntiForgery() can be used:

$.ajaxAntiForgery({
    type: "POST",
    url: url,
    data: {
        productName: "Tofu",
        categoryId: 1
    },
    success: callback, // The same usage as $.ajax(), supporting more options.
    tokenWindow: window.parent // Token is in another window.
});

*/