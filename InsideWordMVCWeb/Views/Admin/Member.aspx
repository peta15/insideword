<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<InsideWordMVCWeb.ViewModels.Admin.MemberManagementVM>" %>

<asp:Content ID="Content2" ContentPlaceHolderID="Head" runat="server">
    <%= Html.Title("Member Management") %>
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="Body" runat="server">
    <h1>Member Management</h1>
    <div class="h1hr"></div>

    <table id="list"></table>
    <div id="pager"></div>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="Scripts" runat="server">
    <script type="text/javascript">
        $(function () {
            var lastSel;
            $("#list").jqGrid({
                url: '<%: Url.Action(MVC.Admin._GetJqGridMemberList()) %>',
                datatype: 'json',
                mtype: 'GET',
                colNames: ['Id', 'User name', 'E-mail', 'Create Date', 'Edit Date', 'Password', 'OpenId', 'Active', 'V. E-mail', 'Banned', 'Super A.',
                           'Master A.', 'Member A.', 'Category A.', 'Article A.'],
                colModel: [
                    { name: 'Id', index: 'Id', width: 32, editable: false },
                    { name: 'UserName', index: 'UserName', width: 128, editable: false },
                    { name: 'Email', index: 'Email', width: 128, editable: false },
                    { name: 'CreateDate', index: 'CreateDate', width: 70, search: false, editable: false },
                    { name: 'EditDate', index: 'EditDate', width: 70, search: false, editable: false },
                    { name: 'HasPassword', index: 'HasPassword', width: 64, stype: 'select', searchoptions: { value: ":;true:true;false:false" }, editable: false },
                    { name: 'HasOpenId', index: 'HasOpenId', width: 64, stype: 'select', searchoptions: { value: ":;true:true;false:false" }, editable: false },
                    { name: 'IsActive', index: 'IsActive', width: 64, stype: 'select', searchoptions: { value: ":;true:true;false:false" }, editable: false },
                    { name: 'IsValidEmail', index: 'IsValidEmail', width: 64, stype: 'select', searchoptions: { value: ":;true:true;false:false" }, editable: false },
                    { name: 'IsBanned', index: 'IsBanned', width: 64, stype: 'select', searchoptions: { value: ":;true:true;false:false" }, editable: true, edittype: 'checkbox', editoptions: { value: "True:False"} },
                    { name: 'IsSuperAdmin', index: 'IsSuperAdmin', width: 64, stype: 'select', searchoptions: { value: ":;true:true;false:false" }, editable: true, edittype: 'checkbox', editoptions: { value: "True:False"} },
                    { name: 'IsMasterAdmin', index: 'IsMasterAdmin', width: 64, stype: 'select', searchoptions: { value: ":;true:true;false:false" }, editable: true, edittype: 'checkbox', editoptions: { value: "True:False"} },
                    { name: 'IsMemberAdmin', index: 'IsMemberAdmin', width: 64, stype: 'select', searchoptions: { value: ":;true:true;false:false" }, editable: true, edittype: 'checkbox', editoptions: { value: "True:False"} },
                    { name: 'IsCategoryAdmin', index: 'IsCategoryAdmin', width: 64, stype: 'select', searchoptions: { value: ":;true:true;false:false" }, editable: true, edittype: 'checkbox', editoptions: { value: "True:False"} },
                    { name: 'IsArticleAdmin', index: 'IsArticleAdmin', width: 64, stype: 'select', searchoptions: { value: ":;true:true;false:false" }, editable: true, edittype: 'checkbox', editoptions: { value: "True:False"} }
                ],
                onSelectRow: function (id) {
                    if (id && id !== lastSel) {
                        jQuery('#list').jqGrid('restoreRow', lastSel);
                        lastSel = id;
                    }
                    jQuery('#list').jqGrid('editRow', id, true, null, JqGridSuccess, null, null, null, JqGridError);
                },
                editurl: '<%: Url.Action(MVC.Admin._EditJqGridMember()) %>',
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
                                               jQuery("#list").jqGrid('delGridRow', rowid, { afterSubmit: JqGridAfterSubmit});
                                           } } )
        }); 
    </script>
</asp:Content>