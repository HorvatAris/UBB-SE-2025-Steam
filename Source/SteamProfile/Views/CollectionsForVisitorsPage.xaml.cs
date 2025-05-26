using System;
using System.Collections.ObjectModel;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using SteamProfile.ViewModels;
using Microsoft.UI.Xaml.Navigation;
using BusinessLayer.Models;

namespace SteamProfile.Views
{
    public sealed partial class CollectionsForVisitorsPage : Page
    {
        private const string FailedToLoadCollectionsErrorMessage = "Failed to load collections. Please try again later.";

        private CollectionsViewModel collectionsViewModel;
        private int userIdentifier;

        public CollectionsForVisitorsPage()
        {
            this.InitializeComponent();
            collectionsViewModel = App.CollectionsViewModel;
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
