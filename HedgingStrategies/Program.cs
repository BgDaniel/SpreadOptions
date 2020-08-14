using CalculationEngine;
using FileHelpers;
using HedgeExchangeOption;
using Hedging;
using Helpers;
using Instruments;
using Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Net.WebSockets;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace HedgingStrategies
{
    class Program
    {
        static void Main(string[] args)
        {
            var S0 = new double[] { 1.0, 1.0 };
            var T = 1.0;
            var nbTimes = 1000;
            var nbSimus = 10000;

            var nbSubGrid = 100;
            var subGrid = Enumerable.Range(0, nbSubGrid).Select(iSubTime => iSubTime * T / nbSubGrid).ToArray();
            var subIndices = Enumerable.Range(0, nbSubGrid).Select(iSubTime => (int)(iSubTime * nbTimes / nbSubGrid)).ToArray();

            var r = .04;
            var drift = new double[] { r, r };

            var sigma1 = .35;
            var sigma2 = .25;
            var rho = -.65;

            var vola = new double[][] { 
                new double[] { sigma1 * sigma1, rho * sigma1 * sigma2}, 
                new double[] { rho * sigma1 * sigma2, sigma2 * sigma2 } };

            var gbm = new GeometricBrownian(r, S0, T, nbTimes, nbSimus, drift, vola);
            (var paths, var B) = gbm.Simulate();

            var notionalExchange = 1.0;
            var K1 = 1.0;
            var K2 = 1.0;

            var deltaHedgeSpreadOption = new DeltaHedge(notionalExchange, sigma1, sigma2, rho,
                nbSimus, subGrid, subIndices, T, paths, r);

            var valuePairsDelta = deltaHedgeSpreadOption.Hedge(paths, B);

            // write values for first path to csv file
            FileWriter.WriteToFile(valuePairsDelta[0], "delta_hedge_along_path.csv");

            // write mean values to csv file
            FileWriter.WriteToFile(TrackingError.Calculate(valuePairsDelta), "delta_hedge_tracking_errors.csv");

            var call1 = new EuropeanCall(S0[0], 1.0, sigma1, r);
            var call2 = new EuropeanCall(S0[1], 1.0, sigma2, r);
            var sigma = Math.Sqrt(sigma1 * sigma1 + sigma2 * sigma2 - 2.0 * rho * sigma1 * sigma2);
            var exchange = new ExchangeOption(1.0, sigma);

            var gammaHedgeSpreadOption = new GammaHedge(call1, call2, exchange, notionalExchange, sigma1, sigma2, rho,
                nbSimus, subGrid, subIndices, T, paths, r);

            var valuePairsGamma = gammaHedgeSpreadOption.Hedge(paths, B);

            // write values for first path to csv file
            FileWriter.WriteToFile(valuePairsGamma[0], "gamma_hedge_along_path.csv");

            // write mean values to csv file
            FileWriter.WriteToFile(TrackingError.Calculate(valuePairsGamma), "gamma_hedge_tracking_errors.csv");

        }
    }
}
