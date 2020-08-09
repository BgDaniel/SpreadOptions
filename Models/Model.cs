using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Models
{
    public class BaseModel
    {
        protected double[] m_S0;
        private double m_T;
        protected int m_nbTimes;
        protected double m_dt;
        protected double m_sqrtDt;
        protected double[] m_times;
        protected int m_nbSimus;

        public BaseModel(double[] S0, double T, int nbTimes, int nbSimus)
        {
            m_S0 = S0;            
            m_T = T;
            m_nbTimes = nbTimes;
            m_dt = T / nbTimes;
            m_sqrtDt = Math.Sqrt(m_dt);
            m_times = Enumerable.Range(0, m_nbTimes).Select(iT => iT * m_dt).ToArray();

            m_nbSimus = nbSimus;
        }
    }
}
