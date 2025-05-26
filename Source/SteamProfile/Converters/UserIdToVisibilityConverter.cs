using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;

namespace SteamProfile.Converters
{
    public class UserIdToVisibilityConverter : IValueConverter
    {
        private const int HardcodedCurrentUserId = 1;

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is int providedUserId && providedUserId == HardcodedCurrentUserId)
            {
                return Visibility.Visible;
            }

            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException("Conversion from visibility to user ID is not supported.");
        }
    }
}
