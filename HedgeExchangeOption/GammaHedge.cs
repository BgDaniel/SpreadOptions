using Hedging;
using Helpers;
using Instruments;
using System;
using System.ComponentModel.Design;
using System.Linq;

namespace HedgeExchangeOption
{
    public class GammaHedge : DeltaHedge
    {
        private const double EPSILON = 1e-5;

        private EuropeanCall m_call1;
        private EuropeanCall m_call2;
        private ExchangeOption m_exchangeOptionForHedge;

        private double[][] m_weightsCall1;
        private double[][] m_weightsCall2;
        private double[][] m_weightsExchange;

        private bool m_simpleHedge;

        public GammaHedge(EuropeanCall call1, EuropeanCall call2, ExchangeOption exchange, 
            double notionalExchange, double sigma1, double sigma2,
            double rho, int nbSimus, double[] t, int[] subIndices, double T, double[][][] underlyings,
            double r, bool simpleHedge=true) : base(notionalExchange, sigma1, sigma2, rho, nbSimus, t, subIndices, T, underlyings, r)
        {
            m_call1 = call1;
            m_call2 = call2;

            m_exchangeOptionForHedge = exchange;
            
            m_weightsCall1 = new double[nbSimus][];
            m_weightsCall2 = new double[nbSimus][];
            m_weightsExchange = new double[nbSimus][];

            for (int iSimu = 0; iSimu < m_nbSimus; iSimu++)
            {
                m_weightsCall1[iSimu] = new double[m_nbTimes];
                m_weightsCall2[iSimu] = new double[m_nbTimes];
                m_weightsExchange[iSimu] = new double[m_nbTimes];
            }

            m_simpleHedge = simpleHedge;
        }

        public override ValuePair[][] Hedge(double[][][] paths, double[] B)
        {
            m_S1 = new double[m_nbSimus][];
            m_S2 = new double[m_nbSimus][];

            m_valuesAnalytical = new double[m_nbSimus][];

            m_B = new double[m_nbTimes];

            var valuePairs = new ValuePair[m_nbSimus][];

            var value0 = m_exchangeOption.Value(paths[0][0][0], paths[0][0][1], m_T);

            for (int jTime = 0; jTime < m_nbTimes; jTime++)
            {
                m_B[jTime] = B[m_subIndices[jTime]];
            }

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

                if(m_simpleHedge)
                    RebalanceSimpleHedge(value0, iSimu, 0, m_T);
                else
                    Rebalance(value0, iSimu, 0, m_T);
            }

            for (int iSimu = 0; iSimu < m_nbSimus; iSimu++)
            {
                for (int jTime = 1; jTime < m_nbTimes; jTime++)
                {
                    var s = m_T - m_t[jTime];

                    m_valuesAnalytical[iSimu][jTime] = m_exchangeOption.Value(m_S1[iSimu][jTime], m_S2[iSimu][jTime], s);

                    // compute new hedge value
                    if (m_simpleHedge)
                        ValueSimpleHedge(iSimu, jTime, s);
                    else
                        Value(iSimu, jTime, s);

                    // rebalance portfolio
                    if (m_simpleHedge)
                        RebalanceSimpleHedge(m_valuesHedge[iSimu][jTime], iSimu, jTime, s);
                    else
                        Rebalance(m_valuesHedge[iSimu][jTime], iSimu, jTime, s);

                    valuePairs[iSimu][jTime] = new ValuePair(m_valuesHedge[iSimu][jTime], m_valuesAnalytical[iSimu][jTime]);
                }
            }

            return valuePairs;
        }

        private void Value(int iSimu, int jTime, double s)
        {
            m_valuesHedge[iSimu][jTime] =
                          m_weightsS1[iSimu][jTime - 1] * m_S1[iSimu][jTime]
                        + m_weightsS2[iSimu][jTime - 1] * m_S2[iSimu][jTime]
                        + m_weightsCall1[iSimu][jTime - 1] * m_call1.Value(m_S1[iSimu][jTime], s)
                        + m_weightsCall2[iSimu][jTime - 1] * m_call2.Value(m_S2[iSimu][jTime], s)
                        + m_weightsExchange[iSimu][jTime - 1] * m_exchangeOptionForHedge.Value(m_S1[iSimu][jTime], m_S2[iSimu][jTime], s)
                        + m_weightsB[iSimu][jTime - 1] * m_B[jTime];
        }

        private void ValueSimpleHedge(int iSimu, int jTime, double s)
        {
            m_valuesHedge[iSimu][jTime] =
                          m_weightsS1[iSimu][jTime - 1] * m_S1[iSimu][jTime]
                        + m_weightsS2[iSimu][jTime - 1] * m_S2[iSimu][jTime]
                        + m_weightsCall1[iSimu][jTime - 1] * m_call1.Value(m_S1[iSimu][jTime], s)
                        + m_weightsCall2[iSimu][jTime - 1] * m_call2.Value(m_S2[iSimu][jTime], s)
                        + m_weightsB[iSimu][jTime - 1] * m_B[jTime];
        }

        private void Rebalance(double value, int iSimu, int jTime, double s)
        {
            var delta1V = m_exchangeOption.Delta1(m_S1[iSimu][jTime], m_S2[iSimu][jTime], s);
            var delta2V = m_exchangeOption.Delta2(m_S1[iSimu][jTime], m_S2[iSimu][jTime], s);
            var gamma11V = m_exchangeOption.Gamma11(m_S1[iSimu][jTime], m_S2[iSimu][jTime], s);
            var gamma12V = m_exchangeOption.Gamma12(m_S1[iSimu][jTime], m_S2[iSimu][jTime], s);
            var gamma22V = m_exchangeOption.Gamma22(m_S1[iSimu][jTime], m_S2[iSimu][jTime], s);

            var delta1Call1 = m_call1.Delta(m_S1[iSimu][jTime], s);
            var delta2Call2 = m_call2.Delta(m_S2[iSimu][jTime], s);
            var delta1Exchange = m_exchangeOptionForHedge.Delta1(m_S1[iSimu][jTime], m_S2[iSimu][jTime], s);
            var delta2Exchange = m_exchangeOptionForHedge.Delta2(m_S1[iSimu][jTime], m_S2[iSimu][jTime], s);
            var gamma11Call1 = m_call1.Gamma(m_S1[iSimu][jTime], s);
            var gamma22Call2 = m_call2.Gamma(m_S2[iSimu][jTime], s);
            var gamma11Exchange = m_exchangeOptionForHedge.Gamma11(m_S1[iSimu][jTime], m_S2[iSimu][jTime], s);
            var gamma12Exchange = m_exchangeOptionForHedge.Gamma12(m_S1[iSimu][jTime], m_S2[iSimu][jTime], s);
            var gamma22Exchange = m_exchangeOptionForHedge.Gamma22(m_S1[iSimu][jTime], m_S2[iSimu][jTime], s);

            (
                m_weightsS1[iSimu][jTime],
                m_weightsS2[iSimu][jTime],
                m_weightsCall1[iSimu][jTime],
                m_weightsCall2[iSimu][jTime],
                m_weightsExchange[iSimu][jTime]
            ) = SolveGammaHedge.Solve(delta1Call1, delta2Call2, delta1Exchange, delta2Exchange,
                gamma11Call1, gamma22Call2, gamma11Exchange, gamma22Exchange, gamma12Exchange,
                delta1V, delta2V, gamma11V, gamma22V, gamma12V);

            m_weightsB[iSimu][jTime] = (value
                - m_weightsS1[iSimu][jTime] * m_S1[iSimu][jTime]
                - m_weightsS2[iSimu][jTime] * m_S2[iSimu][jTime]
                - m_weightsCall1[iSimu][jTime] * m_call1.Value(m_S1[iSimu][jTime], s)
                - m_weightsCall2[iSimu][jTime] * m_call2.Value(m_S2[iSimu][jTime], s)
                - m_weightsExchange[iSimu][jTime] * m_exchangeOptionForHedge.Value(m_S1[iSimu][jTime], m_S2[iSimu][jTime], s)) 
                / m_B[jTime];
        }

        private void RebalanceSimpleHedge(double value, int iSimu, int jTime, double s)
        {
            var delta1V = m_exchangeOption.Delta1(m_S1[iSimu][jTime], m_S2[iSimu][jTime], s);
            var delta2V = m_exchangeOption.Delta2(m_S1[iSimu][jTime], m_S2[iSimu][jTime], s);
            var gamma11V = m_exchangeOption.Gamma11(m_S1[iSimu][jTime], m_S2[iSimu][jTime], s);
            var gamma22V = m_exchangeOption.Gamma22(m_S1[iSimu][jTime], m_S2[iSimu][jTime], s);

            var delta1Call1 = m_call1.Delta(m_S1[iSimu][jTime], s);
            var delta2Call2 = m_call2.Delta(m_S2[iSimu][jTime], s);
            var gamma11Call1 = m_call1.Gamma(m_S1[iSimu][jTime], s);
            var gamma22Call2 = m_call2.Gamma(m_S2[iSimu][jTime], s);

            (
                m_weightsS1[iSimu][jTime],
                m_weightsS2[iSimu][jTime],
                m_weightsCall1[iSimu][jTime],
                m_weightsCall2[iSimu][jTime]
            ) = SolveGammaHedge.Solve(delta1Call1, delta2Call2,
                gamma11Call1, gamma22Call2,
                delta1V, delta2V, gamma11V, gamma22V);

            m_weightsB[iSimu][jTime] = (value
                - m_weightsS1[iSimu][jTime] * m_S1[iSimu][jTime]
                - m_weightsS2[iSimu][jTime] * m_S2[iSimu][jTime]
                - m_weightsCall1[iSimu][jTime] * m_call1.Value(m_S1[iSimu][jTime], s)
                - m_weightsCall2[iSimu][jTime] * m_call2.Value(m_S2[iSimu][jTime], s)) / m_B[jTime];
        }
    }
}
