using System;
using System.Globalization;
using System.Windows.Data;

namespace CHASER_2._0
{
    public class StringToIntConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int intValue)
            {
                return intValue.ToString();
            }
            return "0";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (int.TryParse(value as string, out int intValue))
            {
                return intValue;
            }
            return 0;
        }
    }
}