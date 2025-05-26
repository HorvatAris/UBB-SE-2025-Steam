using System;
using Microsoft.UI.Xaml.Data;

namespace SteamProfile.Converters
{
    public class BoolToOpacityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is bool hasAchievement)
            {
                return hasAchievement ? 1.0 : 0.3; // Full opacity for unlocked, 30% for locked
            }
            return 0.3; // Default to locked state
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}