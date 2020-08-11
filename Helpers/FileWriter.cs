using FileHelpers;
using Hedging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;

namespace Helpers
{
    public static class FileWriter
    {
        public static void WriteToFile<T>(T[] valuePairs, string fileName) where T : class
        {
            var engine = new FileHelperEngine<T>();

            string workingDirectory = Environment.CurrentDirectory;
            string projectDirectory = Directory.GetParent(workingDirectory).Parent.Parent.FullName;

            var fullPath = Path.Combine(new string[] { projectDirectory, "Data", fileName });

            if (File.Exists(fullPath))
                File.Delete(fullPath);

            engine.HeaderText = engine.GetFileHeader();
            engine.WriteFile(fullPath, valuePairs);
        }
    }
}
