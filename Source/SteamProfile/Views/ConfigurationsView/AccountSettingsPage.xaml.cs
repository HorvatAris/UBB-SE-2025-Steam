using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using SteamProfile.ViewModels.ConfigurationsViewModels;

namespace SteamProfile.Views.ConfigurationsView
{
    public sealed partial class AccountSettingsPage : Page
    {
        private AccountSettingsViewModel ViewModel { get; set; }
        private ContentDialog passwordConfirmationDialog;

        public AccountSettingsPage()
        {
            this.InitializeComponent();
            ViewModel = new AccountSettingsViewModel();
            DataContext = ViewModel;
            ViewModel.RequestPasswordConfirmation += ViewModel_RequestPasswordConfirmation;
        }

        private async void ViewModel_RequestPasswordConfirmation(object sender, EventArgs e)
        {
            // Check if XamlRoot is available
            if (this.XamlRoot == null)
            {
                // Debug to see if this is the issue
                System.Diagnostics.Debug.WriteLine("XamlRoot is null");

                // Try to get XamlRoot from the window
                var window = (Application.Current as App)?.MainWindow;
                if (window != null)
                {
                    // Create the password confirmation dialog
                    passwordConfirmationDialog = new ContentDialog
                    {
                        Title = "Confirm Password",
                        Content = CreatePasswordConfirmationContent(),
                        PrimaryButtonText = "Confirm",
                        CloseButtonText = "Cancel",
                        DefaultButton = ContentDialogButton.Primary,
                        XamlRoot = window.Content.XamlRoot
                    };
                }
            }
            else
            {
                // Create the dialog as before
                passwordConfirmationDialog = new ContentDialog
                {
                    Title = "Confirm Password",
                    Content = CreatePasswordConfirmationContent(),
                    PrimaryButtonText = "Confirm",
                    CloseButtonText = "Cancel",
                    DefaultButton = ContentDialogButton.Primary,
                    XamlRoot = this.XamlRoot
                };
            }

            // Make sure dialog was created before showing it
            if (passwordConfirmationDialog != null)
            {
                // Show the dialog and handle the result
                var result = await passwordConfirmationDialog.ShowAsync();
                if (result == ContentDialogResult.Primary)
                {
                    ViewModel.ExecutePendingAction();
                }
                else
                {
                    ViewModel.CancelPendingAction();
                }
            }
        }

        private StackPanel CreatePasswordConfirmationContent()
        {
            var panel = new StackPanel { Spacing = 10 };

            var messageTextBlock = new TextBlock
            {
                Text = "Please enter your current password to confirm this action:",
                TextWrapping = TextWrapping.Wrap
            };
            panel.Children.Add(messageTextBlock);

            var passwordBox = new PasswordBox
            {
                PlaceholderText = "Current password"
            };

            // Bind the password to the ViewModel's CurrentPassword property
            passwordBox.PasswordChanged += (s, e) =>
            {
                ViewModel.CurrentPassword = passwordBox.Password;
            };

            panel.Children.Add(passwordBox);

            var errorTextBlock = new TextBlock
            {
                Text = "Incorrect password",
                Foreground = new SolidColorBrush(Microsoft.UI.Colors.Red),
                Visibility = Visibility.Collapsed
            };
            panel.Children.Add(errorTextBlock);

            return panel;
        }

        // Use the Unloaded event instead of overriding OnUnloaded
        private void AccountSettingsPage_Unloaded(object sender, RoutedEventArgs e)
        {
            // Unsubscribe from events to prevent memory leaks
            if (ViewModel != null)
            {
                ViewModel.RequestPasswordConfirmation -= ViewModel_RequestPasswordConfirmation;
            }

            // Unsubscribe from the Unloaded event itself
            this.Unloaded -= AccountSettingsPage_Unloaded;
        }
        private void GoBack(object sender, RoutedEventArgs e)
        {
            Frame.GoBack();
        }
    }
}