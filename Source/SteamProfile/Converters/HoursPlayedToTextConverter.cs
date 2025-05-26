using System;
using Microsoft.UI.Xaml.Data;

namespace SteamProfile.Converters
{
    public class HoursPlayedToTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is int totalHoursPlayed && totalHoursPlayed > 0)
            {
                return $"Played {totalHoursPlayed} hour{(totalHoursPlayed == 1 ? string.Empty : "s")}";
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
            => throw new NotImplementedException();
    }
}
