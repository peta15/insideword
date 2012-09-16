using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using InsideWordMVCWeb.ViewModels;
using InsideWordProvider;
using InsideWordResource;

namespace InsideWordMVCWeb.ViewModels.Shared
{
    public class MessageVM
    {
        public ImageInfo Image { get; set; }
        public string Title { get; set; } // mandatory
        public string Message { get; set; } // mandatory
        public string CssClassContainer { get; set; }
        public List<string> Details { get; set; } // a bullet point list shown below the message if there are any to show
        public string LinkHref { get; set; }
        public string LinkText { get; set; }
    }

    public class AjaxReturnMsgVM
    {
        public enum Status
        {
            success = 0,
            failure = 1,
            successAndStop = 2
        };

        public Status StatusCode { get; set; } // success, failure, other
        public string StatusMessage { get; set; } // error messages or other status descriptions
        public string Content { get; set; } // any html to return (populate comment for instance)
        public override string ToString()
        {
            return StatusCode.ToString() + " / "+StatusMessage+" / "+Content;
        }
    }

    public class PartialPostVM
    {
        public enum ActionType { redirect = 0, refresh };

        public ActionType Action { get; set; }
        public string Message { get; set; } // error messages or other status descriptions
        public string Content { get; set; } // any html to return (populate comment for instance)
    }

    // **************************************
    // Types used within view models (TODO maybe make these nested?)
    // **************************************

    /// <summary>
    /// View model to represent a number bit pair. Ideal for processing a list of check boxes.
    /// </summary>
    public class NumBitVM
    {
        public NumBitVM() { }
        public long Num { get; set; }
        public bool Bit { get; set; }
    }

    public class AddMediaVM
    {
        public string FilePhotoKey { get { return "filePhoto"; } }
        public AddMediaType Type { get; set; }
        public AddMediaPurpose Purpose { get; set; }
        public long? MemberId { get; set; }

        public enum AddMediaPurpose
        {
            TinyMce,
            ProfileImage,
            ProfileImageAjax
        }

        /// <summary>
        /// Tinymce type is a string value which is either 'image', 'media' or 'file' (called respectively from image plugin, media plugin and link plugin insert/edit dialogs). With this value you can determine whether your file browser is called from a window that inserts images ("insert/edit image" dialogue), multimedia files (media plugin window) or a window that inserts hyperlinks ("insert/edit links" dialogue)
        /// </summary>
        public enum AddMediaType
        {
            Image,
            Media,
            File
        }
    }

    public class AddMediaDoneVM
    {
        /// <summary>
        /// url of original image
        /// </summary>
        public string FileURL { get; set; }
    }

    /// <summary>
    /// Model using JqGrid format to edit a single article.
    /// </summary>
    public class JqGridEditArticleVM
    {
        public JqGridEditArticleVM() { }
        public string Oper { get; set; }
        public int Id { get; set; }
        public bool IsHidden { get; set; }
        public bool IsPublished { get; set; }
        public bool IgnoreFlags { get; set; }
    }

    /// <summary>
    /// Model used for paging and filtering of ProviderArticle data.
    /// Filter is JqGrid compliant.
    /// </summary>
    public class ArticleFilterVM : IProviderArticleFilter
    {
        public ArticleFilterVM() { }

        public int Page { get; set; }
        public int Rows { get; set; }
        public string Sidx { get; set; }
        public string Sord { get; set; }

        public int? Id { get; set; }
        public string Title { get; set; }
        public int CountFlags { get; set; }
        public bool? IsHidden { get; set; }
        public bool? IsPublished { get; set; }
        public bool? IgnoreFlags { get; set; }
        public long? MemberId { get; set; }
    }
}
