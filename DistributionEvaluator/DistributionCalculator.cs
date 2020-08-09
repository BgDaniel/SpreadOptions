using CalculationEngine;
using FileHelpers;
using StochasticControl;
using System;
using System.IO;
using System.Linq;

namespace DistributionEvaluator
{
    public class DistributionCalculator
    {
        private BinomialTree m_model;
        private QSpace m_qSpace;
        private double m_q0;
        private int m_nbStepsR;
        private int m_nbStepsSigma;

        private Tuple<double, double>[][] m_configurationSpace;
        private Tuple<double, double>[][] m_results;

        public DistributionCalculator(BinomialTree model, QSpace qSpace, double q0) 
        {
            new DistributionCalculator(model, qSpace, q0, model.R, model.R, 1, model.Sigma, model.Sigma, 1);
        }

        public DistributionCalculator(BinomialTree model, QSpace qSpace, double q0, double rMin,
            double rMax, int nbStepsR, double sigmaMin, double sigmaMax, int nbStepsSigma)
        {
            m_model = model;
            m_qSpace = qSpace;
            m_q0 = q0;
            m_nbStepsR = nbStepsR;
            m_nbStepsSigma = nbStepsSigma;

            m_configurationSpace = new Tuple<double, double>[nbStepsR][];

            var deltaR = (rMax - rMin) / m_nbStepsR;
            var deltaSigma = (sigmaMax - sigmaMin) / m_nbStepsSigma;

            for (int iR = 0; iR < m_nbStepsR; iR++)
            {
                m_configurationSpace[iR] = new Tuple<double, double>[m_nbStepsSigma];

                for (int jSigma = 0; jSigma < m_nbStepsSigma; jSigma++)
                    m_configurationSpace[iR][jSigma] = new Tuple<double, double>(rMin + iR * deltaR, sigmaMin + jSigma * deltaSigma);
            }

            m_results = new Tuple<double, double>[m_nbStepsR][];

            for (int iR = 0; iR < m_nbStepsR; iR++)
                m_results[iR] = new Tuple<double, double>[m_nbStepsSigma];            
        }

        public void Calculate()
        {
            for (int iR = 0; iR < m_nbStepsR; iR++)
            {
                for (int jSigma = 0; jSigma < m_nbStepsSigma; jSigma++)
                {
                    m_model.R = m_configurationSpace[iR][jSigma].Item1;
                    m_model.Sigma = m_configurationSpace[iR][jSigma].Item2;

                    var optimalController = new OptimalController(m_model, m_qSpace);
                    optimalController.Control();
                    
                    var valueQs = optimalController.ValueQs;
                    
                    WriteToFile(valueQs, String.Format("values0_Q_{0}_{1}.csv", m_configurationSpace[iR][jSigma].Item1,
                        m_configurationSpace[iR][jSigma].Item2));
                }
            }
        }

        private void WriteToFile(ValueQ[] valueQs, string fileName)
        {
            var engine = new FileHelperEngine<ValueQ>();

            string workingDirectory = Environment.CurrentDirectory;
            string projectDirectory = Directory.GetParent(workingDirectory).Parent.Parent.FullName;

            var fullPath = Path.Combine(new string[] { projectDirectory, "Data", fileName });

            if (File.Exists(fullPath))
                File.Delete(fullPath);

            engine.HeaderText = engine.GetFileHeader();
            engine.WriteFile(fullPath, valueQs);
        }
    }
}
