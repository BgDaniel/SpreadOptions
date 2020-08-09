using FileHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace CalculationEngine
{
    [DelimitedRecord(";")]
    public class ValueQ
    {
        double Value;
        double Q;

        public ValueQ()
        {

        }

        public ValueQ(double value, double q)
        {
            Value = value;
            Q = q;
        }
    }
}
