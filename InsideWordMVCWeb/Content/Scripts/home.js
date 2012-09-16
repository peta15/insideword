/// <reference path="/Scripts/thirdparty/jquery-1.4.4.min.js" />

$(document).ready(function () {

    var masonryPixelBuffer = 128;

    var AdditionalIndexMasonry = function (targetMasonObject, parentMasonObject, footerPixelBuffer) {
        var currentHeight = parentMasonObject.height();
        var newHeight = targetMasonObject.height() + footerPixelBuffer;
        // don't let the height get smaller than the current height
        if (newHeight < currentHeight) {
            newHeight = currentHeight;
        }
        parentMasonObject.height(newHeight);
    };

    // Masonry must come before infinitescroll
    $(".blurbList").masonry({
        singleMode: true,
        itemSelector: '.blurb:visible'
    });

    AdditionalIndexMasonry($(".blurbList"), $(".sliderIndex"), masonryPixelBuffer);

    $('.blurbList').infinitescroll({
        navSelector: '#infscr-pageNav',    // selector for the paged navigation 
        nextSelector: '#infscr-pageNav > a',    // selector for the NEXT link (to page 2)
        itemSelector: '.blurbList .blurb',       // selector for all items you'll retrieve
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
    },

    // call masonry as a callback
        function (newElements) {
            $(this).masonry({ appendedContent: $(newElements) });
            AdditionalIndexMasonry($(this), $(".sliderIndex"), masonryPixelBuffer);
        });


});

$(document).ready(function () {
    /*variable that indicates whether the slider is resting on the left or right side*/
    var sliderIsLeft = true;

    /*variables to store the slider image css*/
    var sliderHandleImage = 'transparent url(/Content/img/interface/slider.png) no-repeat scroll 0 0';
    var sliderHandleImageFlipped = 'transparent url(/Content/img/interface/slider.png) no-repeat scroll -36px 0';
    var sliderHandleImageMap = {'background': sliderHandleImage};
    var sliderHandleImageFlippedMap = {'background': sliderHandleImageFlipped};

    /*Initializes the logic for our slider control*/
    $("#readerWriterSlider").slider({
        animate: true,
        stop: function (event, ui) {
            $("#readerWriterSlider").slider("disable");
            if (sliderIsLeft) {
                
                if (ui.value > 65) {
                    /*The slider is past the threshold so continue sliding it 
                    to the opposite end and, upon finishing, execute the scrollable*/
                    $(ui.handle).animate({ left: "100%" },
                                        200,
                                        "swing",
                                        function () {
                                        $(this).css(sliderHandleImageFlippedMap);
                                            sliderIsLeft = false;
                                            $("#readerWriterSlider").slider("enable");
                                            $(".sliderIndex").scrollable().end(400);
                                        });
                } else {
                    /*If the slider does get past the threshold 
                    position then gently slide it back to the start position*/
                    $(ui.handle).animate({ left: 0 },
                                            200,
                                            "swing",
                                            function () {
                                                $("#readerWriterSlider").slider("enable");
                                            });
                }
            } else {
                if (ui.value < 35) {
                    /*The slider is past the threshold so continue sliding it 
                    to the opposite end and, upon finishing, execute the scrollable*/
                    $(ui.handle).animate({ left: 0 },
                                        200,
                                        "swing",
                                        function () {
                                            sliderIsLeft = true;
                                            $(this).css(sliderHandleImageMap);
                                            $("#readerWriterSlider").slider("enable");
                                            $(".sliderIndex").scrollable().begin(400);
                                        });
                } else {
                    /*If the slider does get past the threshold position then gently slide it back to the start position*/
                    $(ui.handle).animate({ left: "100%" },
                                            200,
                                            "swing",
                                            function () {
                                                $("#readerWriterSlider").slider("enable");
                                            });
                }
            };
        }
    });

    /*Replace the slider bar with our own css*/
    $("#readerWriterSlider").css({
        'background': 'transparent none no-repeat scroll',
        'border': '0px',
        'color': 'transparent',
        'height': '25px',
        'width':'103px'
    });

    /*Replace the slider handle with our own css*/
    $("#readerWriterSlider").children(".ui-slider-handle").css({
        'background': sliderHandleImage,
        'border': '0px',
        'color': 'transparent',
        'height': '25px',
        'width': '36px',
        'top': '0px',
        'margin-left' : '0px'
    });

    /*setup the scrollable settings*/
    $(".sliderIndex").scrollable({
        next: ".sliderNext",
        prev: ".sliderPrev"
    });

    /*setup the "I'm a writer" button click to also interact with the slider
    by gently sliding the slider handle to the right*/
    $(".sliderNext").click(function(){
        $("#readerWriterSlider").children(".ui-slider-handle")
                                .animate({ left: "100%" },
                                        200,
                                        "swing",
                                        function () {
                                        $(this).css(sliderHandleImageFlippedMap);
                                            sliderIsLeft = false;
                                            $("#readerWriterSlider").slider("enable");
                                        });
                                
    });

    /*setup the "I'm a reader" button click to also interact with the slider
    by gently sliding the slider handle to the left*/
    $(".sliderPrev").click(function(){
        $("#readerWriterSlider").children(".ui-slider-handle")
                                .animate({ left: 0 },
                                        200,
                                        "swing",
                                        function () {
                                            sliderIsLeft = true;
                                            $(this).css(sliderHandleImageMap);
                                            $("#readerWriterSlider").slider("enable");
                                        });
    });
});