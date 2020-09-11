using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Models
{
    public class GridFactory
    {
        private int m_nbTimes;
        private int m_nbTimesP;
        private int m_nbTimesG;
        private int m_nbTimesC;

        private double[] m_timesP;
        private double[] m_timesG;

        private bool m_PMeshMoreDenseThanGMesh;

        private double m_T;

        public GridFactory(int nbTimes, int nbTimesP, int nbTimesG, double T)
        {
            m_nbTimes = nbTimes;
            m_nbTimesP = nbTimesP;
            m_nbTimesG = nbTimesG;

            m_nbTimesC = Math.Max(m_nbTimesP, m_nbTimesG);
            m_PMeshMoreDenseThanGMesh = m_nbTimesP > m_nbTimesG;

            if (m_nbTimesP > m_nbTimesG)
            {
                if (m_nbTimesP % m_nbTimesG != 0)
                    throw new Exception("Grids are not comparable!");
            }
            else
            {
                if (m_nbTimesG % m_nbTimesP != 0)
                    throw new Exception("Grids are not comparable!");
            }

            m_timesP = new double[m_nbTimesC];
            m_timesG = new double[m_nbTimesC];

            m_T = T;
        }

        public (int[], double[], int[], double[]) Build()
        {
            double[] subGridP = null;
            int[] subIndicesP = null;
            double[] subGridG = null;
            int[] subIndicesG = null;

            if (m_nbTimesP == m_nbTimesG)
            {
                subGridP = Enumerable.Range(0, m_nbTimesP).Select(iSubTime => iSubTime * m_T / m_nbTimesP).ToArray();
                subIndicesP = Enumerable.Range(0, m_nbTimesP).Select(iSubTime => (int)(iSubTime * m_nbTimes / m_nbTimesP)).ToArray();
                subGridG = subGridP;
                subIndicesG = subIndicesP;
                return (subIndicesP, subGridP, subIndicesG, subGridG);
            }

            if (m_PMeshMoreDenseThanGMesh)
            {
                subGridP = Enumerable.Range(0, m_nbTimesP).Select(iSubTime => iSubTime * m_T / m_nbTimesP).ToArray();
                subIndicesP = Enumerable.Range(0, m_nbTimesP).Select(iSubTime => (int)(iSubTime * m_nbTimes / m_nbTimesP)).ToArray();

                subGridG = new double[m_nbTimesP];
                subIndicesG = new int[m_nbTimesP];

                for (int iTime = 0; iTime < m_nbTimesP; iTime++)
                {
                    subGridG[iTime] = subGridP[(iTime / (m_nbTimesP / m_nbTimesG)) * (m_nbTimesP / m_nbTimesG)];
                    subIndicesG[iTime] = subIndicesP[(iTime / (m_nbTimesP / m_nbTimesG)) * (m_nbTimesP / m_nbTimesG)];
                }            
            }
            else
            {
                subGridG = Enumerable.Range(0, m_nbTimesG).Select(iSubTime => iSubTime * m_T / m_nbTimesG).ToArray();
                subIndicesG = Enumerable.Range(0, m_nbTimesG).Select(iSubTime => (int)(iSubTime * m_nbTimes / m_nbTimesG)).ToArray();

                subGridP = new double[m_nbTimesG];
                subIndicesP = new int[m_nbTimesG];

                for (int iTime = 0; iTime < m_nbTimesG; iTime++)
                {
                    subGridP[iTime] = subGridG[(iTime / (m_nbTimesG / m_nbTimesP)) * (m_nbTimesG / m_nbTimesP)];
                    subIndicesP[iTime] = subIndicesG[(iTime / (m_nbTimesG / m_nbTimesP)) * (m_nbTimesG / m_nbTimesP)];
                }
            }

            return (subIndicesP, subGridP, subIndicesG, subGridG);
        }
    }
}
