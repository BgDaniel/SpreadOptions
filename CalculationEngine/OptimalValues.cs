using System;
using System.Collections.Generic;
using System.Text;

namespace CalculationEngine
{
    public struct OptimalValues
    {
        public double Value;
        public double Q;
        public double QNext;
        public int QIndexNext;
        public double Dq;            

        public OptimalValues(double value, double _Q, double _QNext, int _QNextIndex, double dq)
        {
            Value = value;
            Q = _Q;
            QNext = _QNext;
            QIndexNext = _QNextIndex;
            Dq = dq;
        }
    }
}
