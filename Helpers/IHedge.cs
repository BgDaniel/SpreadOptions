using Hedging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Helpers
{
    public interface IHedge
    {
        ValuePair[][] Hedge(double[][][] paths, double[] B);
    }
}
