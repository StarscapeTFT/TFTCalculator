namespace TFTCalculator
{
    public class ObservableValue<T> : ViewModel
    {
        private T value;

        public T Value
        {
            get => value;
            set => SetProperty(ref this.value, value);
        }

        public ObservableValue()
        {

        }

        public ObservableValue(T value)
        {
            Value = value;
        }
    }
}
