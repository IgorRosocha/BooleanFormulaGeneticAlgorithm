using System.Collections;

namespace BooleanFormulaGeneticAlgorithm
{
    /// <summary>
    /// Simple class representing the chromosome of the Genetic Algorithm. 
    /// </summary>
    public class Chromosome
    {
        public BitArray genes { get; set; } // BitArray of Chromosome genes
        public int fitness { get; set; } // fitness of Chromosome
        public int weight { get; set; } // weight of Chromosome

        /// <summary>
        /// Initializes a new instance of the <see cref="Chromosome"/> class.
        /// </summary>
        /// <param name="genes">BitArray of Chromosome genes.</param>
        public Chromosome(BitArray genes)
        {
            this.genes = genes;
        }
    }
}
