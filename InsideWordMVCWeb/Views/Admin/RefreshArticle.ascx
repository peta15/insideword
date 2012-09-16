<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<InsideWordMVCWeb.ViewModels.Admin.RefreshArticleVM>" %>

<script type="text/javascript">
    $(document).ready(function () {

        var statusUpdateActive = false;

        var statusUpdate = function (messageDiv, progressDiv) {
            $.get('<%: Url.Action(MVC.Admin.RefreshArticleStatus()) %>', function (json) {
                if (json.StatusCode == 0 || json.StatusCode == 2) {
                    progressDiv.html(json.StatusMessage);
                    messageDiv.html(json.Content);

                    if (json.StatusCode == 0) {
                        window.setTimeout(function () {
                            statusUpdate(messageDiv, progressDiv);
                        }
                        , 1000);
                        statusUpdateActive = true;
                    } else {
                        statusUpdateActive = false;
                    }
                } else {
                    statusUpdateActive = false;
                }
            });
        };

        var refreshPOST = function (e) {
            e.preventDefault(); //no postback
            var partialDiv = $(this).parent(".InsideWord_refreshArticle");
            var refreshResult = $(this).find(".refreshResults");
            $.post($(this).attr("action"),
                    $(this).serialize(),
                    function (json) {
                        if (json.StatusCode == 0) {
                            if (!statusUpdateActive) {
                                statusUpdate($(".refreshResults"), $(".progressDiv"));
                            }
                        } else if (json.StatusCode == 1 && json.Content != null && json.Content != "") {
                            partialDiv.replaceWith(json.Content);
                        };
                    }
                );
        };

        var refreshDiv = $(".InsideWord_refreshArticle");
        var formDiv = refreshDiv.find("form");
        formDiv.unbind("submit").live("submit", refreshPOST);
        statusUpdate($(".refreshResults"), $(".progressDiv"));
    });
</script>

<div class="InsideWord_refreshArticle">
    <h3>Article Text and Picture refesh</h3>
	<div class="h3hr"></div>
    <% using (Html.BeginForm(MVC.Admin.RefreshArticle())) {%>
        <div class="editor-label">
            <input type="submit" class="button" value="Refresh Articles" />
        </div>
        <div class="editor-label">Progress: </div>
        <div class="editor-field">
            <div class="progressDiv"></div>
        </div>
        <div class="editor-label">Errors: </div>
        <div class="editor-field refreshResults"></div>
    <% } %>
</div>