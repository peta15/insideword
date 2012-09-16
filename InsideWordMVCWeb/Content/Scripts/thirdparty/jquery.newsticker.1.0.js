/*
* jQuery newsTicker plugin
*
* Released: 2009-11-23
* Version: 1.0
*
* Copyright (c) 2009 Fabian de Rijk, Total Active Media
* Dual licensed under the MIT and GPL licenses.
* http://docs.jquery.com/License
*/

(function ($) {

    $.fn.extend({

        newsTicker: function (options) {
            /*
            * Usage:
            * $('#divname').newsTicker();
            * Available options:
            * 	showTime: The time a single news item is showed in milliseconds, defaults to 5000
            * 	childDivClass: The classname of the divs where the actual news is in, defaults to tickerText
            * 	activeClass: The class added to the shown news item, defaults to activeTicker
            * 	effect: The effect type used to show/hide a news item, defaults to fade, other options: slide
            * 	effectTime: The time in milliseconds it takes to do each step in the effect, defaults to 500
            * 	
            * Example with full options
            * $('#divname').newsTicker({
            * 	showTime: 10000,
            * 	childDivClass: 'childClass',
            * 	activeClass: 'activeClass',
            * 	effect: 'slide',
            * 	effectTime: 1000
            * });
            */

            var defaults = {
                showTime: 5000,
                childDivClass: 'tickerText',
                activeClass: 'activeTicker',
                effect: 'fade',
                effectTime: 500
            }

            var options = $.extend(defaults, options);
            var obj = $(this);
            var childLength = obj.children('.' + options.childDivClass).length;
            var children = obj.children('.' + options.childDivClass)
            var firstChild = obj.children('.' + options.childDivClass + ':first');
            var t;

            return this.each(function () {
                if (childLength > 1) {
                    var el = obj.children('.' + options.childDivClass + ':first');
                    el.addClass(options.activeClass);
                    startTimer();
                }
            });

            function startTimer() {
                var delay = options.showTime;
                t = setTimeout(function () {
                    showNewTicker();
                }, delay);
            }

            function resetTimer() {
                clearTimeout(t);
                startTimer();
            }

            function showNewTicker() {
                var active = obj.children('.' + options.activeClass);
                var next = active.next();
                if (next.length == 0) {
                    next = firstChild;
                }
                switch (options.effect) {
                    default:
                    case 'fade':
                        // fade out active div
                        children.each(function () {
                            if (!$(this).hasClass(options.activeClass)) {
                                $(this).hide();
                            }
                        });
                        active.fadeOut(options.effectTime, function () {
                            active.removeClass(options.activeClass);
                            next.fadeIn(options.effectTime, function () {
                                next.addClass(options.activeClass);
                                resetTimer();
                            })
                        })
                        break;
                    case 'slide':
                        // slide active div up
                        var i = 0;
                        children.each(function () {
                            if ($(this).css('position') != 'relative') {
                                if ($(this).hasClass(options.activeClass)) {
                                    $(this).css('position', 'relative').css('top', '0px').attr('rel', i);
                                }
                                else {
                                    var t = 32 - i * 32;
                                    $(this).css('position', 'relative').css('top', t + 'px').attr('rel', i);
                                }
                                i++;
                            }
                        })
                        active.animate({
                            top: parseInt(active.css('top').replace('px', '')) - 32
                        }, options.effectTime, 'swing', function () {
                            var t = 32 - parseInt(active.attr('rel')) * 32;
                            active.css('top', t + 'px');
                        });
                        next.animate({
                            top: parseInt(next.css('top').replace('px', '')) - 32
                        }, options.effectTime, 'swing', function () {
                            active.removeClass(options.activeClass);
                            next.addClass(options.activeClass);
                            resetTimer();
                        });
                        break;
                }
            }
        }
    });

})(jQuery);