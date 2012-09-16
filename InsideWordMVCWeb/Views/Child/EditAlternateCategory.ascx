<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<InsideWordMVCWeb.ViewModels.Child.AlternateCategoryListVM>" %>
<div class="left InsideWord_partial">
    <% using (Html.BeginForm(MVC.Child.EditAlternateCategory(Model.MemberId))) {%>
        <%= Html.ValidationSummary(true, "Category map was incorrect, try again.")%>

        <div>
            <div class="left">
                <% for(int index = 0; index < Model.AlternateCategoryMapList.Count; index++) { %>
                    <div class="editor-label">
                        <%: Html.LabelFor(model => model.AlternateCategoryMapList[index].AlternateTitle, 
                                                   Model.AlternateCategoryMapList[index].AlternateTitle ) %>
                    </div>
                    <div class="editor-field">
                        <%: Html.DropDownListFor(model => model.AlternateCategoryMapList[index].MapId, Model.CategoryList[index]) %>
                        <%: Html.ValidationMessageFor(model => model.AlternateCategoryMapList[index].MapId)%>
                        <%: Html.HiddenFor( model => model.AlternateCategoryMapList[index].AlternateId ) %>
                        <%: Html.HiddenFor(model => model.AlternateCategoryMapList[index].AlternateTitle)%>
                    </div>
                <% } %>

                    <div class="editor-label">
                        <%: Html.LabelFor(model => model.OverrideFlag) %>
                    </div>
                    <div class="editor-field">
                        <%: Html.CheckBoxFor(model => model.OverrideFlag)%>
                        <%: Html.ValidationMessageFor(model => model.OverrideFlag)%>
                    </div>
            </div>
        </div>
   
        <div class="editor-submit">
            <input type="submit" value="Save" />
            <a href="<%: Url.Action(MVC.Member.Account(Model.MemberId, null)) %>" class="button">Cancel</a>
        </div>
    <% } %>
</div>
