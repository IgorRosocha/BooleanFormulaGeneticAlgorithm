using System;
namespace BooleanFormulaGeneticAlgorithm
{
    /// <summary>
    /// Simple class representing the Boolean Formula.
    /// </summary>
    public class Formula
    {
        public int nbvar { get; }
        public int nbclauses { get; }

        public int[] weights { get; }
        public int[] formula { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Formula"/> class.
        /// </summary>
        /// <param name="nbvar">Number of Formula variables.</param>
        /// <param name="nbclauses">Number of Formula clauses.</param>
        /// <param name="weights">Weights of Formula variables.</param>
        /// <param name="formula">Integer array representing the formula.</param>
        public Formula(int nbvar, int nbclauses, int[] weights, int[] formula)
        {
            this.nbvar = nbvar;
            this.nbclauses = nbclauses;
            this.weights = weights;
            this.formula = formula;
        }
    }
}
