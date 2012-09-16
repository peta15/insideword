using System.Linq;
using System.Web.Mvc;
using InsideWordProvider;
using InsideWordMVCWeb.Models.WebProvider;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Collections.Generic;
using InsideWordMVCWeb.Models.Utility;
using log4net;

namespace InsideWordMVCWeb.ViewModels.Admin
{
    /// <summary>
    /// Model used for setting up the Admin index management page.
    /// </summary>
    public class AdminIndexVM
    {
        public AdminIndexVM()
        {

        }

        public AdminIndexVM(ProviderCurrentMember currentMember)
        {
            CanAccessArticleManagement = currentMember.CanAccess(ProviderCurrentMember.AccessGroup.ArticleManagement);
            CanAccessCategoryManagement = currentMember.CanAccess(ProviderCurrentMember.AccessGroup.CategoryManagement);
            CanAccessConversationManagement = currentMember.CanAccess(ProviderCurrentMember.AccessGroup.ConversationManagement);
            CanAccessMemberManagement = currentMember.CanAccess(ProviderCurrentMember.AccessGroup.MemberManagement);
            CanAccessSettingsManagement = currentMember.CanAccess(ProviderCurrentMember.AccessGroup.Administration);
        }

        public bool CanAccessMemberManagement       { get; set; }
        public bool CanAccessCategoryManagement     { get; set; }
        public bool CanAccessConversationManagement { get; set; }
        public bool CanAccessArticleManagement      { get; set; }
        public bool CanAccessSettingsManagement     { get; set; }
    }

    /// <summary>
    /// Model used for setting up the Admin index management page.
    /// </summary>
    public class SettingsManagementVM
    {
        static protected KeyValuePair<long, string> _nullCategory;
        static SettingsManagementVM()
        {
            _nullCategory = new KeyValuePair<long, string>(-1, "Select one...");
        }

        [ScaffoldColumn(false)]
        public SelectList CategoryList { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "You must select a category")]
        [DisplayName("Default Category")]
        public long DefaultCategoryId { get; set; }

        [ScaffoldColumn(false)]
        public SelectList LogLevelList { get; set; }

        [DisplayName("Log Threshold")]
        public string LogThresholdName { get; set; }

        [ScaffoldColumn(false)]
        public SelectList LogEmailLevelList { get; set; }

        [DisplayName("Log Email Threshold")]
        public string LogEmailThresholdName { get; set; }

        public SettingsManagementVM()
        {

        }

        public SettingsManagementVM(InsideWordSettingsDictionary iwSettings, List<ProviderCategory> categoryList, InsideWordWebLog webLog)
        {
            long? defaultCategory = iwSettings.DefaultCategoryId;
            if (defaultCategory.HasValue)
            {
                DefaultCategoryId = defaultCategory.Value;
            }
            else
            {
                DefaultCategoryId = -1;
            }
            Refresh(categoryList, webLog);
        }

        public bool Refresh(List<ProviderCategory> categoryList, InsideWordWebLog webLog)
        {
            List<KeyValuePair<long, string>> ddList = new List<KeyValuePair<long, string>>();

            if (DefaultCategoryId == -1)
            {
                ddList.Add(_nullCategory);
            }
            ddList.AddRange(categoryList.ToDictionary(aCategory => aCategory.Id.Value, aCategory => aCategory.Title));

            CategoryList = new SelectList(ddList, "key", "value", DefaultCategoryId);

            LogThresholdName = webLog.IWThreshold.Name;
            LogLevelList = new SelectList(webLog.LevelList, "key", "value", LogThresholdName);

            LogEmailThresholdName = webLog.IWSmtpThreshold.Name;
            LogEmailLevelList = new SelectList(webLog.LevelList, "key", "value", LogEmailThresholdName);

            return true;
        }
    }

    /// <summary>
    /// Model used for setting up the Member management page.
    /// </summary>
    public class MemberManagementVM
    {

    }

    /// <summary>
    /// Model using JqGrid format to edit a single member.
    /// </summary>
    public class EditMemberManagementVM
    {
        public EditMemberManagementVM() { }
        public string Oper { get; set; }
        public int Id { get; set; }
        public bool IsBanned { get; set; }
        public bool IsSuperAdmin { get; set; }
        public bool IsMasterAdmin { get; set; }
        public bool IsMemberAdmin { get; set; }
        public bool IsCategoryAdmin { get; set; }
        public bool IsArticleAdmin { get; set; }
    }

    /// <summary>
    /// Model used for paging and filtering of ProviderMember data.
    /// Filter is JqGrid compliant.
    /// </summary>
    public class MemberManagementFilterVM : IProviderMemberFilter
    {
        public MemberManagementFilterVM() { }

        public int Page { get; set; }
        public int Rows { get; set; }
        public string Sidx { get; set; }
        public string Sord { get; set; }

        public int? Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public bool? IsValidEmail { get; set; }
        public bool? HasPassword { get; set; }
        public bool? HasOpenId { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsBanned { get; set; }
        public bool? IsSuperAdmin { get; set; }
        public bool? IsMasterAdmin { get; set; }
        public bool? IsMemberAdmin { get; set; }
        public bool? IsCategoryAdmin { get; set; }
        public bool? IsArticleAdmin { get; set; }
    }

    /// <summary>
    /// Model used for setting up the Category management page.
    /// </summary>
    public class CategoryManagementVM
    {
        
    }

    /// <summary>
    /// Model used to edit a single category.
    /// </summary>
    public class CategoryEditVM
    {
        public CategoryEditVM()
        {
            Id = -1;
            Title = "";
            ParentId = -1;
            IsHidden = false;
            PotentialParentList = new SelectList(ProviderCategory.LoadAll()
                                                                  .ToList(),
                                                 "Id",
                                                 "Title",
                                                 ProviderCategory.Root);
        }

        public CategoryEditVM(ProviderCategory aCategory)
        {
            Id = (aCategory.Id.HasValue)?aCategory.Id.Value:-1;
            Title = aCategory.Title;
            ParentId = (aCategory.ParentId.HasValue)? aCategory.ParentId.Value : -1;
            IsHidden = aCategory.IsHidden;

            PotentialParentList = new SelectList( ProviderCategory.LoadAll()
                                                                  .Where(category => category.Id != Id)
                                                                  .ToList(),
                                                  "Id",
                                                  "Title",
                                                  new ProviderCategory(Id));
        }

        public long Id       { get; set; }
        public string Title  { get; set; }
        public long ParentId { get; set; }
        public bool IsHidden { get; set; }

        public SelectList PotentialParentList { get; set; }
    }

    /// <summary>
    /// Model used for setting up the Conversation management page.
    /// </summary>
    public class ConversationManagementVM
    {

    }

    /// <summary>
    /// Model using JqGrid format to edit a single comment.
    /// </summary>
    public class EditCommentManagementVM
    {
        public EditCommentManagementVM() { }
        public string Oper { get; set; }
        public int Id { get; set; }
        public bool IsHidden { get; set; }
        public bool IgnoreFlags { get; set; }
    }

    /// <summary>
    /// Model used for paging and filtering of ProviderComment data.
    /// Filter is JqGrid compliant.
    /// </summary>
    public class CommentManagementFilterVM : IProviderCommentFilter
    {
        public CommentManagementFilterVM() { }

        public int Page    { get; set; }
        public int Rows    { get; set; }
        public string Sidx { get; set; }
        public string Sord { get; set; }

        public int? Id           { get; set; }
        public int CountFlags    { get; set; }
        public bool? IsHidden    { get; set; }
        public bool? IgnoreFlags { get; set; }
    }

    /// <summary>
    /// Model used for setting up the Article management page.
    /// </summary>
    public class ArticleManagementVM
    {

    }

    /// <summary>
    /// Model used for setting up the Refresh article management partial.
    /// </summary>
    public class RefreshArticleVM
    {

    }
}