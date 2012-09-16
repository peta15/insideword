<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<InsideWordMVCWeb.ViewModels.Member.AccountVM>" %>

<asp:Content ID="Content2" ContentPlaceHolderID="Head" runat="server">
    <%= Html.Title(Model.PageTitle)%>
    <link rel="stylesheet" type="text/css" href="<%= Links.Content.css.account_css %>" media="all" />
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="Body" runat="server">

    <h1>My Profile Settings</h1>
    <div class="h1hr"></div>

        <% Html.RenderAction(MVC.Child.EditPersonalInfo(Model.Member.Id.Value)); %>

        <div class="left center profileImage">
            <img src="<%: Model.ProfileImage.Src %>"
                 alt="<%: Model.ProfileImage.Alt %>"
                 width="<%: Model.ProfileImage.Width %>"
                 height="<%: Model.ProfileImage.Height %>"
                 class="profileImg" />
            <p><a href="<%: Url.Action(MVC.Shared.AddMedia(InsideWordMVCWeb.ViewModels.Shared.AddMediaVM.AddMediaPurpose.ProfileImage, 
                                                           InsideWordMVCWeb.ViewModels.Shared.AddMediaVM.AddMediaType.Image, null)) %>"
                  class="button">
                Change Profile Image
            </a></p>
        </div>

        <h3>Authentication</h3>
		<div class="h3hr"></div>

        <div class="editor-label">
        </div>
        <div class="editor-field buttons">
            <a href="<%: Url.Action(MVC.Member.ChangePassword()) %>" class="button">Change Password</a>
        </div>

        <% if( Model.EmailAddresses.Count > 0) { %>
            <div class="editor-label">
                <%: Html.LabelFor(model => model.EmailAddresses) %>
            </div>
            <div class="editor-field">
                <%: Html.TextBoxFor(model => model.EmailAddresses[0]) %>
                <!-- TODO call a controller that removes the old email and replaces it with an unactivated one 
                before sending the activation email rather than just adding a providerEmail object.  then ajaxify the post -->
                <%: Html.ValidationMessageFor(model => model.EmailAddresses[0]) %>
            </div>

            <% for (int i=1; i < Model.EmailAddresses.Count; i++) { %>
                <div class="editor-label"></div>
                <div class="editor-field">
                    <%: Html.TextBoxFor(model => model.EmailAddresses[i]) %>
                </div>
            <% }
           }
        %>

        <% using (Html.BeginForm(MVC.Member.RequestValidateEmail())) {%>
            <div class="editor-label">
                <%: Html.LabelFor(model => model.NewEmail) %>
            </div>
            <div class="editor-field">
                <input type="hidden" name="memberId" value="<%: Model.Member.Id.Value %>" />
                <%: Html.TextBoxFor(model => model.NewEmail) %>
                <input type="submit" class="button" value="Confirm new e-mail" />
                <p class="note">
                    Email authenticity must be confirmed to use them for login, 
                    email notification etc.  You will receive an email with a link to confirm.
                     &nbsp;&nbsp;<%: Html.ActionLink("Learn more", MVC.Info.Faq() ) %>
                </p>
            </div>
        <% } %>

        <div class="editor-label">
            OpenIds
        </div>
        <div class="editor-field-large">
            <table>
                <tr>
                    <th>Provider</th><th>Email</th><th>OpenId Url</th><th></th>
                </tr>
                <% foreach (InsideWordProvider.ProviderAlternateMemberId openId in Model.OpenIds)
                   { %>
                <tr>
                    <td><%: openId.DisplayName %></td><td><%: openId.Data %></td><td><%: openId.Id %></td><td><!--<a href="/member/openid/delete">remove</a>--></td>
                </tr>
                <% } %>
            </table>
        </div>
        <!--
        <div class="editor-label">
        </div>
        <div class="editor-field buttons">
            <a href="#" class="button">Add OpenId</a>
        </div>
        -->

    <% if(Model.DisplayAlternateCategories) { %>
        <h3>Alternate Categories</h3>
	    <div class="h3hr"></div>
        <% Html.RenderAction(MVC.Child.EditAlternateCategory(Model.Member.Id.Value)); %>
    <% } %>

    <h3>My Articles</h3>
	<div class="h3hr"></div>
    
    <% if (!Model.NoArticles) { %>
        <!-- jqgrid for article list -->
        <table id="articleGrid">
        </table>
        <div id="articleGridPager">
        </div>
    <% } else { %>
        <h3 class="articleMessage">You have no articles yet. <%: Html.ActionLink("Click here to compose one", MVC.Article.ArticleEdit(null, null))%></h3>
    <% } %>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="Scripts" runat="server">
    <script src="<%= Links.Content.Scripts.thirdparty.charCount_js %>" type="text/javascript"></script>
    <script src="<%= Links.Content.Scripts.account_js %>" type="text/javascript"></script>
    <% if (!Model.NoArticles) { %>
        <script type="text/javascript">
            $(function () {
                var lastSel;
                $("#articleGrid").jqGrid({
                    url: '<%: Url.Action( MVC.Member._GetJqGridArticleList().AddRouteValue("memberId", Model.Member.Id) ) %>',
                    datatype: 'json',
                    mtype: 'GET',
                    colNames: ['Title', 'Edited', 'Votes', 'Comments', 'Times Read', 'Flagged',
                               'Published', 'Edit'],
                    colModel: [
                        { name: 'Title', index: 'Title', width: 128, editable: false },
                        { name: 'EditDate', index: 'EditDate', width: 90, search: false, editable: false },
                        { name: 'Votes', index: 'Votes', width: 90, search: false, editable: false },
                        { name: 'Comments', index: 'Comments', width: 90, search: false, editable: false },
                        { name: 'ReadCount', index: 'ReadCount', width: 90, search: false, editable: false },
                        { name: 'IsFlagged', index: 'IsFlagged', width: 90, stype: 'select', searchoptions: { value: ":;true:true;false:false" }, editable: false },
                        { name: 'IsPublished', index: 'IsPublished', width: 90, stype: 'select', searchoptions: { value: ":;true:true;false:false" }, editable: true, edittype: 'checkbox', editoptions: { value: "True:False"} },
                        { name: 'EditLink', index: 'EditLink', width: 90, search: false, editable: false }
                    ],
                    onSelectRow: function (id) {
                        if (id && id !== lastSel) {
                            jQuery('#articleGrid').jqGrid('restoreRow', lastSel);
                            lastSel = id;
                        }
                        jQuery('#articleGrid').jqGrid('editRow', id, true, null, JqGridSuccess, null, null, null, JqGridError);
                    },
                    editurl: '<%: Url.Action( MVC.Member._EditJqGridArticle() ) %>',
                    height: 512,
                    pager: '#articleGridPager',
                    rowNum: 16,
                    rowList: [16, 32, 64],
                    sortname: 'EditDate',
                    sortorder: 'desc',
                    viewrecords: true,
                    ignoreCase: true,
                    altRows: true
                })
                .jqGrid('filterToolbar', {})
                .jqGrid('navGrid', '#articleGridPager', { add: false,
                    search: false,
                    delfunc: function (rowid) {
                        jQuery("#articleGrid").jqGrid('delGridRow', rowid, { afterSubmit: JqGridAfterSubmit });
                    } 
                })
            }); 
        </script>
    <% } %>
</asp:Content>

