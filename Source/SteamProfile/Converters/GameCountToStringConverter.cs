using System;
using Microsoft.UI.Xaml.Data;

namespace SteamProfile.Converters
{
    public class GameCountToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is int count)
            {
                return $"{count} games";
            }
            return "0 games";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}