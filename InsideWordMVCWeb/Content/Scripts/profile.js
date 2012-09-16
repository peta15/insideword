/// <reference path="/Scripts/thirdparty/jquery-1.4.4.min.js" />

$(document).ready(function () {
    $('.articleList').infinitescroll({
            navSelector: '#infscr-pageNav',    // selector for the paged navigation 
            nextSelector: '#infscr-pageNav > a',    // selector for the NEXT link (to page 2)
            itemSelector: '.articleList .article',       // selector for all items you'll retrieve
            loadingImg: '/Content/img/interface/loader_spiral.gif',
            debug: false,
            resizeable: false,
            animate: false,
            extraScrollPx: 150,
            loadingText: "<em>Loading the next set of posts...</em>",
            donetext: 'Please visit our archives for more content',
            localMode: false,
            bufferPx: $(this).height(),
            errorCallback: function () {
                // fade out the error message after 2 seconds
                $('#infscr-loading').animate({ opacity: .8 }, 2000).fadeOut('normal');
            }
        });
});