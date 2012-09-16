/// <reference path="/Scripts/thirdparty/jquery-1.4.4.min.js" />

//
// Javascript for the category view
//

// JSON li builder object for the insideword jquery createList function.
var categoryLiBuilder = {
    callCount: 0,
    liObject: {},
    editUrl: "",
    createLi: function () {
        var idHidden = $(document.createElement("input")).attr("type", "hidden")
                                                         .attr("name", "model[" + this.callCount + "].Num")
                                                         .attr("value", this.liObject.Id);
        var editButton = $(document.createElement("a")).text("Edit")
                                                       .attr("href", this.editUrl + "/" + this.liObject.Id);
        var titleSpan = $(document.createElement("span")).text(this.liObject.Title);
        var deleteChkbx = $(document.createElement("input")).val("true")
                                                            .attr("type", "checkbox")
                                                            .attr("name", "model[" + this.callCount + "].Bit");
        var deleteSpan = $(document.createElement("span")).text("Delete");
        this.callCount++;
        return $(document.createElement("li")).append(idHidden)
                                              .append(editButton)
                                              .append(titleSpan)
                                              .append(deleteChkbx)
                                              .append(deleteSpan);
    }
};