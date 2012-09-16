using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using InsideWordProvider;
using InsideWordMVCWeb.Models.WebProvider;

namespace InsideWordMVCWeb.Models.BusinessLogic
{
    public class Workspace
    {
        public Workspace()
        {
            ArticleIdList = new List<long>();
        }

        public List<long> ArticleIdList { get; set; }

        public bool Save(ProviderCurrentMember currentMember)
        {
            if (!currentMember.IsNew)
            {
                ProviderArticle anArticle;
                List<long> tempList = new List<long>(ArticleIdList);
                foreach (long id in tempList)
                {
                    anArticle = new ProviderArticle(id);
                    // if the article already has a value then don't override or add a new one.
                    if (!anArticle.MemberId.HasValue)
                    {
                        anArticle.MemberId = currentMember.Id;
                        anArticle.Save();
                    }
                    ArticleIdList.Remove(id);
                }
            }
            return true;
        }
    }
}