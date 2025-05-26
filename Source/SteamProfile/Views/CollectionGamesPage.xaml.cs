using System;
using System.Diagnostics;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using BusinessLayer.Models;
using SteamProfile.ViewModels;

namespace SteamProfile.Views
{
    public sealed partial class CollectionGamesPage : Page
    {
        // Constants to replace magic strings
        private const string ErrorDialogTitle = "Error";
        private const string RemoveGameErrorMessage = "Failed to remove game from collection. Please try again.";
        private const string CloseButtonTextValue = "OK";

        private CollectionGamesViewModel collectionGamesViewModel;
        private CollectionsViewModel collectionsViewModel;
        private UsersViewModel userViewModel;
        private int collectionIdentifier;
        private string collectionName = string.Empty;

        public CollectionGamesPage()
        {
            this.InitializeComponent();
            collectionGamesViewModel = App.CollectionGamesViewModel;
            collectionsViewModel = App.CollectionsViewModel;
            collectionsViewModel.LoadCollections();

            userViewModel = App.UsersViewModel;
            this.DataContext = collectionGamesViewModel;
        }

        protected override void OnNavigatedTo(NavigationEventArgs eventArgs)
        {
            base.OnNavigatedTo(eventArgs);
            if (eventArgs.Parameter is (int collectionId, string collectionName))
            {
                collectionIdentifier = collectionId;
                collectionName = collectionName;
                LoadCollectionGames();
            }
            else if (eventArgs.Parameter is int backCollectionId)
            {
                // Handle back navigation from AddGameToCollectionPage
                collectionIdentifier = backCollectionId;
                var userId = userViewModel.GetCurrentUser().UserId;
                var collection = collectionsViewModel.GetCollectionById(collectionIdentifier, userId);
                if (collection != null)
                {
                    collectionName = collection.CollectionName;
                    LoadCollectionGames();
                }
            }
        }

        private void LoadCollectionGames()
        {
            collectionGamesViewModel.CollectionName = collectionName;
            collectionGamesViewModel.LoadGames(collectionIdentifier);
        }

        private void BackButton_Click(object sender, RoutedEventArgs eventArgs)
        {
            Frame.Navigate(typeof(CollectionsPage));
        }

        private void AddGameToCollection_Click(object sender, RoutedEventArgs eventArgs)
        {
            Frame.Navigate(typeof(AddGameToCollectionPage), collectionIdentifier);
        }

        private void RemoveGame_Click(object sender, RoutedEventArgs eventArgs)
        {
            try
            {
                var button = sender as Button;
                if (button?.Tag == null)
                {
                    return;
                }

                int gameId = Convert.ToInt32(button.Tag);

                collectionsViewModel.RemoveGameFromCollection(collectionIdentifier, gameId);
                collectionGamesViewModel.LoadGames(collectionIdentifier);
            }
            catch (Exception exception)
            {
                var dialog = new ContentDialog
                {
                    Title = ErrorDialogTitle,
                    Content = RemoveGameErrorMessage,
                    CloseButtonText = CloseButtonTextValue,
                    XamlRoot = this.XamlRoot
                };
                dialog.ShowAsync();
            }
        }

        private void ViewGame_Click(object sender, RoutedEventArgs eventArgs)
        {
            var button = sender as Button;
            if (button?.Tag == null)
            {
                return;
            }

            int gameId = Convert.ToInt32(button.Tag);
            Frame.Navigate(typeof(GamePage), gameId);
        }
    }
}
