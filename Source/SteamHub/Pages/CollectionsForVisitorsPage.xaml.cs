using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using SteamHub.ApiContract.Models.Collections;
using SteamHub.ApiContract.Services.Interfaces;
using SteamHub.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    public sealed partial class CollectionsForVisitorsPage : Page
    {
        private const string FailedToLoadCollectionsErrorMessage = "Failed to load collections. Please try again later.";

        private CollectionsViewModel collectionsViewModel;
        private int userIdentifier;

        public CollectionsForVisitorsPage(ICollectionsService collectionsService, IUserService userService)
        {
            this.InitializeComponent();
            collectionsViewModel = new CollectionsViewModel(collectionsService , userService);
            collectionsViewModel.LoadCollections();
            this.DataContext = collectionsViewModel;
        }

        protected override void OnNavigatedTo(NavigationEventArgs eventArgs)
        {
            base.OnNavigatedTo(eventArgs);
            if (eventArgs.Parameter is int userId)
            {
                userIdentifier = userId;
                LoadCollections();
            }
        }

        private void LoadCollections()
        {
            try
            {
                collectionsViewModel.IsLoading = true;
                collectionsViewModel.ErrorMessage = string.Empty;

                var collections = collectionsViewModel.GetPublicCollectionsForUser(userIdentifier);
                collectionsViewModel.Collections = new ObservableCollection<Collection>(collections);
            }
            catch (Exception)
            {
                collectionsViewModel.ErrorMessage = FailedToLoadCollectionsErrorMessage;
            }
            finally
            {
                collectionsViewModel.IsLoading = false;
            }
        }

        private void ViewCollection_Click(object sender, RoutedEventArgs eventArgs)
        {
            if (sender is Button button && button.Tag is int collectionId)
            {
                Frame.Navigate(typeof(CollectionGamesPage), collectionId);
            }
        }
    }
}
