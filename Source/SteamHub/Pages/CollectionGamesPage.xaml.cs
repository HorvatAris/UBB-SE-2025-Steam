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
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;


// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace SteamHub.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class CollectionGamesPage : Page
    {
        private const string ErrorDialogTitle = "Error";
        private const string RemoveGameErrorMessage = "Failed to remove game from collection. Please try again.";
        private const string CloseButtonTextValue = "OK";

        private CollectionGamesViewModel collectionGamesViewModel;
        private CollectionsViewModel collectionsViewModel;
        private UsersViewModel userViewModel;
        private int collectionIdentifier;
        private string collectionName = string.Empty;
        private readonly ICollectionsService collectionsService;
        private readonly IUserService userService;

        public CollectionGamesPage(ICollectionsService collectionsService, IUserService userService)
        {
            this.InitializeComponent();
            this.collectionsService = collectionsService;
            this.userService = userService;
            collectionGamesViewModel = new CollectionGamesViewModel(collectionsService);
            collectionsViewModel = new CollectionsViewModel(collectionsService, userService);
            _ = collectionsViewModel.LoadCollectionsAsync(); // fire-and-forget

            userViewModel = new UsersViewModel(userService);
            this.DataContext = collectionGamesViewModel;
        }

        protected override async void OnNavigatedTo(NavigationEventArgs eventArgs)
        {
            base.OnNavigatedTo(eventArgs);

            if (eventArgs.Parameter is (int collectionId, string collectionNameParam))
            {
                collectionIdentifier = collectionId;
                collectionName = collectionNameParam;
                await LoadCollectionGamesAsync();
            }
            else if (eventArgs.Parameter is int backCollectionId)
            {
                collectionIdentifier = backCollectionId;
                var user = await userViewModel.GetCurrentUserAsync();
                if (user != null)
                {
                    var collection = await collectionsViewModel.GetCollectionByIdAsync(collectionIdentifier, user.UserId);
                    if (collection != null)
                    {
                        collectionName = collection.CollectionName;
                        await LoadCollectionGamesAsync();
                    }
                }
            }
        }

        private async Task LoadCollectionGamesAsync()
        {
            collectionGamesViewModel.CollectionName = collectionName;
            await collectionGamesViewModel.LoadGamesAsync(collectionIdentifier);
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            ContentFrame.Content = new CollectionGamesPage(collectionsService, userService);
        }

        private void AddGameToCollection_Click(object sender, RoutedEventArgs e)
        {
            ContentFrame.Content = new AddGameToCollectionPage(collectionsService, userService);
        }

        private async void RemoveGame_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sender is Button button && button.Tag is int gameId)
                {
                    await collectionsViewModel.RemoveGameFromCollectionAsync(collectionIdentifier, gameId);
                    await LoadCollectionGamesAsync();
                }
            }
            catch (Exception)
            {
                var dialog = new ContentDialog
                {
                    Title = ErrorDialogTitle,
                    Content = RemoveGameErrorMessage,
                    CloseButtonText = CloseButtonTextValue,
                    XamlRoot = this.XamlRoot
                };
                await dialog.ShowAsync();
            }
        }

        private void ViewGame_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is int gameId)
            {
                Frame.Navigate(typeof(GamePage), gameId);
            }
        }
    }
}