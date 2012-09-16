using System;
using System.Web.Mvc;
using InsideWordProvider;
using InsideWordMVCWeb.Models.WebProvider;
using InsideWordMVCWeb.Models.Annotation;
using InsideWordMVCWeb.ViewModels.ProviderViewModels;
using InsideWordMVCWeb.ViewModels.Data;
using InsideWordMVCWeb.ViewModels.Shared;
using InsideWordMVCWeb.Models.Utility;
using InsideWordMVCWeb.Models.BusinessLogic;
using InsideWordAdvancedResource;
using JelleDruyts.Mollom.Client;
using InsideWordResource;

namespace InsideWordMVCWeb.Controllers
{
    public partial class ConversationController : BugFixController
    {
        // POST: /conversation/add/article/5
        [AjaxOnly]
        [HttpPost]
        public virtual JsonResult AddComment(long articleId, string textComment, long? conversationId)
        {
            ProviderCurrentMember currentMember = ProviderCurrentMember.Instance;
            AjaxReturnMsgVM viewModel = new AjaxReturnMsgVM
            {
                StatusCode = AjaxReturnMsgVM.Status.success,
                StatusMessage = String.Empty
            };

            ContentCheck result = null;
            if (!currentMember.IsSuperAdmin)
            {
                string email = string.Empty;
                string domain = string.Empty;

                if (currentMember.HasValidAltId(ProviderAlternateMemberId.AlternateType.Email))
                {
                    email = currentMember.Emails[0].Email.Address;
                }

                if (currentMember.HasValidAltId(ProviderAlternateMemberId.AlternateType.Domain))
                {
                    domain = currentMember.Domains[0].Domain.AbsoluteUri;
                }

                // The mollom client crashes if passed in special html chars (&nbsp;) so strip those before sending it over
                string cleanedTextComment = HtmlParser.StripSpecialChars(textComment);
                MollomClient client = new MollomClient(InsideWordWebSettings.MollomPrivateKey, InsideWordWebSettings.MollomPublicKey);
                result = client.CheckContent(string.Empty, cleanedTextComment,
                                                          currentMember.DisplayAdministrativeName,
                                                          email,
                                                          domain,
                                                          HttpContext.Request.UserHostAddress);
            }

            if (!currentMember.IsLoggedOn)
            {
                viewModel.StatusCode = AjaxReturnMsgVM.Status.failure;
                viewModel.StatusMessage = "You must be logged in to Comment.";
            }
            else if (!ProviderArticle.Exists(articleId))
            {
                viewModel.StatusCode = AjaxReturnMsgVM.Status.failure;
                viewModel.StatusMessage = "The article you commented on no longer exists.";
            }
            else if(string.IsNullOrEmpty(textComment) || textComment.Length < 2)
            {
                viewModel.StatusCode = AjaxReturnMsgVM.Status.failure;
                viewModel.StatusMessage = "A comment needs more than 1 character.";
            }
            else if(textComment.Length > ProviderComment.TextSize)
            {
                viewModel.StatusCode = AjaxReturnMsgVM.Status.failure;
                viewModel.StatusMessage = "A comment can't be more than "+ProviderComment.TextSize+ " characters.";
            }
            else if (result != null && result.Classification == ContentClassification.Spam)
            {
                viewModel.StatusCode = AjaxReturnMsgVM.Status.failure;
                viewModel.StatusMessage = "Your comment has been blocked as spam.";
            }
            else if (result != null && result.Quality < InsideWordWebSettings.MollomCommentQuality)
            {
                viewModel.StatusCode = AjaxReturnMsgVM.Status.failure;
                viewModel.StatusMessage = "The quality of your comment is too low. Try improving things such as spelling and grammar.";
            }
            else
            {
                ProviderArticle anArticle = new ProviderArticle(articleId);
                ProviderConversation conversation;
                if (conversationId.HasValue)
                {
                    conversation = new ProviderConversation(conversationId.Value);
                }
                else
                {
                    conversation = new ProviderConversation();
                }

                ProviderComment comment = new ProviderComment();
                CommentBL.AddComment(textComment,
                                     currentMember,
                                     anArticle,
                                     ref conversation,
                                     ref comment);

                ProviderMember author = anArticle.Author;
                long? articleAuthorId = null;
                if (author != null)
                {
                    articleAuthorId = author.Id;
                }

                bool isReply = conversation.Comments.Count > 1;
                if (isReply)
                {
                    CommentVM commentVM = new CommentVM(comment, currentMember, articleAuthorId);
                    commentVM.IsReply = isReply;
                    viewModel.Content = ControllerExtension.RenderPartialViewToString(this, MVC.Child.Views.Comment, (object)commentVM);
                }
                else
                {
                    ConversationVM conversationVM = new ConversationVM(conversation, ProviderCurrentMember.Instance, articleAuthorId);
                    viewModel.Content = ControllerExtension.RenderPartialViewToString(this, MVC.Child.Views.Conversation, (object)conversationVM);
                }

                if (author != null &&
                    author.Id != currentMember.Id &&
                    author.HasValidAltId(ProviderAlternateMemberId.AlternateType.Email))
                {
                    // do some calculations to determine if we should send the e-mail
                    int commentCount = anArticle.CountComments;
                    int n = (int)Math.Log((double)commentCount, 2);
                    if (n < 0) { n = 0; }
                    if (commentCount % (1 << n) == 0)
                    {
                        EmailManager.Instance.SendCommentNotificationEmail(author.Emails[0].Email, anArticle, currentMember);
                    }
                }
            }

            return Json(viewModel);
        }
    }
}
