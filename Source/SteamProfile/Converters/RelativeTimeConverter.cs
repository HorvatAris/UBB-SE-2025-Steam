using System;
using Microsoft.UI.Xaml.Data;

namespace SteamProfile.Converters
{
    public class RelativeTimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is not DateTime inputDateTime)
            {
                return string.Empty;
            }

            TimeSpan timeDifference = DateTime.Now - inputDateTime;

            if (timeDifference.TotalSeconds < 60)
            {
                return $"{(int)timeDifference.TotalSeconds} seconds ago";
            }

            if (timeDifference.TotalMinutes < 60)
            {
                return $"{(int)timeDifference.TotalMinutes} minutes ago";
            }

            if (timeDifference.TotalHours < 24)
            {
                return $"{(int)timeDifference.TotalHours} hours ago";
            }

            if (timeDifference.TotalDays < 30)
            {
                return $"{(int)timeDifference.TotalDays} days ago";
            }

            if (timeDifference.TotalDays < 365)
            {
                return $"{(int)(timeDifference.TotalDays / 30)} months ago";
            }

            return $"{(int)(timeDifference.TotalDays / 365)} years ago";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException("Conversion from relative time string to DateTime is not supported.");
        }
    }
}
