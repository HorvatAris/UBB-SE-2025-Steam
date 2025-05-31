using Microsoft.UI.Xaml.Data;
using System;

namespace SteamHub.Pages.Converters
{
    public class DateToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is DateTime dateTime)
            {
                return dateTime.ToString("MMM dd, yyyy HH:mm");
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
} 