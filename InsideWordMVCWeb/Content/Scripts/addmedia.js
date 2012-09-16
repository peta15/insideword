/// <reference path="/Scripts/thirdparty/jquery-1.4.4.min.js" />

var FileBrowserDialogue = {
    init: function () {
        FileBrowserDialogue.mySubmit();
    },
    mySubmit: function () {
        var URL = document.getElementById("fileURL").firstChild.nodeValue;
        var win = tinyMCEPopup.getWindowArg("window");

        // console.log("submit URL: " + URL);

        // insert information now
        win.document.getElementById(tinyMCEPopup.getWindowArg("input")).value = URL;

        // are we an image browser
        if (typeof (win.ImageDialog) != "undefined") {
            // we are, so update image dimensions...
            if (win.ImageDialog.getImageData)
                win.ImageDialog.getImageData();

            // ... and preview if necessary
            if (win.ImageDialog.showPreviewImage)
                win.ImageDialog.showPreviewImage(URL);
        }

        // close popup window
        tinyMCEPopup.close();
    }
}

tinyMCEPopup.onInit.add(FileBrowserDialogue.init, FileBrowserDialogue);