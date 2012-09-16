using InsideWordMVCWeb.ViewModels.Member;
using System.ComponentModel.DataAnnotations;
using InsideWordProvider;
using InsideWordMVCWeb.Models.Annotation;
using System.ComponentModel;
using System.Web.Mvc;
using System.Collections.Generic;
using InsideWordMVCWeb.ViewModels.API;
using System.Linq;

namespace InsideWordMVCWeb.ViewModels.Child
{
    public class MasterLoginModalVM
    {
        public bool IsLoggedOn { get; set; }
        public bool DisplayLoginModal { get; set; }
    }

    public class MasterNavigationBarVM
    {
        public bool DisplayAdminTools { get; set; }
    }

    public class RegisterVM
    {
        [StringLength(ProviderMember.UserNameSize)]
        [UserNameUnique(ErrorMessage = "User name is already taken.")]
        [DisplayName("User Name (optional)")]
        public string UserName { get; set; }

        [Required]
        [StringLength(ProviderMember.EmailSize)]
        [Email(ErrorMessage = "Invalid Email Address")]
        [EmailUnique(ErrorMessage = "E-mail is already taken.")]
        [DisplayName("Email Address")]
        public string Email { get; set; }

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

        [BooleanRequiredToBeTrue(ErrorMessage = "Please agree to the terms")]
        public bool AgreeLegal { get; set; }
    }

    public class LoginVM
    {
        [Required]
        [StringLength(ProviderMember.EmailSize)]
        [DisplayName("E-mail")]
        public string Email { get; set; }

        [Required]
        [RegularExpression(@"\w{8,16}", ErrorMessage = "Passwords must be 8 to 16 numbers and letters")]
        [DataType(DataType.Password)]
        [DisplayName("Password")]
        public string Password { get; set; }

        public bool RememberMe { get; set; }
    }

    public class LoginOpenIdVM
    {
        [Required]
        public string openid_identifier { get; set; }
    }


    public class PersonalInfoVM
    {
        public long MemberId { get; set; }

        [StringLength(ProviderMember.BioSize)]
        [DisplayName("Bio")]
        public string Bio { get; set; }

        [StringLength(ProviderMember.UserNameSize)]
        [DisplayName("User Name")]
        public string UserName { get; set; }

        public PersonalInfoVM() { }

        public PersonalInfoVM(ProviderMember aMember)
        {
            MemberId = aMember.Id.Value;
            Bio = aMember.Bio;
            if (aMember.UserNames.Count > 0)
            {
                UserName = aMember.UserNames[0].UserName;
            }
            else
            {
                UserName = "";
            }
        }
    }

    public class AlternateCategoryListVM
    {
        static protected KeyValuePair<long, string> _nullCategory;
        static AlternateCategoryListVM()
        {
            _nullCategory = new KeyValuePair<long, string>(-1, "auto");
        }

        [DisplayName("Override flag")]
        public bool OverrideFlag { get; set; }

        public long MemberId { get; set; }
        
        public List<AlternateCategoryMapVM> AlternateCategoryMapList { get; set; }

        [ScaffoldColumn(false)]
        public List<SelectList> CategoryList { get; set; }

        public AlternateCategoryListVM()
        {
            OverrideFlag = false;
            CategoryList = new List<SelectList>();
            AlternateCategoryMapList = new List<AlternateCategoryMapVM>();
        }

        public AlternateCategoryListVM(ProviderMember aMember, List<ProviderCategory> categoryList)
        {
            OverrideFlag = false;
            MemberId = aMember.Id.Value;
            Refresh(aMember, categoryList);
        }

        public bool Refresh(ProviderMember aMember, List<ProviderCategory> categoryList)
        {
            CategoryList = new List<SelectList>();
            AlternateCategoryMapList = new List<AlternateCategoryMapVM>();

            List<KeyValuePair<long, string>> ddList = new List<KeyValuePair<long, string>>();
            ddList.Add(_nullCategory);
            ddList.AddRange(categoryList.ToDictionary(aCategory => aCategory.Id.Value, aCategory => aCategory.Title));

            foreach (ProviderAlternateCategoryId altCategory in aMember.AlternateCategoryList)
            {
                if(!string.IsNullOrEmpty(altCategory.DisplayName))
                {
                    long selectedCatId = altCategory.CategoryId ?? -1;
                    AlternateCategoryMapList.Add(new AlternateCategoryMapVM
                    {
                        AlternateTitle = altCategory.DisplayName,
                        AlternateId = altCategory.AlternateId,
                        MapId = selectedCatId
                    });
                
                    CategoryList.Add(new SelectList(ddList, "key", "value", selectedCatId));
                }
            }
            return true;
        }
        
    }
}