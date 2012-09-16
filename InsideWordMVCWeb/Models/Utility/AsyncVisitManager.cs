using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Timers;
using System.Net;

namespace InsideWordMVCWeb.Models.Utility
{
    static public class AsyncVisitManager
    {
        //=========================================================
        // STATIC FUNCTIONS
        //=========================================================
        static private Dictionary<Timer, List<string>> _execTimerList;

        static AsyncVisitManager()
        {
            _execTimerList = new Dictionary<Timer, List<string>>();
        }

        static public void Add(Uri pathUri, int secDelay)
        {
            InsideWordWebLog.Instance.Log.Debug("AsyncVisitManager.Add(" + pathUri.AbsoluteUri + ", " + secDelay + ")");
            Timer execTimer = new Timer(secDelay*1000);
            execTimer.Elapsed += new ElapsedEventHandler(CallBackVisit);
            execTimer.AutoReset = false;

            List<string> args = new List<string>();
            args.Add(pathUri.AbsoluteUri);
            _execTimerList.Add(execTimer, args);

            // enable the timer last otherwise it can fire-off before we store it in the dictionary
            execTimer.Enabled = true;
        }

        static public void CallBackVisit(object caller, ElapsedEventArgs args)
        {
            InsideWordWebLog.Instance.Log.Debug("AsyncVisitManager.CallBackVisit(caller, args)");
            Timer execTimer = (Timer)caller;
            List<string> timerArgs = new List<string>();
            if (_execTimerList.TryGetValue(execTimer, out timerArgs))
            {
                _execTimerList.Remove(execTimer);
                execTimer.Dispose();

                string pathUri = timerArgs[0];
                InsideWordWebLog.Instance.Log.Debug("AsyncVisitManager.CallBackVisit - visit request path = " + pathUri);
                try
                {
                    using (WebClient client = new WebClient())
                    {
                        // TODO: DOS attack is possible here by sending us to a page with a gig of data.
                        // put some sort of precautionary check here to avoid loading too much data.
                        byte[] ignoreData = client.DownloadData(pathUri);
                        // download the data and throw it away. We are only interested in causing a page visit here
                        // we don't actually care about it's content.
                    }
                    InsideWordWebLog.Instance.Log.Info("AsyncVisitManager.CallBackVisit - Success! Visit on " + pathUri);
                }
                catch (Exception caughtException)
                {
                    InsideWordWebLog.Instance.Log.Error("Exception while performing a visit request at page "+pathUri+": ", caughtException);
                }
            }
        }
    }
}