using CalculationEngine;
using FileHelpers;
using Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Reflection;
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
            var drift = new double[] { .01, 0.02 };
            var vola = new double[][] { new double[] { .35, -.1 }, new double[] { -.1, .4 } };

            var gbm = new GeometricBrownian(S0, T, nbTimes, nbSimus, drift, vola);
            var paths = gbm.Simulate();
        }
    }
}
