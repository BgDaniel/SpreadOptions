using CalculationEngine;
using FileHelpers;
using HedgeExchangeOption;
using Hedging;
using Helpers;
using Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace StochasticControl
{
    class Program
    {
        static void Main(string[] args)
        {
            var S0 = new double[] { 1.0, 1.0 };
            var T = 2.0;
            var nbTimes = 200;
            var nbSimus = 1000;

            var nbSubGrid = 50;
            var subGrid = Enumerable.Range(0, nbSubGrid).Select(iSubTime => iSubTime * T / nbSubGrid).ToArray();

            var r = .02;
            var drift = new double[] { r, r };

            var sigma1 = .35;
            var sigma2 = .4;
            var rho = -.1;

            var vola = new double[][] { new double[] { sigma1, rho }, new double[] { rho, sigma2 } };            

            var gbm = new GeometricBrownian(r, S0, T, nbTimes, nbSimus, drift, vola);
            (var paths, var B) = gbm.Simulate();

            var notionalExchange = 1.0;
            var K1 = 1.0;
            var K2 = 1.0;

            var deltaHedgeSpreadOption = new DeltaHedge(notionalExchange, sigma1, sigma2, K1, K2, rho,
                nbSimus, subGrid, T, paths, gbm.Dt, r);

            var valuePairs = deltaHedgeSpreadOption.Hedge(paths, B);

            // write values for first path to csv file
            FileWriter.WriteToFile(valuePairs[0], "value_hedge_analytical.csv");

            // write mean values to csv file
            FileWriter.WriteToFile(ValuePair.Mean(valuePairs), "value_hedge_analytical_mean.csv");

            // write tracking error to csv file
            FileWriter.WriteToFile(ValuePair.Mean(valuePairs), "value_hedge_analytical_mean.csv");
        }
    }
}
