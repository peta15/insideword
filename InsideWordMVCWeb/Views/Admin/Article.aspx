<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<InsideWordMVCWeb.ViewModels.Admin.ArticleManagementVM>" %>

<asp:Content ID="Content2" ContentPlaceHolderID="Head" runat="server">
    <%= Html.Title("Article Management") %>
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="Body" runat="server">
    <h1>Article Management</h1>
    <div class="h1hr"></div>

    <table id="list"></table>
    <div id="pager"></div>
    <br />
    <br />
    <% Html.RenderAction(MVC.Admin.RefreshArticle()); %>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="Scripts" runat="server">
    <script type="text/javascript">
        $(function () {
            var lastSel;
            $("#list").jqGrid({
                url: '<%: Url.Action( MVC.Admin._GetJqGridArticleList() ) %>',
                datatype: 'json',
                mtype: 'GET',
                colNames: ['Id', 'Title', 'Create Date', 'Edit Date', 'Author', 'Author Id',
                           'Flag count', 'Ignore Flags', 'Hidden', 'Published'],
                colModel: [
                    { name: 'Id', index: 'Id', width: 32, editable: false },
                    { name: 'Title', index: 'Title', width: 128, editable: false },
                    { name: 'CreateDate', index: 'CreateDate', width: 90, search: false, editable: false },
                    { name: 'EditDate', index: 'EditDate', width: 90, search: false, editable: false },
                    { name: 'AuthorName', index: 'AuthorName', width: 70, search: false, editable: false },
                    { name: 'MemberId', index: 'MemberId', width: 70, search: false, editable: false },
                    { name: 'CountFlags', index: 'CountFlags', width: 90, editable: false, searchoptions: { defaultValue: "0"} },
                    { name: 'IgnoreFlags', index: 'IgnoreFlags', width: 90, stype: 'select', searchoptions: { value: ":;true:true;false:false" }, editable: true, edittype: 'checkbox', editoptions: { value: "True:False"} },
                    { name: 'IsHidden', index: 'IsHidden', width: 90, stype: 'select', searchoptions: { value: ":;true:true;false:false" }, editable: true, edittype: 'checkbox', editoptions: { value: "True:False"} },
                    { name: 'IsPublished', index: 'IsPublished', width: 90, stype: 'select', searchoptions: { value: ":;true:true;false:false" }, editable: true, edittype: 'checkbox', editoptions: { value: "True:False"} }
                ],
                onSelectRow: function (id) {
                    if (id && id !== lastSel) {
                        jQuery('#list').jqGrid('restoreRow', lastSel);
                        lastSel = id;
                    }
                    jQuery('#list').jqGrid('editRow', id, true, null, JqGridSuccess, null, null, null, JqGridError);
                },
                editurl: '<%: Url.Action( MVC.Admin._EditJqGridArticle() ) %>',
                height: 512,
                pager: '#pager',
                rowNum: 64,
                rowList: [64, 128, 256],
                sortname: 'EditDate',
                sortorder: 'desc',
                viewrecords: true,
                ignoreCase: true,
                altRows: true
            })
            .jqGrid('filterToolbar', {})
            .jqGrid('navGrid', '#pager', { add: false,
                                           search: false,
                                           delfunc: function (rowid) {
                                               jQuery("#list").jqGrid('delGridRow', rowid, { afterSubmit: JqGridAfterSubmit });
                                           } } )
        });
    </script>
</asp:Content>
