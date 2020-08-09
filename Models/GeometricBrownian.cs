using MathNet.Numerics.Differentiation;
using MathNet.Numerics.Distributions;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Factorization;
using Models;
using System;
using System.Text;


namespace CalculationEngine
{
    public class GeometricBrownian : BaseModel, IStochModel
    {
        private double[] m_drift;
        private double[][] m_volStructure;
        private int m_nbUnderlyings;

        public int NbTimes => m_nbTimes;

        public int NbSimus => m_nbSimus;

        public GeometricBrownian(double[] S0, double T, int nbTimes, int nbSimus, double[] drift, double[][] vola) 
            : base(S0, T, nbTimes, nbSimus)
        {
            m_drift = drift;
            m_volStructure = vola;
            m_nbUnderlyings = S0.Length;
        }        

        public double[][][] Simulate()
        {
            var paths = new double[NbSimus][][];
            var dWt = new double[NbSimus][][];
            var dZt = new double[NbSimus][][];

            var normal = new Normal();

            var volStructure = Matrix<double>.Build.DenseOfColumnArrays(m_volStructure);
            var cholesky = volStructure.Cholesky();
            var L = cholesky.Factor;

            for (int iSimu = 0; iSimu < NbSimus; iSimu++)
            {
                paths[iSimu] = new double[NbTimes][];
                dWt[iSimu] = new double[NbTimes][];
                dZt[iSimu] = new double[NbTimes][];

                for (int jTime = 0; jTime < NbTimes; jTime++)
                {
                    paths[iSimu][jTime] = new double[m_nbUnderlyings];
                    dWt[iSimu][jTime] = new double[m_nbUnderlyings];

                    for (int kUnd = 0; kUnd < m_nbUnderlyings; kUnd++)
                        dWt[iSimu][jTime][kUnd] = normal.Sample();

                    dZt[iSimu][jTime] = (L * Vector<double>.Build.DenseOfArray(dWt[iSimu][jTime])).ToArray();
                }                    

                for (int iUnd = 0; iUnd < m_nbUnderlyings; iUnd++)
                    paths[iSimu][0][iUnd] = m_S0[iUnd];

                for (int jTime = 1; jTime < NbTimes; jTime++)
                {
                    for (int kUnd = 0; kUnd < m_nbUnderlyings; kUnd++)
                        paths[iSimu][jTime][kUnd] = paths[iSimu][jTime - 1][kUnd] * (1.0 + m_drift[kUnd] * m_dt + dZt[iSimu][jTime][kUnd] * m_sqrtDt);                                           
                }
            }            

            return paths;
        }
    }
}
