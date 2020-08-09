using System;
using System.Collections.Generic;
using System.Text;

namespace Instruments
{
    public class SpreadOption : Instrument
    {
        protected double m_K;

        public SpreadOption(double k)
        {
            m_K = k;
        }

        public override double[] Deltas(double[] S)
        {
            throw new NotImplementedException();
        }

        public override double Value(double[] S)
        {
            return Math.Max(S[0] - S[1] - m_K, .0);
        }
    }
}
