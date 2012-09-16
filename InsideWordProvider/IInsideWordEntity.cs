using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Objects.DataClasses;

namespace InsideWordProvider
{
    public interface IInsideWordEntity
    {
        long Id { get; set; }
        DateTime SystemCreateDate { get; set; }
        DateTime SystemEditDate { get; set; }
    }

    public partial class Article : IInsideWordEntity { }
    public partial class AlternateArticleId : IInsideWordEntity { }
    public partial class ArticleVote : IInsideWordEntity { }
    public partial class ArticleScore : IInsideWordEntity { }

    public partial class Permission : IInsideWordEntity { }
    public partial class Role : IInsideWordEntity { }
    public partial class Group : IInsideWordEntity { }
    public partial class Membership : IInsideWordEntity { }

    public partial class Member : IInsideWordEntity { }
    public partial class AlternateMemberId : IInsideWordEntity { }
    
    public partial class Category : IInsideWordEntity { }
    public partial class AlternateCategoryId : IInsideWordEntity { }

    public partial class Conversation : IInsideWordEntity { }
    public partial class Comment : IInsideWordEntity { }
    public partial class ConversationVote : IInsideWordEntity { }
    public partial class ConversationScore : IInsideWordEntity { }
    public partial class Comment : IInsideWordEntity { }

    public partial class PhotoVote : IInsideWordEntity { }
    public partial class PhotoScore : IInsideWordEntity { }
    public partial class Photo : IInsideWordEntity { }

    public partial class InsideWordSetting : IInsideWordEntity { }
}
