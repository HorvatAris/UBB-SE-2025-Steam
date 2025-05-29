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
        private string collectionName;
        private  ICollectionsService collectionsService;
        private  IUserService userService;


        public CollectionGamesPage() => this.InitializeComponent();

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);



            if (e.Parameter is ValueTuple<ICollectionsService, IUserService, int, string> fullParams)
            {
                var (svc, usrSvc, colId, colName) = fullParams;

                if (svc is null || usrSvc is null)
                {
                    
                    return;
                }

                collectionsService = svc;
                userService = usrSvc;
                collectionIdentifier = colId;
                collectionName = colName;

                collectionGamesViewModel = new CollectionGamesViewModel(collectionsService);
                collectionsViewModel = new CollectionsViewModel(collectionsService, userService);
                userViewModel = new UsersViewModel(userService);
                this.DataContext = collectionGamesViewModel;

                await collectionsViewModel.LoadCollectionsAsync();
                await LoadCollectionGamesAsync();
            }
            else if (e.Parameter is ValueTuple<ICollectionsService, IUserService, int> backParams)
            {
                var (svc, usrSvc, colId) = backParams;

                collectionsService = svc;
                userService = usrSvc;
                collectionIdentifier = colId;

                collectionGamesViewModel = new CollectionGamesViewModel(collectionsService);
                collectionsViewModel = new CollectionsViewModel(collectionsService, userService);
                userViewModel = new UsersViewModel(userService);
                this.DataContext = collectionGamesViewModel;

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
            Frame.Navigate(typeof(CollectionsPage), (collectionsService, userService));
        }

        private void AddGameToCollection_Click(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine($"Navigating to AddGameToCollectionPage with CollectionId={collectionIdentifier}");

            Frame.Navigate(typeof(AddGameToCollectionPage), (collectionsService, userService, collectionIdentifier));
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