using Instruments;
using System;
using System.Collections.Generic;

namespace Hedging
{
    public class DeltaHedge
    {
        private List<Instrument> m_instruments;
        private double m_value;
        private double[] m_weights;

        public DeltaHedge(List<Instrument> instruments, double value0, double[] weights0)
        {
            m_instruments = instruments;
            m_weights = weights0;
        }
    }
}
