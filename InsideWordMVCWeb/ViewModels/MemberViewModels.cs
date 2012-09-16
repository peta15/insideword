using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using InsideWordMVCWeb.Models.Annotation;
using InsideWordMVCWeb.ViewModels.ProviderViewModels;
using InsideWordProvider;
using Foolproof;
using System;
using InsideWordMVCWeb.ViewModels.Shared;
using System.Linq;
using System.Web.Mvc;
using InsideWordMVCWeb.ViewModels.Admin;
using InsideWordMVCWeb.Models.WebProvider;
using InsideWordMVCWeb.Models.BusinessLogic;
using InsideWordResource;

namespace InsideWordMVCWeb.ViewModels.Member
{
    public class ResetPasswordRequestVM
    {
        [Required]
        [StringLength(ProviderMember.EmailSize)]
        [DisplayName("E-mail")]
        public string Email { get; set; }
    }

    public class ResetPasswordVM
    {
        [Required]
        [RegularExpression(@"\w{8,16}", ErrorMessage = "Passwords must be 8 to 16 numbers and letters")]
        [DataType(DataType.Password)]
        [DisplayName("Password")]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [DisplayName("Confirm Password")]
        [Compare("Password", ErrorMessage = "Passwords do not match")]
        public string ConfirmPassword { get; set; }

        public string Key { get; set; }
        public long MemberId { get; set; }
    }

    public class ChangePasswordVM
    {
        [Required]
        [RegularExpression(@"\w{8,16}", ErrorMessage = "Passwords must be 8 to 16 numbers and letters")]
        [DataType(DataType.Password)]
        [DisplayName("Password")]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [DisplayName("Confirm Password")]
        [Compare("Password", ErrorMessage = "Passwords do not match")]
        public string ConfirmPassword { get; set; }

        public long CurrentMemberId { get; set; }
    }

    public class ProfileVM
    {
        public string PageTitle { get; set; }
        public string HeaderTitle { get; set; }
        public string Birthday { get; set; }
        public bool NoArticles { get; set; }
        public List<ArticleVM> ArticleList { get; set; }
        public CurrentMemberVM CurrentMember { get; set; }
        public long? MemberId { get; set; }
        public ImageInfo ProfileImage { get; set; }
        public string UserName { get; set; }
        public string Bio { get; set; }
        public bool HasBio { get; set; }
        public string WebUri { get; set; }
        public string WebDisplay { get; set; }
        public bool HasWeb { get; set; }
        public bool IsLastPage { get; set; }
        public int NextPage { get; set; }

        protected const int MAX_BLURBS = 1000;
        protected const int BLURBS_PER_PAGE = 8;

        public ProfileVM() { }
        public ProfileVM(ProviderMember aMember, ProviderCurrentMember currentMember, int page)
        {
            string userName = null;
            CurrentMember = new CurrentMemberVM(currentMember, aMember);

            if (CurrentMember.CanEdit)
            {
                userName = aMember.DisplayAdministrativeName;
            }
            else
            {
                userName = aMember.DisplayName;
            }

            int skip = page * BLURBS_PER_PAGE;
            if(skip > MAX_BLURBS)
            {
                ArticleList = new List<ArticleVM>();
            }
            else
            {
                bool? showPublished = true;
                if (CurrentMember.CanEdit)
                {
                    showPublished = null;
                }
                bool? showHidden = false;
                if (CurrentMember.CanEdit)
                {
                    showHidden = null;
                }
                ArticleList = ProviderArticle.LoadNewestBy(aMember, skip, BLURBS_PER_PAGE, showHidden, showPublished)
                                               .ConvertAll<ArticleVM>(anArticle => new ArticleVM(anArticle, currentMember));
            }
            IsLastPage = ArticleList.Count < BLURBS_PER_PAGE;
            NextPage = page + 1;

            if (aMember.HasValidAltId(ProviderAlternateMemberId.AlternateType.Domain))
            {
                ProviderDomain aDomain = aMember.Domains[0];
                HasWeb = true;
                WebUri = aDomain.Domain.AbsoluteUri;
                WebDisplay = IWStringUtility.TruncateClean(aDomain.DisplayName, ProviderMember.UserNameSize);
            }

            PageTitle = userName + " Profile";
            HeaderTitle = IWStringUtility.TruncateClean(userName, 28);
            Birthday = String.Format("{0:MMMM d, yyyy}", aMember.CreateDate);
            MemberId = aMember.Id;
            UserName = userName;
            ProfileImage = MemberBL.GetProfileImage(aMember);
            HasBio = !string.IsNullOrWhiteSpace(aMember.Bio);
            Bio = aMember.Bio;
        }
    }

    public class AccountVM
    {
        // TODO add validation and display names
        [Required]
        [StringLength(ProviderMember.EmailSize)]
        [Email(ErrorMessage = "Invalid Email Address")]
        [EmailUnique(ErrorMessage = "E-mail is already taken.")]
        [DisplayName("Email Address")]
        public List<string> EmailAddresses { get; set; }

        [StringLength(ProviderMember.EmailSize)]
        [Email(ErrorMessage = "Invalid Email Address")]
        [EmailUnique(ErrorMessage = "E-mail is already taken.")]
        [DisplayName("Add Email")]
        public string NewEmail { get; set; }

        public MemberVM Member { get; set; }
        public string PageTitle { get; set; }
        public List<ProviderOpenId> OpenIds { get; set; }
        public List<ArticleVM> ArticleList { get; set; }
        public ImageInfo ProfileImage { get; set; }
        public bool NoArticles { get; set; }
        public bool DisplayAlternateCategories { get; set; }

        public AccountVM() { }

        public AccountVM(ProviderMember aMember, ProviderCurrentMember currentMember)
        {
            Member = new MemberVM(aMember, currentMember);
            PageTitle = aMember.DisplayAdministrativeName + " Account";
            EmailAddresses = aMember.Emails.Select(email => email.Email.Address).ToList();
            OpenIds = aMember.OpenIds;

            ProfileImage = MemberBL.GetProfileImage(aMember);

            if (currentMember.CanEdit(aMember) && currentMember.HasAdminRights)
            {
                ArticleList = ProviderArticle.LoadBy(aMember.Id.Value, null, null).ConvertAll<ArticleVM>(anArticle => new ArticleVM(anArticle, currentMember));
            }
            else
            {
                ArticleList = ProviderArticle.LoadBy(aMember.Id.Value, null, false).ConvertAll<ArticleVM>(anArticle => new ArticleVM(anArticle, currentMember));
            }
            NoArticles = ArticleList.Count == 0;
            DisplayAlternateCategories = aMember.HasAlternateCategories && currentMember.HasAdminRights;
        }
    }

    /// <summary>
    /// Model using JqGrid format to edit a single article.
    /// </summary>
    public class AccountEditArticleVM
    {
        protected JqGridEditArticleVM _jqGridEdit;
        public AccountEditArticleVM()
        {
            _jqGridEdit = new JqGridEditArticleVM();
        }

        public JqGridEditArticleVM GetJqGridEdit { get { return _jqGridEdit; } }

        public string Oper
        {
            get { return _jqGridEdit.Oper; }
            set { _jqGridEdit.Oper = value ;}
        }

        public int Id
        {
            get { return _jqGridEdit.Id; }
            set { _jqGridEdit.Id = value; }
        }

        public bool IsPublished
        {
            get { return _jqGridEdit.IsPublished; }
            set { _jqGridEdit.IsPublished = value; }
        }
    }

    /// <summary>
    /// Model used for paging and filtering of ProviderArticle data.
    /// Filter is JqGrid compliant.
    /// </summary>
    public class AccountArticleFilterVM
    {
        protected ArticleFilterVM _articleFilter;
        public AccountArticleFilterVM() {
            _articleFilter = new ArticleFilterVM();
            _articleFilter.IsHidden = false;
        }

        public ArticleFilterVM GetFilter
        {
            get { return _articleFilter; }
        }

        public int Page
        {
            get { return _articleFilter.Page; }
            set { _articleFilter.Page = value; }
        }
        public int Rows
        {
            get { return _articleFilter.Rows; }
            set { _articleFilter.Rows = value; }
        }
        public string Sidx
        {
            get { return _articleFilter.Sidx; }
            set { _articleFilter.Sidx = value; }
        }
        public string Sord
        {
            get { return _articleFilter.Sord; }
            set { _articleFilter.Sord = value; }
        }
        public long? MemberId
        {
            get { return _articleFilter.MemberId; }
            set { _articleFilter.MemberId = value; }
        }
        public string Title
        {
            get { return _articleFilter.Title; }
            set { _articleFilter.Title = value; }
        }
        public bool? IsFlagged
        {
            get { return _articleFilter.CountFlags > 0; }
            set { _articleFilter.CountFlags = (value ?? 0 > 0 ? 1 : 0); }
        }
        public bool? IsPublished
        {
            get { return _articleFilter.IsPublished; }
            set { _articleFilter.IsPublished = value; }
        }
    }
}