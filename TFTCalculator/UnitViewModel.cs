using System.Collections;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace TFTCalculator
{
    public class UnitViewModel : ViewModel, INotifyDataErrorInfo
    {
        bool isEnabled;
        public bool IsEnabled
        {
            get => isEnabled;
            set => SetProperty(ref isEnabled, value);
        }

        int tier;
        public int Tier
        {
            get => tier;
            set => SetProperty(ref tier, value);
        }

        int alreadyTaken;
        public int AlreadyTaken
        {
            get => alreadyTaken;
            set
            {
                SetProperty(ref alreadyTaken, value);
                OnErrorsChanged(nameof(LookingFor));
            }
        }

        int lookingFor;
        public int LookingFor
        {
            get => lookingFor;
            set
            {
                SetProperty(ref lookingFor, value);
                OnErrorsChanged(nameof(AlreadyTaken));
            }
        }

        int poolSize;
        public int PoolSize
        {
            get => poolSize;
            set => SetProperty(ref poolSize, value);
        }

        public UnitViewModel()
        {

        }

        public Unit ToUnit()
        {
            return new Unit(Tier, AlreadyTaken, LookingFor);
        }

        public override IEnumerable GetErrors([CallerMemberName] string propertyName = null)
        {
            if (AlreadyTaken < 0 || LookingFor < 0)
            {
                yield return "cannot be negative";
            }

            if (AlreadyTaken + LookingFor > PoolSize)
            {
                yield return "too many";
            }
        }
    }
}
