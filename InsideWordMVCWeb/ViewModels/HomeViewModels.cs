using InsideWordProvider;
using System.Collections.Generic;
using InsideWordMVCWeb.ViewModels.ProviderViewModels;

namespace InsideWordMVCWeb.ViewModels.Home
{
    public class IndexVM
    {
        public string PageTitle { get; set; }
        public ProviderCategory Category { get; set; }
        public bool ShowTitle { get; set; }
        public List<BlurbVM> BlurbList { get; set; }
        public int? NextPage { get; set; }
        public bool IsLastPage { get; set; }

        public IndexVM(ProviderCategory aCategory, List<BlurbVM> blurbList, int currentPage, bool isLastPage)
        {
            
            if (aCategory.IsRoot)
            {
                PageTitle = string.Empty;
                ShowTitle = false;
            }
            else
            {
                ShowTitle = true;
                PageTitle = aCategory.Title;
            }
            NextPage = currentPage + 1;
            Category = aCategory;
            BlurbList = blurbList;
            IsLastPage = isLastPage;
        }
    }
}