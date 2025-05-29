using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Imaging;

namespace SteamHub.Pages
{
    public sealed partial class AdaptiveProfileControl : UserControl
    {
        public static readonly DependencyProperty ProfilePictureSizeProperty =
            DependencyProperty.Register(nameof(ProfilePictureSize), typeof(double), typeof(AdaptiveProfileControl), new PropertyMetadata(150.0));

        public static readonly DependencyProperty HatSizeProperty =
            DependencyProperty.Register(nameof(HatSize), typeof(double), typeof(AdaptiveProfileControl), new PropertyMetadata(60.0));

        public static readonly DependencyProperty PetSizeProperty =
            DependencyProperty.Register(nameof(PetSize), typeof(double), typeof(AdaptiveProfileControl), new PropertyMetadata(100.0));

        public double ProfilePictureSize
        {
            get => (double)GetValue(ProfilePictureSizeProperty);
            set => SetValue(ProfilePictureSizeProperty, value);
        }

        public double HatSize
        {
            get => (double)GetValue(HatSizeProperty);
            set => SetValue(HatSizeProperty, value);
        }

        public double PetSize
        {
            get => (double)GetValue(PetSizeProperty);
            set => SetValue(PetSizeProperty, value);
        }

        public AdaptiveProfileControl()
        {
            this.InitializeComponent();
        }

        public void UpdateProfile(string username, string description, string profilePicture = null,
            string hat = null, string pet = null, string emoji = null, string frame = null, string background = null)
        {
            try
            {
                if (UsernameTextBlock != null)
                    UsernameTextBlock.Text = username;
                if (DescriptionTextBlock != null)
                    DescriptionTextBlock.Text = description;

                // Update profile picture
                if (!string.IsNullOrEmpty(profilePicture) && ProfilePictureBrush != null)
                {
                    ProfilePictureBrush.ImageSource = new BitmapImage(new System.Uri(profilePicture));
                }

                // Update hat
                if (!string.IsNullOrEmpty(hat) && HatImage != null)
                {
                    HatImage.Source = new BitmapImage(new System.Uri(hat));
                    HatImage.Visibility = Visibility.Visible;
                }

                // Update pet
                if (!string.IsNullOrEmpty(pet) && PetImage != null)
                {
                    PetImage.Source = new BitmapImage(new System.Uri(pet));
                    PetImage.Visibility = Visibility.Visible;
                }

                // Update emoji
                if (!string.IsNullOrEmpty(emoji) && EmojiImage != null)
                {
                    EmojiImage.Source = new BitmapImage(new System.Uri(emoji));
                    EmojiImage.Visibility = Visibility.Visible;
                }

                // Update frame
                if (!string.IsNullOrEmpty(frame) && FrameImage != null)
                {
                    FrameImage.Source = new BitmapImage(new System.Uri(frame));
                    FrameImage.Visibility = Visibility.Visible;
                }

                // Update background
                if (!string.IsNullOrEmpty(background) && BackgroundImage != null)
                {
                    BackgroundImage.Source = new BitmapImage(new System.Uri(background));
                    BackgroundImage.Visibility = Visibility.Visible;
                }
            }
            catch
            {
                System.Diagnostics.Debug.WriteLine($"AdaptiveProfileControl.UpdateProfile Exception");
                // Set all images and text to default/empty values to prevent UI crashes
                if (UsernameTextBlock != null) UsernameTextBlock.Text = string.Empty;
                if (DescriptionTextBlock != null) DescriptionTextBlock.Text = string.Empty;
                if (ProfilePictureBrush != null) ProfilePictureBrush.ImageSource = null;
                if (HatImage != null) { HatImage.Source = null; HatImage.Visibility = Visibility.Collapsed; }
                if (PetImage != null) { PetImage.Source = null; PetImage.Visibility = Visibility.Collapsed; }
                if (EmojiImage != null) { EmojiImage.Source = null; EmojiImage.Visibility = Visibility.Collapsed; }
                if (FrameImage != null) { FrameImage.Source = null; FrameImage.Visibility = Visibility.Collapsed; }
                if (BackgroundImage != null) { BackgroundImage.Source = null; BackgroundImage.Visibility = Visibility.Collapsed; }
            }
        }
    }
}