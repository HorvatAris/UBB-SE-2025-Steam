using System;
using Microsoft.UI.Xaml.Data;

namespace SteamProfile.Converters
{
    public class DateOnlyToStringConverter : IValueConverter
    {
        public object Convert(object value, System.Type targetType, object parameter, string language)
        {
            if (value is DateOnly date)
            {
                return date.ToString("d"); // Short date format
            }
            return string.Empty;
        }

        public object ConvertBack(object value, System.Type targetType, object parameter, string language)
        {
            throw new System.NotImplementedException();
        }
    }
}