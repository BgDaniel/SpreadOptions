using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Text;

namespace CalculationEngine
{
    public interface IStochModel
    {
        int NbTimes { get; }
        int NbSimus { get; }
        double[][][] Simulate();
    }
}
