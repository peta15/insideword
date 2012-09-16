using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InsideWordProvider;
using System.Threading;
using System.Runtime.InteropServices;
using log4net;
using log4net.Config;

namespace SparrowRanker
{
    class Program
    {
        static protected bool _continue;
        static protected ILog _log;
        static private TimeSpan _sleepTime;
        static protected ObjectContextManager _ctxManager;

        static int Main(string[] args)
        {
            int returnValue = 0;
            DateTime? startTime = null;
            DateTime? crashTime = null;

            do
            {
                try
                {
                    startTime = DateTime.UtcNow;
                    Initialize();
                    _log.Warn("Entering application.");
                    MainLoop();
                    _log.Warn("Exiting application.");
                }
                catch (Exception caughtException)
                {
                    if (_log == null)
                    {
                        _continue = false;
                        returnValue = 1;
                    }
                    else
                    {
                        _log.Error("Program threw exception", caughtException);
                        if (_continue)
                        {
                            if (!startTime.HasValue)
                            {
                                startTime = DateTime.UtcNow;
                            }
                            crashTime = DateTime.UtcNow;
                            TimeSpan runtimeBeforeCrash = crashTime.Value.Subtract(startTime.Value);
                            double runtimeSecs = runtimeBeforeCrash.TotalSeconds;
                            if (runtimeSecs > 5.0)
                            {
                                _continue = true;
                                returnValue = 0;
                                _log.Error("Program crashed after " + runtimeBeforeCrash.TotalHours + " hours. Restarting Program.");
                            }
                            else
                            {
                                _continue = false;
                                returnValue = 1;
                                _log.Error("Program crashed after " + runtimeSecs + " seconds. Will not restart Program to avoid infinite crash loop.");
                            }
                        }
                    }
                }
            } while(_continue);

            return returnValue;
        }

        static public bool Initialize()
        {
            _continue = true;

            _log = LogManager.GetLogger(typeof(Program));
            XmlConfigurator.Configure();
            _log.Info("Logging initialized");

            SetConsoleCtrlHandler(new HandlerRoutine(ConsoleCtrlCheck), true);
            _log.Info("Exit handler initialized");

            //Set our thread priority to low
            Thread.CurrentThread.Priority = ThreadPriority.Lowest;
            _log.Info("Thread priority adjusted");
            
            _sleepTime = new TimeSpan(0, 0, 1); // Set the sleep to 1 second
            _log.Info("Sleep time set");

            MovingAverage.Initialise(336); //Set the moving average to 14 days
            _log.Info("Moving average distribution functions calculated");

            _ctxManager = new ObjectContextManager();
            Provider.DbCtx = _ctxManager;
            _log.Info("InsideWord Database object context manager initialized");

            return true;
        }

        static public int MainLoop()
        {
            DateTime maxVoteTime;
            long articleId;
            List<ProviderArticleVote> voteList;
            
            DateTime highestCutOffTime = ProviderArticleScore.GetMaxSystemEditDate();
            DateTime lastRecordedAction = highestCutOffTime;

            //loop for all eternity
            while (_continue)
            {
                IEnumerable<IGrouping<long, ProviderArticleVote>> voteGroups = ProviderArticleVote.LoadBySystemEditDateTime(highestCutOffTime)
                                                                                    .GroupBy(vote => vote.ArticleId);
                foreach (IGrouping<long, ProviderArticleVote> aVoteGroup in voteGroups)
                {
                    _log.Info("======== Working on scores for articleId " + aVoteGroup.Key);
                    articleId = aVoteGroup.Key;
                    voteList = aVoteGroup.ToList();

                    ComputeSparrowScores(articleId, voteList);
                    ComputePublicVoteSums(articleId, voteList);
                    ComputeCommentSum(articleId);

                    // adjust the next cut off time
                    maxVoteTime = voteList.Max(aVote => aVote.SystemEditDate);
                    if (highestCutOffTime.CompareTo(maxVoteTime) < 0)
                    {
                        highestCutOffTime = maxVoteTime;
                    }
                    _log.Info("======== Done working on scores for articleId " + aVoteGroup.Key);
                }

                if (MovingAverage.HasIncremented(lastRecordedAction))
                {
                    lastRecordedAction = DateTime.UtcNow;
                    SparrowScoreMassDecay();
                }

                _ctxManager.DisposeCtx();
                Thread.Sleep(_sleepTime);
            }

            return 0;
        }

        static public void ComputeSparrowScores(long articleId, List<ProviderArticleVote> voteList)
        {
            ProviderArticleVote aVote = voteList[0];
            if (!MovingAverage.IsValidDate(aVote.ArticleSystemCreateDate))
            {
                _log.Info("Votes for article id " + articleId + " ignored; Passed date");
            }
            else
            {
                double weight = 0.0;
                double change = 0.0;
                ProviderArticleScore aScore = new ProviderArticleScore();
                aScore.LoadOrCreate(articleId, ProviderArticleScore.ScoreTypeEnum.SparrowRank, 1.0);
                weight = MovingAverage.CumWeight(aVote.ArticleSystemCreateDate, aScore.SystemEditDate, DateTime.UtcNow);
                change = (aScore.Score * (1 - weight)) + MovingAverage.CumWeight(aVote.ArticleSystemCreateDate) * SparrowSum(voteList);
                aScore.Score += change;
                aScore.Save();

                _log.Info("Sparrow Score - " + String.Format("{0:f}", aScore.Score) + " score / "
                                    + String.Format("{0:f}", change) + " change / "
                                    + voteList.Count + " votes / "
                                    + weight + " weight");
            }
        }

        static public double SparrowSum(List<ProviderArticleVote> voteList)
        {
            double sum = 0.0;
            foreach (ProviderArticleVote aVote in voteList)
            {
                if (aVote.IsDownVote)
                {
                    sum -= 1.0;
                }
                else if (aVote.IsUpVote)
                {
                    sum += 1.5;
                }
                else if (aVote.IsShadowVote)
                {
                    sum += 0.1;
                }
            }
            return sum;
        }

        static public void SparrowScoreMassDecay()
        {
            //Cause time decay on the score of EVERY article and delete those articles that have
            //passed the moving average date
            string logText = "";
            double weight = 0.0;
            List<ProviderArticleScore> scoreList = ProviderArticleScore.LoadBy(ProviderArticleScore.ScoreTypeEnum.SparrowRank);
            foreach (ProviderArticleScore aScore in scoreList)
            {
                if (MovingAverage.IsValidDate(aScore.ArticleSystemCreateDate))
                {
                    if (aScore.Score == 0.0)
                    {
                        // avoid a computation whose result wont change
                        logText = "Mass Decay (" + aScore.Id + ") - Skipping, score is 0.0";
                    }
                    else
                    {
                        weight = MovingAverage.CumWeight(aScore.ArticleSystemCreateDate, aScore.SystemEditDate, DateTime.UtcNow);
                        if (weight == 1.0)
                        {
                            // avoid a computation whose result wont change
                            logText = "Mass Decay (" + aScore.Id + ") - Skipping, weight is 1.0";
                        }
                        else
                        {
                            aScore.Score *= weight;
                            aScore.Save();
                            logText = "Mass Decay (" + aScore.Id + ") - " + String.Format("{0:f}", aScore.Score) + " / " + weight + " weight";
                        }
                    }
                }
                else
                {
                    logText = "Mass Decay (" + aScore.Id + ") - deleted";
                    aScore.Delete();
                }

                _log.Info(logText);
            }
        }

        static public void ComputePublicVoteSums(long articleId, List<ProviderArticleVote> voteList)
        {
            double change = 0.0;
            ProviderArticleScore aScore = new ProviderArticleScore();
            aScore.LoadOrCreate(articleId, ProviderArticleScore.ScoreTypeEnum.PublicVoteSum, 0.0);
            change = PublicVoteSum(voteList);
            aScore.Score += change;
            aScore.Save();

            _log.Info("Public Vote sum Score - " + String.Format("{0:f}", aScore.Score) + " score / "
                                + String.Format("{0:f}", change) + " change / "
                                + voteList.Count + " votes");
        }

        static public double PublicVoteSum(List<ProviderArticleVote> voteList)
        {
            double sum = 0.0;
            foreach (ProviderArticleVote aVote in voteList)
            {
                if (aVote.IsDownVote)
                {
                    sum -= 1.0;
                }
                else if (aVote.IsUpVote)
                {
                    sum += 1.0;
                }
                else if (aVote.IsShadowVote)
                {
                    sum += 0.0;
                }
            }
            return sum;
        }

        static public void ComputeCommentSum(long articleId)
        {
            ProviderArticleScore aScore = new ProviderArticleScore();
            aScore.LoadOrCreate(articleId, ProviderArticleScore.ScoreTypeEnum.CommentSum, 0.0);
            List<ProviderConversation> conversationList = ProviderConversation.LoadByArticleId(articleId);
            if(conversationList.Count == 0)
            {
                aScore.Score = 0;
            }
            else
            {
                aScore.Score = conversationList.Sum(aConversation => aConversation.CommentCount);
            }
            aScore.Save();

            _log.Info("Comment Sum Score - " + String.Format("{0:f}", aScore.Score) + " score");
        }

        static public bool ConsoleCtrlCheck(CtrlTypes ctrlType)
        {
            switch (ctrlType)
            {
                case CtrlTypes.CTRL_C_EVENT:
                case CtrlTypes.CTRL_BREAK_EVENT:
                case CtrlTypes.CTRL_CLOSE_EVENT:
                case CtrlTypes.CTRL_LOGOFF_EVENT:
                case CtrlTypes.CTRL_SHUTDOWN_EVENT:
                    _continue = false;
                    break;
            }
            return true;
        }

        #region unmanaged
        // Declare the SetConsoleCtrlHandler function
        // as external and receiving a delegate.
        [DllImport("Kernel32")]
        public static extern bool SetConsoleCtrlHandler(HandlerRoutine Handler, bool Add);

        // A delegate type to be used as the handler routine
        // for SetConsoleCtrlHandler.
        public delegate bool HandlerRoutine(CtrlTypes CtrlType);

        // An enumerated type for the control messages
        // sent to the handler routine.
        public enum CtrlTypes
        {
            CTRL_C_EVENT = 0,
            CTRL_BREAK_EVENT,
            CTRL_CLOSE_EVENT,
            CTRL_LOGOFF_EVENT = 5,
            CTRL_SHUTDOWN_EVENT
        }
        #endregion

    }
}
