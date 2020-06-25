using System;
using System.Globalization;
using System.Windows.Data;

namespace TFTCalculator
{
    public class TrackerCenterConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter is bool offsetHeight && offsetHeight)
            {
                double height = (double)values[0];
                double offset = (double)values[1];
                return -height + offset;
            }

            else
            {
                double width = (double)values[0];
                double offset = (double)values[1];
                return -width / 2.0 + offset;
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
