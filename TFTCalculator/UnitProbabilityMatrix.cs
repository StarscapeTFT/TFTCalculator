using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace TFTCalculator
{
    public static class MatrixExtensions
    {
        private static void Swap(ref MemoryAlignedMatrix A, ref MemoryAlignedMatrix B)
        {
            MemoryAlignedMatrix temp = A;
            A = B;
            B = temp;
        }

        public static MemoryAlignedMatrix Power(this MemoryAlignedMatrix a, int n)
        {
            MemoryAlignedMatrix z = null;
            MemoryAlignedMatrix result = null;
            MemoryAlignedMatrix output = new MemoryAlignedMatrix(a.Size);

            while (n > 0)
            {
                if (z == null)
                {
                    z = a.Copy();
                }
                else
                {
                    CBLAS.Square(z, output);
                    Swap(ref z, ref output);
                }

                int bit = n % 2;
                n /= 2;

                if (bit == 1)
                {
                    if (result == null)
                    {
                        result = z.Copy();
                    }
                    else
                    {
                        CBLAS.Multiply(result, z, output);
                        Swap(ref result, ref output);
                    }
                }
            }

            return result;
        }
    }
    public class UnitProbabilityMatrix : MemoryAlignedMatrix
    {
        public List<Unit> Units
        {
            get;
        }

        public ShopProbability ShopProbability
        {
            get;
        }

        public UnitProbabilityMatrix(int playerLevel, ShopProbability shopProbability, List<Unit> units)
            // Matrix size is the product of all units we are looking for (+1 to include the possiblity of finding 0)
            : base(units.Select(unit => unit.LookingFor + 1).Aggregate((count, x) => count * x))
        {
            Units = new List<Unit>(units);
            ShopProbability = shopProbability;

            // Every possible permutation of the number of units we are looking for.
            // e.g. if we are looking for 3 of 2 different units, makes a list containing (0, 0), (0, 1), (0, 2), (0, 3), (1, 0), (1, 1)
            var permutations = (from unit in Units select Enumerable.Range(0, unit.LookingFor + 1)).Product();

            foreach (IEnumerable<int> numFound2 in permutations)
            {
                List<int> numFound = numFound2.ToList();

                int index = StateIndex(numFound);
                Debug.Assert(index < Size);
                Debug.Assert(numFound.Count() == Units.Count);

                List<int> unitTiersFound = new List<int>(Enumerable.Repeat(0, ShopProbability.UnitPool.Count));

                // First tally up how many of each tier have been found for this permutation
                foreach (var unit in Units.Zip(numFound, (unit, found) => new { Tier = unit.Tier, Found = found + unit.AlreadyTaken }))
                {
                    unitTiersFound[unit.Tier] += unit.Found;
                }

                // For each unit in this permutation, add a transition to the matrix for finding 1 more
                foreach (var unit in Units.Zip(numFound, (one, two) => new Tuple<Unit, int>(one, two)).Zip(Enumerable.Range(0, Units.Count),
                    (unit, unitIndex) => new { unit.Item1.Tier, Found = unit.Item2, Index = unitIndex }))
                {
                    // If we are looking for 2 of a unit, we need to go from (1, 0), (1, 1), (1, 2) to (2, 0), (2, 1), and (2, 2).
                    // Without this if statement, the permutation list would also trip us over the index going from (1, 2) to (1, 3)
                    // So let's just skip that case
                    if (unit.Found + 1 > units[unit.Index].LookingFor)
                        continue;

                    int unitsAvailable = ShopProbability.UnitPool[unit.Tier] - Units[unit.Index].AlreadyTaken - unit.Found;
                    int totalPool = ShopProbability.GetRemainingTierPool(unit.Tier) - unitTiersFound[unit.Tier];

                    // Probability a single shop slot contains this unit
                    // Cap probability to 1 in case user has done something insane with the other units taken
                    double probability = Math.Min(ShopProbability.TierProbabilities[playerLevel][unit.Tier] * unitsAvailable / totalPool, 1);
                    if (totalPool == 0 || probability < 0)
                    {
                        throw new ArgumentException("The search parameters led to a negative probability.\n\nAre you searching for more units than exist in the game?");
                    }

                    // Find the matrix index corresponding to +1 of this unit
                    List<int> numFoundPlusOne = new List<int>(numFound);
                    numFoundPlusOne[unit.Index] += 1;
                    int indexPlusOne = StateIndex(numFoundPlusOne);

                    // The probability of staying at the same unit count is 1 - probability of finding any unit
                    // Since we're only considering 1 shop slot here, the probability of finding a unit is additive
                    if (this[index, index] == 0)
                    {
                        this[index, index] = 1 - probability;
                    }
                    else
                    {
                        this[index, index] -= probability;
                        //Debug.Assert(this[index, index] >= 0);
                        if (this[index, index] < -0.0001)
                        {
                            throw new ArgumentException("The search parameters led to a negative probability.\n\nAre you searching for more units than exist in the game?");
                        }
                    }

                    Debug.Assert(this[index, indexPlusOne] == 0);
                    this[index, indexPlusOne] = probability;
                }
            }

            // Probability of staying in the final state (= we found every unit) is 1, since we stop buying more units at that point.
            int lastIndex = StateIndex(Units.Select(unit => unit.LookingFor));
            Debug.Assert(this[lastIndex, lastIndex] == 0);
            this[lastIndex, lastIndex] = 1;
        }

        public IEnumerable<int> IndicesForFinding(IEnumerable<bool> findUnits)
        {
            Debug.Assert(findUnits.Count() == Units.Count);
            Debug.Assert(findUnits.Where(u => u == true).Count() > 0);

            var unitRanges = from z in Units.Zip(findUnits, (unit, find) => new { Unit = unit, Find = find })
                             select z.Find ? new List<int>(1) { z.Unit.LookingFor } : Enumerable.Range(0, z.Unit.LookingFor);
            Debug.Assert(unitRanges.Count() == Units.Count);

            var rangePermutations = unitRanges.Product();

            var matrixIndices = from p in rangePermutations
                                select StateIndex(p);

            return matrixIndices;
        }

        /// <summary>
        /// Returns a multidimensional matrix index mapped onto a 1D array index.
        /// </summary>
        /// <param name="unitCounts">List of how many of each unit to consider.</param>
        /// <returns>1D array index</returns>
        public int StateIndex(IEnumerable<int> unitCounts)
        {
            int index = 0;
            int multiplier = 1;

            foreach (var unit in unitCounts.Zip(Units, (count, unit) => new { Count = count, unit.LookingFor }))
            {
                Debug.Assert(unit.Count >= 0);
                Debug.Assert(unit.LookingFor >= 0);
                Debug.Assert(unit.Count <= unit.LookingFor);

                index += multiplier * unit.Count;
                multiplier *= unit.LookingFor + 1;
            }

            return index;
        }
    }
}
