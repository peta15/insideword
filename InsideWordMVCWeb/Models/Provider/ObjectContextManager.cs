using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using InsideWordProvider;

namespace InsideWordMVCWeb.Models.WebProvider
{
    public class ObjectContextManager : IObjectContextManager
    {
        protected Dictionary<int, InsideWordEntities> _ctxThreadDict = null;

        public InsideWordEntities Instance
        {
            get
            {
                InsideWordEntities _ctx = null;
                if(HttpContext.Current != null)
                {
                    string ocKey = "ocm_" + HttpContext.Current.GetHashCode().ToString("x");
                    if (!HttpContext.Current.Items.Contains(ocKey))
                    {
                        InsideWordEntities dbCtx = new InsideWordEntities();
                        HttpContext.Current.Items.Add(ocKey, dbCtx);
                    }
                    _ctx = HttpContext.Current.Items[ocKey] as InsideWordEntities;
                }
                else
                {
                    if (_ctxThreadDict == null)
                    {
                        _ctxThreadDict = new Dictionary<int, InsideWordEntities>();
                    }
                    int threadId = System.Threading.Thread.CurrentThread.ManagedThreadId;
                    if (!_ctxThreadDict.TryGetValue(threadId, out _ctx))
                    {
                        _ctx = new InsideWordEntities();
                        _ctxThreadDict[threadId] = _ctx;
                    }
                }

                return _ctx;
            }
        }

        public void DisposeCtx()
        {
            if (_ctxThreadDict != null)
            {
                InsideWordEntities _ctx = null;
                if (_ctxThreadDict.TryGetValue(System.Threading.Thread.CurrentThread.ManagedThreadId, out _ctx))
                {
                    _ctxThreadDict.Remove(System.Threading.Thread.CurrentThread.ManagedThreadId);
                    _ctx.Dispose();
                }
            }
        }
    }
}