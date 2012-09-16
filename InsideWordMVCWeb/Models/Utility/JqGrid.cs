using System;
using System.Collections.Generic;
using InsideWordProvider;
using System.Web.Mvc;
using System.Web;
using System.Globalization;

namespace InsideWordMVCWeb.Models.Utility
{
    /// <summary>
    /// Class used to represent a C# list in jqGrid list format
    /// </summary>
    /// <typeparam name="T">Class type that this jqGrid list will represent</typeparam>
    public class JqGridList<T>
    {
        protected int _records;
        protected int _total;
        protected int _page;
        protected List<JqGridRow> _rows;

        public JqGridList()
        {
            _records = 0;
            _total = 0;
            _page = 0;
            _rows = new List<JqGridRow>();
        }

        /// <summary>
        /// Constructor takes the given inputs and constructs a valid jqGrid list
        /// </summary>
        /// <param name="filter">Filter information used to decide how many rows to show</param>
        /// <param name="objectList">Object list to transform into jqGrid list</param>
        /// <param name="jqGridConverter">Converter class used to transform an object into a jqGridRow</param>
        public JqGridList(IInsideWordFilter filter,
                          List<T> objectList,
                          Converter<T, JqGridRow> jqGridConverter,
                          int totalRecords)
        {
            _records = objectList.Count;
            _total = (int)Math.Ceiling((float)totalRecords / (float)filter.Rows);
            _page = filter.Page;
            _rows = objectList.ConvertAll<JqGridRow>(jqGridConverter);
        }

        public int total            { get { return _total; } }
        public int page             { get { return _page; } }
        public int records          { get { return _records; } }
        public List<JqGridRow> rows { get { return _rows; } }
    }

    /// <summary>
    /// Class represents a single row in a jqGrid list
    /// </summary>
    public class JqGridRow
    {
        public JqGridRow()
        {
            cell = new List<string>();
        }

        public long id           { get; set; }
        public List<String> cell { get; set; }
    }

    /// <summary>
    /// Class represents a json response sent back to a JqGrid after adding/editing/deleting a row
    /// </summary>
    public class JqGridResponse
    {
        public JqGridResponse()
        {

        }

        /// <summary>
        /// Boolean that indicates if the operation was a success
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Message to display if the operation did not succeed
        /// </summary>
        public string Message { get; set; }
    }

    /// <summary>
    /// Multiton class of converter classes used to transform a list of a given class into a jqGrid list
    /// </summary>
    public static class JqGridConverter
    {
        /// <summary>
        /// Converter for Member Management admin page
        /// </summary>
        static public Converter<ProviderMember, JqGridRow> MemberManagement { get { return _memberManagementConverter; } }
        static private readonly Converter<ProviderMember, JqGridRow> _memberManagementConverter = new Converter<ProviderMember, JqGridRow>(_MemberManagementConverter);
        static private JqGridRow _MemberManagementConverter(ProviderMember aMember)
        {
            UrlHelper url = new UrlHelper(HttpContext.Current.Request.RequestContext);
            string profileLink = "<a href='" + url.ActionAbsolute(MVC.Member.Profile(aMember.Id.Value, null)) + "'>" + aMember.DisplayAdministrativeName + "</a>";
            List<ProviderEmail> emailList = aMember.Emails;
            string email = (emailList.Count > 0) ? emailList[0].Email.Address : "none";

            JqGridRow aRow = new JqGridRow();
            aRow.id = aMember.Id.Value;
            aRow.cell.Add(aMember.Id.Value.ToString());
            aRow.cell.Add(profileLink);
            aRow.cell.Add(email);
            aRow.cell.Add(aMember.CreateDate.ToShortDateString());
            aRow.cell.Add(aMember.EditDate.ToShortDateString());
            aRow.cell.Add(aMember.HasPassword.ToString());
            aRow.cell.Add(aMember.HasValidAltId(ProviderAlternateMemberId.AlternateType.OpenId).ToString());
            aRow.cell.Add(aMember.IsActive.ToString());
            aRow.cell.Add(aMember.HasValidAltId(ProviderAlternateMemberId.AlternateType.Email).ToString());
            aRow.cell.Add(aMember.IsBanned.ToString());
            aRow.cell.Add(aMember.IsSuperAdmin.ToString());
            aRow.cell.Add(aMember.IsMasterAdmin.ToString());
            aRow.cell.Add(aMember.IsMemberAdmin.ToString());
            aRow.cell.Add(aMember.IsCategoryAdmin.ToString());
            aRow.cell.Add(aMember.IsArticleAdmin.ToString());
            return aRow;
        }

        /// <summary>
        /// Converter for Comment Management admin page
        /// </summary>
        static public Converter<ProviderComment, JqGridRow> CommentManagement { get { return _commentManagementConverter; } }
        static private readonly Converter<ProviderComment, JqGridRow> _commentManagementConverter = new Converter<ProviderComment, JqGridRow>(_CommentManagementConverter);
        static private JqGridRow _CommentManagementConverter(ProviderComment aComment)
        {
            ProviderMember author = aComment.Author;
            string authorAdministrativeName = "Anonymous";
            if (author != null)
            {
                authorAdministrativeName = author.DisplayAdministrativeName;
            }
            JqGridRow aRow = new JqGridRow();
            aRow.id = aComment.Id.Value;
            aRow.cell.Add(aComment.Id.Value.ToString());
            aRow.cell.Add(aComment.CreateDate.ToShortDateString());
            aRow.cell.Add(aComment.EditDate.ToShortDateString());
            aRow.cell.Add(authorAdministrativeName);
            aRow.cell.Add(aComment.MemberId.ToString());
            aRow.cell.Add(aComment.ConversationId.ToString());
            aRow.cell.Add(aComment.CountFlags.ToString());
            aRow.cell.Add(aComment.IgnoreFlags.ToString());
            aRow.cell.Add(aComment.IsHidden.ToString());
            aRow.cell.Add(aComment.Text);
            return aRow;
        }

        /// <summary>
        /// Converter for Article Management admin page
        /// </summary>
        static public Converter<ProviderArticle, JqGridRow> ArticleManagement { get { return _articleManagementConverter; } }
        static private readonly Converter<ProviderArticle, JqGridRow> _articleManagementConverter = new Converter<ProviderArticle, JqGridRow>(_ArticleManagementConverter);
        static private JqGridRow _ArticleManagementConverter(ProviderArticle anArticle)
        {
            UrlHelper url = new UrlHelper(HttpContext.Current.Request.RequestContext);
            string articleLink = "<a href='" + url.ActionAbsolute(MVC.Article.ArticleDetails(anArticle.Id.Value)) + "'>" + anArticle.Title + "</a>";
            ProviderMember author = anArticle.Author;
            string authorAdministrativeName = "Anonymous";
            if (author != null)
            {
                authorAdministrativeName = author.DisplayAdministrativeName;
            }
            JqGridRow aRow = new JqGridRow();
            aRow.id = anArticle.Id.Value;
            aRow.cell.Add(anArticle.Id.Value.ToString());
            aRow.cell.Add(articleLink);
            aRow.cell.Add(anArticle.CreateDate.ToShortDateString());
            aRow.cell.Add(anArticle.EditDate.ToShortDateString());
            aRow.cell.Add(authorAdministrativeName);
            aRow.cell.Add(anArticle.MemberId.ToString());
            aRow.cell.Add(anArticle.CountFlags.ToString());
            aRow.cell.Add(anArticle.IgnoreFlags.ToString());
            aRow.cell.Add(anArticle.IsHidden.ToString());
            aRow.cell.Add(anArticle.IsPublished.ToString());
            return aRow;
        }

        /// <summary>
        /// Converter for Article on Account page
        /// </summary>
        static public Converter<ProviderArticle, JqGridRow> Article { get { return _articleConverter; } }
        static private readonly Converter<ProviderArticle, JqGridRow> _articleConverter = new Converter<ProviderArticle, JqGridRow>(_ArticleConverter);
        static private JqGridRow _ArticleConverter(ProviderArticle anArticle)
        {
            UrlHelper url = new UrlHelper(HttpContext.Current.Request.RequestContext);
            string titleLink = "<a href='" + url.ActionAbsolute(MVC.Article.ArticleDetails(anArticle.Id.Value)) + "'>" + anArticle.Title + "</a>";
            string commentsLink = "<a href='" + url.ActionAbsolute(MVC.Article.ArticleDetails(anArticle.Id.Value)) + "#comments'>" + anArticle.CountComments.ToString() + "</a>";
            string editLink = "<a href='" + url.ActionAbsolute(MVC.Article.ArticleEdit(anArticle.Id.Value, null)) + "'>Edit</a>";
            string articleVotes = InsideWordUtility.FormatVotes(anArticle.CountVotes);

            JqGridRow aRow = new JqGridRow();
            aRow.id = anArticle.Id.Value;
            aRow.cell.Add(titleLink);
            aRow.cell.Add(anArticle.EditDate.ToShortDateString());
            aRow.cell.Add(articleVotes);
            aRow.cell.Add(commentsLink);
            aRow.cell.Add(anArticle.ViewCount.ToString());
            aRow.cell.Add((anArticle.CountFlags > 0).ToString());
            aRow.cell.Add(anArticle.IsPublished.ToString());
            aRow.cell.Add(editLink);
            return aRow;
        }
    }
}
