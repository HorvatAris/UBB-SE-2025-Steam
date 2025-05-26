using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using BusinessLayer.Models;

namespace SteamProfile.Views
{
    public sealed partial class ProfileInfoControl : UserControl
    {
        public string Username
        {
            get => UsernameTextBlock.Text;
            set => UsernameTextBlock.Text = value;
        }

        public string Description
        {
            get => DescriptionTextBlock.Text;
            set => DescriptionTextBlock.Text = value;
        }

        public ImageSource ProfilePicture
        {
            get => ProfilePictureBrush.ImageSource;
            set => ProfilePictureBrush.ImageSource = value;
        }

        public ProfileInfoControl()
        {
            this.InitializeComponent();
        }

        public void ApplyFeature(Feature feature)
        {
            if (feature == null)
            {
                return;
            }

            try
            {
                var imageSource = new BitmapImage(new Uri($"ms-appx:///{feature.Source}"));

                switch (feature.Type.ToLower())
                {
                    case "frame":
                        FrameImage.Source = imageSource;
                        FrameImage.Visibility = Visibility.Visible;
                        break;
                    case "emoji":
                        EmojiImage.Source = imageSource;
                        EmojiImage.Visibility = Visibility.Visible;
                        break;
                    case "background":
                        BackgroundImage.Source = imageSource;
                        BackgroundImage.Visibility = Visibility.Visible;
                        break;
                    case "pet":
                        PetImage.Source = imageSource;
                        PetImage.Visibility = Visibility.Visible;
                        break;
                    case "hat":
                        HatImage.Source = imageSource;
                        HatImage.Visibility = Visibility.Visible;
                        break;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error applying feature: {ex.Message}");
            }
        }

        public void ClearFeature(string featureType)
        {
            switch (featureType.ToLower())
            {
                case "frame":
                    FrameImage.Source = null;
                    FrameImage.Visibility = Visibility.Collapsed;
                    break;
                case "emoji":
                    EmojiImage.Source = null;
                    EmojiImage.Visibility = Visibility.Collapsed;
                    break;
                case "background":
                    BackgroundImage.Source = null;
                    BackgroundImage.Visibility = Visibility.Collapsed;
                    break;
                case "pet":
                    PetImage.Source = null;
                    PetImage.Visibility = Visibility.Collapsed;
                    break;
                case "hat":
                    HatImage.Source = null;
                    HatImage.Visibility = Visibility.Collapsed;
                    break;
            }
        }

        public void ApplyUserFeatures(User user, List<Feature> equippedFeatures)
        {
            Username = user.Username;

            // Apply equipped features
            if (equippedFeatures != null)
            {
                foreach (var feature in equippedFeatures)
                {
                    if (feature.Equipped)
                    {
                        ApplyFeature(feature);
                    }
                }
            }
        }
    }
}