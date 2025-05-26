using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;

namespace SteamProfile.Converters
{
    public class IntToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is int numericValue && numericValue > 0)
            {
                return Visibility.Visible;
            }

            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException("Conversion from visibility to integer is not supported.");
        }
    }
}
