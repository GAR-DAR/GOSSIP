using System.Globalization;
using System.Windows.Data;

namespace GOSSIP.Converters
{
    //Конвертація з DateTime до стрічки. Містить реалізацію різних часових варіантах у різних одиницях вимірювання
    //Враховано варіанти від "тільки що" до "n років тому", разом із відповідними формами однини і множини.
    public class DateTimeToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DateTime date)
            {
                TimeSpan difference = DateTime.Now - date;

                if (difference.TotalMinutes < 1)
                    return "Just now";
                else if (difference.TotalHours < 1)
                    return difference.TotalMinutes == 1 ? $"1 minute ago" : $"{(int)difference.TotalMinutes} minutes ago" ;
                else if (difference.TotalDays < 1)
                    return difference.TotalHours == 1 ? $"1 hour ago" : $"{(int)difference.TotalHours} hours ago";
                else if (difference.TotalDays < 30)
                    return difference.TotalDays == 1 ? $"1 day ago" : $"{(int)difference.TotalDays} days ago";
                else if (difference.TotalDays < 365)
                    return difference.TotalDays / 30 == 1 ? $"1 month ago" : $"{(int)(difference.TotalDays / 30)} months ago";
                else
                    return difference.TotalDays / 365 == 1 ? $"1 year ago" : $"{(int)(difference.TotalDays / 365)} years ago";
            }

            return "Unknown time";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
