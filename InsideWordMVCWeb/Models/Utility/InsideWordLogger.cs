using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using log4net;
using log4net.Config;
using log4net.Repository.Hierarchy;
using log4net.Appender;
using log4net.Core;
using InsideWordProvider;
using InsideWordResource;
using System.ComponentModel;
using System.Globalization;

namespace InsideWordMVCWeb.Models.Utility
{
    public class InsideWordWebLog
    {
        protected ILog _logger;
        protected List<KeyValuePair<string, string>> _llList;
        protected SmtpAppender _smtp;
        protected Hierarchy _hier;
        protected RootLogger _root;
        protected Dictionary<string, List<string>> _buffer;
        protected BackgroundWorker _currentFlushWorker;
        protected Queue<string> _flushQueue;

        public ILog Log
        {
            get { return _logger; }
        }

        public void Buffer(string from, string message)
        {
            if (IWThreshold < log4net.Core.Level.Info)
            {
                List<string> messageList = null;
                if(_buffer.TryGetValue(from, out messageList))
                {
                    messageList.Add("["+DateTime.UtcNow.ToString(IWConstants.FullDateTime)+"] "+message);
                }
                else
                {
                    messageList = new List<string>();
                    messageList.Add("[" + DateTime.UtcNow.ToString(IWConstants.FullDateTime) + "] " + message);
                    _buffer.Add(from, messageList);
                }
                
                if(messageList.Count >= MaxBufferSize)
                {   
                    AsyncBufferFlush(from);
                }
            }
        }

        public int MaxBufferSize { get; set; }

        public void AsyncBufferFlush(string from)
        {
            lock (_flushQueue)
            {
                if (!_flushQueue.Contains(from))
                {
                    _flushQueue.Enqueue(from);
                }
            }

            if (!_currentFlushWorker.IsBusy)
            {
                _currentFlushWorker.RunWorkerAsync();
            }
        }

        public Level IWThreshold
        {
            get
            {
                return (_logger.Logger as Logger).EffectiveLevel;
            }
            set
            {
                _root.Level = value;
                _logger.Logger.Repository.Threshold = value;
                foreach (AppenderSkeleton anAppender in _hier.GetAppenders())
                {
                    anAppender.Threshold = value;
                }
            }
        }

        public Level IWSmtpThreshold
        {
            get
            {
                Level evalLevel = log4net.Core.Level.Off;
                if (_smtp == null)
                {
                    foreach (AppenderSkeleton anAppender in _hier.GetAppenders())
                    {
                        if (anAppender is log4net.Appender.SmtpAppender)
                        {
                            _smtp = anAppender as SmtpAppender;
                            evalLevel = (_smtp.Evaluator as LevelEvaluator).Threshold;
                            break;
                        }
                    }
                }
                else
                {
                    evalLevel = (_smtp.Evaluator as LevelEvaluator).Threshold;
                }

                return evalLevel;
            }
            set
            {
                if (_smtp == null)
                {
                    foreach (AppenderSkeleton anAppender in _hier.GetAppenders())
                    {
                        if (anAppender is log4net.Appender.SmtpAppender)
                        {
                            _smtp = anAppender as SmtpAppender;
                            _smtp.Evaluator = new LevelEvaluator(value);
                            break;
                        }
                    }
                }
                else
                {
                    _smtp.Evaluator = new LevelEvaluator(value);
                }
            }
        }

        public List<KeyValuePair<string, string>> LevelList
        {
            get
            {
                if (_llList == null)
                {
                    _llList = new List<KeyValuePair<string, string>>();
                    KeyValuePair<string, string> logLevel = new KeyValuePair<string, string>(Level.All.Name,
                                                                                             Level.All.DisplayName);
                    _llList.Add(logLevel);

                    logLevel = new KeyValuePair<string, string>(Level.Debug.Name,
                                                                Level.Debug.DisplayName);
                    _llList.Add(logLevel);

                    logLevel = new KeyValuePair<string, string>(Level.Info.Name,
                                                                Level.Info.DisplayName);
                    _llList.Add(logLevel);

                    logLevel = new KeyValuePair<string, string>(Level.Warn.Name,
                                                                Level.Warn.DisplayName);
                    _llList.Add(logLevel);

                    logLevel = new KeyValuePair<string, string>(Level.Error.Name,
                                                                Level.Error.DisplayName);
                    _llList.Add(logLevel);

                    logLevel = new KeyValuePair<string, string>(Level.Fatal.Name,
                                                                Level.Fatal.DisplayName);
                    _llList.Add(logLevel);

                    logLevel = new KeyValuePair<string, string>(Level.Off.Name,
                                                                Level.Off.DisplayName);
                    _llList.Add(logLevel);
                }

                return _llList;
            }
        }

        //=========================================================
        // PRIVATE
        //=========================================================
        protected InsideWordWebLog()
        {
            _logger = LogManager.GetLogger("InsideWordWeb");
            XmlConfigurator.Configure();
            _hier = _logger.Logger.Repository as Hierarchy;
            _root = _hier.Root as RootLogger;
            // Funky line, but used to normalize the root, current logger 
            // and all it's appenders to the same login threshold.
            IWThreshold = IWThreshold;
            IWSmtpThreshold = IWThreshold;
            _logger.Info("Logging initialized");
            MaxBufferSize = 128;
            _buffer = new Dictionary<string, List<string>>();
            _currentFlushWorker = new BackgroundWorker();
            _currentFlushWorker.DoWork += DelegateFlushBuffer;
            _flushQueue = new Queue<string>();
        }

        protected void DelegateFlushBuffer(object sender, DoWorkEventArgs e)
        {
            while(_flushQueue.Count > 0)
            {
                bool getResult = false;
                List<string> messageList = null;
                string from = _flushQueue.Dequeue();
                
                lock (_buffer)
                {
                    getResult = _buffer.TryGetValue(from, out messageList);
                    if (getResult)
                    {
                        _buffer.Remove(from);
                        _buffer.Add(from, new List<string>());
                    }
                }

                if (getResult)
                {
                    // keep this part out of the lock since it is the most intensive task (2sec!) and the reason we are doing this asynch to start with
                    Log.Info(from + "\n" + string.Join("\n", messageList));
                }
            }
        }

        //=========================================================
        // STATIC
        //=========================================================
        static private InsideWordWebLog _instance = null;
        static public InsideWordWebLog Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new InsideWordWebLog();
                }

                return _instance;
            }
        }

        static public bool Initialize(InsideWordSettingsDictionary settingsDict)
        {
            string logThreshold = settingsDict.LogThresholdName;
            if(!string.IsNullOrWhiteSpace(logThreshold))
            {
                Instance.IWThreshold = InsideWordWebLog.Instance.Log.Logger.Repository.LevelMap[logThreshold];
            }

            string emailLogThreshold = settingsDict.LogEmailThresholdName;
            if (!string.IsNullOrWhiteSpace(emailLogThreshold))
            {
                Instance.IWSmtpThreshold = InsideWordWebLog.Instance.Log.Logger.Repository.LevelMap[emailLogThreshold];
            }
            return true;
        }
    }
}