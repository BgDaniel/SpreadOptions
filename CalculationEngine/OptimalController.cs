using MathNet.Numerics.Distributions;
using StochasticControl;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Text;

namespace CalculationEngine
{
    public class OptimalController
    {     
        private double m_QMin;
        private double m_QMax;
        private double m_qMin;
        private double m_qMax;
        private int m_nbStepsQ;
        private double m_dQ;
        private IStochModel m_model;
        private QSpace m_qSpace;
        private OptimalValues[][][] m_optimalValues;

        public int[][] PathIndices { private set; get; }

        public double Q(int iQ)
        {
            return m_QMin + iQ * m_dQ; 
        }

        public ValueQ[] ValueQs => Enumerable.Range(0, m_qSpace.NbStepsQ).Select(iQ => new ValueQ(m_optimalValues[0][iQ][0].Value, m_QMin + iQ * m_dQ)).ToArray();

        public OptimalController(IStochModel model, QSpace qSpace)
        {
            m_model = model;

            m_qSpace = qSpace;
            m_QMax = qSpace.QMax;
            m_QMin = qSpace.QMin;
            m_qMax = qSpace.DeltaQMax;
            m_qMin = qSpace.DeltaQMin;
            m_dQ = qSpace.Dq;
            m_nbStepsQ = qSpace.NbStepsQ;
        }

        public void Control()
        {
            int nbTimes = m_model.NbTimes;
            m_optimalValues = new OptimalValues[nbTimes][][];

            if (m_model.Grid == null)
                m_model.RollOutGrid();

            var grid = m_model.Grid;

            for (int iTime = 0; iTime < nbTimes; iTime++)
            {
                m_optimalValues[iTime] = new OptimalValues[m_nbStepsQ][];
                
                var nbS = grid[iTime].Length;

                for (int jQ = 0; jQ < m_nbStepsQ; jQ++)
                    m_optimalValues[iTime][jQ] = new OptimalValues[nbS];                
            }

            var nbSLast = grid[nbTimes - 1].Length;

            for (int jQ = 0; jQ < m_nbStepsQ; jQ++)
            {
                for (int iS = 0; iS < nbSLast; iS++)
                    m_optimalValues[nbTimes - 1][jQ][iS] = new OptimalValues(.0, .0, .0, -1, .0);                
            }
            
            for (int iTime = nbTimes - 2; iTime > -1; iTime--)
            {
                for (int jS = 0; jS < grid[iTime].Length; jS++)
                {
                    (var transitionProb, var sNext) = m_model.TransitionProb(iTime, jS);

                    for (int kQ = 0; kQ < m_nbStepsQ; kQ++)
                    {
                        var nextQs = new List<int>();

                        for (int lQ = 0; lQ < m_nbStepsQ; lQ++)
                        {
                            if (m_qMin <= (lQ - kQ) * m_dQ && (lQ - kQ) * m_dQ <= m_qMax)
                                nextQs.Add(lQ);                            
                        }

                        var optimalStep = new OptimalValues(double.MinValue, .0, .0, -1, .0);

                        foreach (var nextQ in nextQs)
                        {
                            var expectation = .0;

                            for(int l = 0; l < sNext.Length; l++)
                                expectation += transitionProb[l] * m_optimalValues[iTime + 1][nextQ][sNext[l]].Value;

                            var value_next = - (nextQ - kQ)  * m_dQ * grid[iTime][jS] + expectation;
                            
                            if (value_next > optimalStep.Value)
                                optimalStep = new OptimalValues(value_next, m_QMin + kQ * m_dQ, m_QMin + nextQ * m_dQ, nextQ, (nextQ - kQ) * m_dQ);                                                       
                        }

                        m_optimalValues[iTime][kQ][jS] = optimalStep;
                    }
                }
            }
        }

        public List<QStep[]> RollOut(List<int> iPaths, double Q0)
        {
            var qSteps = new List<QStep[]>();

            foreach (var iPath in iPaths)
                qSteps.Add(RollOut(iPath, Q0));

            return qSteps;
        }

        public QStep[] RollOut(int iPath, double Q0)
        {
            var nbTimes = m_model.NbTimes;
            var qSteps = new QStep[nbTimes];

            double value;
            double Q = Q0;
            var indexQ = m_qSpace.Q(Q0);

            double dQ;
            var pathIndices = m_model.PathIndices[iPath];

            for(int iTime = 0; iTime < nbTimes; iTime++)
            {
                value = m_optimalValues[iTime][indexQ][pathIndices[iTime]].Value;
                
                if(iTime != 0)
                    Q = m_optimalValues[iTime][indexQ][pathIndices[iTime]].Q;

                dQ = m_optimalValues[iTime][indexQ][pathIndices[iTime]].Dq;

                indexQ = m_optimalValues[iTime][indexQ][pathIndices[iTime]].QIndexNext;

                qSteps[iTime] = new QStep(value, Q, dQ, m_model.Paths[iPath][iTime]);
            }

            return qSteps;
        }
    }
}
