using System;
using System.IO;
using System.Linq;

namespace BooleanFormulaGeneticAlgorithm
{
    /// <summary>
    /// Class to generate and write the weights of Boolean Formula variables.
    /// </summary>
    public class WeightsGenerator
    {

        static readonly Random random = new Random();

        /// <summary>
        /// Method to differ between writing into one DIMACS .cnf file or multiple.
        /// </summary>
        /// <param name="path">Path of DIMACS .cnf file/s.</param>
        public void WriteWeights(string path)
        {
            if (!Directory.Exists(path) && !File.Exists(path))
            {
                Console.WriteLine("[ERROR] Specified directory, which should" +
                                  " contain DIMACS format files to generate " +
                                  "weights, doesn't exist!");
                Environment.Exit(1);
                return;
            }

            if (Path.GetFileName(path) == "")
            {
                foreach (var file in Directory.EnumerateFiles(path, "*.cnf"))
                {
                    WriteToFile(file);
                }
            } else {
                WriteToFile(path);
            }
        }

        /// <summary>
        /// Function to generate random weights Boolean Formula variables.
        /// </summary>
        /// <param name="count">Number of Boolean Formula variables.</param>
        private static int[] GenerateRandomWeights(int count)
        {
            var weights = new int[count];
            for (int i = 0; i < count; i++)
            {
                weights[i] = random.Next(1, count + 1);
            }
            return weights;
        }

        /// <summary>
        /// Method to write weights of Boolean Formula variables into DIMACS .cnf file.
        /// </summary>
        /// <param name="file">Path of DIMACS .cnf file.</param>
        public void WriteToFile(string file){
            var lines = File.ReadAllLines(file).ToList();
            for (int i = 0; i < lines.Count; i++)
            {
                var line = lines[i];

                if (line.StartsWith("c"))
                    continue;

                if (line.StartsWith("w"))
                    break;

                if (line.StartsWith("p"))
                {
                    var formulaValues = line.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries)
                                             .Skip(2)
                                             .Select(int.Parse)
                                             .ToArray();

                    int nbvar = formulaValues[0];
                    var weightsLine = $"w {string.Join(" ", GenerateRandomWeights(nbvar))}";
                    lines.Insert(i, weightsLine);
                    File.WriteAllLines(file, lines);
                    break;
                }
            }
        }
    }
}
