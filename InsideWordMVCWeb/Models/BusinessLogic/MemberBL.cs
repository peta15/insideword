using InsideWordProvider;
using InsideWordMVCWeb.Models.Utility;
using InsideWordMVCWeb.ViewModels.Admin;
using System;
using System.Net.Mail;
using InsideWordMVCWeb.ViewModels.Member;
using InsideWordMVCWeb.ViewModels.Child;
using System.Collections.Generic;
using InsideWordMVCWeb.ViewModels.Shared;
using InsideWordMVCWeb.Models.WebProvider;
using InsideWordResource;
using InsideWordMVCWeb.ViewModels.API;

namespace InsideWordMVCWeb.Models.BusinessLogic
{
    public static class MemberBL
    {
        public static JqGridResponse Process(EditMemberManagementVM model, ProviderCurrentMember currentMember)
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

        public static JqGridResponse Edit(EditMemberManagementVM model, ProviderCurrentMember currentMember)
        {
            JqGridResponse aResponse = new JqGridResponse();
            ProviderMember aMember = new ProviderMember(model.Id);
            
            if (currentMember.CanEdit(aMember))
            {
                aMember.IsArticleAdmin = model.IsArticleAdmin;
                aMember.IsBanned = model.IsBanned;
                aMember.IsCategoryAdmin = model.IsCategoryAdmin;
                aMember.IsMasterAdmin = model.IsMasterAdmin;
                aMember.IsMemberAdmin = model.IsMemberAdmin;
                aMember.IsSuperAdmin = model.IsSuperAdmin;

                try
                {
                    aMember.Save();
                    aResponse.Success = true;
                }
                catch (Exception caughtException)
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

        public static JqGridResponse Delete(EditMemberManagementVM model, ProviderCurrentMember currentMember)
        {
            JqGridResponse aResponse = new JqGridResponse();
            ProviderMember aMember = new ProviderMember(model.Id);
            if (currentMember.CanEdit(aMember))
            {
                if (aMember.Delete())
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

        static public bool Save(RegisterVM model, ref ProviderMember registerMember)
        {
            registerMember.Password = model.Password;
            registerMember.CreateDate = DateTime.UtcNow;
            registerMember.EditDate = DateTime.UtcNow;
            registerMember.Save();

            if (!string.IsNullOrWhiteSpace(model.Email))
            {
                ProviderEmail anEmail = new ProviderEmail();
                anEmail.MemberId = registerMember.Id.Value;
                anEmail.IsValidated = false;
                anEmail.CreateDate = DateTime.UtcNow;
                anEmail.EditDate = DateTime.UtcNow;
                anEmail.Email = new MailAddress(model.Email);
                anEmail.Save();
            }

            if(!string.IsNullOrWhiteSpace(model.UserName))
            {
                ProviderUserName aUserName = new ProviderUserName();
                aUserName.MemberId = registerMember.Id.Value;
                aUserName.CreateDate = DateTime.UtcNow;
                aUserName.EditDate = DateTime.UtcNow;
                aUserName.UserName = model.UserName;
                aUserName.Save();
            }

            return true;
        }

        /// <summary>
        /// Function to update or create an member account with openId information.
        /// </summary>
        /// <param name="openId">openId of the member account we wish to create or update</param>
        /// <param name="email">e-mail information of the member</param>
        /// <param name="host">the open id provider hostname (www.google.com for instance)</param>
        /// <returns>true if the member was successfully updated/created.</returns>
        static public bool UpdateMemberOpenId(string openId, MailAddress email, string host)
        {
            // check if this member exists already
            ProviderMember aMember;
            ProviderEmail anEmailId = new ProviderEmail();
            ProviderOpenId anOpenId = new ProviderOpenId();

            // Prepare the e-mail
            if (email != null)
            {
                if (!anEmailId.Load(email.Address))
                {
                    anEmailId.CreateDate = DateTime.UtcNow;
                    anEmailId.EditDate = DateTime.UtcNow;
                    anEmailId.Email = email;
                    anEmailId.IsValidated = true;
                }
            }

            // Prepare the openId
            if (!anOpenId.Load(openId))
            {
                anOpenId.CreateDate = DateTime.UtcNow;
                anOpenId.EditDate = DateTime.UtcNow;
                anOpenId.OpenId = openId;
                anOpenId.DisplayName = host;
                anOpenId.IsValidated = true;
            }

            // TODO: my gut tells me these if statements can possibly be reduced to something simpler
            if (anOpenId.IsNew && anEmailId.IsNew)
            {
                // This person doesn't exist yet so create them
                aMember = new ProviderMember();
                aMember.CreateDate = DateTime.UtcNow;
                aMember.EditDate = DateTime.UtcNow;
                aMember.Save();

                // create the e-mail
                if (email != null)
                {
                    anEmailId.MemberId = aMember.Id.Value;
                    anEmailId.Save();
                }

                // create the openId
                anOpenId.MemberId = aMember.Id.Value;
                anOpenId.Save();
            }
            else if (!anOpenId.IsNew && anEmailId.IsNew && email != null)
            {
                // this person has an account already with an open id but no e-mail so just add the e-mail to the account
                aMember = new ProviderMember(anOpenId.MemberId);
                aMember.EditDate = DateTime.UtcNow;
                aMember.Save();

                anEmailId.MemberId = aMember.Id.Value;
                anEmailId.Save();
            }
            else if (anOpenId.IsNew && !anEmailId.IsNew)
            {
                // this person has an account already with an e-mail but no openId so just add the openId to the account
                aMember = new ProviderMember(anEmailId.MemberId);
                aMember.EditDate = DateTime.UtcNow;
                aMember.Save();

                anOpenId.MemberId = aMember.Id.Value;
                anOpenId.Save();
            }
            else if ( !anOpenId.IsNew && !anEmailId.IsNew && anOpenId.MemberId != anEmailId.MemberId)
            {
                // TODO: Crap two different accounts!
                // Do nothing for now but what we really want to do is try and merge the two accounts (with user permission of course).
            }
            else
            {
                // No updates are required
            }

            return true;
        }

        static public bool SavePersonalInfo(PersonalInfoVM model, ProviderMember aMember)
        {
            // save username and bio.  everything else is saved asynchronously through other methods
            aMember.Bio = model.Bio;
            aMember.Save();

            
            if (string.IsNullOrWhiteSpace(model.UserName))
            {
                // If someone blanked out their username then delete it.
                // Otherwise it was just blank so do nothing.
                if (aMember.UserNames.Count > 0)
                {
                    aMember.UserNames[0].Delete();
                }
            }
            else
            {
                // If the user name was not blank then create it
                ProviderUserName userName;
                if (aMember.UserNames.Count > 0)
                {
                    userName = aMember.UserNames[0];
                }
                else
                {
                    userName = new ProviderUserName();
                    userName.MemberId = aMember.Id.Value;
                    userName.CreateDate = DateTime.UtcNow;
                }
                userName.EditDate = DateTime.UtcNow;
                userName.UserName = model.UserName;
                userName.Save();
            }

            return true;
        }

        static public ImageInfo GetProfileImage(ProviderMember aMember)
        {
            bool useDefault = true;
            ProviderPhotoRecord profileImage = null;
            ImageInfo profileImageInfo = null;

            if (aMember.ProfilePhotoId != null)
            {
                profileImage = (new ProviderPhotoRecord(aMember.ProfilePhotoId.Value)).Thumbnail(ProviderPhotoRecord.ImageTypeEnum.ProfileThumbnail);
                if (profileImage != null)
                {
                    useDefault = false;
                }
            }

            if (useDefault)
            {
                profileImageInfo = ImageLibrary.DefaultProfile;
            }
            else
            {
                profileImageInfo = profileImage.PhotoInfo;
                profileImageInfo.Alt = "Author Profile Image";
            }
            return profileImageInfo;
        }

        static public bool SaveAlternateCategories(AlternateCategoryListVM model, ProviderMember aMember)
        {
            List<ProviderAlternateCategoryId> alternateList = aMember.AlternateCategoryList;
            ProviderCategory defaultCategory = new ProviderCategory(InsideWordSettingsDictionary.Instance.DefaultCategoryId.Value);
            foreach(AlternateCategoryMapVM aMap in model.AlternateCategoryMapList)
            {
                ProviderAlternateCategoryId altCategory = null;
                if (alternateList.Exists(altId => altId.AlternateId == aMap.AlternateId))
                {
                    altCategory = alternateList.Find(altId => altId.AlternateId == aMap.AlternateId);
                }
                else
                {
                    altCategory = new ProviderAlternateCategoryId();
                    altCategory.AlternateId = aMap.AlternateId;
                    altCategory.MemberId = aMember.Id.Value;
                }

                if (aMap.MapId.HasValue)
                {
                    if (aMap.MapId == -1)
                    {
                        aMap.MapId = defaultCategory.Id;
                    }

                    if (altCategory.CategoryId != aMap.MapId)
                    {
                        altCategory.CategoryId = aMap.MapId;
                        if (!altCategory.IsNew)
                        {
                            // The category map changed. Re-categorize all articles related to this alternate category.
                            AsyncRefreshArticleManager.Instance.AddBy(AsyncRefreshArticleManager.LoadEnum.AltCategory, altCategory.Id.Value);
                        }
                    }
                }

                if (!string.IsNullOrWhiteSpace(aMap.AlternateTitle))
                {
                    altCategory.DisplayName = aMap.AlternateTitle;
                }
                altCategory.OverrideFlag = model.OverrideFlag;
                altCategory.Save();
            }

            return true;
        }
    }
}