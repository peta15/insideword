using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InsideWordProvider;

namespace SparrowRanker
{
    public class ObjectContextManager : IObjectContextManager
    {
        InsideWordEntities _ctx = null;
        public InsideWordEntities Instance
        {
            get
            {
                if (_ctx == null)
                {
                    _ctx = new InsideWordEntities();
                }
                return _ctx;
            }
        }

        public void DisposeCtx()
        {
            _ctx.Dispose();
            _ctx = null;
        }
    }
}
