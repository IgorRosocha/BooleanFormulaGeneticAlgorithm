using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace BooleanFormulaGeneticAlgorithm
{
    /// <summary>
    /// The main Genetic Algorithm class. 
    /// </summary>
    public class GeneticAlgorithm
    {
        public int populationSize { get; } // size of the population
        public int generations { get; } // number of generations
        public double mutationProbability { get; set; } // probability of mutation
        public int tournamentSize { get; } // size of the tournament
        public int numberOfElites { get; } // number of elites (fittest organisms to carry over from current generation to the next one)

        Formula currentFormula; //current Formula
        int chromosomeLength; // length of chromosome
        public List<Chromosome> matingPool; // mating pool (list) of chromosome

        static Random random = new Random();

        /// <summary>
        /// Initializes a new instance of the <see cref="GeneticAlgorithm"/> class.
        /// </summary>
        /// <param name="populationSize">Size of the population.</param>
        /// <param name="generations">Number of generations.</param>
        /// <param name="mutationProbability">Probability of mutation.</param>
        /// <param name="tournamentSize">Size of the tournament.</param>
        /// <param name="numberOfElites">Number of elites (fittest organisms to carry over from current generation to the next one).</param>
        public GeneticAlgorithm(int populationSize, int generations, double mutationProbability, int tournamentSize, int numberOfElites)
        {
            this.populationSize = populationSize;
            this.generations = generations;
            this.mutationProbability = mutationProbability;
            this.tournamentSize = tournamentSize;
            this.numberOfElites = numberOfElites;
            matingPool = new List<Chromosome>(populationSize);
        }

        /// <summary>
        /// Function used to get the value of a Boolean Formula variables.
        /// </summary>
        /// <param name="formulaIndex">Index of formula variable.</param>
        /// <param name="chromosome">Represents the chromosome.</param>
        private bool GetNbvarValue(int formulaIndex, Chromosome chromosome)
        {
            if (currentFormula.formula[formulaIndex] < 0){
                return !chromosome.genes[Math.Abs(currentFormula.formula[formulaIndex]) - 1];
            }

            return chromosome.genes[currentFormula.formula[formulaIndex] - 1];
        }

        /// <summary>
        /// Method used to compute the fitness of chromosome. 
        /// </summary>
        /// <param name="chromosome">Represents the chromosome.</param>
        public void ComputeFitness(Chromosome chromosome)
        {
            chromosome.fitness = 0;
            for (int i = 0; i < currentFormula.formula.Length; i += 3)
            {
                var first = GetNbvarValue(i, chromosome);
                var second = GetNbvarValue(i + 1, chromosome);
                var third = GetNbvarValue(i + 2, chromosome);
                if (!(first || second || third))
                {
                    return;
                }         
            }

            for (int i = 0; i < chromosomeLength; i++)
            {
                if (chromosome.genes[i])
                    chromosome.fitness += currentFormula.weights[i];
            }
        }

        /// <summary>
        /// Function used to create a new population. 
        /// </summary>
        public Chromosome[] CreatePopulation()
        {
            var population = new Chromosome[populationSize];
            for (int i = 0; i < populationSize; i++)
            {
                var genes = new BitArray(chromosomeLength);
                for (int j = 0; j < chromosomeLength; j++)
                {
                    if (random.NextDouble() < 0.5)
                        genes.Set(j, true);
                }
                population[i] = new Chromosome(genes);
                ComputeFitness(population[i]);
            }      
            return population;
        }

        /// <summary>
        /// Function used to create a new generation.
        /// </summary>
        /// <param name="currentPopulation">Represents the current population.</param>
        /// <param name="elitesCount">Represents the count of elites.</param>
        public Chromosome[] CreateGeneration(Chromosome[] currentPopulation, out int elitesCount)
        {
            elitesCount = 0;
            var newGeneration = new Chromosome[populationSize];
            if (numberOfElites > 0)
            {
                var elites = currentPopulation.Where(chromosome => chromosome.fitness > 0).OrderByDescending(chromosome => chromosome.fitness)
                                              .Take(numberOfElites).ToArray();
                elitesCount = elites.Length;
                Array.Copy(elites, newGeneration, elites.Length);
            }
            return newGeneration;
        }

        /// <summary>
        /// Function used get the parents (fittest ones) and add them to the mating pool (list) of chromosomes.
        /// </summary>
        /// <param name="population">Represents the population.</param>
        /// <param name="parentsCount">Represents the count of parents.</param>
        public List<Chromosome> GetParents(Chromosome[] population, int parentsCount)
        {
            matingPool.Clear();
            for (int i = 0; i < parentsCount; i++)
            {
                Chromosome fittest = null;
                for (int j = 0; j < tournamentSize; j++)
                {
                    var index = random.Next(0, populationSize);
                    if (fittest == null || population[index].fitness > fittest.fitness)
                        fittest = population[index];
                }
                matingPool.Add(fittest);
            }
            return matingPool;
        }

        /// <summary>
        /// Method used to mutate two random chromosomes.
        /// </summary>
        /// <param name="chromosome">Represents the chromosome.</param>
        public void Mutate(Chromosome chromosome)
        {
            var index = random.Next(0, chromosomeLength);
            chromosome.genes[index] = !chromosome.genes[index];
        }

        /// <summary>
        /// Function used to perfom the chromosomal crossover.
        /// </summary>
        /// <param name="first">Represents the first chromosome of crossover.</param>
        /// <param name="second">Represents the second chromosome of crossover.</param>
        public Chromosome Crossover(Chromosome first, Chromosome second)
        {
            var index = random.Next(1, chromosomeLength);
            var newGenes = new BitArray(first.genes);

            for (int i = index; i < chromosomeLength; i++)
            {
                newGenes.Set(i, second.genes[i]);
            }

            var productChromosome = new Chromosome(newGenes);
            if (random.NextDouble() < mutationProbability)
            {
                Mutate(productChromosome);
            }
            ComputeFitness(productChromosome);
            return productChromosome;
        }

        /// <summary>
        /// Function representing the Genetic Algorithm to solve the Weighted Boolean 3SAT Formula.
        /// </summary>
        /// <param name="formula">Weighted 3SAT Boolean Formula.</param>
        public double SolveFormula(Formula formula)
        {
            currentFormula = formula;
            chromosomeLength = formula.nbvar;

            var population = CreatePopulation();
            int generation = 0;
            int lastBestFitness = 0;
            int fitnessChange = 0;

            while (true)
            {
                if (generation == generations || fitnessChange == -generations)
                    break;

                int maxFitness = population.Max(chromosome => chromosome.fitness);
                Console.WriteLine("GENERATION: " + generation + " FITNESS: " + maxFitness);

                List<Chromosome> parents;
                int elitesCount = 0;
                var newPopulation = CreateGeneration(population, out elitesCount);
                parents = GetParents(population, populationSize - elitesCount);

                for (int i = elitesCount; i < populationSize - 1; i++)
                {
                    newPopulation[i] = Crossover(parents[i - elitesCount], parents[i+1 - elitesCount]);
                }
                newPopulation[populationSize - 1] = Crossover(parents[populationSize - 1 - elitesCount], parents[0]);
                population = newPopulation;
                generation++;

                if (generations < 0)
                {
                    int bestFitness = population.Max(chromosome => chromosome.fitness);
                    if (bestFitness == lastBestFitness)
                        fitnessChange++;
                    else
                    {
                        lastBestFitness = bestFitness;
                        fitnessChange = 0;
                    }
                }
            }
            var fittest = population.OrderBy(chromosome => chromosome.fitness).Last();
            return fittest.fitness;
        }
    }
}
