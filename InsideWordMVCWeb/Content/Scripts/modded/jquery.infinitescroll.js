
/*!
// Infinite Scroll jQuery plugin
// copyright Paul Irish, licensed GPL & MIT
// version 1.5.100504

// home and docs: http://www.infinite-scroll.com
// CUSTOMIZED BY INSIDEWORD
*/
 
(function ($) {

    $.fn.infinitescroll = function (options, callback) {

        // console log wrapper.
        function debug() {
            if (opts.debug) { window.console && console.log.call(console, arguments) }
        }

        // grab each selector option and see if any fail.
        function areSelectorsValid(opts) {
            for (var key in opts) {
                if (key.indexOf && key.indexOf('Selector') > -1 && $(opts[key]).length === 0) {
                    debug('Your ' + key + ' found no elements.');
                    return false;
                }
                return true;
            }
        }


        // find the number to increment in the path.
        // FUNCTION CUSTOMIZED BY INSIDEWORD
        function determinePath(path) {

            path.match(relurl) ? path.match(relurl)[2] : path;

            // matching for the insideword.com index page http://www.insideword.com/23/1 where 23 here is the categoryId and 1 is the page
            var nextRegexp = /^(.*?\/\d+?\/)1$/;
            if (path.match(nextRegexp)) {
                path = path.match(nextRegexp).slice(1);
            }
            else {
                debug('Sorry, we couldn\'t parse your Next (Previous Posts) URL. Verify your the css selector points to the correct A tag. If you still get this error: yell, scream, and kindly ask for help at infinite-scroll.com.');
                props.isInvalidPage = true;  //prevent it from running on this page.
            }

            return path;
        }


        // 'document' means the full document usually, but sometimes the content of the overflow'd div in local mode
        // FUNCTION CUSTOMIZED BY INSIDEWORD
        function getDocumentHeight() {
            var docHeight = $(document).height();

            // weird doubletouch of scrollheight because http://soulpass.com/2006/07/24/ie-and-scrollheight/
            var containerHeight = $(props.container)[0].scrollHeight && $(props.container)[0].scrollHeight;

            // needs to be document's height. (not props.container's) html's height is wrong in IE.
            return opts.localMode ? (containerHeight) : docHeight;
        }



        function isNearBottom() {

            // distance remaining in the scroll
            // computed as: document height - distance already scroll - viewport height - buffer
            var pixelsFromWindowBottomToBottom = getDocumentHeight() - (
                    opts.localMode ? $(props.container).scrollTop() :
            // have to do this bs because safari doesnt report a scrollTop on the html element
                    ($(props.container).scrollTop() || $(props.container.ownerDocument.body).scrollTop())
                    ) - $(opts.localMode ? props.container : window).height();

            debug('math:', pixelsFromWindowBottomToBottom, props.pixelsFromNavToBottom);

            // if distance remaining in the scroll (including buffer) is less than the orignal nav to bottom....
            return (pixelsFromWindowBottomToBottom - opts.bufferPx < props.pixelsFromNavToBottom);
        }

        function showDoneMsg() {
            props.loadingMsg
        .find('img').hide()
        .parent()
          .find('div').html(opts.donetext).animate({ opacity: 1 }, 2000).fadeOut('normal');

            // user provided callback when done    
            opts.errorCallback();
        }

        function infscrSetup() {

            if (props.isDuringAjax || props.isInvalidPage || props.isDone) return;

            if (!isNearBottom(opts, props)) return;

            $(document).trigger('retrieve.infscr');


        }  // end of infscrSetup()

        // FUNCTION CUSTOMIZED BY INSIDEWORD
        function kickOffAjax() {

            // we dont want to fire the ajax multiple times
            props.isDuringAjax = true;

            if (props.debug) {
                // show the loading message
                props.loadingMsg.appendTo(opts.contentSelector).show();
            }

            // hide the previous/next links
            $(opts.navSelector).hide();

            // increment the URL bit. e.g. /page/3/
            props.currPage++;

            debug('heading into ajax', path + props.currPage);

            // if we're dealing with a table we can't use DIVs
            box = $(opts.contentSelector).is('table') ? $('<tbody/>') : $('<div/>');
            frag = document.createDocumentFragment();

            box.load(path + props.currPage + ' ' + opts.itemSelector, null, loadCallback);

        }

        // FUNCTION CUSTOMIZED BY INSIDEWORD
        function loadCallback() {
            // if we've hit the last page...
            if (props.isDone) {
                showDoneMsg();
                return false;

            } else {

                var children = box.children().get();

                // if it didn't return anything
                if (children.length == 0) {
                    // fake an ajaxError so we can quit.
                    return $.event.trigger("ajaxError", [{ status: 404}]);
                }

                // use a documentFragment because it works when content is going into a table or UL
                while (box[0].firstChild) {
                    frag.appendChild(box[0].firstChild);
                }

                $(opts.contentSelector)[0].appendChild(frag);

                if (props.debug) {
                    // fadeout currently makes the <em>'d text ugly in IE6
                    props.loadingMsg.fadeOut('normal');
                }

                // smooth scroll to ease in the new content
                if (opts.animate) {
                    var scrollTo = $(window).scrollTop() + $('#infscr-loading').height() + opts.extraScrollPx + 'px';
                    $('html,body').animate({ scrollTop: scrollTo }, 800, function () { props.isDuringAjax = false; });
                }

                // previously, we would pass in the new DOM element as context for the callback
                // however we're now using a documentfragment, which doesnt havent parents or children,
                // so the context is the contentContainer guy, and we pass in an array
                //   of the elements collected as the first argument.
                callback.call($(opts.contentSelector)[0], children);

                if (!opts.animate) props.isDuringAjax = false; // once the call is done, we can allow it again.
            }
        }


        // lets get started.
        $.browser.ie6 = $.browser.msie && $.browser.version < 7;

        var opts = $.extend({}, $.infinitescroll.defaults, options),
        props = $.infinitescroll, // shorthand
        box, frag;

        callback = callback || function () { };

        if (!areSelectorsValid(opts)) { return false; }

        // we doing this on an overflow:auto div?
        props.container = opts.localMode ? this : document.documentElement;

        // contentSelector we'll use for our .load()
        opts.contentSelector = opts.contentSelector || this;


        // get the relative URL - everything past the domain name.
        var relurl = /(.*?\/\/).*?(\/.*)/,
        path = $(opts.nextSelector).attr('href');


        if (!path) { debug('Navigation selector not found'); return; }

        // set the path to be a relative URL from root.
        path = determinePath(path);


        // reset scrollTop in case of page refresh:
        if (opts.localMode) $(props.container)[0].scrollTop = 0;

        // distance from nav links to bottom
        // computed as: height of the document + top offset of container - top offset of nav link
        // FUNCTION CUSTOMIZED BY INSIDEWORD
        var docHeight = getDocumentHeight();
        var contTopOffSet = props.container == document.documentElement ? 0 : $(props.container).offset().top;
        var navTopOffSet = $(opts.navSelector).offset().top;
        props.pixelsFromNavToBottom = docHeight + contTopOffSet - navTopOffSet;

        // define loading msg
        props.loadingMsg = $('<div id="infscr-loading" style="text-align: center;"><img alt="Loading..." src="' +
                                  opts.loadingImg + '" /><div>' + opts.loadingText + '</div></div>');
        // preload the image
        (new Image()).src = opts.loadingImg;



        // set up our bindings
        $(document).ajaxError(function (e, xhr, opt) {
            debug('Page not found. Self-destructing...');

            // die if we're out of pages.
            if (xhr.status == 404) {
                showDoneMsg();
                props.isDone = true;
                $(opts.localMode ? this : window).unbind('scroll.infscr');
            }
        });

        // bind scroll handler to element (if its a local scroll) or window  
        $(opts.localMode ? this : window)
      .bind('scroll.infscr', infscrSetup)
      .trigger('scroll.infscr'); // trigger the event, in case it's a short page

        $(document).bind('retrieve.infscr', kickOffAjax);

        return this;

    }  // end of $.fn.infinitescroll()



    // options and read-only properties object

    $.infinitescroll = {
        defaults: {
            debug: false,
            preload: false,
            nextSelector: "div.navigation a:first",
            loadingImg: "http://www.infinite-scroll.com/loading.gif",
            loadingText: "<em>Loading the next set of posts...</em>",
            donetext: "<em>Congratulations, you've reached the end of the internet.</em>",
            navSelector: "div.navigation",
            contentSelector: null,           // not really a selector. :) it's whatever the method was called on..
            extraScrollPx: 150,
            itemSelector: "div.post",
            animate: false,
            localMode: false,
            bufferPx: 40,
            errorCallback: function () { }
        },
        loadingImg: undefined,
        loadingMsg: undefined,
        container: undefined,
        currPage: 0,
        currDOMChunk: null,  // defined in setup()'s load()
        isDuringAjax: false,
        isInvalidPage: false,
        isDone: false  // for when it goes all the way through the archive.
    };



})(jQuery);
