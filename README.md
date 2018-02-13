# Implementation of Genetic Algorithm to solve 3SAT Weighted Boolean Formula

This project is a C# Implementation of Genetic Algorithm to solve 3SAT Weighted Boolean Formula, created as a semestral work for MI-PAA (Problems and Algorithms) at FIT CTU.

## Usage:

Build the project (.exe), and then use the following command in your CLI:

	> mono BooleanFormulaGeneticAlgorithm.exe path population_size generations mutation_probability tournament_size number_of_elites

## Arguments:

1. path - Path of DIMACS .cnf file / directory containing multiple DIMACS .cnf files.
2. population_size - Size of the population.
3. generations - Number of generations.
4. mutation_probability - Probability of mutation.
5. tournament_size - Size of the tournament.
6. number_of_elites - Number of fittest organisms to carry over from current generation to the next one).

**Note: All of the parameters are required!**