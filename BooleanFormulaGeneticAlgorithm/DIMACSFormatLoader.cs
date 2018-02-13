using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BooleanFormulaGeneticAlgorithm
{
    /// <summary>
    /// Class to load the Boolean Formulas from DIMACS .cnf file.
    /// </summary>
    public class DIMACSFormatLoader
    {
        /// <summary>
        /// Function to load the Boolean Formulas from multiple DIMACS .cnf files.
        /// </summary>
        /// <param name="path">Path of directory containing multiple DIMACS .cnf files.</param>
        public static IEnumerable<Formula> LoadFormulas(string path)
        {
            if (!Directory.Exists(path))
            {
                Console.WriteLine("[ERROR] Specified directory doesn't exist!");
                Environment.Exit(1);
            }

            return Directory.EnumerateFiles(path, "*.cnf").Select(LoadDIMACSFile);
        }

        /// <summary>
        /// Function to load the Boolean Formula from single DIMACS .cnf file.
        /// </summary>
        /// <param name="filePath">Path of DIMACS .cnf file.</param>
        public static Formula LoadDIMACSFile(string filePath)
        {
            int[] weights = null;
            int[] formula = null;
            int nbvar = 0;
            int nbclauses = 0;
            int counter = 0;

            foreach (var line in File.ReadAllLines(filePath).Where(l => !string.IsNullOrWhiteSpace(l)))
            {
                var lineStart = line[0];
                switch (lineStart)
                {
                    case 'c':
                        break;
                    case 'w':
                        {
                            weights = line.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries)
                                          .Skip(1)
                                          .Select(int.Parse)
                                          .ToArray();
                            break;
                        }
                    case 'p':
                        {
                            var nb = line.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries)
                                                     .Skip(2)
                                                     .Select(int.Parse)
                                                     .ToArray();
                            nbvar = nb[0];
                            nbclauses = nb[1];

                            if (weights == null)
                            {
                                Console.WriteLine("[ERROR] No weights specified!");
                                Environment.Exit(1);
                            }

                            if (weights.Length != nbvar)
                            {
                                Console.WriteLine("[ERROR] Number of weights doesn't equal to the number of variables!");
                                Environment.Exit(1);
                            }

                            formula = new int[nbclauses * 3];
                            break;
                        }
                    default:
                        {
                            if (formula == null)
                            {
                                Console.WriteLine("[ERROR] Wrong file format! (file format has to be DIMACS)");
                                Environment.Exit(1);
                            }

                            if (counter == formula.Length)
                                break;

                            var formulaValues = line.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries)
                                                    .Select(int.Parse)
                                                    .TakeWhile(v => v != 0)
                                                    .ToArray();
                            if (formulaValues.Length != 3)
                                Console.WriteLine($"[ERROR] Wrong number of clause variables in clause {(counter == 0 ? 1 : counter / 3 + 1)}.");

                            Array.Copy(formulaValues, 0, formula, counter, 3);
                            counter += 3;
                            break;
                        }
                }
            }
            return new Formula(nbvar, nbclauses, weights, formula);
        }
    }
}
