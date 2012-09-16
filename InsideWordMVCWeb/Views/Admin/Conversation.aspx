<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<InsideWordMVCWeb.ViewModels.Admin.ConversationManagementVM>" %>

<asp:Content ID="Content2" ContentPlaceHolderID="Head" runat="server">
    <%= Html.Title("Conversation Management") %>
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="Body" runat="server">
    <h1>Conversation Management</h1>
    <div class="h1hr"></div>

    <table id="list"></table>
    <div id="pager"></div>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="Scripts" runat="server">
    <script type="text/javascript">
        $(function () {
            var lastSel;
            $("#list").jqGrid({
                url: '<%: Url.Action( MVC.Admin._GetJqGridCommentList() ) %>',
                datatype: 'json',
                mtype: 'GET',
                colNames: ['Id', 'Create Date', 'Edit Date', 'Author', 'Author Id',
                           'Conver. Id', 'Flag count', 'Ignore Flags', 'Hidden', 'Text'],
                colModel: [
                    { name: 'Id', index: 'Id', width: 32, editable: false },
                    { name: 'CreateDate', index: 'CreateDate', width: 90, search: false, editable: false },
                    { name: 'EditDate', index: 'EditDate', width: 90, search: false, editable: false },
                    { name: 'AuthorName', index: 'AuthorName', width: 95, search: false, editable: false },
                    { name: 'MemberId', index: 'MemberId', width: 90, search: false, editable: false },
                    { name: 'ConversationId', index: 'ConversationId', width: 90, search: false, editable: false },
                    { name: 'CountFlags', index: 'CountFlags', width: 80, editable: false, searchoptions: { defaultValue: "0"} },
                    { name: 'IgnoreFlags', index: 'IgnoreFlags', width: 90, stype: 'select', searchoptions: { value: ":;true:true;false:false" }, editable: true, edittype: 'checkbox', editoptions: { value: "True:False"} },
                    { name: 'IsHidden', index: 'IsHidden', width: 64, stype: 'select', searchoptions: { value: ":;true:true;false:false" }, editable: true, edittype: 'checkbox', editoptions: { value: "True:False"} },
                    { name: 'Text', index: 'Text', width: 128, search: false, editable: false }
                ],
                onSelectRow: function (id) {
                    if (id && id !== lastSel) {
                        jQuery('#list').jqGrid('restoreRow', lastSel);
                        lastSel = id;
                    }
                    jQuery('#list').jqGrid('editRow', id, true, null, JqGridSuccess, null, null, null, JqGridError);
                },
                editurl: '<%: Url.Action( MVC.Admin._EditJqGridComment() ) %>',
                height: 512,
                pager: '#pager',
                rowNum: 64,
                rowList: [64, 128, 256],
                sortname: 'CreateDate',
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
