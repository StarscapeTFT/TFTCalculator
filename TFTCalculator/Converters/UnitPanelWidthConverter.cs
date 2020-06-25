using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace TFTCalculator
{
    public class UnitPanelWidthConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            double result = (double)values[0] * (double)values[1];

            if (result > 0)
            {
                Thickness margin = (Thickness)values[2];
                result += margin.Left + margin.Right;
            }

            return result;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
