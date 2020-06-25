using System;
using System.Collections.Generic;
using System.Linq;

namespace TFTCalculator
{
    public class IEnumerableComparer<T> : IEqualityComparer<IEnumerable<T>>
    {
        public bool Equals(IEnumerable<T> x, IEnumerable<T> y)
        {
            return Object.ReferenceEquals(x, y) || (x != null && y != null && x.SequenceEqual(y));
        }

        public int GetHashCode(IEnumerable<T> obj)
        {
            return obj.Where(e => e != null).Select(e => e.GetHashCode()).Aggregate(17, (a, b) => 23 * a + b);
        }
    }

    public static class IterTools
    {
        public static IEnumerable<IEnumerable<T>> Product<T>(this IEnumerable<IEnumerable<T>> sequences)
        {
            IEnumerable<IEnumerable<T>> emptyProduct = new[] { Enumerable.Empty<T>() };
            return sequences.Aggregate(
                emptyProduct,
                (accumulator, sequence) =>
                    from accumulatedSequence in accumulator
                    from item in sequence
                    select accumulatedSequence.Concat(new[] { item })
                );
        }

        public static IEnumerable<IEnumerable<T>> Permutations<T>(this IEnumerable<T> list)
        {
            T[] listArray = list.ToArray();
            int count = list.Count();
            var indices = Enumerable.Range(0, count);

            var indexPermutations = GetUniquePermutations(indices, count);

            //return indexPermutations;
            foreach (var i in indexPermutations)
            {
                yield return i.Select(index => listArray[index]);
            }
        }

        private static IEnumerable<IEnumerable<T>> GetUniquePermutations<T>(IEnumerable<T> list, int length)
        {
            if (length == 1) return list.Select(t => new T[] { t });

            return GetUniquePermutations(list, length - 1)
                   .SelectMany(t => list.Where(e => !t.Contains(e)),
                               (t1, t2) => t1.Concat(new T[] { t2 }));
        }
    }
}
