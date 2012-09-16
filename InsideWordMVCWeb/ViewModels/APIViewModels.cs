using System.Linq;
using System.Web.Mvc;
using InsideWordProvider;
using System;
using System.Collections.Generic;
using InsideWordMVCWeb.Models.WebProvider;
using System.Text;

namespace InsideWordMVCWeb.ViewModels.API
{
    /// <summary>
    /// Model used for gathering the values of an article
    /// </summary>
    public class APIArticleVM
    {
        protected string _altId;
        protected string _title;
        protected string _blurb;
        protected string _text;
        protected DateTime? _editDate;
        protected DateTime? _createDate;

        public APIArticleVM()
        {

        }

        public long? Id { get; set; }
        public string AltId
        {
            get { return _altId; }
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    _altId = Uri.UnescapeDataString(value);
                }
            }
        }
        public string Title
        {
            get { return _title; }
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    _title = Uri.UnescapeDataString(value);
                }
            }
        }
        public string Blurb
        {
            get { return _blurb; }
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    _blurb = Uri.UnescapeDataString(value);
                }
            }
        }
        public string Text
        {
            get { return _text; }
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    _text = Uri.UnescapeDataString(value);
                }
            }
        }

        public long? CategoryId { get; set; }
        public long? AlternateCategoryId { get; set; }
        public bool? IsPublished { get; set; }

        public string EditDate
        {
            get { return (_editDate.HasValue)? _editDate.Value.ToString(): null ; }
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    DateTime parsedDateTime = new DateTime();
                    if (DateTime.TryParse(Uri.UnescapeDataString(value), out parsedDateTime))
                    {
                        _editDate = parsedDateTime.ToUniversalTime();
                    }
                }
            }
        }

        public DateTime? getEditDate() { return _editDate; }

        public string CreateDate
        {
            get { return (_createDate.HasValue) ? _createDate.Value.ToString() : null; }
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    DateTime parsedDateTime = new DateTime();
                    if (DateTime.TryParse(Uri.UnescapeDataString(value), out parsedDateTime))
                    {
                        _createDate = parsedDateTime.ToUniversalTime();
                    }
                }
            }
        }

        public DateTime? getCreateDate() { return _createDate; }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder("");
            sb.Append("APIArticleVM\n");
            sb.Append("\tId - " + Id + "\n");
            sb.Append("\tAltId - " + AltId + "\n");
            sb.Append("\tTitle - " + Title + "\n");
            sb.Append("\tBlurb - " + Blurb + "\n");
            sb.Append("\tText - " + Text + "\n");
            sb.Append("\tAlternateCategoryId - "+AlternateCategoryId+"\n");
            sb.Append("\tCategoryId - "+CategoryId+"\n");
            sb.Append("\tIsPublished - "+IsPublished+"\n");
            sb.Append("\tEditDate - "+EditDate+"\n");
            sb.Append("\tCreateDate - "+CreateDate+"\n");
            return sb.ToString();
        }
    }

    public class AlternateCategoryMapVM
    {
        protected string _title;
        public string AlternateTitle
        {
            get { return _title; }
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    _title = Uri.UnescapeDataString(value);
                }
            }
        }

        public long AlternateId { get; set; }
        public long? MapId { get; set; }
    }

    public class MemberDataVM
    {
        protected string _email;
        public string Email
        {
            get { return _email; }
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    _email = Uri.UnescapeDataString(value);
                }
            }
        }

        public List<AlternateCategoryMapVM> MemberToIWMap { get; set; }
    }

    /// <summary>
    /// A category that only represents the most basic attributes of a category with no children.
    /// </summary>
    public class SimpleCategoryVM
    {
        public SimpleCategoryVM() { }

        public SimpleCategoryVM(ProviderCategory aCategory)
        {
            Id = aCategory.Id;
            Title = aCategory.Title;
        }

        public long? Id { get; set; }
        public string Title { get; set; }
    }

    /// <summary>
    /// Model view used to represent an entire category tree structure.
    /// </summary>
    public class CategoryVM : SimpleCategoryVM
    {
        public CategoryVM() { }

        public CategoryVM(ProviderCategory aCategory)
        {
            Id = aCategory.Id;
            Title = aCategory.Title;
            Children = new List<CategoryVM>();
            foreach (ProviderCategory childCategory in aCategory.Children(true))
            {
                Children.Add(new CategoryVM(childCategory));
            }
        }

        public List<CategoryVM> Children { get; set; }
    }

    public abstract class ApiMsgAbstract
    {
        int StatusCode { get; set; }
        int StatusMessage { get; set; }

        public enum StatusEnum : int
        {
            success = 0,
            failure = 1
        }
    }

    public class ApiMsgVM : ApiMsgAbstract
    {
        public int StatusCode { get; set; }
        public string StatusMessage { get; set; }
        public string Content { get; set; }

        public ApiMsgVM()
        {
            StatusCode = (int)StatusEnum.success;
            StatusMessage = string.Empty;
            Content = string.Empty;
        }

        public ApiMsgVM(int status)
        {
            StatusCode = status;
            StatusMessage = string.Empty;
            Content = string.Empty;
        }

        public override string ToString()
        {
            return StatusCode.ToString() + " / " + StatusMessage + " / " + Content;
        }
    }

    public class MemberServerDataVM : ApiMsgAbstract
    {
        public int StatusCode { get; set; }
        public string StatusMessage { get; set; }
        public string ProfileLink { get; set; }
        public string AccountLink { get; set; }

        public List<AlternateCategoryMapVM> Map { get; set; }
        public CategoryVM CategoryTree { get; set; }

        public MemberServerDataVM()
        {

        }

        public MemberServerDataVM(ProviderCurrentMember currentMember, List<AlternateCategoryMapVM> map, ProviderCategory treeRoot)
        {
            StatusCode = (int)StatusEnum.success;
            Map = map;
            CategoryTree = new CategoryVM(treeRoot);
        }
    }
}