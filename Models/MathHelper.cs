using System;
using System.Collections.Generic;
using System.Text;

namespace Models
{
    public static class MathHelper
    {
        public static double D1(double S, double K, double r, double sigma, double s)
        {
            return (Math.Log(S / K) + (r + .5 * sigma * sigma) * s) / (sigma *Math.Sqrt(s));
        }

        public static double D2(double S, double K, double r, double sigma, double s)
        {
            return D1(S, K, r, sigma, s) - sigma * Math.Sqrt(s);
        }
    }
}
