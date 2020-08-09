using System;

namespace Instruments
{
    public abstract class Instrument
    {
        public abstract double Value(double[] S);

        public abstract double[] Deltas(double[] S);
    }
}
