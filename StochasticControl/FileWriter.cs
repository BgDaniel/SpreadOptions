using CalculationEngine;
using FileHelpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;

namespace StochasticControl
{
    public static class FileWriter
    {
        /*
        public static void WriteToFile(QStep[] qSteps, string fileName)
        {
            var engine = new FileHelperEngine<QStep>();

            string workingDirectory = Environment.CurrentDirectory;
            string projectDirectory = Directory.GetParent(workingDirectory).Parent.Parent.FullName;

            var fullPath = Path.Combine(new string[] { projectDirectory, "Data", fileName });
            
            if (File.Exists(fullPath))
                File.Delete(fullPath);

            engine.HeaderText = engine.GetFileHeader();
            engine.WriteFile(fullPath, qSteps);
        }
        */
    }
}
