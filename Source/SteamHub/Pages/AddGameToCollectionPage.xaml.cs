using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using SteamHub.ApiContract.Models.Game;
using SteamHub.ApiContract.Services.Interfaces;
using SteamHub.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace SteamHub.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AddGameToCollectionPage : Page
    {
        private AddGameToCollectionViewModel addGamesToCollectionViewModel;
        private int collectionIdentifier;

        public AddGameToCollectionPage(ICollectionsService collectionsService, IUserService userService)
        {
            this.InitializeComponent();
            addGamesToCollectionViewModel = new AddGameToCollectionViewModel(collectionsService, userService);
            this.DataContext = addGamesToCollectionViewModel;
        }

        protected override async void OnNavigatedTo(NavigationEventArgs eventArgs)
        {
            base.OnNavigatedTo(eventArgs);
            if (eventArgs.Parameter is int collectionId)
            {
                collectionIdentifier = collectionId;
                await addGamesToCollectionViewModel.InitializeAsync(collectionId);
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs eventArgs)
        {
            Frame.Navigate(typeof(CollectionGamesPage), (collectionIdentifier, string.Empty));
        }

        private async void AddGame_Click(object sender, RoutedEventArgs eventArgs)
        {
            if (sender is Button button && button.DataContext is OwnedGame game)
            {
                await addGamesToCollectionViewModel.AddGameToCollectionAsync(game);
            }
        }
    }
}