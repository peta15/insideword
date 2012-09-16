using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using InsideWordMVCWeb.Models.Annotation;
using InsideWordMVCWeb.ViewModels.ProviderViewModels;
using InsideWordProvider;
using Foolproof;
using System;
using InsideWordMVCWeb.Models.WebProvider;

namespace InsideWordMVCWeb.ViewModels.Group
{
    public class GroupIndexVM
    {
        public List<GroupVM> AllGroups { get; set; }
        public List<GroupVM> MemberGroups { get; set; }

        public GroupIndexVM(ProviderCurrentMember currentMember)
        {
            Parse(currentMember);
        }

        public GroupIndexVM(ProviderMember aMember, ProviderCurrentMember currentMember)
        {
            Parse(aMember, currentMember);
        }

        public bool Parse(ProviderCurrentMember currentMember)
        {
            return Parse(null, currentMember);
        }

        public bool Parse(ProviderMember aMember, ProviderCurrentMember currentMember)
        {
            AllGroups = ProviderGroup.LoadAll().ConvertAll<GroupVM>(aGroup => new GroupVM(aGroup, currentMember));
            if (aMember != null)
            {
                MemberGroups = aMember.Groups.ConvertAll<GroupVM>(aGroup => new GroupVM(aGroup, currentMember)); ;
            }
            return true;
        }
    }

    public class GroupDetailVM
    {
        public string PageTitle { get; set; }
        public List<MemberVM> Members { get; set; }
        public List<ProviderRole> RolesWithoutGlobal { get; set; }
        public List<ProviderRole> RolesWithGlobal { get; set; }
        public List<ProviderRole> RolesGloballyInherited { get; set; }
        public List<ProviderArticle> Articles { get; set; }
        public List<ProviderPhotoRecord> Photos { get; set; }

        public GroupDetailVM()
        {

        }

        public GroupDetailVM(ProviderGroup aGroup, ProviderCurrentMember currentMember)
        {
            Parse(aGroup, currentMember);
        }

        public bool Parse(ProviderGroup aGroup, ProviderCurrentMember currentMember)
        {
            PageTitle = "Group: " + aGroup.Name;
            Members = aGroup.Members.ConvertAll<MemberVM>(aMember => new MemberVM(aMember, currentMember));
            RolesWithoutGlobal = aGroup.RolesWithoutGlobal;
            RolesWithGlobal = aGroup.RolesWithGlobal;
            RolesGloballyInherited = aGroup.RolesGloballyInherited;
            Articles = aGroup.Articles;
            Photos = aGroup.Photos;
            return true;
        }
    }

    public class GroupRegisterVM
    {
        [StringLength(ProviderGroup.NameSize)]
        [GroupNameUnique(ErrorMessage = "Name is already taken.")]
        public string Name { get; set; }

        // review this.  will have to figure out how u pick members.
        [DisplayName("Members")]
        public List<string> Members { get; set; }

        public GroupRegisterVM() { }
    }

    public class GroupManageVM
    {
        public string PageTitle { get; set; }
        public GroupManageVM()
        {

        }

        public GroupManageVM(ProviderGroup aGroup)
        {
            Parse(aGroup);
        }

        public bool Parse(ProviderGroup aGroup)
        {
            PageTitle = "Manage: " + aGroup.Name;
            return true;
        }
    }
}