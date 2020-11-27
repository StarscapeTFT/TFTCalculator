using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;

namespace TFTCalculator
{
    public class ShopProbabilityViewModel : ViewModel
    {
        public ObservableCollection<double[]> TierProbabilities
        {
            get;
        }

        public ObservableCollection<int> NumberOfUnits
        {
            get;
        }

        public ObservableCollection<int> UnitPool
        {
            get;
        }

        private string name;
        public string Name
        {
            get => name;
            set => SetProperty(ref name, value);
        }

        public override string ToString()
        {
            return Name;
        }

        ShopProbabilityViewModel(string name, List<int> numberOfUnits, List<int> unitPool, List<double[]> tierProbabilities)
        {
            Name = name;
            NumberOfUnits = new ObservableCollection<int>(numberOfUnits);
            UnitPool = new ObservableCollection<int>(unitPool);
            TierProbabilities = new ObservableCollection<double[]>(tierProbabilities);
        }

        public ShopProbability ToShopProbability(List<int> othersTaken)
        {
            return new ShopProbability(
                (from level in Enumerable.Range(0, 9) select (from p in TierProbabilities[level] select p / 100.0).ToList()).ToArray(),
                NumberOfUnits.ToList(), UnitPool.ToList(), othersTaken);
        }

        static public List<ShopProbabilityViewModel> CreateAllShops()
        {
            List<ShopProbabilityViewModel> shops = new List<ShopProbabilityViewModel>();

            shops.Add(new ShopProbabilityViewModel(
                "Set 4 Patch 10.24",
                new List<int>() { 13, 13, 13, 11, 8 },
                new List<int>() { 29, 22, 18, 12, 10 },
                new List<double[]>()
                {
                    new double[] { 100, 000, 000, 000, 000 },
                    new double[] { 100, 000, 000, 000, 000 },
                    new double[] { 075, 025, 000, 000, 000 },
                    new double[] { 055, 030, 015, 000, 000 },
                    new double[] { 045, 033, 020, 002, 000 },
                    new double[] { 035, 035, 025, 005, 000 },
                    new double[] { 024, 035, 030, 010, 001 },
                    new double[] { 015, 025, 035, 020, 005 },
                    new double[] { 010, 015, 030, 030, 015 },
                }));

            shops.Add(new ShopProbabilityViewModel(
                "Set 4 Patch 10.20",
                new List<int>() { 13, 13, 13, 11, 8 },
                new List<int>() { 29, 22, 18, 12, 10 },
                new List<double[]>()
                {
                    new double[] { 100, 000, 000, 000, 000 },
                    new double[] { 100, 000, 000, 000, 000 },
                    new double[] { 075, 025, 000, 000, 000 },
                    new double[] { 055, 030, 015, 000, 000 },
                    new double[] { 045, 030, 020, 005, 000 },
                    new double[] { 030, 035, 025, 010, 000 },
                    new double[] { 019, 035, 030, 015, 001 },
                    new double[] { 014, 025, 035, 020, 006 },
                    new double[] { 010, 015, 030, 030, 015 },
                }));

            shops.Add(new ShopProbabilityViewModel(
                "Set 4 Release",
                new List<int>() { 13, 13, 13, 11, 8 },
                new List<int>() { 29, 22, 18, 12, 10 },
                new List<double[]>()
                {
                                new double[] { 100, 000, 000, 000, 000 },
                                new double[] { 100, 000, 000, 000, 000 },
                                new double[] { 075, 025, 000, 000, 000 },
                                new double[] { 055, 030, 015, 000, 000 },
                                new double[] { 045, 030, 020, 005, 000 },
                                new double[] { 030, 035, 025, 010, 000 },
                                new double[] { 019, 035, 030, 015, 001 },
                                new double[] { 014, 020, 035, 025, 006 },
                                new double[] { 010, 015, 030, 030, 015 },
                }));

            shops.Add(new ShopProbabilityViewModel(
                "Set 3.5 Release",
                new List<int>() { 13, 13, 13, 10, 8 },
                new List<int>() { 29, 22, 18, 12, 10 },
                new List<double[]>()
                {
                    new double[] { 100, 000, 000, 000, 000 },
                    new double[] { 100, 000, 000, 000, 000 },
                    new double[] { 075, 025, 000, 000, 000 },
                    new double[] { 055, 030, 015, 000, 000 },
                    new double[] { 040, 035, 020, 005, 000 },
                    new double[] { 025, 035, 030, 010, 000 },
                    new double[] { 019, 030, 035, 015, 001 },
                    new double[] { 014, 020, 035, 025, 006 },
                    new double[] { 010, 015, 030, 030, 015 },
                }));

            shops.Add(new ShopProbabilityViewModel(
                "Set 3 Patch 10.9",
                new List<int>() { 12, 12, 12, 9, 7 },
                new List<int>() { 29, 22, 16, 12, 10 },
                new List<double[]>()
                {
                    new double[] { 100, 000, 000, 000, 000 },
                    new double[] { 100, 000, 000, 000, 000 },
                    new double[] { 075, 025, 000, 000, 000 },
                    new double[] { 060, 030, 010, 000, 000 },
                    new double[] { 040, 035, 020, 005, 000 },
                    new double[] { 025, 035, 030, 010, 000 },
                    new double[] { 019, 030, 035, 015, 001 },
                    new double[] { 014, 020, 035, 025, 006 },
                    new double[] { 010, 015, 025, 035, 015 },
                }));

            shops.Add(new ShopProbabilityViewModel(
                "Set 3 Patch 10.8",
                new List<int>() { 12, 12, 12, 9, 7 },
                new List<int>() { 29, 22, 16, 12, 10 },
                new List<double[]>()
                {
                    new double[] { 100, 000, 000, 000, 000 },
                    new double[] { 100, 000, 000, 000, 000 },
                    new double[] { 070, 030, 000, 000, 000 },
                    new double[] { 050, 035, 015, 000, 000 },
                    new double[] { 035, 040, 020, 005, 000 },
                    new double[] { 020, 035, 035, 010, 000 },
                    new double[] { 014, 030, 040, 015, 001 },
                    new double[] { 014, 020, 035, 025, 006 },
                    new double[] { 010, 015, 025, 035, 015 },
                }));

            shops.Add(new ShopProbabilityViewModel(
                "Set 3 Patch 10.7",
                new List<int>() { 12, 12, 12, 9, 6 },
                new List<int>() { 29, 22, 16, 12, 10 },
                new List<double[]>()
                {
                    new double[] { 100, 000, 000, 000, 000 },
                    new double[] { 100, 000, 000, 000, 000 },
                    new double[] { 070, 030, 000, 000, 000 },
                    new double[] { 050, 035, 015, 000, 000 },
                    new double[] { 035, 040, 020, 005, 000 },
                    new double[] { 020, 035, 035, 010, 000 },
                    new double[] { 014, 030, 040, 015, 001 },
                    new double[] { 014, 020, 035, 025, 006 },
                    new double[] { 010, 015, 025, 035, 015 },
                }));

            shops.Add(new ShopProbabilityViewModel(
                "Set 3 Release",
                new List<int>() { 12, 12, 12, 9, 6 },
                new List<int>() { 29, 22, 16, 12, 10 },
                new List<double[]>()
                {
                    new double[] { 100, 000, 000, 000, 000 },
                    new double[] { 100, 000, 000, 000, 000 },
                    new double[] { 070, 030, 000, 000, 000 },
                    new double[] { 050, 035, 015, 000, 000 },
                    new double[] { 035, 040, 020, 005, 000 },
                    new double[] { 020, 035, 035, 010, 000 },
                    new double[] { 014, 030, 040, 015, 001 },
                    new double[] { 013, 020, 035, 025, 007 },
                    new double[] { 010, 015, 025, 035, 015 },
                }));

            shops.Add(new ShopProbabilityViewModel(
                "Set 2 Patch 10.1",
                new List<int>() { 13, 13, 13, 10, 7 },
                new List<int>() { 29, 22, 16, 12, 10 },
                new List<double[]>()
                {
                    new double[] { 100, 000, 000, 000, 000 },
                    new double[] { 100, 000, 000, 000, 000 },
                    new double[] { 070, 025, 005, 000, 000 },
                    new double[] { 050, 035, 015, 000, 000 },
                    new double[] { 035, 035, 025, 005, 000 },
                    new double[] { 025, 035, 030, 010, 000 },
                    new double[] { 020, 030, 033, 015, 002 },
                    new double[] { 015, 020, 035, 024, 006 },
                    new double[] { 010, 015, 030, 030, 015 },
                }));

            shops.Add(new ShopProbabilityViewModel(
                "Set 2 Patch 9.24",
                new List<int>() { 12, 13, 12, 10, 7 },
                new List<int>() { 29, 22, 16, 12, 10 },
                new List<double[]>()
                {
                    new double[] { 100, 000, 000, 000, 000 },
                    new double[] { 100, 000, 000, 000, 000 },
                    new double[] { 070, 025, 005, 000, 000 },
                    new double[] { 050, 035, 015, 000, 000 },
                    new double[] { 035, 035, 025, 005, 000 },
                    new double[] { 025, 035, 030, 010, 000 },
                    new double[] { 020, 030, 033, 015, 002 },
                    new double[] { 015, 020, 035, 024, 006 },
                    new double[] { 010, 015, 030, 030, 015 },
                }));

            shops.Add(new ShopProbabilityViewModel(
               "Set 2 Release",
                new List<int>() { 12, 12, 12, 9, 6 },
                new List<int>() { 29, 22, 16, 12, 10 },
                new List<double[]>()
                {
                    new double[] { 100, 000, 000, 000, 000 },
                    new double[] { 100, 000, 000, 000, 000 },
                    new double[] { 070, 025, 005, 000, 000 },
                    new double[] { 050, 035, 015, 000, 000 },
                    new double[] { 035, 035, 025, 005, 000 },
                    new double[] { 025, 035, 030, 010, 000 },
                    new double[] { 020, 030, 033, 015, 002 },
                    new double[] { 015, 020, 035, 024, 006 },
                    new double[] { 010, 015, 030, 030, 015 },
                }));

            shops.Add(new ShopProbabilityViewModel(
                "Set 1 Patch 9.20",
                new List<int>() { 13, 13, 13, 10, 8 },
                new List<int>() { 39, 26, 18, 13, 10 },
                new List<double[]>()
                {
                    new double[] { 100, 000, 000, 000, 000 },
                    new double[] { 100, 000, 000, 000, 000 },
                    new double[] { 070, 025, 005, 000, 000 },
                    new double[] { 050, 035, 015, 000, 000 },
                    new double[] { 035, 035, 025, 005, 000 },
                    new double[] { 025, 035, 030, 010, 000 },
                    new double[] { 020, 030, 033, 015, 002 },
                    new double[] { 015, 020, 035, 022, 008 },
                    new double[] { 010, 015, 030, 030, 015 },
                }));

            shops.Add(new ShopProbabilityViewModel(
                "Set 1 Patch 9.19",
                new List<int>() { 13, 13, 13, 10, 8 },
                new List<int>() { 39, 26, 18, 13, 10 },
                new List<double[]>()
                {
                    new double[] { 100, 000, 000, 000, 000 },
                    new double[] { 100, 000, 000, 000, 000 },
                    new double[] { 070, 025, 005, 000, 000 },
                    new double[] { 050, 035, 015, 000, 000 },
                    new double[] { 035, 035, 025, 005, 000 },
                    new double[] { 025, 035, 030, 010, 000 },
                    new double[] { 020, 030, 033, 015, 002 },
                    new double[] { 015, 025, 035, 020, 005 },
                    new double[] { 010, 015, 033, 030, 012 },
                }));

            shops.Add(new ShopProbabilityViewModel(
                "Set 1 Patch 9.18",
                new List<int>() { 13, 13, 13, 10, 7 },
                new List<int>() { 39, 26, 18, 13, 10 },
                new List<double[]>()
                {
                    new double[] { 100, 000, 000, 000, 000 },
                    new double[] { 100, 000, 000, 000, 000 },
                    new double[] { 070, 025, 005, 000, 000 },
                    new double[] { 050, 035, 015, 000, 000 },
                    new double[] { 035, 035, 025, 005, 000 },
                    new double[] { 025, 035, 030, 010, 000 },
                    new double[] { 020, 030, 033, 015, 002 },
                    new double[] { 015, 025, 035, 020, 005 },
                    new double[] { 010, 015, 033, 030, 012 },
                }));

            // PATCH 9.17: +Pantheon (5)
            shops.Add(new ShopProbabilityViewModel(
                "Set 1 Patch 9.17",
                new List<int>() { 13, 13, 13, 10, 7 },
                new List<int>() { 39, 26, 21, 13, 10 },
                new List<double[]>()
                {
                    new double[] { 100, 000, 000, 000, 000 },
                    new double[] { 100, 000, 000, 000, 000 },
                    new double[] { 065, 030, 005, 000, 000 },
                    new double[] { 050, 035, 015, 000, 000 },
                    new double[] { 037, 035, 025, 003, 000 },
                    new double[] { 024.5, 35, 030, 010, 0.5 },
                    new double[] { 020, 030, 033, 015, 002 },
                    new double[] { 015, 025, 035, 020, 005 },
                    new double[] { 010, 015, 035, 030, 010 },
                }));

            // PATCH 9.16: +Camille (1), +Jayce (2), +Vi (3), +Jinx (4)
            shops.Add(new ShopProbabilityViewModel(
                "Set 1 Patch 9.16",
                new List<int>() { 13, 13, 13, 10, 6 },
                new List<int>() { 39, 26, 21, 13, 10 },
                new List<double[]>()
                {
                    new double[] { 100, 000, 000, 000, 000 },
                    new double[] { 100, 000, 000, 000, 000 },
                    new double[] { 065, 030, 005, 000, 000 },
                    new double[] { 050, 035, 015, 000, 000 },
                    new double[] { 037, 035, 025, 003, 000 },
                    new double[] { 024.5, 35, 030, 010, 0.5 },
                    new double[] { 020, 030, 033, 015, 002 },
                    new double[] { 015, 025, 035, 020, 005 },
                    new double[] { 010, 015, 035, 030, 010 },
                }));

            // PATCH 9.14: +Twisted Fate (2 g)
            // Elise from 2 cost to 1 cost
            shops.Add(new ShopProbabilityViewModel(
                "Set 1 Patch 9.14",
                new List<int>() { 12, 12, 12, 9, 6 },
                new List<int>() { 39, 26, 21, 13, 10 },
                new List<double[]>()
                {
                    new double[] { 100, 000, 000, 000, 000 },
                    new double[] { 100, 000, 000, 000, 000 },
                    new double[] { 065, 030, 005, 000, 000 },
                    new double[] { 050, 035, 015, 000, 000 },
                    new double[] { 037, 035, 025, 003, 000 },
                    new double[] { 024.5, 35, 030, 010, 0.5 },
                    new double[] { 020, 030, 033, 015, 002 },
                    new double[] { 015, 025, 035, 020, 005 },
                    new double[] { 010, 015, 035, 030, 010 },
                }));


            shops.Add(new ShopProbabilityViewModel(
                "Set 1 Release",
                new List<int>() { 11, 12, 12, 9, 6 },
                new List<int>() { 39, 26, 21, 13, 10 },
                new List<double[]>()
                {
                    new double[] { 100, 000, 000, 000, 000 },
                    new double[] { 100, 000, 000, 000, 000 },
                    new double[] { 065, 030, 005, 000, 000 },
                    new double[] { 050, 035, 015, 000, 000 },
                    new double[] { 037, 035, 025, 003, 000 },
                    new double[] { 024.5, 35, 030, 010, 0.5 },
                    new double[] { 020, 030, 033, 015, 002 },
                    new double[] { 015, 025, 035, 020, 005 },
                    new double[] { 010, 015, 035, 030, 010 },
                }));

#if DEBUG
            foreach (ShopProbabilityViewModel shopProbability in shops)
            {
                foreach (double[] p in shopProbability.TierProbabilities)
                {
                    Debug.Assert(System.Math.Abs(p.Sum() - 100) < 0.00001);
                }
            }
#endif

            return shops;
        }
    }
}
