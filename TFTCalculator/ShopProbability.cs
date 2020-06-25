using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace TFTCalculator
{
    public struct ShopProbability
    {
        public List<double>[] TierProbabilities
        {
            get;
        }

        public List<int> NumberOfUnits
        {
            get;
        }

        public List<int> UnitPool
        {
            get;
        }

        public List<int> OthersTaken
        {
            get;
        }

        public ShopProbability(List<double>[] tierProbabilities, List<int> numberOfUnits, List<int> unitPool, List<int> othersTaken)
        {
            TierProbabilities = tierProbabilities;
            NumberOfUnits = numberOfUnits;
            UnitPool = unitPool;
            OthersTaken = othersTaken;
        }

        public int GetRemainingTierPool(int tier)
        {
            Debug.Assert(tier >= 0);
            Debug.Assert(tier < UnitPool.Count);

            return UnitPool[tier] * NumberOfUnits[tier] - OthersTaken[tier];
        }

        public bool IsZeroProbability(int playerLevel, List<Unit> units, int atLeast)
        {
            List<double> probabilities = TierProbabilities[playerLevel];
            List<int> unitPool = UnitPool;
            int numZero = units.Where(unit => probabilities[unit.Tier] == 0 || unit.AlreadyTaken + unit.LookingFor > unitPool[unit.Tier]).Count();

            return atLeast > units.Count - numZero;
        }

        public bool IsValid(List<Unit> units)
        {
            foreach (Unit unit in units)
            {
                if (unit.LookingFor < 0 || unit.AlreadyTaken < 0)
                    return false;

                if (unit.LookingFor + unit.AlreadyTaken > UnitPool[unit.Tier])
                    return false;

                if (unit.LookingFor + unit.AlreadyTaken > UnitPool[unit.Tier] * NumberOfUnits[unit.Tier])
                    return false;
            }

            if (OthersTaken.Any(taken => taken < 0))
                return false;

            for (int tier = 0; tier < OthersTaken.Count; ++tier)
            {
                if (OthersTaken[tier] + units.Where(unit => unit.Tier == tier).Count() * UnitPool[tier]
                    > UnitPool[tier] * NumberOfUnits[tier])
                    return false;
            }

            return true;
        }
    }
}