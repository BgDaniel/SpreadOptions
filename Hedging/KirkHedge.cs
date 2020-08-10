using Instruments;
using System;
using System.Collections.Generic;

namespace Hedging
{
    public class KirkHedge
    {
        private SpreadOption m_spreadOption;
        
        private EuropeanCall m_call1;
        private EuropeanCall m_call2;
        
        private double[] m_values;
        
        private double[] m_weights1;
        private double[] m_weights2;


    }
}
