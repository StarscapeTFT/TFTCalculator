using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace TFTCalculator
{
    public enum CalculationMode
    {
        Probability = 0,
        LevelComparison = 1
    }

    public enum CalculationAccuracy
    {
        Exact = 0,
        Approximate = 1
    }

    class CalculationModel
    {
        public const int MaxShops = 50;

        private readonly object lockToken;
        private readonly CalculationAccuracy accuracy;
        private readonly ShopProbability shopProbability;
        private readonly List<Unit> units;

        public List<double>[] CumulativeProbability {
            get;
        } = new List<double>[]
        {
            new List<double>(),
            new List<double>(),
            new List<double>(),
            new List<double>(),
            new List<double>(),
            new List<double>(),
            new List<double>(),
            new List<double>(),
            new List<double>(),
        };

        public CalculationModel(object lockToken,
                                CalculationAccuracy accuracy,
                                ShopProbability shopProbability,
                                List<Unit> units)
        {
            this.lockToken = lockToken;
            this.shopProbability = shopProbability;
            this.units = units;
            this.accuracy = accuracy;
        }

        private void CalculateProbability(CancellationToken cancellationToken, int playerLevel, int atLeast)
        {
            Calculator calculator = accuracy == CalculationAccuracy.Approximate ?
                             (Calculator)new ApproximateCalculator(playerLevel, shopProbability, units) :
                             new ExactCalculator(playerLevel, shopProbability, units);

            // If the calculation was paused, resume where we left off.
            int startShop;
            lock (lockToken)
            {
                startShop = CumulativeProbability[playerLevel].Count + 1;
            }

            for (int numShops = startShop; numShops < MaxShops + 1 && !cancellationToken.IsCancellationRequested; ++numShops)
            {
                double result = calculator.Calculate(atLeast, numShops);

                lock (lockToken)
                {
                    // If cancellation was requested, we can't add this number because another thread might
                    // have started to recalculate the same number.
                    if (!cancellationToken.IsCancellationRequested)
                    {
                        CumulativeProbability[playerLevel].Add(result);
                        Debug.Assert(CumulativeProbability[playerLevel].Count == numShops);
                    }
                }
            }
        }

        private void CalculateComparison(CancellationToken cancellationToken, int atLeast)
        {
            for(int level = 0; level < 9 && !cancellationToken.IsCancellationRequested; ++level)
            {
                if (shopProbability.IsZeroProbability(level, units, atLeast))
                    continue;

                double lastProbability;
                int numShops;

                lock (lockToken)
                {
                    lastProbability = CumulativeProbability[level].LastOrDefault();
                    numShops = CumulativeProbability[level].Count + 1;
                }

                Calculator levelCalculator = accuracy == CalculationAccuracy.Approximate ?
                             (Calculator)new ApproximateCalculator(level, shopProbability, units) :
                             new ExactCalculator(level, shopProbability, units);

                while (lastProbability < 0.5 && !cancellationToken.IsCancellationRequested)
                {
                    lastProbability = levelCalculator.Calculate(atLeast, numShops);

                    if (!cancellationToken.IsCancellationRequested)
                    {
                        lock(lockToken)
                        {
                            CumulativeProbability[level].Add(lastProbability);
                            Debug.Assert(CumulativeProbability[level].Count == numShops);
                        }
                    }

                    ++numShops;
                }
            }
        }

        public Task Calculate(CancellationToken cancellationToken, CalculationMode mode, int playerLevel, int atLeast)
        {
            // If units is empty, the probability is 0.
            if (units.Count == 0)
                return Task.CompletedTask;

            if (mode == CalculationMode.Probability)
            {
                return Task.Run(() => CalculateProbability(cancellationToken, playerLevel, atLeast));
            }
            else
            {
                return Task.Run(() => CalculateComparison(cancellationToken, atLeast));
            }
        }
    }
}
