using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading;
using System.Net;
using InsideWordProvider;
using InsideWordMVCWeb.Models.BusinessLogic;
using System.Collections;
using System.ComponentModel;

namespace InsideWordMVCWeb.Models.Utility
{
    public class AsyncRefreshArticleManager
    {
        protected Queue<long> _articleIdQueue;
        protected Queue<Tuple< LoadEnum, long>> _loadQueue;
        protected BackgroundWorker _currentRefreshWorker;
        protected BackgroundWorker _currentLoadWorker;

        public bool IsBusy { get { return _currentRefreshWorker.IsBusy; } }
        //public int TotalArticles { get; set; }
        public int Progress { get ; protected set; }
        public bool Completed { get; protected set; }
        public List<string> ErrorList { get; set; }

        public void Clear()
        {
            ErrorList = new List<string>();
            Completed = false;
            Progress = 0;
        }

        public bool AddArticle(long articleId)
        {
            bool returnValue = false;
            lock (_articleIdQueue)
            {
                if (!_articleIdQueue.Contains(articleId))
                {
                    _articleIdQueue.Enqueue(articleId);
                    returnValue = true;
                    if (!_currentRefreshWorker.IsBusy)
                    {
                        _currentRefreshWorker.RunWorkerAsync();
                    }
                }
            }
            return returnValue;
        }

        public void AddArticleList(List<long> articleList)
        {
            lock (_articleIdQueue)
            {
                foreach (long articleId in articleList)
                {
                    if (!_articleIdQueue.Contains(articleId))
                    {
                        _articleIdQueue.Enqueue(articleId);
                    }
                }
                if (!_currentRefreshWorker.IsBusy)
                {
                    _currentRefreshWorker.RunWorkerAsync();
                }
            }
        }

        public void AddBy(LoadEnum type, long id = 0)
        {
            lock (_loadQueue)
            {
                _loadQueue.Enqueue(new Tuple<LoadEnum, long>(type, id));
                if (!_currentLoadWorker.IsBusy)
                {
                    _currentLoadWorker.RunWorkerAsync();
                }
            }
        }

        //=========================================================
        // PRIVATE
        //=========================================================
        protected AsyncRefreshArticleManager()
        {
            Clear();
            _articleIdQueue = new Queue<long>();
            _loadQueue = new Queue<Tuple<LoadEnum, long>>();

            _currentRefreshWorker = new BackgroundWorker {
                WorkerReportsProgress = true
            };
            _currentRefreshWorker.DoWork += DelegateRefreshArticle;
            _currentRefreshWorker.ProgressChanged += DelegateRefreshProgressChange;
            _currentRefreshWorker.RunWorkerCompleted += DelegateRefreshWorkComplete;

            _currentLoadWorker = new BackgroundWorker();
            _currentLoadWorker.DoWork += DelegateLoadArticle;
        }

        protected void DelegateRefreshArticle(object sender, DoWorkEventArgs e)
        {
            InsideWordWebLog.Instance.Log.Debug("AsyncRefreshArticleManager.DelegateRefreshArticle(sender, e)");
            while(_articleIdQueue.Count > 0)
            {
                List<string> tempErrorList = new List<string>();
                ProviderArticle anArticle = new ProviderArticle(_articleIdQueue.Dequeue());
                try
                {
                    // Refresh it once to force re-parsing of the text
                    anArticle.ForceRefresh();

                    // Associate the photos, but don't bother returning any errors.
                    ArticleBL.AssociatePhotos(anArticle.ParsedText, anArticle, ref tempErrorList);
                    if (tempErrorList.Count > 0)
                    {
                        InsideWordWebLog.Instance.Log.Debug(string.Join(", ", tempErrorList));
                    }

                    // save it one more time to save the relationships
                    anArticle.Save();
                }
                catch (Exception caughtException)
                {
                    tempErrorList.Add("article " + anArticle.Id.Value + " exception: " + caughtException);
                }
                ErrorList.AddRange(tempErrorList);
                _currentRefreshWorker.ReportProgress(_articleIdQueue.Count());
            }
            Provider.DbCtx.DisposeCtx();
        }

        protected void DelegateRefreshProgressChange(object sender, ProgressChangedEventArgs e)
        {
            Progress = e.ProgressPercentage;
        }

        protected void DelegateRefreshWorkComplete(object sender, RunWorkerCompletedEventArgs e)
        {
            Completed = true;
        }

        protected void DelegateLoadArticle(object sender, DoWorkEventArgs e)
        {
            List<long> articleList = new List<long>();
            while (_loadQueue.Count > 0)
            {
                Tuple<LoadEnum, long> loadType = _loadQueue.Dequeue();
                switch (loadType.Item1)
                {
                    case LoadEnum.All:
                        articleList.AddRange(ProviderArticle.GetAllArticleId());
                        break;
                    case LoadEnum.AltCategory:
                        articleList.AddRange(ProviderArticle.GetArticleIdByAltCategory(loadType.Item2));
                        break;
                    case LoadEnum.Member:
                        articleList.AddRange(ProviderArticle.GetArticleIdByMember(loadType.Item2));
                        break;
                    default:
                        // do nothing
                        break;
                }
            }
            AddArticleList(articleList);
            Provider.DbCtx.DisposeCtx();
        }

        //=========================================================
        // STATIC
        //=========================================================
        static protected AsyncRefreshArticleManager _instance;
        static public AsyncRefreshArticleManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new AsyncRefreshArticleManager();
                }
                return _instance;
            }
        }

        public enum LoadEnum
        {
            All = 0,
            Member,
            AltCategory
        }
    }
}