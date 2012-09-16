/// <reference path="/Scripts/thirdparty/jquery-1.4.4.min.js" />

/*=========================================*/
/* Account Page
/*=========================================*/

$(document).ready(function () {
    $(".charCount_textBio").charCount({
        allowed: 1024,
        warning: 20,
        counterText: 'Characters left: '
    });
});