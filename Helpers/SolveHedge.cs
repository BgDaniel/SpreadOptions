using System;
using System.Collections.Generic;
using System.Text;

namespace Helpers
{
    public static class SolveHedge
    {
        public static (double, double, double, double, double) Solve(double delta1Call1, 
            double delta1Call2, double delta1Spread, double delta2Spread,
            double gamma11Call1, double gamma22Call2, double gamma11Spread, double gamma22Spread,
            double gamma12Spread, 
            double delta1V, double delta2V, double gamma11V, double gamma22V, double gamma12V)
        {
            double weightS1;
            double weightS2;
            double weightCall1;
            double weightCall2;
            double weightSpread;

            weightSpread = gamma12V / gamma12Spread;
            weightCall1 = (gamma11V - weightSpread * gamma11Spread) / gamma11Call1;
            weightCall2 = (gamma22V - weightSpread * gamma22Spread) / gamma22Call2;

            weightS1 = delta1V - weightCall1 * delta1Call1 - weightSpread * delta1Spread;
            weightS2 = delta2V - weightCall2 * delta1Call2 - weightSpread * delta2Spread;

            return (weightS1, weightS2, weightCall1, weightCall2, weightSpread);
        }

        public static (double, double, double, double, double) Delta(double delta1Call1,
            double delta2Call2, double delta1Spread, double delta2Spread,
            double gamma11Call1, double gamma22Call2, double gamma11Spread, double gamma22Spread,
            double gamma12Spread,
            double weightS1, double weightS2, double weightCall1, double weightCall2, double weightSpread)
        {
            double delta1V;
            double delta2V;
            double gamma11V;
            double gamma22V;
            double gamma12V;

            delta1V = weightS1 + weightCall1 * delta1Call1 + weightSpread * delta1Spread;
            delta2V = weightS2 + weightCall2 * delta2Call2 + weightSpread * delta2Spread;
            gamma11V = weightCall1 * gamma11Call1 + weightSpread * gamma11Spread;
            gamma22V = weightCall2 * gamma22Call2 + weightSpread * gamma22Spread;
            gamma12V = weightSpread * gamma12Spread;
            
            return (delta1V, delta2V, gamma11V, gamma22V, gamma12V);
        }
    }
}
