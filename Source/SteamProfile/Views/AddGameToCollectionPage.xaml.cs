using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using BusinessLayer.Models;
using SteamProfile.ViewModels;

namespace SteamProfile.Views
{
    public sealed partial class AddGameToCollectionPage : Page
    {
        private AddGameToCollectionViewModel addGamesToCollectionViewModel;
        private int collectionIdentifier;

        public AddGameToCollectionPage()
        {
            this.InitializeComponent();
            addGamesToCollectionViewModel = App.AddGameToCollectionViewModel;
            this.DataContext = addGamesToCollectionViewModel;
        }

        protected override void OnNavigatedTo(NavigationEventArgs eventArgs)
        {
            base.OnNavigatedTo(eventArgs);
            if (eventArgs.Parameter is int collectionId)
            {
                collectionIdentifier = collectionId;
                addGamesToCollectionViewModel.Initialize(collectionId);
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs eventArgs)
        {
            Frame.Navigate(typeof(CollectionGamesPage), (collectionIdentifier, string.Empty));
        }

        private void AddGame_Click(object sender, RoutedEventArgs eventArgs)
        {
            if (sender is Button button && button.DataContext is OwnedGame game)
            {
                addGamesToCollectionViewModel.AddGameToCollection(game);
            }
        }
    }
}