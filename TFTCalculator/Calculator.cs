using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace TFTCalculator
{
    class ExactCalculator : Calculator
    {
        public ExactCalculator(int playerLevel, ShopProbability shopProbability, List<Unit> units)
            : base(shopProbability, units)
        {
            unitMatrices = new List<UnitProbabilityMatrix>() { new UnitProbabilityMatrix(playerLevel, shopProbability, units) };
        }

        protected override double ProbabilityOfFindingExactly(int number, List<MemoryAlignedMatrix> results)
        {
            Debug.Assert(results.Count == 1);
            // Build a list of all combinations of units that satisfy the search condition
            var yesno = Enumerable.Repeat(true, number).Concat(Enumerable.Repeat(false, units.Count - number)).ToArray();
            var permutations = IterTools.Permutations(yesno).Distinct(new IEnumerableComparer<bool>()).ToArray();

            // For each unique matrix index from the permutations, take the probability and sum it
            return permutations.SelectMany(p => unitMatrices[0].IndicesForFinding(p)).Distinct().Select(i => results[0][0, i]).Sum();
        }
    }

    class ApproximateCalculator : Calculator
    {
        public ApproximateCalculator(int playerLevel, ShopProbability shopProbability, List<Unit> units)
            : base(shopProbability, units)
        {
            unitMatrices = units.Select(unit => new UnitProbabilityMatrix(playerLevel, shopProbability, new List<Unit>() { unit })).ToList();
        }

        protected override double ProbabilityOfFindingExactly(int number, List<MemoryAlignedMatrix> results)
        {
            Debug.Assert(number > 0 && number <= units.Count);
            Debug.Assert(results.Count == units.Count);

            // Build a list of all combinations of units that satisfy the search condition (atLeast)
            var yesno = Enumerable.Repeat(true, number).Concat(Enumerable.Repeat(false, units.Count - number)).ToArray();
            var permutations = IterTools.Permutations(yesno).Distinct(new IEnumerableComparer<bool>()).ToArray();

            double accumulator = 0;

            foreach (var permutation in permutations)
            {
                Debug.Assert(permutation.Count() == units.Count());

                double multiplier = 1;

                for (int i = 0; i < units.Count; ++i)
                {
                    int index = results[i].Size - 1;
                    double p = results[i][0, index];
                    multiplier *= permutation.ElementAt(i) ? p : 1 - p;
                }

                accumulator += multiplier;
            }
            return accumulator;
        }
    }

    abstract class Calculator
    {
        protected readonly List<Unit> units;
        protected readonly ShopProbability shopProbability;
        protected List<UnitProbabilityMatrix> unitMatrices;

        public Calculator(ShopProbability shopProbability, List<Unit> units)
        {
            this.shopProbability = shopProbability;
            this.units = units;
        }

        public double Calculate(int atLeast, int numShops)
        {
            Debug.Assert(numShops > 0);
            var matrixResults = unitMatrices.Select(m => m.Power(numShops * 5)).ToList();

            double accumulator = 0;
            for (; atLeast <= units.Count(); ++atLeast)
            {
                accumulator += ProbabilityOfFindingExactly(atLeast, matrixResults);
            }

            return accumulator;
        }

        protected abstract double ProbabilityOfFindingExactly(int number, List<MemoryAlignedMatrix> results);
    }
}
