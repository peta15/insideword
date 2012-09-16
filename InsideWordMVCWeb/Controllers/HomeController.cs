using System;
using System.Linq;
using System.Web.Mvc;
using System.Collections.Generic;
using InsideWordProvider;
using InsideWordMVCWeb.ViewModels.Home;
using System.Web.Routing;
using InsideWordMVCWeb.Models.BusinessLogic;
using InsideWordMVCWeb.ViewModels.ProviderViewModels;
using System.Web.Caching;

namespace InsideWordMVCWeb.Controllers
{
    public partial class HomeController : BugFixController
    {
        private const string _rankedBlurbViewKey = "rankedBlurbViewListKey";

        // OutputCache cannot be used on this page as it will cache the entire master page and not just this controller
        // GET: /{categoryId}/{page}
        // categoryId and page are optional
        [AcceptVerbs(HttpVerbs.Head | HttpVerbs.Get)]
        public virtual ActionResult Index(long? categoryId, int? page)
        {
            ActionResult returnValue = null;

            if (ArticleBL.MAX_BLURB_PAGE < page)
            {
                returnValue = RedirectToAction(MVC.Home.Index(null, null));
            }
            else
            {
                ProviderCategory aCategory = new ProviderCategory();
                if (!categoryId.HasValue || !aCategory.Load(categoryId.Value))
                {
                    aCategory = ProviderCategory.Root;
                }

                if (page == null || page < 1)
                {
                    page = 0;
                }

                string cacheKey = _rankedBlurbViewKey + aCategory.Id.Value + "-" + page;
                if (HttpContext.Cache[cacheKey] == null)
                {
                    List<BlurbVM> blurbList = ArticleBL.LoadCachedRankedBlurbs(aCategory, page.Value);
                    IndexVM viewModel = new IndexVM(aCategory, blurbList, page.Value, blurbList.Count < ArticleBL.BLURBS_PER_PAGE);
                    DateTime expiration = DateTime.UtcNow.AddSeconds(ArticleBL.BLURB_CACHE_SECONDS);
                    HttpContext.Cache.Add(cacheKey,
                                            View(viewModel),
                                            null,
                                            expiration,
                                            Cache.NoSlidingExpiration,
                                            CacheItemPriority.Low,
                                            null);
                }
                returnValue = (ViewResult)HttpContext.Cache[cacheKey];
            }

            return returnValue;
        }
    }
}
