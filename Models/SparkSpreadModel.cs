using CalculationEngine;
using MathNet.Numerics.Distributions;
using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Models
{
    public class SparkSpreadModel : BaseModel, IStochModel
    {
        public int NbTimes => m_nbTimes;

        public int NbSimus => m_nbSimus;

        private double m_r;
        private double m_mu;
        private double m_sigmaG;
        private double m_sigmaH;
        private double m_kappa;
        private double m_theta;
        private double m_rho;

        private double[][] m_volStructure;

        public SparkSpreadModel(double r, double mu, double sigmaG, double sigmaH, double kappa, double theta,
            double rho, double[] S0, double T, int nbTimes, int nbSimus)
            : base(S0, T, nbTimes, nbSimus)
        {
            m_mu = mu;
            m_r = r;
            m_sigmaG = sigmaG;
            m_sigmaH = sigmaH;
            m_kappa = kappa;
            m_theta = theta;
            m_rho = rho;

            m_volStructure = new double[][] {
                new double[] { sigmaG * sigmaG, rho * sigmaG * sigmaH},
                new double[] { rho * sigmaG * sigmaH, sigmaH * sigmaH } };
        }

        public (double[][][], double[]) Simulate()
        {
            var paths_g = new double[NbSimus][];
            var paths_h = new double[NbSimus][];
            var paths_p = new double[NbSimus][];
            var paths_G = new double[NbSimus][];
            var paths_P = new double[NbSimus][];
            var paths = new double[NbSimus][][];
            var dWt = new double[NbSimus][][];
            var dZt = new double[NbSimus][][];

            var normal = new Normal();

            var volStructure = Matrix<double>.Build.DenseOfColumnArrays(m_volStructure);
            var cholesky = volStructure.Cholesky();
            var L = cholesky.Factor;

            for (int iSimu = 0; iSimu < NbSimus; iSimu++)
            {
                paths_g[iSimu] = new double[NbTimes];
                paths_h[iSimu] = new double[NbTimes];
                paths_p[iSimu] = new double[NbTimes];
                paths_G[iSimu] = new double[NbTimes];
                paths_P[iSimu] = new double[NbTimes];
                paths[iSimu] = new double[NbTimes][];

                dWt[iSimu] = new double[NbTimes][];
                dZt[iSimu] = new double[NbTimes][];

                for (int jTime = 0; jTime < NbTimes; jTime++)
                {
                    dWt[iSimu][jTime] = new double[2];
                    paths[iSimu][jTime] = new double[2];

                    for (int kUnd = 0; kUnd < 2; kUnd++)
                        dWt[iSimu][jTime][kUnd] = normal.Sample();

                    dZt[iSimu][jTime] = (L * Vector<double>.Build.DenseOfArray(dWt[iSimu][jTime]) * m_sqrtDt).ToArray();
                }

                paths_g[iSimu][0] = Math.Log(m_S0[0]);
                paths_h[iSimu][0] = Math.Log(m_S0[1]);
                paths_p[iSimu][0] = paths_g[iSimu][0] + paths_g[iSimu][1];
                paths_G[iSimu][0] = m_S0[0];
                paths_P[iSimu][0] = m_S0[0] * m_S0[1];

                paths[iSimu][0][0] = paths_P[iSimu][0];
                paths[iSimu][0][1] = paths_G[iSimu][0];

                for (int jTime = 1; jTime < NbTimes; jTime++)
                {
                    // first underlying: g driven by dg = mu * dt + sigma_g dw_g_t 
                    paths_g[iSimu][jTime] = paths_g[iSimu][jTime - 1] + m_mu * m_dt + m_sigmaG * dZt[iSimu][jTime][0];
                    paths_h[iSimu][jTime] = paths_h[iSimu][jTime - 1] + m_kappa * (m_theta - paths_h[iSimu][jTime - 1]) * m_dt
                        + m_sigmaH * dZt[iSimu][jTime][1];
                    paths_p[iSimu][jTime] = paths_g[iSimu][jTime] + paths_h[iSimu][jTime];
                    paths_G[iSimu][jTime] = Math.Exp(paths_g[iSimu][jTime]);
                    paths_P[iSimu][jTime] = Math.Exp(paths_p[iSimu][jTime]);
                    paths[iSimu][jTime][0] = paths_P[iSimu][jTime];
                    paths[iSimu][jTime][1] = paths_G[iSimu][jTime];
                }
            }

            return (paths, Enumerable.Range(0, m_nbTimes).Select(iTime => Math.Exp(m_r * iTime * m_dt)).ToArray());

        }
    }
}
