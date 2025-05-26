using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;

namespace SteamProfile.Converters
{
    public class NumberToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null || parameter == null)
            {
                return Visibility.Collapsed;
            }

            bool isInverse = parameter.ToString()?.ToLower() == "inverse";
            int number = System.Convert.ToInt32(value);
            if (isInverse)
            {
                return number > 0 ? Visibility.Visible : Visibility.Collapsed;
            }
            else
            {
                return number == 0 ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}