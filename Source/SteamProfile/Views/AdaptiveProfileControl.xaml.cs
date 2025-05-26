using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Imaging;

namespace SteamProfile.Views
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
            UsernameTextBlock.Text = username;
            DescriptionTextBlock.Text = description;

            // Update profile picture
            if (!string.IsNullOrEmpty(profilePicture))
            {
                ProfilePictureBrush.ImageSource = new BitmapImage(new System.Uri(profilePicture));
            }

            // Update hat
            if (!string.IsNullOrEmpty(hat))
            {
                HatImage.Source = new BitmapImage(new System.Uri(hat));
                HatImage.Visibility = Visibility.Visible;
            }

            // Update pet
            if (!string.IsNullOrEmpty(pet))
            {
                PetImage.Source = new BitmapImage(new System.Uri(pet));
                PetImage.Visibility = Visibility.Visible;
            }

            // Update emoji
            if (!string.IsNullOrEmpty(emoji))
            {
                EmojiImage.Source = new BitmapImage(new System.Uri(emoji));
                EmojiImage.Visibility = Visibility.Visible;
            }

            // Update frame
            if (!string.IsNullOrEmpty(frame))
            {
                FrameImage.Source = new BitmapImage(new System.Uri(frame));
                FrameImage.Visibility = Visibility.Visible;
            }

            // Update background
            if (!string.IsNullOrEmpty(background))
            {
                BackgroundImage.Source = new BitmapImage(new System.Uri(background));
                BackgroundImage.Visibility = Visibility.Visible;
            }
        }
    }
}