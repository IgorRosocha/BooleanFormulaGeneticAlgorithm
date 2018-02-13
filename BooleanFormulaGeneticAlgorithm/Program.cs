using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace BooleanFormulaGeneticAlgorithm
{
    static class MainClass
    {
        public static void Main(string[] args)
        {
            // if wrong number of arguments is given, print help
            if (args.Length < 6)
            {
                PrintHelp();
            }

            // variables used as parameters for Genetic Algorithm
            int POPULATION_SIZE = Int32.Parse(args[1]);
            int GENERATIONS = Int32.Parse(args[2]);
            double MUTATION_PROBABILITY = Double.Parse(args[3]);
            int TOURNAMENT_SIZE = Int32.Parse(args[4]);
            int NUMBER_OF_ELITES = Int32.Parse(args[5]);

            double totalTime = 0;

            // generate random weights of Boolean Formula variables and write them into DIMACS .cnf file/files
            WeightsGenerator generator = new WeightsGenerator();
            string path = args[0];
            generator.WriteWeights(path);

            // initialization of Genetic Algorithm with parameters given as arguments
            var GeneticAlgorithm = new GeneticAlgorithm(POPULATION_SIZE, GENERATIONS, MUTATION_PROBABILITY, TOURNAMENT_SIZE, NUMBER_OF_ELITES);

            // if specified path is path to single DIMACS .cnf file, solve it; else solve all of the DIMCAS .cnf files in given directory
            if (Path.HasExtension(path))
            {
                var formula = DIMACSFormatLoader.LoadDIMACSFile(path);
                Console.WriteLine("\n### SOLVING FORMULA: {0} ###", Path.GetFileName(path));
                var sw = Stopwatch.StartNew();
                GeneticAlgorithm.SolveFormula(formula);
                Console.WriteLine("TIME OF GENETIC ALGORITHM: " + sw.ElapsedMilliseconds + " ms");
            }
            else {
                var sw = new Stopwatch();
                int i = 0;
                string[] files = Directory.GetFiles(path, "*.cnf");

                foreach (var formula in DIMACSFormatLoader.LoadFormulas(path))
                {
                    Console.WriteLine("\n### SOLVING FORMULA: {0} ###", Path.GetFileName(files[i]));
                    sw.Start();
                    var solution = GeneticAlgorithm.SolveFormula(formula);
                    sw.Stop();
                    totalTime += sw.ElapsedMilliseconds;
                    Console.WriteLine("TIME OF GENETIC ALGORITHM: " + sw.ElapsedMilliseconds + " ms"); 
                    sw.Reset();
                    i++;
                }
                Console.WriteLine("AVERAGE TIME OF GENETIC ALGORITHM: " + totalTime / i + " ms");             
            }
        }

        /// <summary>
        /// Method to print help (when wrong number of arguments is given).
        /// </summary>
        public static void PrintHelp(){
            var sb = new StringBuilder();
            sb.AppendLine("###############################################################################");
            sb.AppendLine("# Implementation of Genetic Algorithm to solve Weighted 3SAT Boolean Formulas #");
            sb.AppendLine("###############################################################################");
            sb.AppendLine();
            sb.AppendLine("Usage:");
            sb.AppendLine();
            sb.AppendLine("path population_size generations mutation_probability tournament_size number_of_elites");
            sb.AppendLine();
            sb.AppendLine("Parameters:");
            sb.AppendLine();
            sb.AppendLine("path - Path of DIMACS .cnf file / directory containing multiple DIMACS .cnf files.");
            sb.AppendLine("population_size - Size of the population.");
            sb.AppendLine("generations - Number of generations.");
            sb.AppendLine("mutation_probability - Probability of mutation.");
            sb.AppendLine("tournament_size - Size of the tournament.");
            sb.AppendLine("number_of_elites - Number of fittest organisms to carry over from current generation to the next one).");
            sb.AppendLine();
            sb.AppendLine("Note: All of the parameters are required!");
            Console.WriteLine(sb);
            Environment.Exit(1);
        }
    }
}
