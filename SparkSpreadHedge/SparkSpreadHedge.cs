using Hedging;
using Helpers;
using Instruments;
using System;

namespace DynamicStaticHedging
{
    public class SparkSpreadHedge : IHedge
    {
        protected ExchangeOption m_exchangeOption;

        protected double[] m_t;

        protected double m_notionalExchange;
        protected double m_T;

        protected double[][] m_weightsP;
        protected double[][] m_weightsG;
        protected double[][] m_weightsB;

        protected double[][] m_P;
        protected double[][] m_G;

        protected double[][] m_valuesHedge;
        protected double[][] m_valuesAnalytical;

        protected int m_nbSimus;
        protected int m_nbTimes;

        protected int[] m_subIndicesP;
        protected int[] m_subIndicesG;
        protected int[] m_subIndices;

        protected double[][][] m_underlyings;

        public SparkSpreadHedge(double notionalExchange, double sigmaH,
            int nbSimus, int[] subIndicesP, int[] subIndicesG, int[] subIndices,
            double T, double[] t, double[][][] underlyings)
        {
            m_exchangeOption = new ExchangeOption(notionalExchange, sigmaH);

            m_nbSimus = nbSimus;

            m_T = T;
            m_t = t;

            m_subIndicesP = subIndicesP;
            m_subIndicesG = subIndicesG;            
            m_subIndices = subIndices;

            m_nbTimes = subIndices.Length;

            m_weightsP = new double[nbSimus][];
            m_weightsG = new double[nbSimus][];
            m_weightsB = new double[nbSimus][];

            m_valuesHedge = new double[nbSimus][];

            for (int iSimu = 0; iSimu < m_nbSimus; iSimu++)
            {
                m_weightsP[iSimu] = new double[m_nbTimes];
                m_weightsG[iSimu] = new double[m_nbTimes];
                m_weightsB[iSimu] = new double[m_nbTimes];
                m_valuesHedge[iSimu] = new double[m_nbTimes];
            }

            m_underlyings = underlyings;
        }

        public virtual ValuePair[][] Hedge(double[][][] paths, double[] B)
        {
            m_P = new double[m_nbSimus][];
            m_G = new double[m_nbSimus][];

            m_valuesAnalytical = new double[m_nbSimus][];

            var valuePairs = new ValuePair[m_nbSimus][];

            var value0 = m_exchangeOption.Value(paths[0][0][1], paths[0][0][0], m_T);

            for (int iSimu = 0; iSimu < m_nbSimus; iSimu++)
            {
                m_valuesHedge[iSimu] = new double[m_nbTimes];
                m_valuesAnalytical[iSimu] = new double[m_nbTimes];
                valuePairs[iSimu] = new ValuePair[m_nbTimes];
                m_valuesHedge[iSimu][0] = value0;
                m_valuesAnalytical[iSimu][0] = value0;

                m_P[iSimu] = new double[m_nbTimes];
                m_G[iSimu] = new double[m_nbTimes];

                for (int jTime = 0; jTime < m_nbTimes; jTime++)
                {
                    m_P[iSimu][jTime] = paths[iSimu][m_subIndicesP[jTime]][0];
                    m_G[iSimu][jTime] = paths[iSimu][m_subIndicesG[jTime]][1];
                    valuePairs[iSimu][jTime] = new ValuePair(value0, value0);
                }

                m_weightsP[iSimu][0] = m_exchangeOption.Delta2(m_G[iSimu][0], m_P[iSimu][0], m_T);
                m_weightsG[iSimu][0] = m_exchangeOption.Delta1(m_G[iSimu][0], m_P[iSimu][0], m_T);
                m_weightsB[iSimu][0] = value0 - m_weightsP[iSimu][0] * m_P[iSimu][0] - m_weightsG[iSimu][0] * m_G[iSimu][0];
            }

            for (int iSimu = 0; iSimu < m_nbSimus; iSimu++)
            {
                for (int jTime = 1; jTime < m_nbTimes; jTime++)
                {
                    var s = m_T - m_t[m_subIndices[jTime]];

                    m_valuesAnalytical[iSimu][jTime] = m_exchangeOption.Value(m_G[iSimu][jTime], m_P[iSimu][jTime], s);

                    // new value
                    m_valuesHedge[iSimu][jTime] = m_weightsP[iSimu][jTime - 1] * m_P[iSimu][jTime]
                        + m_weightsG[iSimu][jTime - 1] * m_G[iSimu][jTime];

                    // rebalance
                    var sP = m_T - m_t[m_subIndicesP[jTime]];
                    var sG = m_T - m_t[m_subIndicesG[jTime]];
                    m_weightsP[iSimu][jTime] = m_exchangeOption.Delta2(m_G[iSimu][jTime], m_P[iSimu][jTime], sP);
                    m_weightsG[iSimu][jTime] = m_exchangeOption.Delta1(m_G[iSimu][jTime], m_P[iSimu][jTime], sG);
                    m_weightsB[iSimu][0] = m_valuesHedge[iSimu][jTime] - m_weightsP[iSimu][jTime] * m_P[iSimu][jTime] - m_weightsG[iSimu][jTime] * m_G[iSimu][jTime];

                    valuePairs[iSimu][jTime] = new ValuePair(m_valuesHedge[iSimu][jTime], m_valuesAnalytical[iSimu][jTime]);
                }
            }

            return valuePairs;
        }
    }
}
