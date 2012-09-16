using System;
using InsideWordProvider;
using InsideWordMVCWeb.Models.Utility;
using InsideWordMVCWeb.ViewModels.Admin;
using InsideWordMVCWeb.Models.WebProvider;

namespace InsideWordMVCWeb.Models.BusinessLogic
{
    public static class CommentBL
    {
        public static JqGridResponse Process(EditCommentManagementVM model, ProviderCurrentMember currentMember)
        {
            JqGridResponse aResponse;
            if (model.Oper.CompareTo("edit") == 0)
            {
                aResponse = Edit(model, currentMember);
            }
            else if (model.Oper.CompareTo("del") == 0)
            {
                aResponse = Delete(model, currentMember);
            }
            else
            {
                aResponse = new JqGridResponse();
                aResponse.Success = false;
                aResponse.Message = ErrorStrings.OPERATION_UNKNOWN(model.Oper);
            }
            return aResponse;
        }

        public static JqGridResponse Edit(EditCommentManagementVM model, ProviderCurrentMember currentMember)
        {
            JqGridResponse aResponse = new JqGridResponse();
            ProviderComment aComment = new ProviderComment(model.Id);
            if (currentMember.CanEdit(aComment))
            {
                aComment.IgnoreFlags = model.IgnoreFlags;
                aComment.IsHidden = model.IsHidden;
                try
                {
                    aComment.Save();
                    aResponse.Success = true;
                }
                catch(Exception caughtException)
                {
                    aResponse.Success = false;
                    aResponse.Message = ErrorStrings.OPERATION_FAILED;
                }
            }
            else
            {
                aResponse.Success = false;
                aResponse.Message = ErrorStrings.OPERATION_NO_RIGHTS;
            }

            return aResponse;
        }

        public static JqGridResponse Delete(EditCommentManagementVM model, ProviderCurrentMember currentMember)
        {
            JqGridResponse aResponse = new JqGridResponse();
            ProviderComment aComment = new ProviderComment(model.Id);
            if (currentMember.CanEdit(aComment))
            {
                if(aComment.Delete())
                {
                    aResponse.Success = true;
                }
                else
                {
                    aResponse.Success = false;
                    aResponse.Message = ErrorStrings.OPERATION_FAILED;
                }
            }
            else
            {
                aResponse.Success = false;
                aResponse.Message = ErrorStrings.OPERATION_NO_RIGHTS;
            }
            return aResponse;
        }

        public static void AddComment(string textComment,
                                      ProviderCurrentMember currentMember,
                                      ProviderArticle anArticle ,
                                      ref ProviderConversation conversation,
                                      ref ProviderComment comment)
        {
            if (!currentMember.CanEdit(comment))
            {
                throw new Exception(ErrorStrings.OPERATION_NO_RIGHTS);
            }

            try
            {
                if (conversation.IsNew)
                {
                    conversation.MemberId = currentMember.Id;
                    conversation.ArticleId = anArticle.Id;
                    conversation.CreateDate = DateTime.UtcNow;
                }
                conversation.EditDate = DateTime.UtcNow;
                conversation.Save();

                //make sure the comment code is after the save since this
                //conversation doesn't exist yet.
                comment.ConversationId = conversation.Id.Value;
                comment.MemberId = currentMember.Id;
                comment.Text = textComment;
                comment.EditDate = DateTime.UtcNow;
                comment.CreateDate = DateTime.UtcNow;
                comment.IsHidden = false;
                comment.Save();
            }
            catch (Exception caughtException)
            {
                // DO NOT LOG THIS. It is the responsibility of the calling program to handle the exception
                throw new Exception("Failed to save comment", caughtException);
            }
        }
    }
}