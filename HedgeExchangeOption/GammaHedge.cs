using Hedging;
using Instruments;
using System;
using System.Linq;

namespace HedgeExchangeOption
{
    public class GammaHedge : DeltaHedge
    {
        private EuropeanCall m_call1;
        private EuropeanCall m_call2;
        private ExchangeOption m_exchange1;
        private ExchangeOption m_exchange2;

        private double[][] m_weightsCall1;
        private double[][] m_weightsCall2;
        private double[][] m_weightsSpread1;
        private double[][] m_weightsSpread2;

        public GammaHedge(EuropeanCall call1, EuropeanCall call2, ExchangeOption exchange1, ExchangeOption exchange2, 
            double notionalExchange, double sigma1, double sigma2,
            double rho, int nbSimus, double[] t, int[] subIndices, double T, double[][][] underlyings, double dt,
            double r) : base(notionalExchange, sigma1, sigma2, rho, nbSimus, t, subIndices, T, underlyings, r)
        {
            m_call1 = call1;
            m_call2 = call2;

            m_exchange1 = exchange1;
            m_exchange1 = exchange2;
            
            m_weightsCall1 = new double[nbSimus][];
            m_weightsCall2 = new double[nbSimus][];
            m_weightsSpread1 = new double[nbSimus][];
            m_weightsSpread2 = new double[nbSimus][];

            for (int iSimu = 0; iSimu < m_nbSimus; iSimu++)
            {
                m_weightsCall1[iSimu] = new double[m_nbTimes];
                m_weightsCall2[iSimu] = new double[m_nbTimes];
                m_weightsSpread1[iSimu] = new double[m_nbTimes];
                m_weightsSpread2[iSimu] = new double[m_nbTimes];
            }
        }

        public ValuePair[][] Hedge(double[][][] paths, double[] B)
        {
            m_S1 = new double[m_nbSimus][];
            m_S2 = new double[m_nbSimus][];

            m_valuesAnalytical = new double[m_nbSimus][];

            m_B = B;

            var valuePairs = new ValuePair[m_nbSimus][];

            var value0 = m_exchangeOption.Value(paths[0][0][0], paths[0][0][1], m_T);

            for (int iSimu = 0; iSimu < m_nbSimus; iSimu++)
            {
                m_valuesHedge[iSimu] = new double[m_nbTimes];
                m_valuesAnalytical[iSimu] = new double[m_nbTimes];
                valuePairs[iSimu] = new ValuePair[m_nbTimes];
                m_valuesHedge[iSimu][0] = value0;
                m_valuesAnalytical[iSimu][0] = value0;

                m_S1[iSimu] = new double[m_nbTimes];
                m_S2[iSimu] = new double[m_nbTimes];

                for (int jTime = 0; jTime < m_nbTimes; jTime++)
                {
                    m_S1[iSimu][jTime] = paths[iSimu][m_subIndices[jTime]][0];
                    m_S2[iSimu][jTime] = paths[iSimu][m_subIndices[jTime]][1];
                    valuePairs[iSimu][jTime] = new ValuePair(value0, value0);
                }

                m_weightsS1[iSimu][0] = m_exchangeOption.Delta1(m_S1[iSimu][0], m_S2[iSimu][0], m_T);
                m_weightsS2[iSimu][0] = m_exchangeOption.Delta2(m_S1[iSimu][0], m_S2[iSimu][0], m_T);

                m_weightsB[iSimu][0] = (value0 - m_weightsS1[iSimu][0] * m_S1[iSimu][0]
                    - m_weightsS2[iSimu][0] * m_S2[iSimu][0]) / m_B[0];
            }

            for (int iSimu = 0; iSimu < m_nbSimus; iSimu++)
            {
                for (int jTime = 1; jTime < m_nbTimes; jTime++)
                {
                    var s = m_T - m_t[jTime];

                    m_valuesAnalytical[iSimu][jTime] = m_exchangeOption.Value(m_S1[iSimu][jTime], m_S2[iSimu][jTime], s);

                    // new value
                    m_valuesHedge[iSimu][jTime] = m_weightsS1[iSimu][jTime - 1] * m_S1[iSimu][jTime]
                        + m_weightsS2[iSimu][jTime - 1] * m_S2[iSimu][jTime]
                        + m_weightsB[iSimu][jTime - 1] * m_B[jTime];

                    // rebalance
                    m_weightsS1[iSimu][jTime] = m_exchangeOption.Delta1(m_S1[iSimu][jTime], m_S2[iSimu][jTime], s);
                    m_weightsS2[iSimu][jTime] = m_exchangeOption.Delta2(m_S1[iSimu][jTime], m_S2[iSimu][jTime], s);
                    m_weightsB[iSimu][jTime] = (m_valuesHedge[iSimu][jTime] - m_weightsS1[iSimu][jTime] * m_S1[iSimu][jTime]
                        - m_weightsS2[iSimu][jTime] * m_S2[iSimu][jTime]) / m_B[jTime];

                    valuePairs[iSimu][jTime] = new ValuePair(m_valuesHedge[iSimu][jTime], m_valuesAnalytical[iSimu][jTime]);
                }
            }

            return valuePairs;
        }
    }
}
