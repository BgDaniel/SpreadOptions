using Hedging;
using Instruments;
using System;
using System.Linq;

namespace HedgeExchangeOption
{
    public class DeltaGammaHedge : DeltaHedge
    {
        private EuropeanCall m_call3;
        private EuropeanCall m_call4;

        private double[][] m_weightsCall3;
        private double[][] m_weightsCall4;

        public DeltaGammaHedge(double notionalExchange, double sigma1, double sigma2, double _K1, double _K2,
            double rho, int nbSimus, double[] t, int[] subIndices, double T, double[][][] underlyings, double dt,
            double r) : base(notionalExchange, sigma1, sigma2, _K1, _K2, rho, nbSimus, t, subIndices, T, underlyings, dt, r)
        {
            m_call3 = null;
            m_call4 = null;
            
            m_weightsCall3 = new double[nbSimus][];
            m_weightsCall4 = new double[nbSimus][];           

            for (int iSimu = 0; iSimu < m_nbSimus; iSimu++)
            {
                m_weightsCall3[iSimu] = new double[m_nbTimes];
                m_weightsCall4[iSimu] = new double[m_nbTimes];
            }
        }

        public new ValuePair[][] Hedge(double[][][] paths)
        {
            var valuePairs = new ValuePair[m_nbSimus][];

            var value0 = m_exchangeOption.Value(m_S1[0][0], m_S1[0][0], m_T);

            for (int iSimu = 0; iSimu < m_nbTimes; iSimu++)
            {
                m_valuesHedge[iSimu] = new double[m_nbTimes];
                valuePairs[iSimu] = new ValuePair[m_nbTimes];
                m_valuesHedge[iSimu][0] = value0;

                m_weights1[iSimu][0] = m_call1.Delta(m_S1[iSimu][0], m_T);
                m_weights1[iSimu][0] = m_call2.Delta(m_S2[iSimu][0], m_T);
                m_weights1[iSimu][0] = (m_valuesHedge[iSimu][0] - m_weights1[iSimu][0] * m_S1[iSimu][0]
                    - m_weights2[iSimu][0] * m_S2[iSimu][0]) / m_B[0];
            }

            for (int iSimu = 0; iSimu < m_nbSimus; iSimu++)
            {
                for (int jTime = 1; jTime < m_nbTimes; jTime++)
                {
                    // new value
                    m_valuesHedge[iSimu][jTime] = m_weights1[iSimu][jTime - 1] * m_S1[iSimu][jTime]
                        + m_weights2[iSimu][jTime - 1] * m_S2[iSimu][jTime]
                        + m_weightsB[iSimu][jTime - 1] * m_B[jTime];

                    var s = m_T - m_t[jTime];

                    // rebalance
                    m_weights1[iSimu][jTime] = m_call1.Delta(m_S1[iSimu][jTime], s);
                    m_weights1[iSimu][jTime] = m_call2.Delta(m_S2[iSimu][jTime], s);
                    m_weights1[iSimu][jTime] = (m_valuesHedge[iSimu][jTime] - m_weights1[iSimu][jTime] * m_S1[iSimu][jTime]
                        - m_weights2[iSimu][jTime] * m_S2[iSimu][jTime]) / m_B[jTime];
                }
            }

            return valuePairs;
        }
    }
}
