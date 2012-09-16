using System.Web.Mvc;
using InsideWordMVCWeb.Models.Annotation;
using InsideWordMVCWeb.ViewModels.Shared;
using InsideWordProvider;
using System.Collections.Generic;
using System.Web;
using InsideWordMVCWeb.Models.WebProvider;
using InsideWordMVCWeb.ViewModels;
using System;
using System.Linq;
using InsideWordMVCWeb.Models.Utility;
using InsideWordMVCWeb.Models.BusinessLogic;
using InsideWordAdvancedResource;

namespace InsideWordMVCWeb.Controllers
{
    public partial class SharedController : BugFixController
    {
        // POST: /Shared/vote/article
        [AjaxOnly]
        [RestrictAccess]
        [HttpPost]
        public virtual JsonResult Vote(long articleId, long voteValue)
        {
            // direction: true = vote up, false = vote down

            AjaxReturnMsgVM viewModel = new AjaxReturnMsgVM
            {
                StatusCode = AjaxReturnMsgVM.Status.failure,
                StatusMessage = string.Empty,
                Content = string.Empty
            };

            ProviderCurrentMember currentMember = ProviderCurrentMember.Instance;
            
            if ( !currentMember.IsLoggedOn )
            {
                viewModel.StatusCode = AjaxReturnMsgVM.Status.failure;
                viewModel.Content = "You must be logged on to vote.";
            }
            else if (!ProviderArticle.Exists(articleId))
            {
                viewModel.StatusCode = AjaxReturnMsgVM.Status.failure;
                viewModel.Content = "The article you voted on no longer exists.";
            }
            else if(ProviderArticleVote.HasVoted(articleId, currentMember.Id.Value))
            {
                viewModel.StatusCode = AjaxReturnMsgVM.Status.failure;
                viewModel.Content = "You have already voted on this article.";
            }
            else
            {
                ProviderArticleVote aVote = new ProviderArticleVote();
                aVote.ArticleId = articleId;
                aVote.MemberId = currentMember.Id.Value;
                aVote.IsUpVote = voteValue > 0;
                aVote.IsDownVote = voteValue < 0;
                aVote.Save();

                ProviderArticle anArticle = new ProviderArticle(articleId);
                viewModel.StatusCode = AjaxReturnMsgVM.Status.success;
                viewModel.Content = InsideWordUtility.FormatVotes(anArticle.CountVotes);
                ProviderMember author = anArticle.Author;
                if (aVote.IsUpVote && 
                    author != null &&
                    author.Id != currentMember.Id &&
                    author.HasValidAltId(ProviderAlternateMemberId.AlternateType.Email))
                {
                    // do some calculations to determine if we should send the e-mail
                    int voteCount = anArticle.CountVotes;
                    int n = (int)Math.Log((double)voteCount, 2);
                    if (n < 0) { n = 0; }
                    if (voteCount % (1 << n) == 0)
                    {
                        EmailManager.Instance.SendVoteNotificationEmail(author.Emails[0].Email, anArticle);
                    }
                }
            }

            return Json(viewModel);
        }

        // GET: /Shared/AddMedia
        [AcceptVerbs(HttpVerbs.Head | HttpVerbs.Get)]
        public virtual ActionResult AddMedia(AddMediaVM.AddMediaPurpose purpose, AddMediaVM.AddMediaType type, long? memberId)
        {
            ActionResult returnValue = null;
            AddMediaVM viewModel = new AddMediaVM
            {
                Type = type,
                Purpose = purpose
            };
            if (purpose == AddMediaVM.AddMediaPurpose.TinyMce || purpose == AddMediaVM.AddMediaPurpose.ProfileImageAjax)
            {
                returnValue = View(MVC.Shared.Views.AddMediaStandalone, viewModel);
            }
            else if (purpose == AddMediaVM.AddMediaPurpose.ProfileImage)
            {
                returnValue = View("AddMedia", viewModel);
            }
            else
            {
                if (this.Request.IsAjaxRequest())
                {
                    var viewModelError = new AjaxReturnMsgVM
                    {
                        StatusCode = AjaxReturnMsgVM.Status.failure,
                        StatusMessage = "Unsupported purpose"
                    };

                    returnValue = Json(viewModelError);
                }
                else
                {
                    var viewModelError = new MessageVM
                    {
                        Image = ImageLibrary.Alert,
                        CssClassContainer = "failure",
                        Message = "Unsupported purpose ",
                        Title = ErrorStrings.TITLE_ERROR,
                        LinkText = "Continue",
                        LinkHref = Url.Action(MVC.Home.Index()),
                    };

                    returnValue = View("Message", viewModelError);
                }
            }

            return returnValue;
        }

        // POST: /Shared/AddMedia
        [HttpPost]
        public virtual ActionResult AddMedia(AddMediaVM model)
        {
            ActionResult returnValue = null;
            ProviderCurrentMember currentMember = ProviderCurrentMember.Instance;
            ProviderMember aMember = null;

            // try to get the member from the model, otherwise just use current member
            // TODO check this logic
            if (model.MemberId != null)
            {
                aMember = new ProviderMember(model.MemberId.Value);
            }
            else
            {
                aMember = currentMember;
            }

            HttpPostedFileBase filePhoto = Request.Files[model.FilePhotoKey] as HttpPostedFileBase;
            bool isValidPhoto = ProviderPhoto.Validate(filePhoto);
            ProviderPhoto originalPhoto = null;
			
            if (isValidPhoto)
            {
                originalPhoto = PhotoBL.CreatePhoto(aMember, filePhoto);
            }

            if (model.Purpose == AddMediaVM.AddMediaPurpose.TinyMce)
            {
                if (isValidPhoto)
                {
                    returnValue = View("AddMediaDone", new AddMediaDoneVM { FileURL = originalPhoto.ImageUrl.AbsoluteUri });
                }
                else
                {
                    ModelState.AddModelError("", ErrorStrings.INVALID_IMAGE);
                    returnValue = View(MVC.Shared.Views.AddMediaStandalone, model);
                }
            }
            else if (model.Purpose == AddMediaVM.AddMediaPurpose.ProfileImage || model.Purpose == AddMediaVM.AddMediaPurpose.ProfileImageAjax)
            {
				if(isValidPhoto)
                {
                    aMember.ProfilePhotoId = originalPhoto.Id;
                    aMember.Save();
				}

                if (model.Purpose == AddMediaVM.AddMediaPurpose.ProfileImageAjax)
                {
					var viewModel = new AjaxReturnMsgVM();
					if(isValidPhoto)
					{
						viewModel.StatusCode = AjaxReturnMsgVM.Status.success;
                        viewModel.StatusMessage = originalPhoto.Thumbnail(ProviderPhotoRecord.ImageTypeEnum.ProfileThumbnail)
                                                               .ImageUrl
                                                               .AbsoluteUri;
					}
					else
					{
						viewModel.StatusCode = AjaxReturnMsgVM.Status.failure;
						viewModel.StatusMessage = ErrorStrings.INVALID_IMAGE;
					}
                    returnValue = Json(viewModel);
                }
                else if (model.Purpose == AddMediaVM.AddMediaPurpose.ProfileImage)
                {
                    if (isValidPhoto)
                    {
                        var viewModel = new MessageVM
                        {
                            Image = ImageLibrary.Success,
                            CssClassContainer = "success",
                            Message = "Profile Image updated",
                            Title = "Profile Image updated",
                            LinkText = "Continue",
                            LinkHref = Url.Action(MVC.Member.Account(aMember.Id.Value, null)),
                        };
                        returnValue = View("Message", viewModel);
                    }
                    else
                    {
                        ModelState.AddModelError("", ErrorStrings.INVALID_IMAGE);
                        returnValue = View(model);
                    }
                }
            }
            else
            {
                if(this.Request.IsAjaxRequest())
                {
                    var viewModel = new AjaxReturnMsgVM
                    {
                        StatusCode = AjaxReturnMsgVM.Status.failure,
                        StatusMessage = "Unsupported purpose"
                    };

                    returnValue = Json(viewModel);
                }
                else
                {
                    var viewModel = new MessageVM
                    {
                        Image = ImageLibrary.Alert,
                        CssClassContainer = "failure",
                        Message = "Unsupported purpose ",
                        Title = ErrorStrings.TITLE_ERROR,
                        LinkText = "Continue",
                        LinkHref = Url.Action(MVC.Home.Index()),
                    };

                    returnValue = View("Message", viewModel);
                }
            }

            return returnValue;
        }
    }
}