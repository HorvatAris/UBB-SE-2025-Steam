using Microsoft.UI.Xaml.Data;
using System;

namespace SteamHub.Pages.Converters
{
    public class BoolToSeeFeaturesTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is bool b)
                return b ? "See All Features" : "See My Features";
            return "See My Features";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
} 