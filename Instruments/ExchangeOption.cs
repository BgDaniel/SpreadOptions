using MathNet.Numerics.Distributions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Instruments
{
    public class ExchangeOption
    {
        private double m_N;
        private double m_sigma;

        public ExchangeOption(double _N, double sigma)
        {
            m_N = _N;
            m_sigma = sigma;
        }

        //Margrabe's formula
        public double Value(double S1, double S2, double s)
        {
            return m_N * (S2 * Normal.CDF(.0, 1.0, D2(S1, S2, s)) 
                - S1 * Normal.CDF(.0, 1.0, D1(S1, S2, s)));
        }

        private double D1(double S1, double S2, double s)
        {
            return (Math.Log(S2 / S1) / (m_sigma * Math.Sqrt(s)) - .5 * m_sigma * Math.Sqrt(s));
        }

        private double D2(double S1, double S2, double s)
        {
            return (Math.Log(S2 / S1) / (m_sigma * Math.Sqrt(s)) + .5 * m_sigma * Math.Sqrt(s));
        }

        public double Delta1(double S1, double S2, double s)
        {
            return - m_N * Normal.CDF(.0, 1.0, D1(S1, S2, s));
        }

        public double Delta2(double S1, double S2, double s)
        {
            return + m_N * Normal.CDF(.0, 1.0, D2(S1, S2, s));
        }

        public double Gamma11(double S1, double S2, double s)
        {
            throw new NotImplementedException();
        }

        public double Gamma22(double S1, double S2, double s)
        {
            throw new NotImplementedException();
        }

        public double Gamma12(double S1, double S2, double s)
        {
            throw new NotImplementedException();
        }
    }
}
