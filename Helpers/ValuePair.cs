using FileHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hedging
{
    [DelimitedRecord(";")]
    public class ValuePair
    {
        public double ValueHedge;
        public double ValueAnalytical;

        public ValuePair()
        {
            ValueHedge = .0;
            ValueAnalytical = .0;
        }

        public ValuePair(double valueHedge, double valueAnalytical)
        {
            ValueHedge = valueHedge;
            ValueAnalytical = valueAnalytical;
        }

        public static ValuePair[] Mean(ValuePair[][] valuePairs)
        {
            int nbSimus = valuePairs.Length;
            int nbTimes = valuePairs[0].Length;
            var valuePairsMean = new ValuePair[nbTimes];

            for (int iTime = 0; iTime < nbTimes; iTime++)
            {
                var meanValueHedge = .0;
                var meanValueAnalytical = .0;

                for (int jSimu = 0; jSimu < nbSimus; jSimu++)
                {
                    meanValueHedge += valuePairs[jSimu][iTime].ValueHedge / nbSimus;
                    meanValueAnalytical += valuePairs[jSimu][iTime].ValueAnalytical/ nbSimus;
                }

                valuePairsMean[iTime] = new ValuePair(meanValueHedge, meanValueAnalytical);
            }

            return valuePairsMean;
        }
    }
}
