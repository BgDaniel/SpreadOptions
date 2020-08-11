using CalculationEngine;
using MathNet.Numerics.Distributions;
using Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Instruments
{
    public class EuropeanCall
    {
        private double m_K;
        private double m_N;

        private double m_sigma;
        private double m_r;

        public EuropeanCall(double _K, double _N, double sigma, double r)
        {
            m_K = _K;
            m_N = _N;
            m_sigma = sigma;
            m_r = r;
        }

        public double Delta(double S, double s)
        {
            return m_N * Normal.CDF(.0, 1.0, D1(S, m_K, m_r, m_sigma, s));
        }

        public double Gamma(double S, double s)
        {
            return m_N * Normal.PDF(.0, 1.0, D1(S, m_K, m_r, m_sigma, s)) / (S * m_sigma * Math.Sqrt(s));
        }

        private static double D1(double S, double K, double r, double sigma, double s)
        {
            return (Math.Log(S / K) + (r + .5 * sigma * sigma) * s) / (sigma * Math.Sqrt(s));
        }

        private static double D2(double S, double K, double r, double sigma, double s)
        {
            return D1(S, K, r, sigma, s) - sigma * Math.Sqrt(s);
        }

        public double Value(double S)
        {
            return m_N * Math.Max(S - m_K, .0);
        }
    }
}
