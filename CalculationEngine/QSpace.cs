using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StochasticControl
{
    public class QSpace
    {
        public double QMin { private set; get; }
        public double QMax { private set; get; }
        public double DeltaQMin { private set; get; }
        public double DeltaQMax { private set; get; }
        public int NbStepsQ { private set; get; }
        public double Dq { private set; get; }

        public double[] Qs => Enumerable.Range(0, NbStepsQ).Select(iQ => QMin + iQ * Dq).ToArray();

        public int Q(double _Q)
        {
            if (_Q < QMin || _Q > QMax)
                throw new Exception("Q value not in range!");

            for(int iQ = 0; iQ < NbStepsQ; iQ++)
            {
                if (QMin + iQ * Dq <= _Q && _Q < QMin + (iQ + 1) * Dq)
                    return iQ;
            }

            throw new Exception("Error while finding correspondiung index for Q value!");
        }

        public QSpace(double _QMin, double _QMax, double _DeltaQMin, double _DeltaQMax, int nbStepsQ)
        {
            QMin = _QMin;
            QMax = _QMax;
            DeltaQMin = _DeltaQMin;
            DeltaQMax = _DeltaQMax;
            NbStepsQ = nbStepsQ;
            Dq = (_QMax - _QMin) / nbStepsQ;
        }
    }
}
