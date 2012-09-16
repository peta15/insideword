using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using InsideWordProvider;
using InsideWordMVCWeb.ViewModels.API;
using InsideWordMVCWeb.Models.Utility;
using InsideWordResource;
using System.Net.Mail;

namespace InsideWordMVCWeb.Models.WebProvider
{
    public class ApiBL
    {
        static public bool UpdateAccount(ProviderCurrentMember currentMember, MemberDataVM data)
        {
            MailAddress validEmail;
            if (!string.IsNullOrWhiteSpace(data.Email) && IWStringUtility.TryParse(data.Email, out validEmail))
            {
                long? memberId = ProviderEmail.FindOwner(validEmail);

                // the e-mail has not been taken so we can take it
                if(!memberId.HasValue)
                {
                    List<ProviderEmail> emailList = currentMember.Emails;
                    if (!emailList.Exists(anEmail => anEmail.Email.Address == validEmail.Address))
                    {
                        // this e-mail doesn't exist so add it
                        ProviderEmail newEmail = new ProviderEmail();
                        newEmail.CreateDate = DateTime.UtcNow;
                        newEmail.EditDate = DateTime.UtcNow;
                        newEmail.Email = validEmail;
                        newEmail.IsValidated = false;
                        newEmail.MemberId = currentMember.Id.Value;
                        newEmail.Save();
                    }
                }
            }

            return true;
        }

        static public List<AlternateCategoryMapVM> MapCategories(ProviderCurrentMember currentMember, List<AlternateCategoryMapVM> memberToIWMap)
        {
            List<AlternateCategoryMapVM> IWToMemberMap = new List<AlternateCategoryMapVM>();
            ProviderCategory defaultCategory = new ProviderCategory(InsideWordSettingsDictionary.Instance.DefaultCategoryId.Value);
            List<ProviderAlternateCategoryId> alternateList = currentMember.AlternateCategoryList;
            List<ProviderCategory> iwCategoryList = ProviderCategory.LoadAll();
            ProviderAlternateCategoryId alternateCategory = null;
            ProviderCategory iwCategory = null;
            long? bestIWMatch = null;
            bool mapHasChanged = false;

            foreach (AlternateCategoryMapVM aMap in memberToIWMap)
            {
                mapHasChanged = false;
                alternateCategory = null;
                iwCategory = null;

                if (alternateList.Exists(altId => altId.AlternateId == aMap.AlternateId))
                {
                    // The alternate category is already in our system so just load it
                    alternateCategory = alternateList.Find(altId => altId.AlternateId == aMap.AlternateId);
                }
                else
                {
                    // The alternate category is not in our system so do a bit more work
                    alternateCategory = new ProviderAlternateCategoryId();
                    alternateCategory.MemberId = currentMember.Id.Value;
                    alternateCategory.AlternateId = aMap.AlternateId;
                }

                // ORDER OF "IF" STATEMENTS MATTERS HERE
                if (alternateCategory.OverrideFlag)
                {
                    // This is an override so use the server value
                    iwCategory = iwCategoryList.Find(aCategory => aCategory.Id == alternateCategory.CategoryId);
                }
                else if (aMap.MapId.HasValue && iwCategoryList.Exists(aCategory => aCategory.Id == aMap.MapId.Value))
                {
                    // the map is preset by the member so use that
                    iwCategory = iwCategoryList.Find(aCategory => aCategory.Id == aMap.MapId.Value);
                }
                else if (alternateCategory.CategoryId.HasValue)
                {
                    // the map is not preset by the member, but we have a server value so use the server value
                    iwCategory = iwCategoryList.Find(aCategory => aCategory.Id == alternateCategory.CategoryId);
                }
                else
                {
                    // this category map doesn't exist at all so find the best match
                    bestIWMatch = ProviderAlternateCategoryId.BestMatch(aMap.AlternateTitle);
                    if (bestIWMatch.HasValue)
                    {
                        iwCategory = iwCategoryList.Find(aCategory => aCategory.Id.Value == bestIWMatch.Value);
                    }
                    else if (iwCategoryList.Exists(aCategory => !aCategory.IsRoot &&
                                                                aCategory.Title
                                                                        .ToLowerInvariant()
                                                                        .CompareTo(aMap.AlternateTitle.ToLowerInvariant()) == 0))
                    {
                        iwCategory = iwCategoryList.Find(aCategory => aCategory.Title
                                                                        .ToLowerInvariant()
                                                                        .CompareTo(aMap.AlternateTitle.ToLowerInvariant()) == 0);
                    }
                    else
                    {
                        iwCategory = defaultCategory;
                    }
                }

                if (!alternateCategory.CategoryId.HasValue || alternateCategory.CategoryId.Value != iwCategory.Id.Value)
                {
                    alternateCategory.CategoryId = iwCategory.Id.Value;
                    mapHasChanged = true;
                }

                // finish and save the changes to the alternate category
                alternateCategory.OverrideFlag = false; // always reset the override flag here
                alternateCategory.DisplayName = aMap.AlternateTitle;
                alternateCategory.Save();

                IWToMemberMap.Add(new AlternateCategoryMapVM { AlternateTitle = iwCategory.Title, AlternateId = iwCategory.Id.Value, MapId = aMap.AlternateId });

                // determine if we need to refresh the articles on the server
                // Do this last to avoid race condition 
                if (!alternateCategory.IsNew && mapHasChanged)
                {
                    // The category map changed. Re-categorize all articles related to this alternate category.
                    AsyncRefreshArticleManager.Instance.AddBy(AsyncRefreshArticleManager.LoadEnum.AltCategory, alternateCategory.Id.Value);
                }
            }

            return IWToMemberMap;
        }
    }
}