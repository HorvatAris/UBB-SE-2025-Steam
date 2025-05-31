using Microsoft.UI.Xaml.Data;
using System;

namespace SteamHub.Pages.Converters
{
    public class StringFormatConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null || parameter == null)
                return value;

            string format = parameter.ToString();
            return string.Format(format, value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
} 