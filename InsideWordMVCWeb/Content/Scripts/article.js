/// <reference path="/Scripts/thirdparty/jquery-1.4.4.min.js" />

/*=========================================*/
/* Comments
/*=========================================*/

var commentVoteButtonsSelector = ".comments .commentVote";
var ajaxData = {};

$(document).ready(function () {
    // .live() is needed with update panels, otherwise events are 
    // not fired for objects that are reinstantiated through the update panels

    // procedure when user clicks to reply to a conversation
    $(".openCommentReply").live('click', function () {

        // stop procedure and prompt with login modal if logged out
        if (LoginInterrupt()) {
            return true;
        }

        // remove old comment input boxes
        $(".commentReply").remove();

        // replicate template for comment replies
        var replyBox = $(".commentReplyTemplate").clone();
        replyBox.addClass("commentReply");

        // retrieve id of conversation we are replying to and store it
        var convoId = -1;
        var divConvo = $(this).parents("div.conversation:first");
        var convoIdStr = divConvo.attr("id");
        convoId = (convoIdStr.split("-"))[1];
        ajaxData.conversationId = convoId;

        replyBox.find("input.replyCommentSubmit").click(function (e) {
            // call ajax to submit.  on success, fade in new comment.
            $(".commentBodyReply").fadeIn(function () { $(this).removeClass("hidden"); });
            // $(".commentBodyReply .loadingComment").fadeIn(function () { $(this).removeClass("hidden"); }); // loading bar commented out for now
            e.preventDefault();
            e.stopPropagation();
            //Get the form wrapping the button
            var form = $(this).parents("form:first");
            var self = $(this);
            $.post(form.attr("action"), form.serialize() + "&conversationId=" + ajaxData.conversationId, function (data) {
                if (data.StatusCode == 0) {
                    // submit button on comment replies should fade out comment reply
                    $(".commentReply").fadeOut('fast', function () { $(".commentReply").remove(); });
                    // construct and fade in the new comment
                    $(data.Content).hide().appendTo(self.parents(".conversation:first")).fadeIn(1500);
                } else {
                    // TODO replace with a modal
                    alert("There was an error posting your comment:\n" + data.StatusMessage);
                }
            });

            // add hidden back to the template
            // $(".loadingComment").addClass("hidden"); // loading bar commented out for now
        });

        // close buttons on comment replies should fade out comment reply
        replyBox.find(".close").click(function (e) {
            $(".commentReply").fadeOut('slow', function () { $(".commentReply").remove(); });
        });

        // add the reply convo box in the appropriate spot
        divConvo.append(replyBox);

        // add reply box comment counter
        $(".commentReply .charCount_commentReply").charCount({
            allowed: 512,
            warning: 20,
            counterText: 'Characters left: '
        });

        // fade the comment reply box in and put the users cursor in it
        replyBox.fadeIn('slow', function () { replyBox.removeClass("commentReplyTemplate"); });
        replyBox.find(".commentReplyTextArea").focus();

        // if this procedure was executed from a link, don't follow the link
        return false;
    });

    $(".redirectToStartConvo").live('click', function (event) {
        $(".textComment").focus();
    });

    $(commentVoteButtonsSelector).live('click', function () {
        if ($(commentVoteButtonsSelector).hasClass('disabled'))
            return;
        $(commentVoteButtonsSelector).addClass('disabled');
    });

    $(".charCount_comment").charCount({
        allowed: 512,
        warning: 20,
        counterText: 'Characters left: '
    });

    $(".commentsForm input.createConversationSubmit").click(function (e) {
        // $(".commentsForm .loadingComment").fadeIn(function () { $(this).removeClass("hidden"); }); // loading bar commented out for now
        $(".commentsForm").fadeIn(function () { $(this).removeClass("hidden"); });
        // call ajax to submit
        e.preventDefault();
        e.stopPropagation();
        //Get the form wrapping the button
        var form = $(this).parents("form:first");
        var self = $(this);
        $.post(form.attr("action"), form.serialize(), function (data) {
            if (data.StatusCode == 0) {
                // TODO construct and fade in the new comment
                $(".commentsForm .textComment").val("");
                // construct and fade in the new comment
                $.scrollTo($(".commentsList"), 800);
                $(data.Content).hide().prependTo(".comments .commentsList").animateHighlight().fadeIn(1500);
            } else {
                // TODO replace with a modal
                alert("There was an error posting your comment.\n" + data.StatusMessage);
            }
        });
        // $(".loadingComment").hide(); // loading bar commented out for now
    });
});

function LoginInterrupt() {
    return false;
}