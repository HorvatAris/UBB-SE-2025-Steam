using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using SteamHub.ApiContract.Services.Interfaces;
using SteamHub.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

using System.Diagnostics;


// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace SteamHub.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class CollectionGamesPage : Page
    {
        // Constants to replace magic strings
        private const string ErrorDialogTitle = "Error";
        private const string RemoveGameErrorMessage = "Failed to remove game from collection. Please try again.";
        private const string CloseButtonTextValue = "OK";

        private CollectionGamesViewModel collectionGamesViewModel;
        private CollectionsViewModel collectionsViewModel;
        // TO SOLVE private UsersViewModel userViewModel;
        private int collectionIdentifier;
        private string collectionName = string.Empty;

        public CollectionGamesPage(ICollectionsService collectionsService, IUserService userService)
        {
            this.InitializeComponent();
            collectionGamesViewModel = new CollectionGamesViewModel(collectionsService);
            collectionsViewModel = new CollectionsViewModel(collectionsService, userService);
            collectionsViewModel.LoadCollections();

            //TO SOLVE userViewModel = App.UsersViewModel;
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
                //TO SOLVE var userId = userViewModel.GetCurrentUser().UserId;
                // TO SOLVE var collection = collectionsViewModel.GetCollectionById(collectionIdentifier, userId);
                //TO SOLVE  if (collection != null)
                //{
                //    collectionName = collection.CollectionName;
                //    LoadCollectionGames();
                // TO SOLVE  }
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
            // TO SOLVE Frame.Navigate(typeof(AddGameToCollectionPage), collectionIdentifier);
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