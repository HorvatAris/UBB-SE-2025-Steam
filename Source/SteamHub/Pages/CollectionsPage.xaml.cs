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
    public sealed partial class CollectionsPage : Page
    {
        // Constants for dialog titles and button text
        private const string EditCollectionDialogTitle = "Edit Collection";
        private const string CreateCollectionDialogTitle = "Create New Collection";
        private const string PrimaryButtonTextSave = "Save";
        private const string PrimaryButtonTextCreate = "Create";
        private const string CloseButtonTextCancel = "Cancel";
        private const string ErrorDialogTitle = "Error";
        private const string UpdateCollectionErrorMessage = "Failed to update collection. Please try again.";
        private const string CreateCollectionErrorMessage = "Failed to create collection. Please try again.";

        // Constants for TextBox headers and placeholder texts
        private const string CollectionNameHeader = "Collection Name";
        private const string CollectionNamePlaceholder = "Enter collection name";
        private const string CoverPictureHeader = "Cover Picture URL";
        private const string CoverPicturePlaceholderEdit = "Enter cover picture URL (picture.(jpg/png/svg))";
        private const string CoverPicturePlaceholderCreate = "Enter cover picture URL";
        private const string PublicCollectionHeader = "Public Collection";

        private CollectionsViewModel collectionsViewModel;
        private UsersViewModel usersViewModel;
        private ICollectionsService collectionsService;
        private IUserService userService;

        public CollectionsPage() => this.InitializeComponent();

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (e.Parameter is (ICollectionsService collectionsService, IUserService userService))
                {

                this.collectionsService = collectionsService;
                this.userService = userService;
                collectionsViewModel = new CollectionsViewModel(collectionsService, userService);
                usersViewModel = new UsersViewModel(userService);
                this.DataContext = collectionsViewModel;

                _ = LoadCollectionsAsync();
            }
        }

        private async Task LoadCollectionsAsync() =>
            await collectionsViewModel.LoadCollectionsAsync();


        private void LoadCollections()
        {
            collectionsViewModel.LoadCollectionsCommand.Execute(null);
        }

        private void ViewCollection_Click(object sender, RoutedEventArgs eventArgs)
        {
            if (sender is Button button && button.CommandParameter is Collection collection)
            {
                // Example: assuming you have access to the required services
                //var collectionGamesPage = new CollectionGamesPage(collectionsService, userService);
                //ContentFrame.Content = collectionGamesPage;
                Debug.WriteLine($"Selected CollectionId={collection.CollectionId}, Name={collection.CollectionName}");

                Frame.Navigate(
    typeof(CollectionGamesPage),
    (this.collectionsService, this.userService, collection.CollectionId, collection.CollectionName)
);
            }
        }

        private void DeleteCollection_Click(object sender, RoutedEventArgs eventArgs)
        {
            if (sender is Button button && button.CommandParameter is int collectionId)
            {
                collectionsViewModel.DeleteCollectionCommand.Execute(collectionId);
            }
        }

        private async void EditCollection_Click(object sender, RoutedEventArgs eventArgs)
        {
            if (sender is Button button && button.CommandParameter is Collection collection)
            {
                var dialog = new ContentDialog
                {
                    Title = EditCollectionDialogTitle,
                    PrimaryButtonText = PrimaryButtonTextSave,
                    CloseButtonText = CloseButtonTextCancel,
                    DefaultButton = ContentDialogButton.Primary,
                    XamlRoot = this.XamlRoot
                };

                var panel = new StackPanel { Spacing = 10 };

                var nameTextBox = new TextBox
                {
                    Header = CollectionNameHeader,
                    Text = collection.CollectionName,
                    PlaceholderText = CollectionNamePlaceholder
                };

                var coverPictureTextBox = new TextBox
                {
                    Header = CoverPictureHeader,
                    Text = collection.CoverPicture,
                    PlaceholderText = CoverPicturePlaceholderEdit
                };

                var isPublicToggle = new ToggleSwitch
                {
                    Header = PublicCollectionHeader,
                    IsOn = collection.IsPublic
                };

                panel.Children.Add(nameTextBox);
                panel.Children.Add(coverPictureTextBox);
                panel.Children.Add(isPublicToggle);

                dialog.Content = panel;

                var result = await dialog.ShowAsync();

                if (result == ContentDialogResult.Primary)
                {
                    try
                    {
                        collectionsViewModel.UpdateCollectionCommand.Execute(new UpdateCollectionParams
                        {
                            CollectionId = collection.CollectionId,
                            CollectionName = nameTextBox.Text,
                            CoverPicture = coverPictureTextBox.Text,
                            IsPublic = isPublicToggle.IsOn
                        });
                    }
                    catch (Exception)
                    {
                        var errorDialog = new ContentDialog
                        {
                            Title = ErrorDialogTitle,
                            Content = UpdateCollectionErrorMessage,
                            CloseButtonText = CloseButtonTextCancel,
                            XamlRoot = this.XamlRoot
                        };
                        await errorDialog.ShowAsync();
                    }
                }
            }
        }

        private async void CreateCollection_Click(object sender, RoutedEventArgs eventArgs)
        {
            var dialog = new ContentDialog
            {
                Title = CreateCollectionDialogTitle,
                PrimaryButtonText = PrimaryButtonTextCreate,
                CloseButtonText = CloseButtonTextCancel,
                DefaultButton = ContentDialogButton.Primary,
                XamlRoot = this.XamlRoot
            };

            var panel = new StackPanel { Spacing = 10 };

            var nameTextBox = new TextBox
            {
                Header = CollectionNameHeader,
                PlaceholderText = CollectionNamePlaceholder
            };

            var coverPictureTextBox = new TextBox
            {
                Header = CoverPictureHeader,
                PlaceholderText = CoverPicturePlaceholderCreate
            };

            var isPublicToggle = new ToggleSwitch
            {
                Header = PublicCollectionHeader,
                IsOn = true
            };

            panel.Children.Add(nameTextBox);
            panel.Children.Add(coverPictureTextBox);
            panel.Children.Add(isPublicToggle);

            dialog.Content = panel;

            var result = await dialog.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                try
                {
                    collectionsViewModel.CreateCollectionCommand.Execute(new CreateCollectionParams
                    {
                        CollectionName = nameTextBox.Text,
                        CoverPicture = coverPictureTextBox.Text,
                        IsPublic = isPublicToggle.IsOn,
                        CreatedAt = DateOnly.FromDateTime(DateTime.Now)
                    });
                }
                catch (Exception)
                {
                    var errorDialog = new ContentDialog
                    {
                        Title = ErrorDialogTitle,
                        Content = CreateCollectionErrorMessage,
                        CloseButtonText = CloseButtonTextCancel,
                        XamlRoot = this.XamlRoot
                    };
                    await errorDialog.ShowAsync();
                }
            }
        }
    }
}
