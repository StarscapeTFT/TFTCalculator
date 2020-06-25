namespace TFTCalculator
{
    public struct Unit
    {
        public int Tier { get; }
        public int AlreadyTaken { get; }
        public int LookingFor { get; }

        public Unit(int tier, int alreadyTaken, int lookingFor)
        {
            Tier = tier;
            AlreadyTaken = alreadyTaken;
            LookingFor = lookingFor;
        }
    }
}
