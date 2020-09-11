using DynamicStaticHedging;
using Helpers;
using Models;
using System;
using System.Linq;

namespace StatDynamicHedging
{
    class Program
    {
        const double TOLERANCE = 1E-9;

        static void Main(string[] args)
        {
            var G0 = 1.0;
            var H0 = 1.0;
            var P0 = G0 * H0;

            var S0 = new double[] { G0, H0 };
            var T = 1.0;
            var nbTimes = 1000;
            var nbSimus = 5000;

            var grid = Enumerable.Range(0, nbTimes).Select(iSubTime => iSubTime * T / nbTimes).ToArray();
            
            var r = .02;
            var mu = .025;
            var sigmaG = .4;
            var sigmaH = .35;
            var kappa = 0.02;
            var theta = 1.0;
            var rho = .6;

            var sparkSpread = new SparkSpreadModel(r, mu, sigmaG, sigmaH, kappa, theta, rho, S0, T, nbTimes, nbSimus);
            (var paths, var B) = sparkSpread.Simulate();

            // exchange option (P_T - G_T)^+
            var notionalExchange = 1.0;
            var K1 = P0;
            var K2 = G0;
           
            // case 1: both dynamic
            var nbSubGridP = 1000;
            var nbSubGridG = 1000;

            var gridFactory = new GridFactory(nbTimes, nbSubGridP, nbSubGridG, T);
            (var subIndicesP, var subGridP, var subIndicesG, var subGridG) = gridFactory.Build();

            var subIndices = nbSubGridP > nbSubGridG ? subIndicesP : subIndicesG;

            var deltaHedgeSpreadOption = new SparkSpreadHedge(notionalExchange, sigmaH,
                nbSimus, subIndicesP, subIndicesG, subIndices, T, grid, paths);
            
            var valuePairsDelta = deltaHedgeSpreadOption.Hedge(paths, B);
            
            // write values for first path to csv file
            FileWriter.WriteToFile(valuePairsDelta[0], "delta_hedge_both_dynamic.csv");

            // write mean values to csv file
            FileWriter.WriteToFile(TrackingError.Calculate(valuePairsDelta), "tracking_errors_both_dynamic.csv");


            // case 2: G dynamic, P static
            nbSubGridP = 10;
            nbSubGridG = 1000;

            gridFactory = new GridFactory(nbTimes, nbSubGridP, nbSubGridG, T);
            (subIndicesP, subGridP, subIndicesG, subGridG) = gridFactory.Build();

            subIndices = nbSubGridP > nbSubGridG ? subIndicesP : subIndicesG;

            deltaHedgeSpreadOption = new SparkSpreadHedge(notionalExchange, sigmaH,
                nbSimus, subIndicesP, subIndicesG, subIndices, T, grid, paths);

            valuePairsDelta = deltaHedgeSpreadOption.Hedge(paths, B);

            // write values for first path to csv file
            FileWriter.WriteToFile(valuePairsDelta[0], "delta_hedge_G_dynamic.csv");

            // write mean values to csv file
            FileWriter.WriteToFile(TrackingError.Calculate(valuePairsDelta), "tracking_errors_G_dynamic.csv");


            // case 3: G static, P dynamic
            nbSubGridP = 1000;
            nbSubGridG = 10;

            gridFactory = new GridFactory(nbTimes, nbSubGridP, nbSubGridG, T);
            (subIndicesP, subGridP, subIndicesG, subGridG) = gridFactory.Build();

            subIndices = nbSubGridP > nbSubGridG ? subIndicesP : subIndicesG;

            deltaHedgeSpreadOption = new SparkSpreadHedge(notionalExchange, sigmaH,
                nbSimus, subIndicesP, subIndicesG, subIndices, T, grid, paths);

            valuePairsDelta = deltaHedgeSpreadOption.Hedge(paths, B);

            // write values for first path to csv file
            FileWriter.WriteToFile(valuePairsDelta[0], "delta_hedge_P_dynamic.csv");

            // write mean values to csv file
            FileWriter.WriteToFile(TrackingError.Calculate(valuePairsDelta), "tracking_errors_P_dynamic.csv");


            // case 4: G static, P static
            nbSubGridP = 10;
            nbSubGridG = 10;

            gridFactory = new GridFactory(nbTimes, nbSubGridP, nbSubGridG, T);
            (subIndicesP, subGridP, subIndicesG, subGridG) = gridFactory.Build();

            subIndices = nbSubGridP > nbSubGridG ? subIndicesP : subIndicesG;

            deltaHedgeSpreadOption = new SparkSpreadHedge(notionalExchange, sigmaH,
                nbSimus, subIndicesP, subIndicesG, subIndices, T, grid, paths);

            valuePairsDelta = deltaHedgeSpreadOption.Hedge(paths, B);

            // write values for first path to csv file
            FileWriter.WriteToFile(valuePairsDelta[0], "delta_hedge_both_static.csv");

            // write mean values to csv file
            FileWriter.WriteToFile(TrackingError.Calculate(valuePairsDelta), "tracking_errors_both_static.csv");
        }
    }
}
