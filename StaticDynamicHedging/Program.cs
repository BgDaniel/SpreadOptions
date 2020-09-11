using DynamicStaticHedging;
using HedgeExchangeOption;
using Models;
using System;
using System.Linq;

namespace StaticDynamicHedging
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
            var nbSimus = 10000;

            var nbSubGrid = 100;
            var subGrid = Enumerable.Range(0, nbSubGrid).Select(iSubTime => iSubTime * T / nbSubGrid).ToArray();
            var subIndices = Enumerable.Range(0, nbSubGrid).Select(iSubTime => (int)(iSubTime * nbTimes / nbSubGrid)).ToArray();

            var r = .02;
            var sigmaG = .3;
            var sigmaH = .25;
            var kappa = 1.2;
            var rho = .6;

            var sparkSpread = new SparkSpreadModel(r, sigmaG, sigmaH, kappa, rho, S0, T, nbTimes, nbSimus);
            (var paths, var B) = sparkSpread.Simulate();

            // check if underlyings are martingales with respect to risk neutral measure
            var check = true;

            if(check)
            {
                var expectationP = new double[nbTimes];
                var expectationG = new double[nbTimes];

                for (int jTime = 1; jTime < nbTimes; jTime++)
                {
                    for (int iSimu = 0; iSimu < nbSimus; iSimu++)
                    {
                        expectationP[jTime] += paths[iSimu][jTime][0];
                        expectationG[jTime] += paths[iSimu][jTime][1];
                    }

                    expectationP[jTime] /= nbSimus;
                    expectationG[jTime] /= nbSimus;

                    if (Math.Abs(expectationP[jTime]) > TOLERANCE)
                        throw new Exception("Deviation too high!");

                    if (Math.Abs(expectationG[jTime]) > TOLERANCE)
                        throw new Exception("Deviation too high!");
                }
            }
                        

            // exchange option (P_T - G_T)^+
            var notionalExchange = 1.0;
            var K1 = P0;
            var K2 = G0;


            /*
            var nbSubGridP = 100;
            var subGridP = Enumerable.Range(0, nbSubGridP).Select(iSubTime => iSubTime * T / nbSubGridP).ToArray();
            var subIndicesP = Enumerable.Range(0, nbSubGridP).Select(iSubTime => (int)(iSubTime * nbTimes / nbSubGridP)).ToArray();

            var nbSubGridG = 100;
            var subGridG = Enumerable.Range(0, nbSubGridG).Select(iSubTime => iSubTime * T / nbSubGridG).ToArray();
            var subIndicesG = Enumerable.Range(0, nbSubGridG).Select(iSubTime => (int)(iSubTime * nbTimes / nbSubGridG)).ToArray();

            if (nbSubGridP % nbSubGridG != 0 && nbSubGridG % nbSubGridP != 0)
                throw new Exception("Grids are not comparable!");

            var subGridCommon = nbSubGridP >= nbSubGridG ? subGridP : subGridG;
            var subIndicesCommon = nbSubGridP >= nbSubGridG ? subIndicesP : subIndicesG;

            var deltaHedgeSpreadOption = new SparkSpreadHedge(notionalExchange, sigmaH,
                nbSimus, subGridP, subIndicesP, subGridG, subIndicesG, subGridCommon,
                subIndicesCommon, T, paths);

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
            */
        }
    }
}
