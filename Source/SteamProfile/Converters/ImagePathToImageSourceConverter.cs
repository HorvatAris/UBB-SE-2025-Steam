using System;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media.Imaging;

namespace SteamProfile.Converters
{
    public class ImagePathToImageSourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is string path && !string.IsNullOrEmpty(path))
            {
                System.Diagnostics.Debug.WriteLine($"Converting path: {path}");
                try
                {
                    // Ensure the path uses the ms-appx URI scheme for local assets
                    var uri = new Uri(path);
                    return new BitmapImage(uri);
                }
                catch (UriFormatException ex)
                {
                    System.Diagnostics.Debug.WriteLine($"UriFormatException: {ex.Message}");
                    return null;
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Exception: {ex.Message}");
                    return null;
                }
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
