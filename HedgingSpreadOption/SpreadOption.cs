using System;
using System.Collections.Generic;
using System.Text;

namespace HedgingSpreadOption
{
    public class SpreadOption
    {
        protected double m_K;

        public SpreadOption(double k)
        {
            m_K = k;
        }
    }
}
