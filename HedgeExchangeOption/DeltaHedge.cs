using Hedging;
using Instruments;
using System;
using System.Linq;

namespace HedgeExchangeOption
{
    public class DeltaHedge : IHedge
    {
        protected ExchangeOption m_exchangeOption;

        protected EuropeanCall m_call1;
        protected EuropeanCall m_call2;

        protected double[] m_B;
        protected double[] m_t;

        protected double m_notionalExchange;
        protected double m_T;
        protected double m_r;
        protected double m_sigma1;
        protected double m_sigma2;
        protected double m_K1;
        protected double m_k2;
        protected double m_rho;
        protected double m_sigma;

        protected double[][] m_weights1;
        protected double[][] m_weights2;

        protected double[][] m_S1;
        protected double[][] m_S2;

        protected double[][] m_weightsB;
        protected double[][] m_valuesHedge;
        protected double[][] m_valuesAnalytical;

        protected int m_nbSimus;
        protected int m_nbTimes;

        protected int[] m_subIndices;

        protected double[][][] m_underlyings;

        public DeltaHedge(double notionalExchange, double sigma1, double sigma2, double _K1, double _K2, 
            double rho, int nbSimus, double[] t, int[] subIndices, double T, double[][][] underlyings, double dt, 
            double r)
        {
            m_notionalExchange = notionalExchange;

            m_r = r;

            m_sigma1 = sigma1;
            m_sigma2 = sigma2;
            m_rho = rho;
            m_sigma = Math.Sqrt(m_sigma1 * m_sigma1 + m_sigma2 * m_sigma2 - 2.0 * m_rho * m_sigma1 * m_sigma2);

            m_exchangeOption = new ExchangeOption(m_notionalExchange, m_sigma);

            m_K1 = _K1;
            m_k2 = _K2;

            m_call1 = new EuropeanCall(_K1, m_notionalExchange, m_sigma1, m_r);
            m_call2 = new EuropeanCall(_K2, m_notionalExchange, m_sigma2, m_r);
            m_nbSimus = nbSimus;

            m_T = T;

            m_t = t;
            m_nbTimes = m_t.Length;

            m_subIndices = subIndices;

            m_weights1 = new double[nbSimus][];
            m_weights2 = new double[nbSimus][];
            m_weightsB = new double[nbSimus][];

            m_valuesHedge = new double[nbSimus][];

            for(int iSimu = 0; iSimu < m_nbSimus; iSimu++)
            {
                m_weights1[iSimu] = new double[m_nbTimes];
                m_weights2[iSimu] = new double[m_nbTimes];
                m_weightsB[iSimu] = new double[m_nbTimes];
                m_valuesHedge[iSimu] = new double[m_nbTimes];
            }
        
            m_underlyings = underlyings;
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

                m_weights1[iSimu][0] = m_exchangeOption.Delta1(m_S1[iSimu][0], m_S2[iSimu][0], m_T);
                m_weights2[iSimu][0] = m_exchangeOption.Delta2(m_S1[iSimu][0], m_S2[iSimu][0], m_T);

                m_weightsB[iSimu][0] = (value0 - m_weights1[iSimu][0] * m_S1[iSimu][0]
                    - m_weights2[iSimu][0] * m_S2[iSimu][0]) / m_B[0];
            }

            for (int iSimu = 0; iSimu < m_nbSimus; iSimu++)
            {
                for (int jTime = 1; jTime < m_nbTimes; jTime++)
                {
                    var s = m_T - m_t[jTime];

                    m_valuesAnalytical[iSimu][jTime] = m_exchangeOption.Value(m_S1[iSimu][jTime], m_S2[iSimu][jTime], s);

                    // new value
                    m_valuesHedge[iSimu][jTime] = m_weights1[iSimu][jTime - 1] * m_S1[iSimu][jTime]
                        + m_weights2[iSimu][jTime - 1] * m_S2[iSimu][jTime]
                        + m_weightsB[iSimu][jTime - 1] * m_B[jTime];                    

                    // rebalance
                    m_weights1[iSimu][jTime] = m_exchangeOption.Delta1(m_S1[iSimu][jTime], m_S2[iSimu][jTime], s);
                    m_weights2[iSimu][jTime] = m_exchangeOption.Delta2(m_S1[iSimu][jTime], m_S2[iSimu][jTime], s);
                    m_weightsB[iSimu][jTime] = (m_valuesHedge[iSimu][jTime] - m_weights1[iSimu][jTime] * m_S1[iSimu][jTime]
                        - m_weights2[iSimu][jTime] * m_S2[iSimu][jTime]) / m_B[jTime];

                    valuePairs[iSimu][jTime] = new ValuePair(m_valuesHedge[iSimu][jTime], m_valuesAnalytical[iSimu][jTime]);
                }
            }

            return valuePairs;
        }
    }
}
