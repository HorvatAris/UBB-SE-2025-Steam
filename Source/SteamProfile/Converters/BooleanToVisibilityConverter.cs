using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;

namespace SteamProfile.Converters
{
    public class BooleanToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is bool booleanValue)
            {
                // If the parameter is "inverse", invert the boolean value
                if (parameter?.ToString()?.ToLower() == "inverse")
                {
                    booleanValue = !booleanValue;
                }
                return booleanValue ? Visibility.Visible : Visibility.Collapsed;
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (value is Visibility visibilityState)
            {
                bool booleanResult = visibilityState == Visibility.Visible;
                // If the parameter is "inverse", invert the boolean value
                if (parameter?.ToString()?.ToLower() == "inverse")
                {
                    booleanResult = !booleanResult;
                }
                return booleanResult;
            }
            return false;
        }
    }
}