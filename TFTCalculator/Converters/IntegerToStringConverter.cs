using System;
using System.Globalization;
using System.Windows.Data;

namespace TFTCalculator
{
    class IntegerToStringConverter : IValueConverter
    {
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (int.TryParse((string)value, NumberStyles.Integer, culture, out int i))
            {
                return i;
            }
            else
            {
                return null;
            }
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value.ToString();
        }
    }
}
