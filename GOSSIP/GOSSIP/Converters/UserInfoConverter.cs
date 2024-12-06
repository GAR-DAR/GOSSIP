using System;
using System.Globalization;
using System.Windows.Data;

namespace GOSSIP.Converters
{
    public class UserInfoConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || parameter == null) return string.Empty;

            string format = parameter.ToString();
            string stringValue = value.ToString();

            return format switch
            {
                "Status" => $"Status: {value}",
                "FieldOfStudy" => $"Field of study: {value}",
                "Specialization" => $"Specialization: {value}",
                "University" => $"University: {value}",
                "Term" => $"Term: {value}",
                "Degree" => $"Degree: {value}",
                "Role" => $"Role: {value}",
                _ => value.ToString()
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

}
