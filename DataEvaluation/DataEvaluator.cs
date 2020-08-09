using CalculationEngine;
using System;
using System.Collections.Generic;

namespace DataEvaluation
{
    public class DataEvaluator
    {
        public List<QStep[]> QSteps;

        public IMode QSteps;

        public DataEvaluator(List<QStep[]> qSteps)
        {
            QSteps = qSteps;
        }
         
        public void Evaluate()
        {

        }
    }
}
