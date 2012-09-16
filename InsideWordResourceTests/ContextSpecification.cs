using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InsideWordResourceTests
{
    public abstract class ContextSpecification
    {
        protected ContextSpecification()
        {
            EstablishContext();
            Because();
        }

        public abstract void EstablishContext();
        public abstract void Because();
    }
}
