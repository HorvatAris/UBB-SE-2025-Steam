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
using System.Diagnostics;
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
        private ICollectionsService collectionsService;
        private IUserService userService;

        public AddGameToCollectionPage()
        {
            this.InitializeComponent();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (e.Parameter is ValueTuple<ICollectionsService, IUserService, int> parameters)
            {
                var (svc, usrSvc, colId) = parameters;

                if (svc is null || usrSvc is null)
                    return;

                collectionsService = svc;
                userService = usrSvc;
                collectionIdentifier = colId;

                if (userService is null || collectionsService is null)
                {
                    Debug.WriteLine("Services are null in AddGameToCollectionPage");
                    return;
                }

                addGamesToCollectionViewModel = new AddGameToCollectionViewModel(collectionsService, userService);
                this.DataContext = addGamesToCollectionViewModel;

                await addGamesToCollectionViewModel.InitializeAsync(collectionIdentifier);
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs eventArgs)
        {
            Frame.Navigate(typeof(CollectionGamesPage), (collectionsService, userService, collectionIdentifier));
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