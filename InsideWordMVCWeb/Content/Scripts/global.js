/// <reference path="/Scripts/thirdparty/jquery-1.4.4.min.js" />
/*=====================================================*/
/* GLOBAL GLOBAL GLOBAL GLOBAL GLOBAL GLOBAL GLOBAL
/*=====================================================*/
var SETTINGS = {
    debug: false
}

function JqGridSuccess(data) {
    return JSON.parse(data.responseText).Success;
}

function JqGridError(rowid, data, stat) {
    alert(JSON.parse(data.responseText).Message);
}

function JqGridAfterSubmit(data, postdata) {
    var jsonObject = JSON.parse(data.responseText);
    return [jsonObject.Success, jsonObject.Message];
}

function debug() {
    if (SETTINGS.debug) { window.console && console.log.call(console, arguments) }
}

// override alerts with jquery ui dialog (just call alerts as usual with alert('stuff')
$(function () {
    $.altAlert();
});

/*=========================================*/
/* Google analytics
/*=========================================*/
var _gaq = _gaq || [];
_gaq.push(['_setAccount', 'UA-23041137-1']);
_gaq.push(['_trackPageview']);

(function () {
    var ga = document.createElement('script'); ga.type = 'text/javascript'; ga.async = true;
    ga.src = ('https:' == document.location.protocol ? 'https://ssl' : 'http://www') + '.google-analytics.com/ga.js';
    var s = document.getElementsByTagName('script')[0]; s.parentNode.insertBefore(ga, s);
})();

/*=========================================*/
/* Voting
/*=========================================*/
$(document).ready(function () {
    var voteWidget = $("form.voteWidget");
    voteWidget.unbind("submit").live("submit", submitVote);
    voteWidget.find(".up, .down").click(function () {
        //copy over the vote value to the hidden form.
        var form = $(this).parents("form:first");
        var hidden = form.find(".voteValue");
        var value = $(this).val();
        hidden.val(value);
    });
});

var submitVote = function (e) {
    e.preventDefault();
    var voteForm = $(this);
    var resultBox = $(this).find(".result");
    if (!voteForm.hasClass('userVoted') && !voteForm.hasClass('loginToVote')) {
        $.post(voteForm.attr("action"),
                voteForm.serialize(),
                function (json) {
                    if (json.StatusCode == 0) {
                        // success.  update vote number and disable vote buttons.
                        resultBox.text(json.Content);
                        voteForm.addClass('userVoted');
                    } else {
                        // TODO replace with a modal
                        alert("There was an error submitting your vote:\n" + json.StatusMessage);
                    }
                });
    }
}

// GENERAL PARTIAL VIEW UPDATE CODE
$(document).ready(function () {
    $(".InsideWord_partial").find("form").unbind('submit').live("submit", partialViewUpdate);
});

var partialViewUpdate = function (e) {
    e.preventDefault(); //no postback
    var partialDiv = $(this).parent(".InsideWord_partial");
    $.post($(this).attr("action"),
        $(this).serialize(),
        function (json) {
            if (json.Action == 0) {
                window.location = json.Content;
            } else if (json.Action == 1 && json.Content != null && json.Content != "") {
                partialDiv.replaceWith(json.Content);
            };
        }
    );
};

/*=====================================================*/
/* MASTER MASTER MASTER MASTER MASTER MASTER MASTER
/*=====================================================*/

$(document).ready(function () {

    // Just Published News Ticker
    $("#newsTicker").scrollable({
        speed: 1600,
        circular: true,
        mousewheel: true,
        easing: "linear",
        next: ".tickerNext",
        prev: ".tickerPrev"
    }).autoscroll({
        interval: 8000
    });

    // Navigation List
    $("ul.sf-menu").superfish({
        animation: { height: 'show' },   // slide-down effect without fade-in 
        delay: 1200,               // 1.2 second delay on mouseout 
        speed: 'fast'
    });

});

//Login Modal
$(document).ready(function () {

    $("#modalLoginTabs").tabs({
        event: "mouseover"
    });

    $("#modalLogin").dialog({
        autoOpen: false,
        show: "fade",
        hide: "fade",
        draggable: true,
        width: 580,
        minWidth: 580
    });

    //steal the close button
    $('#ui-tab-dialog-close').append($('a.ui-dialog-titlebar-close'));

    //move the tabs out of the content and make them draggable
    $('.ui-dialog').addClass('ui-tabs')
            .prepend($('#modalLoginTabs'))
            .draggable('option', 'handle', '.ui-tabs-nav');

    //switch the titlebar class
    $('.ui-dialog-titlebar').remove();
    $('#modalLoginTabs').addClass('ui-dialog-titlebar');

    $(".callModalLogin").click(function () {
        if (SETTINGS.loggedOn)
            return;
        $("#modalLogin").dialog("open");
        return false;
    });
});