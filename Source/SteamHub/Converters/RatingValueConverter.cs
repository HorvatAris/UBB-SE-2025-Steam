using Microsoft.UI.Xaml.Data;
using System;
using System.Diagnostics;

namespace SteamHub.Converters
{
    public class RatingValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (parameter is string expectedRating)
            {
                double ratingDouble;
                if (value is double doubleValue)
                {
                    ratingDouble = doubleValue;
                }
                else if (value is decimal decimalValue)
                {
                    ratingDouble = (double)decimalValue;
                }
                else if (value is string stringValue)
                {
                    if (!double.TryParse(stringValue, out ratingDouble))
                    {
                        return false;
                    }
                }
                else
                {
                    Debug.WriteLine($"Unexpected value type: {value?.GetType()}");
                    return false;
                }

                double roundedRating = Math.Round(ratingDouble);
                roundedRating = Math.Max(1, Math.Min(5, roundedRating));
                return roundedRating == double.Parse(expectedRating);
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class RatingToDoubleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is decimal decimalValue)
            {
                return (double)decimalValue;
            }
            else if (value is string stringValue)
            {
                if (double.TryParse(stringValue, out double result))
                {
                    return result;
                }
            }
            else if (value is double doubleValue)
            {
                return doubleValue;
            }
            
            return 0.0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
} 