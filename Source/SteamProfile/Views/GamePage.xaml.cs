using System;
using System.Diagnostics;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;

namespace SteamProfile.Views
{
    public sealed partial class GamePage : Page
    {
        private int gameIdentifier;

        public GamePage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs eventArgs)
        {
            base.OnNavigatedTo(eventArgs);
            if (eventArgs.Parameter is int gameId)
            {
                gameIdentifier = gameId;
                GameIdText.Text = gameId.ToString();
                Debug.WriteLine($"GamePage: Loaded game with ID {gameId}");
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.GoBack();
        }
    }
}