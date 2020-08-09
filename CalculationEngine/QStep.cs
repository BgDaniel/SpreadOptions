using FileHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace CalculationEngine
{
    [DelimitedRecord(";")]
    public class QStep
    {
        public double Value;
        public double Q;
        public double Dq;
        public double S;       

        public QStep() { }

        public QStep(double value, double _Q, double dQ, double _S)
        {
            Value = value;
            Q = _Q;
            Dq = dQ;
            S = _S;
        }
    }
}
