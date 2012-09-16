using System;
using System.Linq;
using System.Web.Mvc;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using InsideWordProvider;
using InsideWordMVCWeb.ViewModels.ProviderViewModels;
using InsideWordMVCWeb.Models.Annotation;
using InsideWordMVCWeb.Models.WebProvider;

namespace InsideWordMVCWeb.ViewModels.Article
{
    public class DetailsVM
    {
        public string PageTitle { get; set; }
        public Dictionary<long, string> RelatedHeadlinesByCategory { get; set; }
        public Dictionary<long, string> RelatedHeadlinesByAuthor { get; set; }
        public ArticleVM Article { get; set; }
        public List<ConversationVM> ConversationList { get; set; }

        public DetailsVM(ProviderArticle anArticle, ProviderCurrentMember currentMember)
        {
            Parse(anArticle, currentMember);
        }

        public bool Parse(ProviderArticle anArticle, ProviderCurrentMember currentMember)
        {
            ProviderMember author = anArticle.Author;
            string authorName = null;
            long? authorId = null;
            if (author != null)
            {
                authorId = author.Id;
                authorName = author.DisplayName;
            }
            else
            {
                authorName = null;
            }

            bool authorIsNull = string.IsNullOrEmpty(authorName);
            PageTitle = anArticle.Title + " by " + (authorIsNull ? "Anonymous" : authorName);
            RelatedHeadlinesByCategory = anArticle.RelatedHeadlinesByCategory(5);
            RelatedHeadlinesByAuthor = anArticle.RelatedHeadlinesByAuthor(3);
            ConversationList = anArticle.Conversations.ConvertAll<ConversationVM>(aConversation => new ConversationVM(aConversation, currentMember, authorId));
            Article = new ArticleVM(anArticle, currentMember);

            return true;
        }
    }

    public class ArticleEditVM
    {
        public long? ArticleId { get; set; }
        public string PageTitle { get; set; }

        public ArticleEditVM() { }

        public ArticleEditVM(ProviderArticle anArticle)
        {
            Parse(anArticle);
        }

        public bool Parse(ProviderArticle anArticle)
        {
            if (anArticle.IsNew)
            {
                PageTitle = "Publish Article";
            }
            else
            {
                ArticleId = anArticle.Id;
                PageTitle = "Edit Article: " + anArticle.Title;
            }
                
            return true;
        }
    }

    public class ArticleEditorVM
    {
        static protected KeyValuePair<long, string> _nullCategory;
        static ArticleEditorVM()
        {
            _nullCategory = new KeyValuePair<long, string>(-1, "Select one...");
        }

        public enum SaveStates
        {
            DraftAndContinue = 1,
            DraftAndPreview,
            Published,
        }

        [Required]
        [StringLength(ProviderArticle.TitleSize)]
        [DisplayName("Title")]
        public string Title { get; set; }

        [StringLength(ProviderArticle.BlurbSize)]
        [DisplayName("Blurb")]
        public string Blurb { get; set; }

        [Required]
        [StringLength(ProviderArticle.ArticleBodySize)]
        [DisplayName("Article Body")]
        public string ArticleBody { get; set; }

        [ScaffoldColumn(false)]
        public SelectList CategoryList { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "You must select a category")]
        [DisplayName("Category")]
        public long ArticleCategoryId { get; set; }

        [StringLength(ProviderMember.EmailSize)]
        [Email(ErrorMessage = "Invalid Email Address")]
        [DisplayName("E-mail")]
        public string ArticleEmail { get; set; }

        public long? ArticleId { get; set; }

        // value obtained from submit buttons and set in controller
        public string SaveButton
        {
            set
            {
                switch (value)
                {
                    case "Publish":
                        SaveState = SaveStates.Published;
                        break;
                    case "Preview":
                        SaveState = SaveStates.DraftAndPreview;
                        break;
                    default:
                        SaveState = SaveStates.DraftAndContinue;
                        break;
                }
            }

        }

        // not intended for use in the view, only in the controller and model
        // SaveButton is used in the view and passed directly to the controller
        public SaveStates SaveState { get; set; }

        public string PageTitle { get; set; }
        public bool ShowEmailInput { get; set; }
        public string CreateDate { get; set; }
        public string EditDate { get; set; }

        public bool Parse(ProviderCurrentMember currentMember, List<ProviderCategory> categoryList)
        {
            PageTitle = "Publish Article";
            ShowEmailInput = !currentMember.IsLoggedOn;

            List<KeyValuePair<long, string>> ddList = new List<KeyValuePair<long, string>>();
            ddList.Add(_nullCategory);
            ddList.AddRange( categoryList.ToDictionary(aCategory => aCategory.Id.Value, aCategory => aCategory.Title) );
            ArticleCategoryId = -1;
            CategoryList = new SelectList(ddList, "key", "value", ArticleCategoryId);
            SaveState = SaveStates.DraftAndContinue;

            return true;
        }

        public bool Parse(ProviderArticle anArticle, ProviderCurrentMember currentMember, List<ProviderCategory> categoryList)
        {
            ArticleBody = anArticle.RawText;
            if (!anArticle.BlurbIsAutoGenerated)
            {
                Blurb = anArticle.Blurb;
            }
            Title = anArticle.Title;

            Refresh(anArticle, currentMember, categoryList);

            return true;
        }

        /// <summary>
        /// Function used to refresh the view model with any static data that may have been lost.
        /// This function is specifically used to refresh the model for a failed POST.
        /// </summary>
        /// <param name="anArticle">Article to refresh the data from</param>
        /// <param name="currentMember">Current member to refresh the data from</param>
        /// <param name="categoryList">Category list to refresh the data from</param>
        /// <returns>True if the function refreshed the model and false otherwise.</returns>
        public bool Refresh(ProviderArticle anArticle, ProviderCurrentMember currentMember, List<ProviderCategory> categoryList)
        {
            ArticleId = anArticle.Id;
            if (anArticle.IsNew)
            {
                PageTitle = "Publish Article";
            }
            else
            {
                PageTitle = "Edit Article: "+anArticle.Title;
            }

            // till the article has been associated with someone, keep showing the e-mail input
            ShowEmailInput = !anArticle.MemberId.HasValue;

            List<KeyValuePair<long, string>> ddList = new List<KeyValuePair<long, string>>();

            if (anArticle.CategoryIds.Count == 0)
            {
                ArticleCategoryId = -1;
                ddList.Add(_nullCategory);
            }
            else
            {
                ArticleCategoryId = anArticle.CategoryIds[0];
            }
            ddList.AddRange(categoryList.ToDictionary(aCategory => aCategory.Id.Value, aCategory => aCategory.Title));

            CategoryList = new SelectList(ddList, "key", "value", ArticleCategoryId);
            
            if (anArticle.IsPublished)
            {
                SaveState = SaveStates.Published;
            }
            else
            {
                SaveState = SaveStates.DraftAndContinue;
            }

            if (anArticle.CreateDate == DateTime.MinValue)
            {
                CreateDate = null;
                EditDate = null;
            }
            else
            {
                CreateDate = anArticle.CreateDate.ToShortDateString();
                EditDate = anArticle.EditDate.ToShortDateString();
            }

            return true;
        }
    }
}