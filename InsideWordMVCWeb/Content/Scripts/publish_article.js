/// <reference path="/Scripts/thirdparty/jquery-1.4.4.min.js" />

/*=========================================*/
/* Publish Article Page
/*=========================================*/

$(document).ready(function () {

    $(".charCount_textBlurb").charCount({
        allowed: 256,
        warning: 20,
        counterText: 'Characters left: '
    });
    $(".charCount_textArticleBody").charCount({
        allowed: 32768,
        warning: 300,
        counterText: 'Characters left: '
    });
    $(".charCount_textTitle").charCount({
        allowed: 65,
        warning: 5,
        counterText: 'Characters left: '
    });
    $(".charCount_textArticleEmail").charCount({
        allowed: 65,
        warning: 5,
        counterText: 'Characters left: '
    });

    $('textarea.tinymce').tinymce({
        // Location of TinyMCE script
        script_url: '/Content/tiny_mce/tiny_mce.js',

        // convert image and link urls to absolute urls
        relative_urls: false,
        remove_script_host: false,
        //document_base_url : "http://www.insideword.com/",   // use to convert relative urls to absolute at this domain

        // General options
        theme: "advanced",
        plugins: "iespell,preview,media,searchreplace,print,contextmenu,paste,fullscreen,visualchars,advlist,inlinepopups",
        dialog_type: "modal",

        // browse button callback
        file_browser_callback: 'fileBrowser',

        // Theme options
        theme_advanced_buttons1: "bold,italic,underline,strikethrough,forecolor,|,justifyleft,justifycenter,justifyright,justifyfull,|,image,charmap,|,replace,undo,redo,|,bullist,numlist,|,outdent,indent,|,link,unlink,|,fullscreen,code,print",
        theme_advanced_buttons2: "",
        theme_advanced_buttons3: "",
        theme_advanced_toolbar_location: "top",
        theme_advanced_toolbar_align: "left",
        theme_advanced_statusbar_location: "bottom",
        theme_advanced_resizing: false,  // set false to avoid messing with page layout

        // site css
        content_css: "/Content/css/article.css"

        // Drop lists for link/image/media/template dialogs
        /* TODO: These links don't exist. We should delete this or update it.
        template_external_list_url: "lists/template_list.js",
        external_link_list_url: "lists/link_list.js",
        external_image_list_url: "lists/image_list.js",
        media_external_list_url: "lists/media_list.js"
        */
    });

    $('#publishButton, #saveButton, #previewButton').unbind("click").click(function () {
        //trigger a save of the tinymce text so that MVC can properly validate.
        tinymce.triggerSave();
        //pass the appropriate value from the button to the hidden input
        $("#submitType").val($(this).val());
    });
});

function fileBrowser(field_name, url, type, win) {
    // console.log("Field_Name: " + field_name + "\nURL: " + url + "\nType: " + type + "\nWin: " + win); // debug

    /* If you work with sessions in PHP and your client doesn't accept cookies you might need to carry
    the session name and session ID in the request string (can look like this: "?PHPSESSID=88p0n70s9dsknra96qhuk6etm5").
    These lines of code extract the necessary parameters and add them back to the filebrowser URL again. */

    var editorURL = window.location.toString();
    var host = window.location.protocol + "//" + window.location.host + "/";
    var queryParams = window.location.search.substring(1);

    var cmsURL = host + "shared/add_media/tinymce/" + type;    // script URL - use an absolute path!

    //alert("fileBrowserDialogue URL: " + cmsURL);

    tinyMCE.activeEditor.windowManager.open({
        file: cmsURL,
        title: 'Upload Media',
        width: 420,
        height: 150,
        resizable: "yes",
        inline: "yes",  // makes the window inline/modal.  requires inlinepopups plugin
        close_previous: "no"
    }, {
        window: win,
        input: field_name
    });
    return false;
}

// NOTE: UNUSED at the moment
// retrieve associative array of query string parameters
function getQueryStringParamsArray() {
    var qsParm = new Array();
    var query = window.location.search.substring(1);
    var parms = query.split('&');
    for (var i = 0; i < parms.length; i++) {
        var pos = parms[i].indexOf('=');
        if (pos > 0) {
            var key = parms[i].substring(0, pos);
            var val = parms[i].substring(pos + 1);
            qsParm[key] = val;
        }
    }
    return qsParm;
}