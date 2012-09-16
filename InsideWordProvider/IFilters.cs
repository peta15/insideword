using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InsideWordProvider
{
    public interface IInsideWordFilter
    {
        int Page    { get; set; }
        int Rows    { get; set; }
        string Sidx { get; set; }
        string Sord { get; set; }
    }

    public interface IProviderMemberFilter : IInsideWordFilter
    {
        int? Id               { get; set; }
        string UserName       { get; set; }
        string Email          { get; set; }
        bool? IsValidEmail    { get; set; }
        bool? HasPassword     { get; set; }
        bool? HasOpenId       { get; set; }
        bool? IsActive        { get; set; }
        bool? IsBanned        { get; set; }
        bool? IsSuperAdmin    { get; set; }
        bool? IsMasterAdmin   { get; set; }
        bool? IsMemberAdmin   { get; set; }
        bool? IsCategoryAdmin { get; set; }
        bool? IsArticleAdmin  { get; set; }
    }

    public interface IProviderCommentFilter : IInsideWordFilter
    {
        int?  Id          { get; set; }
        int   CountFlags  { get; set; }
        bool? IsHidden    { get; set; }
        bool? IgnoreFlags { get; set; }
    }

    public interface IProviderArticleFilter : IInsideWordFilter
    {
        int? Id           { get; set; }
        String Title   { get; set; }
        int CountFlags    { get; set; }
        bool? IsHidden    { get; set; }
        bool? IsPublished { get; set; }
        bool? IgnoreFlags { get; set; }
        long? MemberId    { get; set; }
    }
}
