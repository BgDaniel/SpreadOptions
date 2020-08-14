using FileHelpers;
using Hedging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Helpers
{
    [DelimitedRecord(";")]
    public class TrackingError
    {
        public double TrackingErr;

        public TrackingError()
        {

        }

        public TrackingError(double tackingError)
        {
            TrackingErr = tackingError;
        }

        public static TrackingError[] Calculate(ValuePair[][] valuePairs)
        {
            int nbSimus = valuePairs.Length;
            int nbTimes = valuePairs[0].Length;
            var trackingErrors = new TrackingError[nbTimes];

            for (int iTime = 0; iTime < nbTimes; iTime++)
            {
                var trackingError = .0;

                for (int jSimu = 0; jSimu < nbSimus; jSimu++)
                {
                    trackingError += (valuePairs[jSimu][iTime].ValueHedge - valuePairs[jSimu][iTime].ValueAnalytical)
                        * (valuePairs[jSimu][iTime].ValueHedge - valuePairs[jSimu][iTime].ValueAnalytical) / (nbSimus - 1);

                    if (trackingError > .0)
                       break;
                }                    

                trackingErrors[iTime] = new TrackingError(Math.Sqrt(trackingError));
            }

            return trackingErrors;
        }
    }
}
