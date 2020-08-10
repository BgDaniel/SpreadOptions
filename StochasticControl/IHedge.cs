using System;
using System.Collections.Generic;
using System.Text;

namespace Hedging
{
    public interface IHedge
    {
        ValuePair[] Hedge();
    }
}
