using System;
using System.Collections.Generic;
using System.Text;

namespace Helpers
{
    public static class SolveGammaHedge
    {
        public static (double, double, double, double, double) Solve(double delta1Call1, 
            double delta2Call2, double delta1Spread, double delta2Spread,
            double gamma11Call1, double gamma22Call2, double gamma11Exchange, double gamma22Exchange,
            double gamma12Exchange, 
            double delta1V, double delta2V, double gamma11V, double gamma22V, double gamma12V)
        {
            double weightS1;
            double weightS2;
            double weightCall1;
            double weightCall2;
            double weightExchange;

            weightExchange = gamma12V / gamma12Exchange;
            weightCall1 = (gamma11V - weightExchange * gamma11Exchange) / gamma11Call1;
            weightCall2 = (gamma22V - weightExchange * gamma22Exchange) / gamma22Call2;

            weightS1 = delta1V - weightCall1 * delta1Call1 - weightExchange * delta1Spread;
            weightS2 = delta2V - weightCall2 * delta2Call2 - weightExchange * delta2Spread;

            return (weightS1, weightS2, weightCall1, weightCall2, weightExchange);
        }

        public static (double, double, double, double, double) Delta(double delta1Call1,
            double delta2Call2, double delta1Exchange, double delta2Exchange,
            double gamma11Call1, double gamma22Call2, double gamma11Exchange, double gamma22Exchange,
            double gamma12Exchange,
            double weightS1, double weightS2, double weightCall1, double weightCall2, double weightExchange)
        {
            double delta1V;
            double delta2V;
            double gamma11V;
            double gamma22V;
            double gamma12V;

            delta1V = weightS1 + weightCall1 * delta1Call1 + weightExchange * delta1Exchange;
            delta2V = weightS2 + weightCall2 * delta2Call2 + weightExchange * delta2Exchange;
            gamma11V = weightCall1 * gamma11Call1 + weightExchange * gamma11Exchange;
            gamma22V = weightCall2 * gamma22Call2 + weightExchange * gamma22Exchange;
            gamma12V = weightExchange * gamma12Exchange;
            
            return (delta1V, delta2V, gamma11V, gamma22V, gamma12V);
        }

        public static (double, double, double, double) Solve(double delta1Call1,
            double delta2Call2, double gamma11Call1, double gamma22Call2,
            double delta1V, double delta2V, double gamma11V, double gamma22V)
        {
            double weightS1;
            double weightS2;
            double weightCall1;
            double weightCall2;

            weightCall1 = gamma11V / gamma11Call1;
            weightCall2 = gamma22V / gamma22Call2;

            weightS1 = delta1V - weightCall1 * delta1Call1;
            weightS2 = delta2V - weightCall2 * delta2Call2;

            return (weightS1, weightS2, weightCall1, weightCall2);
        }

        public static (double, double, double, double) Delta(double delta1Call1, double delta2Call2,
            double gamma11Call1, double gamma22Call2, 
            double weightS1, double weightS2, double weightCall1, double weightCall2)
        {
            double delta1V;
            double delta2V;
            double gamma11V;
            double gamma22V;

            delta1V = weightS1 + weightCall1 * delta1Call1;
            delta2V = weightS2 + weightCall2 * delta2Call2;
            gamma11V = weightCall1 * gamma11Call1;
            gamma22V = weightCall2 * gamma22Call2;

            return (delta1V, delta2V, gamma11V, gamma22V);
        }
    }
}
