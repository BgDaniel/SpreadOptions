using Helpers;
using MathNet.Numerics.Distributions;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Complex;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace UnitTests
{
    [TestClass]
    public class MathHelperUnitTtests
    {
        public const int NUMBER_SAMPLES = 10000;
        public const double THRESHOLD = 1e-5;

        [TestMethod]
        public void TestUpperTriangularInversion()
        {
            var listA = GenerateRandomMatrices(2, 2, NUMBER_SAMPLES, false);
            var listB = GenerateRandomMatrices(2, 2, NUMBER_SAMPLES, false);
            var listC = GenerateRandomMatrices(2, 2, NUMBER_SAMPLES, true);
            var listD = GenerateRandomMatrices(2, 2, NUMBER_SAMPLES, false);
            var listE = GenerateRandomMatrices(2, 2, NUMBER_SAMPLES, true);

            var listY = GenerateRandomArrays(6, NUMBER_SAMPLES);

            for (int i = 0; i < NUMBER_SAMPLES; i++)
            {
                var tms = new TriangularMatrixSolver(listA[i], listB[i], listC[i], listD[i], listE[i], 2);
                var x = tms.Solve(listY[i]);

                var _y = tms.Map(x);
                var y = Vector<double>.Build.DenseOfArray(_y);
                var ySolution = Vector<double>.Build.DenseOfArray(listY[i]);

                var deviation = Math.Sqrt((y - ySolution).L2Norm());

                if (deviation >= THRESHOLD)
                    throw new Exception("Deviation too high!");
            }
        }

        [TestMethod]
        public void TestSolveGammaHedge()
        {
            for (int i = 0; i < NUMBER_SAMPLES; i++)
            {
                var normal = new Normal();

                var delta1Call1 = normal.Sample();
                var delta2Call2 = normal.Sample();
                var delta1Exchange = normal.Sample();
                var delta2Exchange = normal.Sample();
                var gamma11Call1 = normal.Sample();
                var gamma22Call2 = normal.Sample();
                var gamma11Exchange = normal.Sample();
                var gamma22Exchange = normal.Sample();
                var gamma12Exchange = normal.Sample();
                var delta1V = normal.Sample();
                var delta2V = normal.Sample();
                var gamma11V = normal.Sample();
                var gamma22V = normal.Sample();
                var gamma12V = normal.Sample();

                (var weightS1, var weightS2, var weightCall1, var weightCall2, var weightExchange) =
                    SolveGammaHedge.Solve(delta1Call1, delta2Call2, delta1Exchange, delta2Exchange, gamma11Call1,
                    gamma22Call2, gamma11Exchange, gamma22Exchange, gamma12Exchange,
                    delta1V, delta2V, gamma11V, gamma22V, gamma12V);

                (var _delta1V, var _delta2V, var _gamma11V, var _gamma22V, var _gamma12V) =
                    SolveGammaHedge.Delta(delta1Call1, delta2Call2, delta1Exchange, delta2Exchange, 
                    gamma11Call1, gamma22Call2, gamma11Exchange, gamma22Exchange, gamma12Exchange,
                    weightS1, weightS2, weightCall1, weightCall2, weightExchange);

                if (Math.Abs(delta1V - _delta1V) >= THRESHOLD)
                    throw new Exception("Deviation too high!");

                if (Math.Abs(delta2V - _delta2V) >= THRESHOLD)
                    throw new Exception("Deviation too high!");

                if (Math.Abs(gamma11V - _gamma11V) >= THRESHOLD)
                    throw new Exception("Deviation too high!");

                if (Math.Abs(gamma22V - _gamma22V) >= THRESHOLD)
                    throw new Exception("Deviation too high!");

                if (Math.Abs(gamma12V - _gamma12V) >= THRESHOLD)
                    throw new Exception("Deviation too high!");
            }
        }

        [TestMethod]
        public void TestSolveSimpleGammaHedge()
        {
            for (int i = 0; i < NUMBER_SAMPLES; i++)
            {
                var normal = new Normal();

                var delta1Call1 = normal.Sample();
                var delta2Call2 = normal.Sample();
                var gamma11Call1 = normal.Sample();
                var gamma22Call2 = normal.Sample();
                var delta1V = normal.Sample();
                var delta2V = normal.Sample();
                var gamma11V = normal.Sample();
                var gamma22V = normal.Sample();
                var gamma12V = normal.Sample();

                (var weightS1, var weightS2, var weightCall1, var weightCall2) =
                    SolveGammaHedge.Solve(delta1Call1, delta2Call2, gamma11Call1, gamma22Call2,
                    delta1V, delta2V, gamma11V, gamma22V);

                (var _delta1V, var _delta2V, var _gamma11V, var _gamma22V) =
                    SolveGammaHedge.Delta(delta1Call1, delta2Call2,
                    gamma11Call1, gamma22Call2,
                    weightS1, weightS2, weightCall1, weightCall2);

                if (Math.Abs(delta1V - _delta1V) >= THRESHOLD)
                    throw new Exception("Deviation too high!");

                if (Math.Abs(delta2V - _delta2V) >= THRESHOLD)
                    throw new Exception("Deviation too high!");

                if (Math.Abs(gamma11V - _gamma11V) >= THRESHOLD)
                    throw new Exception("Deviation too high!");

                if (Math.Abs(gamma22V - _gamma22V) >= THRESHOLD)
                    throw new Exception("Deviation too high!");
            }
        }

        private List<double[]> GenerateRandomArrays(int nbRows, int nbSamples)
        {
            var retValues = new List<double[]>();
            var normal = new Normal();

            for (int i = 0; i < nbSamples; i++)
            {
                var entries = new double[nbRows];

                for (int j = 0; j < nbRows; j++)
                    entries[j] = normal.Sample();

                retValues.Add(entries);
            }

            return retValues;
        }

        private List<Matrix<double>> GenerateRandomMatrices(int nbRows, int nbColumns, int nbSamples, bool invertible=false)
        {
            var retValues = new List<Matrix<double>>();
            var normal = new Normal();

            for(int i = 0; i < nbSamples; i++)
            {
                var isInvertible = false;
                Matrix<double> M = null;

                while (!isInvertible)
                {
                    var entries = new double[nbColumns, nbRows];

                    for (int j = 0; j < nbRows; j++)
                    {
                        for (int k = 0; k < nbColumns; k++)
                            entries[j, k] = normal.Sample();
                    }

                    M = Matrix<double>.Build.DenseOfArray(entries);

                    if (invertible)
                        isInvertible = M.Determinant() != .0;
                    else
                        isInvertible = true;
                }

                retValues.Add(M);
            }

            return retValues;
        }
    }
}
