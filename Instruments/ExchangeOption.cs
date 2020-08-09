using System;
using System.Collections.Generic;
using System.Text;

namespace Instruments
{
    public class ExchangeOption : SpreadOption
    {
        public ExchangeOption() : base(.0)
        {
        }

        public override double[] Deltas(double[] S)
        {
            return base.Deltas(S);
        }
    }
}
